using System;
using UnityEditor;
using UnityEngine;

namespace Localization
{
    public class ETSEditorWindow : EditorWindow
    {
        private static Action<int, ETS> _callback;
        private static ETS _ets;
        private static int _index;

        public static void ShowWindow(int index, ETS ets, Action<int, ETS> callback)
        {
            _index = index;
            _ets = ets;
            _callback = callback;

            GetWindow(typeof(ETSEditorWindow), true); //CreateInstance(typeof(ETSEditorWindow)) as ETSEditorWindow;
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ID");

            EditorGUI.BeginChangeCheck();
            string newKey = EditorGUILayout.TextField(_ets.Key);
            if (EditorGUI.EndChangeCheck())
                _ets.SetKey(newKey);
            EditorGUILayout.EndHorizontal();
            if (_ets.IsKeyEdited && GUILayout.Button("Reset ID"))
                _ets.ResetKey();

            for (int i = 0; i < _ets.Languages.Count; i++)
            {
                EditorGUILayout.LabelField(_ets.Languages[i]);
                EditorGUI.BeginChangeCheck();
                string newTranslation = EditorGUILayout.TextArea(_ets.Translations[i]);
                if (EditorGUI.EndChangeCheck())
                {
                    _ets.Set(_ets.Languages[i], newTranslation);
                }

                if (_ets.IsEdited[i] && GUILayout.Button($"Reset {_ets.Languages[i]} translation"))
                    _ets.ResetTranslation(_ets.Languages[i]);
            }

            EditorGUILayout.EndVertical();

            if (GUILayout.Button("Delete"))
            {
                _ets.IsDeleted = true;
                _callback(_index, _ets);
                Close();
                return;
            }

            if (!GUILayout.Button("Save")) return;
            _callback(_index, _ets);
            Close();
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }
    }
}