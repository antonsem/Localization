using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityPickers;

namespace Localization
{
    [CustomEditor(typeof(StringLocalizer))]
    public class StringLocalizerEditor : UnityEditor.Editor
    {
        private IEnumerable<Enum> _displayOrder;
        private Type _type;
        private FieldInfo _fieldInfo;
        private FieldInfo _text;
        private GUIStyle _labelStyle;
        private bool _stylesInitialized = false;
        
        private void OnEnable()
        {
            _type = typeof(StringLocalizer);
            _text = _type.GetField("textField", BindingFlags.NonPublic | BindingFlags.Instance);
            _fieldInfo = _type.GetField("defaultString", BindingFlags.NonPublic | BindingFlags.Instance);
            _displayOrder = GetValues(typeof(Translation));
        }

        private void SetStyles()
        {
            _labelStyle = new GUIStyle(GUI.skin.label) {fixedWidth = 75, stretchWidth = false};
            _stylesInitialized = true;
        }

        public override void OnInspectorGUI()
        {
            if (_text.GetValue(target) == null)
            {
                EditorGUILayout.HelpBox(
                    "The TMPro.TextMeshProUGUI is not set! This component requires it to function...",
                    MessageType.Error);
                if (!GUILayout.Button("Set Text component")) return;

                MethodInfo dynMethod = _type.GetMethod("Reset",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                dynMethod.Invoke(target, null);
                _text = _type.GetField("textField", BindingFlags.NonPublic | BindingFlags.Instance);

                return;
            }
            
            if(!_stylesInitialized) SetStyles();
            
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Translation: ", _labelStyle);
            EnumPicker.Button(_fieldInfo.GetValue(target).ToString(), () => _displayOrder,
                val =>
                {
                    _fieldInfo.SetValue(target, val);
                });
            EditorGUILayout.EndHorizontal();
        }

        private static IEnumerable<Enum> GetValues(Type enumType)
        {
            return enumType.IsEnum
                ? Enum.GetValues(enumType).Cast<Enum>()
                : Enumerable.Empty<Enum>();
        }
    }
}