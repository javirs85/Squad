using System;
using UnityEditor;
using UnityEngine;

namespace Gtec.Bandpower
{
    [CustomEditor(typeof(Device))]
    public class DeviceEditor : Editor
    {
        Texture banner;

        SerializedProperty Type;
        SerializedProperty SimulatorAlphaLevelUv;
        SerializedProperty Signal;
        SerializedProperty AdvancedSettings;
        SerializedProperty BuffersizeInSamples;
        SerializedProperty BufferOverlapInSamples;
        SerializedProperty Delta;
        SerializedProperty Theta;
        SerializedProperty Alpha;
        SerializedProperty BetaLow;
        SerializedProperty BetaMid;
        SerializedProperty BetaHigh;
        SerializedProperty Gamma;

        SerializedProperty OnDevicesAvailable;
        SerializedProperty OnDeviceStateChanged;
        SerializedProperty OnPipelineStateChanged;
        SerializedProperty OnRuntimeExceptionOccured;
        SerializedProperty OnBandpowerAvailable;
        SerializedProperty OnMeanBandpowerAvailable;
        SerializedProperty OnRatiosAvailable;
        SerializedProperty OnMeanRatiosAvailable;
        SerializedProperty OnSignalQualityAvailable;
        SerializedProperty OnBatteryLevelAvailable;
        SerializedProperty OnDataLost;

        private void OnEnable()
        {
            banner = Resources.Load<Texture>("logo/unicorn-logo-horizontal");

            Type = serializedObject.FindProperty("Type");
            SimulatorAlphaLevelUv = serializedObject.FindProperty("SimulatorAlphaLevelUv");
            Signal = serializedObject.FindProperty("Signal");
            AdvancedSettings = serializedObject.FindProperty("AdvancedSettings");
            BuffersizeInSamples = serializedObject.FindProperty("BuffersizeInSamples");
            BufferOverlapInSamples = serializedObject.FindProperty("BufferOverlapInSamples");
            Delta = serializedObject.FindProperty("Delta");
            Theta = serializedObject.FindProperty("Theta");
            Alpha = serializedObject.FindProperty("Alpha");
            BetaLow = serializedObject.FindProperty("BetaLow");
            BetaMid = serializedObject.FindProperty("BetaMid");
            BetaHigh = serializedObject.FindProperty("BetaHigh");
            Gamma = serializedObject.FindProperty("Gamma");

            OnDevicesAvailable = serializedObject.FindProperty("OnDevicesAvailable");
            OnDeviceStateChanged = serializedObject.FindProperty("OnDeviceStateChanged");
            OnPipelineStateChanged = serializedObject.FindProperty("OnPipelineStateChanged");
            OnRuntimeExceptionOccured = serializedObject.FindProperty("OnRuntimeExceptionOccured");
            OnBandpowerAvailable = serializedObject.FindProperty("OnBandpowerAvailable");
            OnMeanBandpowerAvailable = serializedObject.FindProperty("OnMeanBandpowerAvailable");
            OnRatiosAvailable = serializedObject.FindProperty("OnRatiosAvailable");
            OnMeanRatiosAvailable = serializedObject.FindProperty("OnMeanRatiosAvailable");
            OnSignalQualityAvailable = serializedObject.FindProperty("OnSignalQualityAvailable");
            OnBatteryLevelAvailable = serializedObject.FindProperty("OnBatteryLevelAvailable");
            OnDataLost = serializedObject.FindProperty("OnDataLost");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            float imageWidth = (float)Math.Round(EditorGUIUtility.currentViewWidth / 10, MidpointRounding.AwayFromZero) * 10.0f;
            float imageHeight = (float)Math.Round(imageWidth * banner.height / banner.width / 10, MidpointRounding.AwayFromZero) * 10.0f; ;
            Rect rect = GUILayoutUtility.GetRect(imageWidth, imageHeight);
            GUI.DrawTexture(rect, banner, ScaleMode.ScaleToFit);
            GUIStyle s = GUI.skin.GetStyle("HelpBox");
            s.fontSize = 12;
            s.fontStyle = FontStyle.Bold;
            s.padding = new RectOffset(10, 10, 10, 10);
            
            EditorGUILayout.HelpBox(string.Format("The 'device' prefab is designed to interact with your physical BCI device. It manages the device state and interfaces connected signal processing pipelines.\n\nVersion: {0}", Gtec.Bandpower.Utilities.Version), MessageType.None);
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(Type);
            if ((Gtec.Bandpower.Device.DeviceType)Type.enumValueIndex == Gtec.Bandpower.Device.DeviceType.AllDevices || (Gtec.Bandpower.Device.DeviceType)Type.enumValueIndex == Gtec.Bandpower.Device.DeviceType.Simulator)
            {
                EditorGUILayout.PropertyField(Signal);
                if ((Gtec.Bandpower.Device.SimulatorSignal)Signal.enumValueIndex == Gtec.Bandpower.Device.SimulatorSignal.GoodEEG)
                    EditorGUILayout.PropertyField(SimulatorAlphaLevelUv);
            }

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(AdvancedSettings);
            if(AdvancedSettings.boolValue)
            {
                EditorGUILayout.PropertyField(BuffersizeInSamples);
                EditorGUILayout.PropertyField(BufferOverlapInSamples);
                EditorGUILayout.PropertyField(Delta);
                EditorGUILayout.PropertyField(Theta);
                EditorGUILayout.PropertyField(Alpha);
                EditorGUILayout.PropertyField(BetaLow);
                EditorGUILayout.PropertyField(BetaMid);
                EditorGUILayout.PropertyField(BetaHigh);
                EditorGUILayout.PropertyField(Gamma);
            }

            EditorGUILayout.PropertyField(OnDevicesAvailable);
            EditorGUILayout.PropertyField(OnDeviceStateChanged);
            EditorGUILayout.PropertyField(OnPipelineStateChanged);
            EditorGUILayout.PropertyField(OnRuntimeExceptionOccured);
            EditorGUILayout.PropertyField(OnBandpowerAvailable);
            EditorGUILayout.PropertyField(OnMeanBandpowerAvailable);
            EditorGUILayout.PropertyField(OnRatiosAvailable);
            EditorGUILayout.PropertyField(OnMeanRatiosAvailable);
            EditorGUILayout.PropertyField(OnSignalQualityAvailable);
            EditorGUILayout.PropertyField(OnBatteryLevelAvailable);
            EditorGUILayout.PropertyField(OnDataLost);

            serializedObject.ApplyModifiedProperties();
        }
    }
}