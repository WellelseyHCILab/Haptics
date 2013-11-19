using UnityEditor;
using UnityEngine;
using System.Collections;

public class ZSCoreDiagnostic : MonoBehaviour
{
  #region UNITY_CALLBACKS

  // Use this for initialization
  void Start()
  {
    // Check to make sure the application is running in the editor.
    if (!Application.isEditor)
      return;

    GameObject coreObject = GameObject.Find("ZSCore");
    m_coreDiagnosticWindow = (ZSCoreDiagnosticWindow)EditorWindow.GetWindow(typeof(ZSCoreDiagnosticWindow));

    if (coreObject != null)
      m_core = coreObject.GetComponent<ZSCore>();

    if (m_core != null)
    {
      m_stylusButtonStates = new bool[m_core.GetNumTrackerTargetButtons(ZSCore.TrackerTargetType.Primary)];

      LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
      lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
      lineRenderer.SetColors(Color.white, Color.white);
    }
  }

  // Update is called once per frame
  void Update()
  {
    if (m_core == null)
      return;

    if (m_coreDiagnosticWindow == null)
      m_coreDiagnosticWindow = (ZSCoreDiagnosticWindow)EditorWindow.GetWindow(typeof(ZSCoreDiagnosticWindow));

    // Stereo Settings.
    if (m_coreDiagnosticWindow.m_isStereoEnabled != m_core.IsStereoEnabled())
      m_core.SetStereoEnabled(m_coreDiagnosticWindow.m_isStereoEnabled);

    if (m_coreDiagnosticWindow.m_areEyesSwapped != m_core.AreEyesSwapped())
      m_core.SetEyesSwapped(m_coreDiagnosticWindow.m_areEyesSwapped);

    if (m_coreDiagnosticWindow.m_interPupillaryDistance != m_core.GetInterPupillaryDistance())
      m_core.SetInterPupillaryDistance(m_coreDiagnosticWindow.m_interPupillaryDistance);

    if (m_coreDiagnosticWindow.m_stereoLevel != m_core.GetStereoLevel())
      m_core.SetStereoLevel(m_coreDiagnosticWindow.m_stereoLevel);

    if (m_coreDiagnosticWindow.m_worldScale != m_core.GetWorldScale())
      m_core.SetWorldScale(m_coreDiagnosticWindow.m_worldScale);

    if (m_coreDiagnosticWindow.m_fieldOfViewScale != m_core.GetFieldOfViewScale())
      m_core.SetFieldOfViewScale(m_coreDiagnosticWindow.m_fieldOfViewScale);

    if (m_coreDiagnosticWindow.m_zeroParallaxOffset != m_core.GetZeroParallaxOffset())
      m_core.SetZeroParallaxOffset(m_coreDiagnosticWindow.m_zeroParallaxOffset);

    if (m_coreDiagnosticWindow.m_nearClip != m_core.GetNearClip())
      m_core.SetNearClip(m_coreDiagnosticWindow.m_nearClip);

    if (m_coreDiagnosticWindow.m_farClip != m_core.GetFarClip())
      m_core.SetFarClip(m_coreDiagnosticWindow.m_farClip);

    // Head Tracker Settings.
    if (m_coreDiagnosticWindow.m_isHeadTrackingEnabled != m_core.IsHeadTrackingEnabled())
      m_core.SetHeadTrackingEnabled(m_coreDiagnosticWindow.m_isHeadTrackingEnabled);

    if (m_coreDiagnosticWindow.m_headTrackingScale != m_core.GetHeadTrackingScale())
      m_core.SetHeadTrackingScale(m_coreDiagnosticWindow.m_headTrackingScale);

    // Stylus Tracker Settings.
    if (m_coreDiagnosticWindow.m_isStylusTrackingEnabled != m_core.IsStylusTrackingEnabled())
      m_core.SetStylusTrackingEnabled(m_coreDiagnosticWindow.m_isStylusTrackingEnabled);

    // Stylus LED Settings.
    if (m_coreDiagnosticWindow.m_isStylusLedEnabled != m_core.IsStylusLedEnabled())
      m_core.SetStylusLedEnabled(m_coreDiagnosticWindow.m_isStylusLedEnabled);

    if (m_coreDiagnosticWindow.m_stylusLedColor != m_core.GetStylusLedColor())
      m_core.SetStylusLedColor(m_coreDiagnosticWindow.m_stylusLedColor);

    // Stylus Vibration Settings.
    if (m_coreDiagnosticWindow.m_isStylusVibrationEnabled != m_core.IsStylusVibrationEnabled())
      m_core.SetStylusVibrationEnabled(m_coreDiagnosticWindow.m_isStylusVibrationEnabled);

    if (m_coreDiagnosticWindow.m_stylusVibrationOnPeriod != m_previousStylusVibrationOnPeriod)
    {
      m_core.SetStylusVibrationOnPeriod(m_coreDiagnosticWindow.m_stylusVibrationOnPeriod);
      m_previousStylusVibrationOnPeriod = m_coreDiagnosticWindow.m_stylusVibrationOnPeriod;
    }

    if (m_coreDiagnosticWindow.m_stylusVibrationOffPeriod != m_previousStylusVibrationOffPeriod)
    {
      m_core.SetStylusVibrationOffPeriod(m_coreDiagnosticWindow.m_stylusVibrationOffPeriod);
      m_previousStylusVibrationOffPeriod = m_coreDiagnosticWindow.m_stylusVibrationOffPeriod;
    }

    if (m_coreDiagnosticWindow.m_stylusVibrationRepeatCount != m_core.GetStylusVibrationRepeatCount())
      m_core.SetStylusVibrationRepeatCount(m_coreDiagnosticWindow.m_stylusVibrationRepeatCount);

    if (m_coreDiagnosticWindow.m_startStylusVibration)
    {
      m_core.StartStylusVibration();
      m_coreDiagnosticWindow.m_startStylusVibration = false;
    }

    if (m_coreDiagnosticWindow.m_stopStylusVibration)
    {
      m_core.StopStylusVibration();
      m_coreDiagnosticWindow.m_stopStylusVibration = false;
    }

    // Mouse Emulation Settings.
    if (m_coreDiagnosticWindow.m_isMouseEmulationEnabled != m_core.IsMouseEmulationEnabled())
      m_core.SetMouseEmulationEnabled(m_coreDiagnosticWindow.m_isMouseEmulationEnabled);

    if (m_coreDiagnosticWindow.m_mouseEmulationDistance != m_core.GetMouseEmulationDistance())
      m_core.SetMouseEmulationDistance(m_coreDiagnosticWindow.m_mouseEmulationDistance);

    // Read Only Display Information.
    m_coreDiagnosticWindow.m_displayOffset = m_core.GetDisplayOffset();
    m_coreDiagnosticWindow.m_displayPosition = m_core.GetDisplayPosition();
    m_coreDiagnosticWindow.m_displayAngle = m_core.GetDisplayAngle();
    m_coreDiagnosticWindow.m_displayResolution = m_core.GetDisplayResolution();
    m_coreDiagnosticWindow.m_displaySize = m_core.GetDisplaySize();


    // Read Only Head Tracker Information.
    m_headPose = m_core.GetTrackerTargetPose(ZSCore.TrackerTargetType.Head);
    m_coreDiagnosticWindow.m_headPosition = new Vector3(m_headPose[0, 3], m_headPose[1, 3], m_headPose[2, 3]);
    m_coreDiagnosticWindow.m_headDirection = m_headPose * new Vector3(0, 0, 1.0f);

    m_headCameraPose = m_core.GetTrackerTargetCameraPose(ZSCore.TrackerTargetType.Head);
    m_coreDiagnosticWindow.m_headCameraPosition = new Vector3(m_headCameraPose[0, 3], m_headCameraPose[1, 3], m_headCameraPose[2, 3]);
    m_coreDiagnosticWindow.m_headCameraDirection = m_headCameraPose * new Vector3(0, 0, 1.0f);
    
    m_headWorldPose = m_core.GetTrackerTargetWorldPose(ZSCore.TrackerTargetType.Head);
    m_coreDiagnosticWindow.m_headWorldPosition = new Vector3(m_headWorldPose[0, 3], m_headWorldPose[1, 3], m_headWorldPose[2, 3]);
    m_coreDiagnosticWindow.m_headWorldDirection = m_headWorldPose * new Vector3(0, 0, 1.0f);

    // Read Only Stylus Tracker Information.
    m_stylusPose = m_core.GetTrackerTargetPose(ZSCore.TrackerTargetType.Primary);
    m_coreDiagnosticWindow.m_stylusPosition = new Vector3(m_stylusPose[0, 3], m_stylusPose[1, 3], m_stylusPose[2, 3]);
    m_coreDiagnosticWindow.m_stylusDirection = m_stylusPose * new Vector3(0, 0, 1.0f);

    m_stylusCameraPose = m_core.GetTrackerTargetCameraPose(ZSCore.TrackerTargetType.Primary);
    m_coreDiagnosticWindow.m_stylusCameraPosition = new Vector3(m_stylusCameraPose[0, 3], m_stylusCameraPose[1, 3], m_stylusCameraPose[2, 3]);
    m_coreDiagnosticWindow.m_stylusCameraDirection = m_stylusCameraPose * new Vector3(0, 0, 1.0f);

    m_stylusWorldPose = m_core.GetTrackerTargetWorldPose(ZSCore.TrackerTargetType.Primary);
    m_coreDiagnosticWindow.m_stylusWorldPosition = new Vector3(m_stylusWorldPose[0, 3], m_stylusWorldPose[1, 3], m_stylusWorldPose[2, 3]);
    m_coreDiagnosticWindow.m_stylusWorldDirection = m_stylusWorldPose * new Vector3(0, 0, 1.0f);

    bool isAnyStylusButtonPressed = false;

    for (int i = 0; i < m_core.GetNumTrackerTargetButtons(ZSCore.TrackerTargetType.Primary); ++i)
    {
      m_stylusButtonStates[i] = m_core.IsTrackerTargetButtonPressed(ZSCore.TrackerTargetType.Primary, i);
      isAnyStylusButtonPressed |= m_stylusButtonStates[i];
    }

    m_coreDiagnosticWindow.m_isStylusButton0Pressed = m_stylusButtonStates[0];
    m_coreDiagnosticWindow.m_isStylusButton1Pressed = m_stylusButtonStates[1];
    m_coreDiagnosticWindow.m_isStylusButton2Pressed = m_stylusButtonStates[2];

    // Draw the stylus visualization
    float stylusBeamWidth = 0.0004f * m_coreDiagnosticWindow.m_worldScale;
    float stylusBeamLength = 0.1f * m_coreDiagnosticWindow.m_worldScale;

    LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer>();
    lineRenderer.enabled = m_coreDiagnosticWindow.m_isStylusVisualizationEnabled;

    if (lineRenderer.enabled)
    {
      lineRenderer.SetWidth(stylusBeamWidth, stylusBeamWidth);
      lineRenderer.SetPosition(0, m_coreDiagnosticWindow.m_stylusWorldPosition);
      lineRenderer.SetPosition(1, (m_coreDiagnosticWindow.m_stylusWorldPosition + (stylusBeamLength * m_coreDiagnosticWindow.m_stylusWorldDirection)));

      if (!isAnyStylusButtonPressed)
      {
        lineRenderer.SetColors(Color.white, Color.white);
      }
      else
      {
        if (!m_wasAnyButtonPressed)
        {
          if (m_stylusButtonStates[0])
            lineRenderer.SetColors(Color.red, Color.red);
          else if (m_stylusButtonStates[1])
            lineRenderer.SetColors(Color.green, Color.green);
          else if (m_stylusButtonStates[2])
            lineRenderer.SetColors(Color.blue, Color.blue);
        }
      }
    }

    m_wasAnyButtonPressed = isAnyStylusButtonPressed;
  }

  void OnDestroy()
  {
    m_coreDiagnosticWindow.Close();
  }

  #endregion

  #region PRIVATE_MEMBERS

  private ZSCore m_core = null;
  private ZSCoreDiagnosticWindow m_coreDiagnosticWindow = null;

  private bool m_wasAnyButtonPressed = false;

  // Read Only
  private Matrix4x4 m_headPose = new Matrix4x4();
  private Matrix4x4 m_headCameraPose = new Matrix4x4();
  private Matrix4x4 m_headWorldPose = new Matrix4x4();

  private Matrix4x4 m_stylusPose = new Matrix4x4();
  private Matrix4x4 m_stylusCameraPose = new Matrix4x4();
  private Matrix4x4 m_stylusWorldPose = new Matrix4x4();
  private bool[] m_stylusButtonStates;

  private float m_previousStylusVibrationOnPeriod = -1.0f;
  private float m_previousStylusVibrationOffPeriod = -1.0f;

  #endregion
}
