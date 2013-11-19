//////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2012 Infinite Z, Inc.  All Rights Reserved.
//
//  File:       ZSWandStylus.cs
//  Content:    Maintains the precise wand stylus.
//  SVN Info:   $Id$
//
//////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Stylus shape with wand-like behavior.
/// Size is fixed, but can be increased or decreased manually by the user.
/// Contacts are sorted on distance from the stylus tip.
/// </summary>
public class ZSWandStylus : ZSStylusShape
{
  /// <summary>
  /// The increment used to increase or decrease the stylus length.
  /// </summary>
  public float m_scaleIncrement = 1f;

  /// <summary>
  /// The stylus tip.  This visually represents the front end of the stylus.
  /// </summary>
  public GameObject m_tip;

  /// <summary>
  /// The stylus beam.  This visually represents the middle and back of the stylus.
  /// </summary>
  public GameObject m_beam;

  void Start()
  {
    m_hotSpot = m_tip.transform.localPosition;
  }


  void scaleLengthBy(float scaleFactor)
  {
    m_hotSpot *= scaleFactor;
    m_tip.transform.localPosition *= scaleFactor;

    Vector3 beamScale = m_beam.transform.localScale;
    beamScale[1] *= scaleFactor;
    m_beam.transform.localScale = beamScale;

    m_beam.transform.localPosition *= scaleFactor;
  }
  

  void Update()
  {
    if (Input.GetKey(KeyCode.Equals) || Input.GetKey(KeyCode.KeypadPlus))
      scaleLengthBy(1.0f + Time.deltaTime * m_scaleIncrement);
    if (Input.GetKey(KeyCode.Minus) || Input.GetKey(KeyCode.KeypadMinus))
      scaleLengthBy(1.0f/(1.0f + Time.deltaTime * m_scaleIncrement));
  }
}
