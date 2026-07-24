using UnityEngine;
using UnityEditor;
using GyeNyame.Core.Attributes;
using System.Collections.Generic;

namespace GyeNyame.Core.Editor
{
    [CustomPropertyDrawer(typeof(TagSelectorAttribute))]
    public class TagSelectorPropertyDrawer : PropertyDrawer
    {
        private const string EmptyString = "";
        private const int InvalidTagIndex = -1;
        private const int DefaultTagIndex = 0;
        private const int MinimumValidCustomTagIndex = 1;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            EditorGUI.BeginProperty(position, label, property);
            DrawProperty(position, property, label);
            EditorGUI.EndProperty();
        }

        private void DrawProperty(Rect position, SerializedProperty property, GUIContent label)
        {
            var tagSelectorAttribute = attribute as TagSelectorAttribute;
            
            if (tagSelectorAttribute != null && tagSelectorAttribute.UseDefaultTagFieldDrawer)
            {
                property.stringValue = EditorGUI.TagField(position, label, property.stringValue);
                return;
            }

            DrawCustomTagSelector(position, property, label);
        }

        private void DrawCustomTagSelector(Rect position, SerializedProperty property, GUIContent label)
        {
            List<string> availableTags = new List<string>();
            availableTags.AddRange(UnityEditorInternal.InternalEditorUtility.tags);
            
            int currentIndex = GetCurrentTagIndex(availableTags, property.stringValue);
            int newSelectedIndex = EditorGUI.Popup(position, label.text, currentIndex, availableTags.ToArray());
            
            ApplySelectedTag(property, availableTags, newSelectedIndex);
        }

        private int GetCurrentTagIndex(List<string> availableTags, string currentPropertyValue)
        {
            if (currentPropertyValue == EmptyString) return DefaultTagIndex;

            for (int i = MinimumValidCustomTagIndex; i < availableTags.Count; i++)
            {
                if (availableTags[i] == currentPropertyValue) return i;
            }

            return InvalidTagIndex;
        }

        private void ApplySelectedTag(SerializedProperty property, List<string> availableTags, int selectedIndex)
        {
            if (selectedIndex >= MinimumValidCustomTagIndex)
            {
                property.stringValue = availableTags[selectedIndex];
                return;
            }

            property.stringValue = EmptyString;
        }
    }
}
