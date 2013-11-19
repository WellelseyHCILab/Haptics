using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Maintains a list of the objects that overlap with the stylus, sorted by distance from the tip.
/// Uses collision detection system to detect overlaps.
/// </summary>
public class HoverQueue : Comparer<RaycastHit>
{  
  RaycastHit[] m_hitInfos;
  ZSStylusSelector m_stylusSelector;
  int m_layerMask;

  /// <summary>
  /// Creates a HoverQueue for the given ZSStylusSelector.
  /// Only objects in the given bitmask of layers will be considered.
  /// </summary>
  public HoverQueue(ZSStylusSelector stylusSelector, int layerMask)
  {
    m_layerMask = layerMask;
    m_stylusSelector = stylusSelector;
  }


  /// <summary>
  /// Perform collision detection and sort the list of contacts.
  /// </summary>
  public void update()
  {
    float distance = m_stylusSelector.stylus.transform.TransformDirection(m_stylusSelector.stylus.hotSpot).magnitude;
    float castLength = (Mathf.Approximately(distance, 0.0f)) ? Mathf.Infinity : distance;

    //TODO: Sweeps/casts don't notice initial contacts.  And OnCollisionEnter doesn't work between a plain Collider and a Rigidbody.
    m_hitInfos = Physics.RaycastAll(m_stylusSelector.transform.position, m_stylusSelector.transform.forward, castLength, m_layerMask);
    Array.Sort<RaycastHit>(m_hitInfos, this);
  }


  /// <summary>
  /// Returns the first GameObject in the hover queue as a RaycastHit to encode the distance and hit postion.
  /// </summary>
  public RaycastHit getFirst()
  {
    if (m_hitInfos != null && m_hitInfos.Length > 0)
      return m_hitInfos[0];
    RaycastHit result = new RaycastHit();
    result.distance = Mathf.Infinity;
    return result;
  }


  /// <summary>
  /// This callback is called for comparison sorting to order the objects in the hover queue.
  /// It prefers objects closer to the hotspot.
  /// </summary>
  public override int Compare(RaycastHit x, RaycastHit y)
  {
    Vector3 hotSpot = m_stylusSelector.stylus.transform.TransformPoint(m_stylusSelector.stylus.hotSpot);
    float xDistance = (x.point - hotSpot).magnitude;
    float yDistance = (y.point - hotSpot).magnitude;

    return (int)Mathf.Sign(xDistance - yDistance);
  }
}


/// <summary>
/// Class for tracking the stylus configuration and managing its interaction with scene objects.
/// </summary>
public class ZSStylusSelector : MonoBehaviour
{
  /// <summary> Modes for emulating the stylus using mouse input. </summary>
  public enum StylusSimulatorMode
  {
    /// <summary> Do not emulate the stylus with mouse input. </summary>
    None = 0,

    /// <summary>
    /// The stylus will be positioned and oriented along a ray from the camera
    /// through the mouse position on the display plane.
    /// The mouse wheel will cause translation along this vector.
    /// </summary>
    Projection = 1,

    /// <summary>
    /// The stylus will be positioned along a ray as in projection, but the rotation will not change.
    /// The mouse wheel will cause translation along this vector.
    /// </summary>
    Position = 2,

    /// <summary>
    /// Rotation about the Y and X axes will correspond to horizontal and vertical mouse movement, respectively.
    /// The mouse wheel will cause rotation about the z axis.
    /// </summary>
    Rotation = 3,
  };

  /// <summary>
  /// Type for mapping one GameObject to another.
  /// Frequently needed for scene graph operations.
  /// </summary>
  public delegate GameObject ObjectRelation(GameObject collidedObject);

  /// <summary>
  /// Searches upwards until it finds a Rigidbody.
  /// </summary>
  public static GameObject FindRigidBody(GameObject collidedObject)
  {
    GameObject go = collidedObject;
    while (go != null && go.rigidbody == null && go.transform.parent != null)
      go = go.transform.parent.gameObject;

    if (go != null && go.rigidbody != null)
      return go;
    else
      return null;
  }

  /// <summary>
  /// The object (if any) that is overlapping the stylus and is closest to the hot spot.
  /// </summary>
  public GameObject hoveredObject
  {
    set
    {
      if (value != m_hoveredObject)
      {
        if (m_hoveredObject != null)
        {
          stylus.OnHoverEnd(m_hoveredObject);
          m_hoveredObject.SendMessage("OnUnhovered", SendMessageOptions.DontRequireReceiver);
        }
        if (value != null)
        {
          value.SendMessage("OnHovered", SendMessageOptions.DontRequireReceiver);
          stylus.OnHoverBegin(value, m_hoverPoint);
        }
        m_hoveredObject = value;
      }
    }

    get { return m_hoveredObject; }
  }

  /// <summary>
  /// The number of buttons the user can press on the current stylus.
  /// </summary>
  public int numButtons 
  { 
    get 
    { 
      return 3; //FIXME: Plugin crashes on 2nd run in editor here. Math.Max(3, m_zsCore.GetNumTrackerTargetButtons(ZSCore.TrackerTargetType.Primary));
    }
  }

  /// <summary>
  /// The contact point between the stylus and the hovered object.
  /// If there is no hovered object, this is the last such point.
  /// </summary>
  public Vector3 hoverPoint { get { return m_hoverPoint; } }

  ZSCore m_zsCore;

  /// <summary>
  /// The set of selected objects.
  /// Object membership can be queried in constant time.
  /// </summary>
  public HashSet<GameObject> selectedObjects = new HashSet<GameObject>();

  /// <summary>
  /// A bitmask encoding all the layers that will be considered in stylus interaction.
  /// </summary>
  public int layerMask = 1;

  /// <summary>
  /// The minimum distance stylus movement required for a click-release interval to be considered a drag.
  /// </summary>
  public float minDragDistance = 0.01f;

  /// <summary>
  /// The currently active stylus shape.
  /// Any shape-specific tools (such as a dragger or scaler) should be adjacent to this script.
  /// </summary>
  public ZSStylusShape stylus;

  /// <summary>
  /// The ID of the stylus button that will be used for selecting objects.
  /// </summary>
  public int selectButton = 0;

  /// <summary>
  /// A special layer for UI and other elements where hovering is desired but selection is not.
  /// </summary>
  public int uiLayer;

  /// <summary>
  /// The mapping from a collidable object to what should be hovered or selected when that object collides with the stylus.
  /// </summary>
  public ObjectRelation objectRelation = FindRigidBody;

  /// <summary>
  /// The stylus simulator mode.
  /// </summary>
  public StylusSimulatorMode stylusSimulatorMode = StylusSimulatorMode.None;

  bool[] m_isButtonPressed;
  bool[] m_wasButtonPressed;
  bool m_wasHoveredObjectSelected;
  Vector3 m_lastMousePosition;
  float m_mouseWheel = 0.01f;
  Matrix4x4 m_lastPose = Matrix4x4.identity;
  Vector3 m_buttonDownPosition;

  HashSet<GameObject> m_oldSelectedObjects = new HashSet<GameObject>();
  HoverQueue m_hoverQueue;
  GameObject m_hoveredObject;
  Vector3 m_hoverPoint = Vector3.zero;

  /// <summary>
  /// Similar to Input.GetMouseButton(whichButton).  Returns true if the given stylus button is currently down.
  /// </summary>
  public bool GetButton(int whichButton) { return m_isButtonPressed[whichButton]; }

  /// <summary>
  /// Similar to Input.GetMouseButtonDown(whichButton).  Returns true if the given stylus button was pressed during the last frame.
  /// </summary>
  public bool GetButtonDown(int whichButton) { return !m_wasButtonPressed[whichButton] && m_isButtonPressed[whichButton]; }

  /// <summary>
  /// Similar to Input.GetMouseButtonUp(whichButton).  Returns true if the given stylus button was released during the last frame.
  /// </summary>
  public bool GetButtonUp(int whichButton) { return m_wasButtonPressed[whichButton] && !m_isButtonPressed[whichButton]; }


  void Awake()
  {
    m_lastMousePosition = Input.mousePosition;
    
    m_zsCore = GameObject.Find("ZSCore").GetComponent<ZSCore>();

    layerMask |= 1 << uiLayer;
    m_hoverQueue = new HoverQueue(this, layerMask);

    m_isButtonPressed = new bool[numButtons];
    m_wasButtonPressed = new bool[numButtons];
    for (int i = 0; i < numButtons; ++i)
      m_isButtonPressed[i] = m_wasButtonPressed[i] = false;
      
    //TODO: For now, disable stylus collision so it doesn't conflict with raycasting.
    foreach (Collider collider in transform.GetComponentsInChildren<Collider>())
      collider.enabled = false;
  }
  

  void LateUpdate()
  {
    //Update stylus button states.
    for (int i = 0; i < numButtons; ++i)
    {
      m_wasButtonPressed[i] = m_isButtonPressed[i];
      //Have to combine mouse state down here so asynchronous clients see it at the right time.
      m_isButtonPressed[i] = m_zsCore.IsTrackerTargetButtonPressed(ZSCore.TrackerTargetType.Primary, i) || Input.GetMouseButton(i);
    }

    Matrix4x4 pose = m_zsCore.GetTrackerTargetWorldPose(ZSCore.TrackerTargetType.Primary);
    if (pose != m_lastPose)
    {
      m_lastPose = pose;

      Vector3 position = pose * new Vector4(0, 0, 0, 1);
      Vector3 forward = (Vector3)(pose * Vector3.forward);
      Vector3 up = (Vector3)(pose * Vector3.up);

      Quaternion rotation = Quaternion.LookRotation(forward, up);

      transform.position = position;
      transform.rotation = rotation;
      transform.localScale = m_zsCore.GetWorldScale() * Vector3.one;
    }

    //Simulate the stylus based on mouse input.

    Vector3 dMousePosition = Input.mousePosition - m_lastMousePosition;
    dMousePosition[2] = Input.GetAxis("Mouse ScrollWheel");
    if (Input.GetKeyDown(KeyCode.LeftBracket))
      dMousePosition[2] -= 0.01f;
    if (Input.GetKeyDown(KeyCode.RightBracket))
      dMousePosition[2] += 0.01f;

    Camera mainCamera = m_zsCore.m_currentCamera.camera;

    if (mainCamera != null && mainCamera.enabled && dMousePosition != Vector3.zero)
    {
      if (stylusSimulatorMode == StylusSimulatorMode.Projection || stylusSimulatorMode == StylusSimulatorMode.Position)
      {
        //Only update the wheel total if we aren't rotating.  Avoids extra Z translation artifact.
        m_mouseWheel += dMousePosition[2];
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Vector3 rayPoint = ray.GetPoint(0.5f * m_mouseWheel * mainCamera.transform.localScale.magnitude);
        transform.position = rayPoint;

        if (stylusSimulatorMode == StylusSimulatorMode.Projection)
          transform.rotation = Quaternion.LookRotation(ray.GetPoint(1.0f) - mainCamera.transform.position, mainCamera.transform.up);
      }
      else if (stylusSimulatorMode == StylusSimulatorMode.Rotation)
      {
        Vector3 euler = transform.localRotation.eulerAngles;
        euler += new Vector3(-0.1f * dMousePosition.y, 0.1f * dMousePosition.x, -1000.0f * dMousePosition.z);
        transform.localRotation = Quaternion.Euler(euler);
      }
    }

    //Make the hovered object the closest one to the tip.

    m_hoverQueue.update();
    RaycastHit hit = m_hoverQueue.getFirst();
    m_hoverPoint = hit.point;

    hoveredObject = (hit.collider == null) ?
                      null :
                      (hit.collider.gameObject.layer == uiLayer) ?
                        hit.collider.gameObject :
                        objectRelation(hit.collider.gameObject);

    if (hit.collider != null)
      stylus.OnHoverStay(m_hoveredObject, m_hoverPoint);

    //Update the set of selected objects based on clicking.
    m_oldSelectedObjects.Clear();
    foreach (GameObject selectedObject in selectedObjects)
      m_oldSelectedObjects.Add(selectedObject);

    if (GetButtonDown(selectButton))
    {
      if (!Input.GetKey(KeyCode.LeftControl))
        selectedObjects.Clear();

      if (m_hoveredObject != null)
      {
        if (m_hoveredObject.layer == uiLayer)
        {
          m_wasHoveredObjectSelected = false;
        }
        else
        {
          m_wasHoveredObjectSelected = selectedObjects.Contains(m_hoveredObject);
          if (!selectedObjects.Contains(m_hoveredObject))
            selectedObjects.Add(m_hoveredObject);
        }
      }

      m_buttonDownPosition = transform.position;
    }

    if (GetButtonUp(selectButton))
    {
      bool wasDrag = (transform.position - m_buttonDownPosition).magnitude > minDragDistance;
      if (m_hoveredObject != null && m_wasHoveredObjectSelected && !wasDrag && Input.GetKey(KeyCode.LeftControl))
        selectedObjects.Remove(m_hoveredObject);
    }

    //Send messages to objects whose selection state changed.

    foreach (GameObject selectedObject in m_oldSelectedObjects)
    {
      if (!selectedObjects.Contains(selectedObject))
        selectedObject.SendMessage("OnDeselected", SendMessageOptions.DontRequireReceiver);
    }

    foreach (GameObject selectedObject in selectedObjects)
    {
      if (!m_oldSelectedObjects.Contains(selectedObject))
        selectedObject.SendMessage("OnSelected", SendMessageOptions.DontRequireReceiver);
    }

    m_lastMousePosition = Input.mousePosition;
  }
}
