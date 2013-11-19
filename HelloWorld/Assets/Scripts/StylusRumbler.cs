////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2012 Infinite Z, Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Rumbles the stylus when objects are hovered, selected, or acted upon with a tool.
/// </summary>
public class StylusRumbler : MonoBehaviour
{
    public int HoverIntensity = 0;
    public int UnhoverIntensity = 0;
    public int SelectIntesity = 0;
    public int DeselectIntensity = 0;
    public int ToolOnIntensity = 0;
    public int ToolOffIntensity = 0;
    public int SnapOnIntensity = 0;
    public int SnapOffIntensity = 0;

    protected ZSCore _core;
    protected ZSStylusSelector _stylusSelector;

    protected GameObject _oldHoverObject;
    protected HashSet<GameObject> _oldSelectedObjects = new HashSet<GameObject>();
    protected bool _oldIsToolActive = false;
    protected bool _wasSnapped = false;
    protected bool _isVibrating = false;

    void start()
    {
        _core = GameObject.Find("ZSCore").GetComponent<ZSCore>();
        _stylusSelector = GameObject.Find("ZSStylusSelector").GetComponent<ZSStylusSelector>();
    }

    void update() 
    {
        {
            GameObject hoverObject = _stylusSelector.hoveredObject;
            if (hoverObject != _oldHoverObject)
            {
                if (hoverObject != null)
                    OnHoverBegin(hoverObject);
                else
                    OnHoverEnd(_oldHoverObject);
            }
            _oldHoverObject = hoverObject;
        }
/*
        {
            HashSet<GameObject> selectedObjects = _stylusSelector.selectedObjects;

            if (_oldSelectedObjects.Except(selectedObjects).Count() != 0)
                OnSelectEnd(null);

            if (selectedObjects.Except(_oldSelectedObjects).Count() != 0)
                OnSelectBegin(null);

            _oldSelectedObjects = selectedObjects;
        }
        {
            bool isToolActive = _stylusSelector.activeStylus.Tool.IsOperating;
    
            if (isToolActive && !_oldIsToolActive)
                OnToolBegin(null);
    
            if (!isToolActive && _oldIsToolActive)
                OnToolEnd(null);
    
            _oldIsToolActive = isToolActive;
        }
        {
            bool isSnapped = false;
            //TODO: Expensive!  Add events to Snap instead.
            Snap[] snaps = GameObject.FindObjectsOfType(typeof(Snap)) as Snap[];
            foreach (Snap snap in snaps)
                isSnapped |= (snap.mateObject != null);

            if (isSnapped && !_wasSnapped)
                OnSnapBegin(null);

            if (!isSnapped && _wasSnapped)
                OnSnapEnd(null);

            _wasSnapped = isSnapped;
        }
*/
    }

    void OnHoverBegin(GameObject go) { Shake(HoverIntensity); }
    void OnHoverEnd(GameObject go) { Shake(UnhoverIntensity); }
/*
    void OnSelectBegin(GameObject go) { Shake(SelectIntesity); }
    void OnSelectEnd(GameObject go) { Shake(DeselectIntensity); }
    void OnToolBegin(GameObject go) { Shake(ToolOnIntensity); }
    void OnToolEnd(GameObject go) { Shake(ToolOffIntensity); }
    void OnSnapBegin(GameObject go) { Shake(SnapOnIntensity); }
    void OnSnapEnd(GameObject go) { Shake(SnapOffIntensity); }
*/
    public void Shake(int intensity)
    {
        if (_isVibrating)
            return;

        float onPeriod = 0f;
        float offPeriod = 0f;
        int repeatCount = 0;

        switch (intensity)
        {
            case 0:
            break;
            case 1:
                onPeriod = 0.032f;
                offPeriod = 0.128f;
                repeatCount = 0;
            break;
            case 2:
                onPeriod = 0.032f;
                offPeriod = 0.064f;
                repeatCount = 0;
            break;
            case 3:
                onPeriod = 0.032f;
                offPeriod = 0.064f;
                repeatCount = 1;
            break;
            case 4:
                onPeriod = 0.064f;
                offPeriod = 0.128f;
                repeatCount = 0;
            break;
            case 5:
                onPeriod = 0.64f;
                offPeriod = 0.064f;
                repeatCount = 0;
            break;
            case 6:
                onPeriod = 0.64f;
                offPeriod = 0.064f;
                repeatCount = 1;
            break;
            case 7:
                onPeriod = 0.128f;
                offPeriod = 0.256f;
                repeatCount = 0;
            break;
            case 8:
                onPeriod = 0.128f;
                offPeriod = 0.128f;
                repeatCount = 0;
            break;
            case 9:
                onPeriod = 0.128f;
                offPeriod = 0.128f;
                repeatCount = 1;
            break;
            default:
                onPeriod = 0.1f * (float)intensity;
            break;
        }

        _core.SetStylusVibrationOnPeriod(onPeriod);
        _core.SetStylusVibrationOffPeriod(offPeriod);
        _core.SetStylusVibrationRepeatCount(repeatCount);
        _core.SetStylusVibrationEnabled(true);
        _core.StartStylusVibration();
        _isVibrating = true;
    }
}
