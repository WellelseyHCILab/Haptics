using UnityEditor;
using UnityEngine;
using System.Collections;

public class ZSCoreDiagnosticWindow : EditorWindow
{
  #region PUBLIC_MEMBERS

  // Stereo Members
  public bool m_isStereoEnabled = true;
  public bool m_areEyesSwapped = false;

  public float m_interPupillaryDistance = 0.06f;
  public float m_stereoLevel = 1.0f;
  public float m_worldScale = 1.0f;
  public float m_fieldOfViewScale = 1.0f;
  public float m_zeroParallaxOffset = 0.0f;
  public float m_nearClip = 0.1f;
  public float m_farClip = 100000.0f;

  // Tracker Members
  public bool m_isHeadTrackingEnabled = true;
  public float m_headTrackingScale = 1.0f;

  public bool m_isStylusTrackingEnabled = true;
  public bool m_isStylusVisualizationEnabled = true;

  public bool m_isMouseEmulationEnabled = false;
  public float m_mouseEmulationDistance = 1.0f;
  
  public bool m_isStylusLedEnabled = false;
  public ZSCore.StylusLedColor m_stylusLedColor = ZSCore.StylusLedColor.White;

  public bool m_isStylusVibrationEnabled = false;
  public float m_stylusVibrationOnPeriod = 0.0f;
  public float m_stylusVibrationOffPeriod = 0.0f;
  public int m_stylusVibrationRepeatCount = 0;
  public bool m_startStylusVibration = false;
  public bool m_stopStylusVibration = false;

  // Read Only Members
  public Vector3 m_displayOffset = Vector3.zero;
  public Vector2 m_displayPosition = Vector2.zero;
  public Vector2 m_displayAngle = Vector2.zero;
  public Vector2 m_displayResolution = Vector2.zero;
  public Vector2 m_displaySize = Vector2.zero;

  public Vector3 m_headPosition = Vector3.zero;
  public Vector3 m_headDirection = Vector3.zero;
  public Vector3 m_headCameraPosition = Vector3.zero;
  public Vector3 m_headCameraDirection = Vector3.zero;
  public Vector3 m_headWorldPosition = Vector3.zero;
  public Vector3 m_headWorldDirection = Vector3.zero;

  public Vector3 m_stylusPosition = Vector3.zero;
  public Vector3 m_stylusDirection = Vector3.zero;
  public Vector3 m_stylusCameraPosition = Vector3.zero;
  public Vector3 m_stylusCameraDirection = Vector3.zero;
  public Vector3 m_stylusWorldPosition = Vector3.zero;
  public Vector3 m_stylusWorldDirection = Vector3.zero;

  public bool m_isStylusButton0Pressed = false;
  public bool m_isStylusButton1Pressed = false;
  public bool m_isStylusButton2Pressed = false;

  #endregion

  #region UNITY_CALLBACKS

  public void Awake()
  {
    m_isStereoEnabled = EditorPrefs.GetBool("IsStereoEnabled", true);
    m_areEyesSwapped = EditorPrefs.GetBool("AreEyesSwapped", false);

    m_interPupillaryDistance = EditorPrefs.GetFloat("InterPupillaryDistance", 0.06f);
    m_stereoLevel = EditorPrefs.GetFloat("StereoLevel", 1.0f);
    m_worldScale = EditorPrefs.GetFloat("WorldScale", 1.0f);
    m_fieldOfViewScale = EditorPrefs.GetFloat("FieldOfViewScale", 1.0f);
    m_zeroParallaxOffset = EditorPrefs.GetFloat("ZeroParallaxOffset", 0.0f);
    m_nearClip = EditorPrefs.GetFloat("NearClip", 0.1f);
    m_farClip = EditorPrefs.GetFloat("FarClip", 100000.0f);

    m_isHeadTrackingEnabled = EditorPrefs.GetBool("IsHeadTrackingEnabled", true);
    m_headTrackingScale = EditorPrefs.GetFloat("HeadTrackingScale", 1.0f);

    m_isStylusTrackingEnabled = EditorPrefs.GetBool("IsStylusTrackingEnabled", true);
    m_isStylusVisualizationEnabled = EditorPrefs.GetBool("IsStylusVisualizationEnabled", true);

    m_isMouseEmulationEnabled = EditorPrefs.GetBool("IsMouseEmulationEnabled", false);
    m_mouseEmulationDistance = EditorPrefs.GetFloat("MouseEmulationDistance", 1.0f);

    m_isStylusLedEnabled = EditorPrefs.GetBool("IsStylusLedEnabled", false);
    m_stylusLedColor = (ZSCore.StylusLedColor)EditorPrefs.GetInt("StylusLedColor", (int)ZSCore.StylusLedColor.White);

    m_isStylusVibrationEnabled = EditorPrefs.GetBool("IsStylusVibrationEnabled", false);
    m_stylusVibrationOnPeriod = EditorPrefs.GetFloat("StylusVibrationOnPeriod", 0.0f);
    m_stylusVibrationOffPeriod = EditorPrefs.GetFloat("StylusVibrationOffPeriod", 0.0f);
    m_stylusVibrationRepeatCount = EditorPrefs.GetInt("StylusVibrationRepeatCount", 0);
    m_startStylusVibration = EditorPrefs.GetBool("StartStylusVibration", false);
    m_stopStylusVibration = EditorPrefs.GetBool("StopStylusVibration", false); 
  }

  public void OnDestroy()
  {
    EditorPrefs.SetBool("IsStereoEnabled", m_isStereoEnabled);
    EditorPrefs.SetBool("AreEyesSwapped", m_areEyesSwapped);

    EditorPrefs.SetFloat("InterPupillaryDistance", m_interPupillaryDistance);
    EditorPrefs.SetFloat("StereoLevel", m_stereoLevel);
    EditorPrefs.SetFloat("WorldScale", m_worldScale);
    EditorPrefs.SetFloat("FieldOfViewScale", m_fieldOfViewScale);
    EditorPrefs.SetFloat("ZeroParallaxOffset", m_zeroParallaxOffset);
    EditorPrefs.SetFloat("NearClip", m_nearClip);
    EditorPrefs.SetFloat("FarClip", m_farClip);

    EditorPrefs.SetBool("IsHeadTrackingEnabled", m_isHeadTrackingEnabled);
    EditorPrefs.SetFloat("HeadTrackingScale", m_headTrackingScale);

    EditorPrefs.SetBool("IsStylusTrackingEnabled", m_isStylusTrackingEnabled);
    EditorPrefs.SetBool("IsStylusVisualizationEnabled", m_isStylusVisualizationEnabled);

    EditorPrefs.SetBool("IsMouseEmulationEnabled", m_isMouseEmulationEnabled);
    EditorPrefs.SetFloat("MouseEmulationDistance", m_mouseEmulationDistance);

    EditorPrefs.SetBool("IsStylusLedEnabled", m_isStylusLedEnabled);
    EditorPrefs.SetInt("StylusLedColor", (int)m_stylusLedColor);

    EditorPrefs.SetBool("IsStylusVibrationEnabled", m_isStylusVibrationEnabled);
    EditorPrefs.SetFloat("StylusVibrationOnPeriod", m_stylusVibrationOnPeriod);
    EditorPrefs.SetFloat("StylusVibrationOffPeriod", m_stylusVibrationOffPeriod);
    EditorPrefs.SetInt("StylusVibrationRepeatCount", m_stylusVibrationRepeatCount);
    EditorPrefs.SetBool("StartStylusVibration", m_startStylusVibration);
    EditorPrefs.SetBool("StopStylusVibration", m_stopStylusVibration);
  }

  void OnGUI()
  {
    m_scrollPosition = EditorGUILayout.BeginScrollView(m_scrollPosition, GUILayout.Width(position.width), GUILayout.Height(position.height));

    if (GUILayout.Button("Restore Defaults", GUILayout.Width(118), GUILayout.Height(20)))
      RestoreDefaults();

    GUILayout.Label("\nStereo Settings:", EditorStyles.boldLabel);

    m_isStereoEnabled = EditorGUILayout.Toggle("Enable Stereo", m_isStereoEnabled);
    m_areEyesSwapped = EditorGUILayout.Toggle("Swap Eyes", m_areEyesSwapped);

    m_interPupillaryDistance = EditorGUILayout.Slider("Inter Pupillary Distance", m_interPupillaryDistance, 0, 1);
    m_stereoLevel = EditorGUILayout.Slider("Stereo Level", m_stereoLevel, 0, 1);
    m_worldScale = EditorGUILayout.Slider("World Scale", m_worldScale, 0.0001f, 1000);
    m_fieldOfViewScale = EditorGUILayout.Slider("Field of View Scale", m_fieldOfViewScale, 0, 100);
    m_zeroParallaxOffset = EditorGUILayout.Slider("Zero Parallax Offset", m_zeroParallaxOffset, -10, 10);
    m_nearClip = EditorGUILayout.Slider("Near Clip", m_nearClip, 0, 1000);
    m_farClip = EditorGUILayout.Slider("Far Clip", m_farClip, 0, 100000);

    GUILayout.Label("\nHead Tracker Settings:", EditorStyles.boldLabel);

    m_isHeadTrackingEnabled = EditorGUILayout.Toggle("Enable Head Tracking", m_isHeadTrackingEnabled);
    m_headTrackingScale = EditorGUILayout.Slider("Head Tracking Scale", m_headTrackingScale, 0, 1000);

    GUILayout.Label("\nStylus Tracker Settings:", EditorStyles.boldLabel);
    m_isStylusTrackingEnabled = EditorGUILayout.Toggle("Enable Stylus Tracking", m_isStylusTrackingEnabled);
    m_isStylusVisualizationEnabled = EditorGUILayout.Toggle("Enable Stylus Visualization", m_isStylusVisualizationEnabled);

    GUILayout.Label("\nStylus LED Settings:", EditorStyles.boldLabel);
    m_isStylusLedEnabled = EditorGUILayout.Toggle("Enable Stylus LED", m_isStylusLedEnabled);
    m_stylusLedColor = (ZSCore.StylusLedColor)EditorGUILayout.EnumPopup("Stylus Led Color", m_stylusLedColor);

    GUILayout.Label("\nStylus Vibration Settings:", EditorStyles.boldLabel);
    m_isStylusVibrationEnabled = EditorGUILayout.Toggle("Enable Stylus Vibration", m_isStylusVibrationEnabled);
    m_stylusVibrationOnPeriod = EditorGUILayout.Slider("Vibration On Period", m_stylusVibrationOnPeriod, 0, 100);
    m_stylusVibrationOffPeriod = EditorGUILayout.Slider("Vibration Off Period", m_stylusVibrationOffPeriod, 0, 100);
    m_stylusVibrationRepeatCount = EditorGUILayout.IntSlider("Vibration Repeat Count", m_stylusVibrationRepeatCount, 0, 100);
    m_startStylusVibration = EditorGUILayout.Toggle("Start Vibration", m_startStylusVibration);
    m_stopStylusVibration = EditorGUILayout.Toggle("Stop Vibration", m_stopStylusVibration);

    GUILayout.Label("\nMouse Emulation:", EditorStyles.boldLabel);
    m_isMouseEmulationEnabled = EditorGUILayout.Toggle("Enable Mouse Emulation", m_isMouseEmulationEnabled);
    m_mouseEmulationDistance = EditorGUILayout.Slider("Emulation Distance", m_mouseEmulationDistance, 0, 5);

    GUILayout.Label("\nDisplay Information (read only):", EditorStyles.boldLabel);
    m_displayOffset = EditorGUILayout.Vector3Field("Display Offset", m_displayOffset);
    m_displayPosition = EditorGUILayout.Vector2Field("Display Position", m_displayPosition);
    m_displayAngle = EditorGUILayout.Vector2Field("Display Angle", m_displayAngle);
    m_displayResolution = EditorGUILayout.Vector2Field("Display Resolution", m_displayResolution);
    m_displaySize = EditorGUILayout.Vector2Field("Display Size", m_displaySize);

    GUILayout.Label("\nHead Tracker Information (read only):", EditorStyles.boldLabel);
    m_headPosition = EditorGUILayout.Vector3Field("Position (tracker space)", m_headPosition);
    m_headDirection = EditorGUILayout.Vector3Field("Direction (tracker space)", m_headDirection);
    m_headCameraPosition = EditorGUILayout.Vector3Field("Position (camera space)", m_headCameraPosition);
    m_headCameraDirection = EditorGUILayout.Vector3Field("Direction (camera space)", m_headCameraDirection);
    m_headWorldPosition = EditorGUILayout.Vector3Field("Position (world space)", m_headWorldPosition);
    m_headWorldDirection = EditorGUILayout.Vector3Field("Direction (world space)", m_headWorldDirection);
    
    GUILayout.Label("\nStylus Tracker Information (read only):", EditorStyles.boldLabel);
    m_stylusPosition = EditorGUILayout.Vector3Field("Position (tracker space)", m_stylusPosition);
    m_stylusDirection = EditorGUILayout.Vector3Field("Direction (tracker space)", m_stylusDirection);
    m_stylusCameraPosition = EditorGUILayout.Vector3Field("Position (camera space)", m_stylusCameraPosition);
    m_stylusCameraDirection = EditorGUILayout.Vector3Field("Direction (camera space)", m_stylusCameraDirection);
    m_stylusWorldPosition = EditorGUILayout.Vector3Field("Position (world space)", m_stylusWorldPosition);
    m_stylusWorldDirection = EditorGUILayout.Vector3Field("Direction (world space)", m_stylusWorldDirection);

    m_isStylusButton0Pressed = EditorGUILayout.Toggle("Button 0 Pressed", m_isStylusButton0Pressed);
    m_isStylusButton1Pressed = EditorGUILayout.Toggle("Button 1 Pressed", m_isStylusButton1Pressed);
    m_isStylusButton2Pressed = EditorGUILayout.Toggle("Button 2 Pressed", m_isStylusButton2Pressed);

    EditorGUILayout.EndScrollView();
  }

  void OnInspectorUpdate()
  {
    Repaint();
  }

  #endregion

  #region PRIVATE_HELPERS

  private void RestoreDefaults()
  {
    // Stereo Members
    m_isStereoEnabled = true;
    m_areEyesSwapped = false;

    m_interPupillaryDistance = 0.06f;
    m_stereoLevel = 1.0f;
    m_worldScale = 1.0f;
    m_fieldOfViewScale = 1.0f;
    m_zeroParallaxOffset = 0.0f;
    m_nearClip = 0.1f;
    m_farClip = 100000.0f;

    // Tracker Members
    m_isHeadTrackingEnabled = true;
    m_headTrackingScale = 1.0f;

    m_isStylusTrackingEnabled = true;
    m_isStylusVisualizationEnabled = true;

    m_isMouseEmulationEnabled = false;
    m_mouseEmulationDistance = 1.0f;
    
    m_isStylusLedEnabled = false;
    m_stylusLedColor = ZSCore.StylusLedColor.White;

    m_isStylusVibrationEnabled = false;
    m_stylusVibrationOnPeriod = 0.0f;
    m_stylusVibrationOffPeriod = 0.0f;
    m_stylusVibrationRepeatCount = 0;
    m_startStylusVibration = false;
    m_stopStylusVibration = false;
  }

  #endregion

  #region PRIVATE_MEMBERS

  private Vector2 m_scrollPosition = Vector2.zero;

  #endregion
}
