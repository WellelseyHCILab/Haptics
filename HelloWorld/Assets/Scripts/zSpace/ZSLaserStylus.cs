//////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2012 Infinite Z, Inc.  All Rights Reserved.
//
//  File:       ZSLaserStylus.cs
//  Content:    Maintains the casual laser stylus.
//  SVN Info:   $Id$
//
//////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Stylus shape with laser-like behavior.
/// Auto-sizes to the nearest intersecting object.
/// Supports an optional gap at the tip or base to allow for end-point geometry.
/// </summary>
public class ZSLaserStylus : ZSStylusShape
{
  /// <summary>
  /// The length from the stylus position to the stylus tip position when it is not intersecting anything.
  /// This is independent of the in-editor distance between the tip and the base.
  /// </summary>
  public float m_initialLength = 1.0f;

  /// <summary>
  /// The length of the stylus tip object.  The beam will auto fit to this distance from the tip's origin.
  /// </summary>
  public float m_tipLength = 0.0f;

  /// <summary>
  /// The length of the stylus base object.  The beam will auto fit to this distance from the base's origin.
  /// </summary>
  public float m_baseLength = 0.0f;

  /// <summary>
  /// The stylus tip.  This visually represents the front end of the stylus.
  /// </summary>
  public GameObject m_tip;

  /// <summary>
  /// The stylus beam.  This visually represents the middle of the stylus, connecting the tip and base.
  /// </summary>
  public GameObject m_beam;

  /// <summary>
  /// The stylus base.  This visually represents the back end of the stylus.
  /// </summary>
  public GameObject m_base;

  float m_beamLength;

  void Awake()
  {
    m_initialLength = Mathf.Max(m_initialLength, m_tipLength + m_baseLength);

    m_beamLength = (m_tip.transform.position - transform.position).magnitude - m_tipLength - m_baseLength;

    m_beam.transform.localScale = new Vector3(1.0f, 1.0f, (m_initialLength - m_tipLength - m_baseLength) / m_beamLength);
    m_tip.transform.localPosition = m_initialLength * Vector3.forward;

    m_tip.SetActiveRecursively(false); //Tip isn't visible until the stylus hits something.
  }


  /// <summary>
  /// This callback defines the way the stylus responds to overlap with an object in the scene.
  /// It is called whenever an overlap begins.
  /// </summary>
  public override void OnHoverBegin(GameObject gameObject, Vector3 point)
  {
    base.OnHoverBegin(gameObject, point);

    m_tip.SetActiveRecursively(true);
  }


  /// <summary>
  /// This callback defines the way the stylus responds to overlap with an object in the scene.
  /// It is called for each frame as long as the stylus is overlapping with the object.
  /// </summary>
  public override void OnHoverStay(GameObject gameObject, Vector3 point)
  {
    base.OnHoverStay(gameObject, point);

    float hoverDistance = Mathf.Max(transform.InverseTransformPoint(point).z, m_tipLength + m_baseLength);
    m_beam.transform.localScale = new Vector3(1.0f, 1.0f, (hoverDistance - m_tipLength - m_baseLength) / m_beamLength);
    m_tip.transform.localPosition = hoverDistance * Vector3.forward;
  }


  /// <summary>
  /// This callback defines the way the stylus responds to overlap with an object in the scene.
  /// It is called whenever an overlap begins.
  /// </summary>
  public override void OnHoverEnd(GameObject gameObject)
  {
    base.OnHoverEnd(gameObject);

    m_beam.transform.localScale = new Vector3(1.0f, 1.0f, (m_initialLength - m_tipLength - m_baseLength) / m_beamLength);
    m_tip.transform.localPosition = m_initialLength * Vector3.forward;

    m_tip.SetActiveRecursively(false);
  }
}
