using UnityEditor;
using UnityEngine;

namespace Gtec.Bandpower
{
    [CustomPropertyDrawer(typeof(FrequencyBandAttribute))]
    public class FrequencyBand : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Draw the property label (name of the field)
            position = EditorGUI.PrefixLabel(position, label);

            // Get vector components
            SerializedProperty xProp = property.FindPropertyRelative("x");
            SerializedProperty yProp = property.FindPropertyRelative("y");

            // Define layout for fields
            float labelWidth = 30;
            float fieldWidth = (position.width - labelWidth * 2 - 10) / 2;

            Rect lowLabelRect = new Rect(position.x, position.y, labelWidth, position.height);
            Rect lowFieldRect = new Rect(position.x + labelWidth, position.y, fieldWidth, position.height);

            Rect highLabelRect = new Rect(position.x + labelWidth + fieldWidth + 5, position.y, labelWidth, position.height);
            Rect highFieldRect = new Rect(position.x + labelWidth + fieldWidth + labelWidth + 5, position.y, fieldWidth, position.height);

            // Draw labels and fields
            EditorGUI.LabelField(lowLabelRect, "Low");
            xProp.floatValue = EditorGUI.FloatField(lowFieldRect, xProp.floatValue);

            EditorGUI.LabelField(highLabelRect, "High");
            yProp.floatValue = EditorGUI.FloatField(highFieldRect, yProp.floatValue);

            EditorGUI.EndProperty();
        }
    }
}