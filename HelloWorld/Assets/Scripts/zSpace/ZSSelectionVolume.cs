using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A draggable volume that tracks all colliding objects.
/// Disable the GameObject to disable this behavior.
/// </summary>
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Renderer))]
public class ZSSelectionVolume : MonoBehaviour
{
  /// <summary>
  /// If true, the selection volume will be aligned to the X, Y, and Z axes in world space.
  /// If false, it will be aligned to the local X, Y, and Z axes of the stylus on button down.
  /// </summary>
  public bool m_isAxisAligned = true;

  Vector3 m_endPosition;
  Vector3 m_startPosition;
  Quaternion m_startRotation;
  ZSStylusSelector m_stylusSelector;
  int m_selectButton = 0;

  void Awake()
  {
    m_stylusSelector = GameObject.Find("ZSStylus").GetComponent<ZSStylusSelector>();
    collider.enabled = false;
    renderer.enabled = false;
  }


  void Update()
  {
    if (m_stylusSelector.GetButtonDown(m_selectButton))
    {
      if (m_stylusSelector.hoveredObject == null)
      {
        m_startPosition = m_stylusSelector.stylus.transform.TransformPoint(m_stylusSelector.stylus.hotSpot);
        m_startRotation = (m_isAxisAligned) ? Quaternion.identity : m_stylusSelector.stylus.transform.rotation;
  
        collider.enabled = true;
        renderer.enabled = true;
        transform.rotation = m_startRotation;
      }
    }

    if (m_stylusSelector.GetButtonUp(m_selectButton))
    {
      collider.enabled = false;
      renderer.enabled = false;
    }

    if (m_stylusSelector.GetButton(m_selectButton) && renderer.enabled)
    {
      m_endPosition = m_stylusSelector.stylus.transform.TransformPoint(m_stylusSelector.stylus.hotSpot);

      Vector3 diagonal = m_endPosition - m_startPosition;
      transform.localScale = Quaternion.Inverse(transform.rotation) * diagonal;
      transform.position = 0.5f * (m_endPosition + m_startPosition);
    }
  }


  void OnTriggerEnter(Collider selectedCollider)
  {
    GameObject go = m_stylusSelector.objectRelation(selectedCollider.gameObject);
    if ((1 << go.layer & m_stylusSelector.layerMask) == 0)
      return;

    m_stylusSelector.selectedObjects.Add(go);
  }


  void OnTriggerExit(Collider selectedCollider)
  {
    GameObject go = m_stylusSelector.objectRelation(selectedCollider.gameObject);
    if ((1 << go.layer & m_stylusSelector.layerMask) == 0)
      return;

    //FIXME: Need a separate set of "overlapping" objects in case 2 alias to the same selected object.
    m_stylusSelector.selectedObjects.Remove(go);
  }
}