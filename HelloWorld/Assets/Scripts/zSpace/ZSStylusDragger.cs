//////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2012 Infinite Z, Inc.  All Rights Reserved.
//
//  File:       ZSStylusDragger.cs
//  Content:    Allows the user to drag a rigid body with the stylus.
//  SVN Info:   $Id$
//
//////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ZSStylusShape))]
/// <summary>
/// Stylus tool for dragging objects with or without physics.
/// </summary>
public class ZSStylusDragger : MonoBehaviour
{
  ///<summary>
  ///Helper struct for saving the state of a Rigidbody, since they can't be disabled.
  ///All parameters map to parameters in Rigidbody.
  ///</summary>
  public struct RigidbodyProperties
  {
    public float drag;
    public float angularDrag;
    public bool isKinematic;
    public bool useGravity;
    public int solverIterationCount;
    public RigidbodyInterpolation interpolation;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ZSStylusDragger.RigidbodyProperties"/> struct from a Rigidbody.
    /// </summary>
    public RigidbodyProperties(Rigidbody rb)
    {
      drag = rb.drag;
      angularDrag = rb.angularDrag;
      isKinematic = rb.isKinematic;
      useGravity = rb.useGravity;
      solverIterationCount = rb.solverIterationCount;
      interpolation = rb.interpolation;
    }
    
    /// <summary>
    /// Applies the struct's parameters to the given Rigidbody.
    /// </summary>
    public void ApplyTo(Rigidbody rb)
    {
      rb.drag = drag;
      rb.angularDrag = angularDrag;
      rb.isKinematic = isKinematic;
      rb.useGravity = useGravity;
      rb.solverIterationCount = solverIterationCount;
      rb.interpolation = interpolation;
    }
    
    /// <summary>
    /// Gets the default properties to be used by a Rigidbody that the user is dragging.
    /// </summary>
    public static RigidbodyProperties GetDraggingProperties()
    {
      RigidbodyProperties result = new RigidbodyProperties();
      result.drag = 10.0f;
      result.angularDrag = 10.0f;
      result.isKinematic = false;
      result.useGravity = false;
      result.solverIterationCount = 12;
      result.interpolation = RigidbodyInterpolation.Interpolate;
      return result;
    }
  }
  
  /// <summary>
  /// Chooses between physical or non-physical dragging.
  /// During non-physical drag, the dragged object's transform is directly updated.
  /// During physical drag, the dragged object is physically constrained to the stylus.
  /// </summary>
  public bool m_isPhysical = false;
  
  /// <summary>
  /// The ID of the stylus button that will be used for dragging.
  /// </summary>
  public int m_dragButton = 0;

  List<GameObject> m_focusObjects = new List<GameObject>();
  Vector3 m_contactPoint;
  Dictionary<GameObject, Vector3> m_focusOffsets = new Dictionary<GameObject, Vector3>();
  Dictionary<GameObject, Quaternion> m_focusRotations = new Dictionary<GameObject, Quaternion>();
  Dictionary<GameObject, ConfigurableJoint> m_joints = new Dictionary<GameObject, ConfigurableJoint>();
  Dictionary<GameObject, RigidbodyProperties> m_rigidbodyProperties = new Dictionary<GameObject, RigidbodyProperties>();
  ZSStylusSelector m_stylusSelector;

  void Awake()
  {
    m_stylusSelector = GameObject.Find("ZSStylus").GetComponent<ZSStylusSelector>();
  }


  void Update()
  {
    if (m_stylusSelector.GetButtonDown(m_dragButton) && m_stylusSelector.hoveredObject != null)
    {
      m_contactPoint = m_stylusSelector.hoverPoint;

      foreach (GameObject selectedObject in m_stylusSelector.selectedObjects)
      {
        m_focusObjects.Add(selectedObject);
        
        // Save the relative transform from the stylus to the focus object.
        Quaternion invRotation = Quaternion.Inverse(transform.rotation);
        m_focusOffsets[selectedObject] = invRotation * (selectedObject.transform.position - transform.position);
        m_focusRotations[selectedObject] = invRotation * selectedObject.transform.rotation;
        
        // Initiate drag.
        
        selectedObject.SendMessage("OnDragBegin", SendMessageOptions.DontRequireReceiver);
      }

      if (m_focusObjects.Count > 0)
        m_stylusSelector.stylus.OnDragBegin(m_focusObjects[m_focusObjects.Count - 1]);

      if (m_isPhysical)
        BeginPhysicalDrag();
    }

    if (m_stylusSelector.GetButtonUp(m_dragButton))
    {
      // End drag.

      if (m_isPhysical)
        EndPhysicalDrag();

      foreach (GameObject focusObject in m_focusObjects)
        focusObject.SendMessage("OnDragEnd", SendMessageOptions.DontRequireReceiver);

      m_stylusSelector.stylus.OnDragEnd(m_focusObjects.ToArray());

      m_focusObjects.Clear();
      m_focusOffsets.Clear();
      m_focusRotations.Clear();
    }

    // Seamlessly switch between physical and non-physical drag.
    bool shouldBePhysical = Input.GetKey(KeyCode.LeftShift);
    if (shouldBePhysical != m_isPhysical)
    {
      if (m_focusObjects.Count > 0)
      {
        if (m_isPhysical)
          EndPhysicalDrag();
        else
          BeginPhysicalDrag();
      }

      m_isPhysical = shouldBePhysical;
    }

    // For non-physical dragging, update the dragged object transform by applyging the saved relative transform to the new stylus transform.
    if (!m_isPhysical)
    {
      foreach (GameObject focusObject in m_focusObjects)
      {
        focusObject.transform.position = transform.position + transform.rotation * m_focusOffsets[focusObject];
        focusObject.transform.rotation = transform.rotation * m_focusRotations[focusObject];
      }
    }
  }


  void BeginPhysicalDrag()
  {
    foreach (GameObject focusObject in m_focusObjects)
    {
      // Replace any nested Rigidbodies with a master one that has pre-determined properties.
  
      Vector3 velocity = (focusObject.rigidbody != null) ? focusObject.rigidbody.velocity : Vector3.zero;
      Vector3 angularVelocity = (focusObject.rigidbody != null) ? focusObject.rigidbody.angularVelocity : Vector3.zero;
  
      foreach (Rigidbody childRb in focusObject.transform.GetComponentsInChildren<Rigidbody>())
      {
        m_rigidbodyProperties[childRb.gameObject] = new RigidbodyProperties(childRb);
        DestroyImmediate(childRb);
      }
  
      if (focusObject.rigidbody == null)
      {
        Rigidbody rb = focusObject.AddComponent<Rigidbody>();
        RigidbodyProperties.GetDraggingProperties().ApplyTo(rb);
        rb.velocity = velocity;
        rb.angularVelocity = angularVelocity;
      }
  
      // Constrain the dragged object to the stylus.
      ConfigurableJoint joint = focusObject.AddComponent<ConfigurableJoint>();
      joint.connectedBody = rigidbody;
      joint.anchor = m_contactPoint;
      joint.xMotion = ConfigurableJointMotion.Limited;
      joint.yMotion = ConfigurableJointMotion.Limited;
      joint.zMotion = ConfigurableJointMotion.Limited;
      joint.angularXMotion = ConfigurableJointMotion.Limited;
      joint.angularYMotion = ConfigurableJointMotion.Limited;
      joint.angularZMotion = ConfigurableJointMotion.Limited;
      JointDrive jointDrive = new JointDrive();
      jointDrive.mode = JointDriveMode.Position;
      jointDrive.positionSpring = 0.1f;
      jointDrive.positionDamper = 500000;
      joint.xDrive = jointDrive;
      joint.yDrive = jointDrive;
      joint.zDrive = jointDrive;
  //    joint.angularXDrive = jointDrive;
  //    joint.angularYZDrive = jointDrive;
      m_joints[focusObject] = joint;
    }

    // Clear the hover now now because Unity never gives us OnTrigger events during physical sub-assembly drag.
    if (m_focusObjects.Count > 0)
      m_stylusSelector.hoveredObject = null;
  }


  void EndPhysicalDrag()
  {
    foreach (GameObject focusObject in m_focusObjects)
    {
      // Delete dragging physics state and restore any original nested Rigidbodies.
  
      DestroyImmediate(m_joints[focusObject]);
  
      Vector3 velocity = focusObject.rigidbody.velocity;
      Vector3 angularVelocity = focusObject.rigidbody.angularVelocity;
      DestroyImmediate(focusObject.rigidbody);

      if (m_rigidbodyProperties.ContainsKey(focusObject))
      {
        Rigidbody rb = focusObject.AddComponent<Rigidbody>();
        m_rigidbodyProperties[focusObject].ApplyTo(rb);
        m_rigidbodyProperties.Remove(focusObject);

        if (!rb.isKinematic)
        {
          rb.velocity = velocity;
          rb.angularVelocity = angularVelocity;
        }
      }
    }

    foreach (KeyValuePair<GameObject, RigidbodyProperties> entry in m_rigidbodyProperties)
    {
      Rigidbody rb = entry.Key.AddComponent<Rigidbody>();
      entry.Value.ApplyTo(rb);
    }
    m_rigidbodyProperties.Clear();
  }
}
