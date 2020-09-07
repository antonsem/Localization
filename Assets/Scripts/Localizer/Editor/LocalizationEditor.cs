using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Localization
{
    public class LocalizationEditor : EditorWindow
    {
        [Flags]
        private enum FilterOptions
        {
            Edited = 1,
            Deleted = 2,
            New = 4
        }
        
        private DataHolder _data;
        private const string _defaultLanguage = "New Language";
        private static GUIStyle _buttonStyle;
        private static GUIStyle _closeButtonStyle;
        private static GUIStyle _textStyle;
        private static GUIStyle _searchFieldStyle;
        private static GUIStyle _searchLabelStyle;
        private static GUIStyle _readOnlyFieldStyle;
        private static GUIStyle _dropdownStyle;
        private static GUIStyleState _normalState;
        private const float _fixedHeight = 25;
        private static Vector2 _scrollPos;
        private static string _searchParameters = "";
        private static bool _stylesInitialized = false;
        private static int _filter = 0;
        private static string[] _filters;
        private static FilterOptions _modificationFilter = 0;

        [MenuItem("Localization/Show Localization")]
        public static void ShowWindow()
        {
            GetWindow(typeof(LocalizationEditor), false, "Localization");
        }

        private static void SetStyles()
        {
            _normalState = new GUIStyleState {background = new Texture2D(1, 1)};
            _buttonStyle = new GUIStyle(GUI.skin.button) {fixedWidth = _fixedHeight, fixedHeight = _fixedHeight};
            _closeButtonStyle = new GUIStyle(_buttonStyle) {fixedWidth = 100};
            _readOnlyFieldStyle = new GUIStyle(GUI.skin.textField)
            {
                fixedHeight = _fixedHeight,
                alignment = TextAnchor.UpperLeft,
                contentOffset = new Vector2(0, 4),
                stretchWidth = true,
                normal = _normalState
            };
            _textStyle = new GUIStyle(GUI.skin.textField)
            {
                fixedHeight = _fixedHeight,
                alignment = TextAnchor.MiddleLeft, stretchWidth = true
            };
            _searchFieldStyle = new GUIStyle(_textStyle);
            _searchLabelStyle = new GUIStyle(GUI.skin.label)
                {fixedHeight = _fixedHeight, fixedWidth = 50, stretchWidth = false};
            _dropdownStyle = new GUIStyle(EditorStyles.popup)
                {fixedHeight = _fixedHeight, fixedWidth = 150, stretchWidth = true};
            _stylesInitialized = true;
        }

        private void InitializeFilters()
        {
            _filters = new string[_data.Languages.Count + 1];
            _filters[0] = "ID";
            for (int i = 0; i < _data.Languages.Count; i++)
                _filters[i + 1] = _data.Languages[i];
        }

        private void OnGUI()
        {
            if (_data == null)
            {
                if (GUILayout.Button("Load localization file"))
                    OnLoadData();
                return;
            }

            if (!_stylesInitialized)
                SetStyles();
            if (_filters == null)
                InitializeFilters();

            EditorGUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Search", _searchLabelStyle);
            _searchParameters = EditorGUILayout.TextField(_searchParameters, _searchFieldStyle).ToLower();
            EditorGUI.BeginChangeCheck();
            _filter = EditorGUILayout.Popup(_filter, _filters, _dropdownStyle);
            if (EditorGUI.EndChangeCheck())
                _searchParameters = "";

            _modificationFilter = (FilterOptions) EditorGUILayout.EnumFlagsField(_modificationFilter, _dropdownStyle);

            if (GUILayout.Button("Close", _closeButtonStyle))
            {
                _data = null;
                return;
            }

            GUILayout.EndHorizontal();

            _scrollPos =
                EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.Width(position.width),
                    GUILayout.Height(position.height - _fixedHeight * 2 - 5));
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(3 * (_buttonStyle.fixedWidth + 4.5f));
            EditorGUILayout.LabelField("ID");
            GUILayout.Space(_buttonStyle.fixedWidth);
            for (int i = 0; i < _data.Languages.Count; i++)
            {
                EditorGUI.BeginChangeCheck();
                string newLanguage = EditorGUILayout.TextField(_data.Languages[i], _textStyle);
                if (GUILayout.Button("X", _buttonStyle))
                {
                    _data.RemoveLanguage(_data.Languages[i]);
                    continue;
                }

                if (!EditorGUI.EndChangeCheck()) continue;
                _data.RenameLanguage(_data.Languages[i], newLanguage);
                InitializeFilters();
            }

            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < _data.TranslationStrings.Count; i++)
            {
                ETS ets = _data.TranslationStrings[i];

                if (IsFiltered(ets)) continue;

                bool hasChanged = false;
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("+", _buttonStyle))
                    OnAddString(i + 1);

                if (GUILayout.Button("R", _buttonStyle))
                {
                    hasChanged = true;
                    ets.ResetAll();
                    _data.HasNonUniqueKeys();
                }

                if (GUILayout.Button("E", _buttonStyle))
                    OnEdit(i);

                if (ets.IsDeleted) GUI.color = Color.grey;
                else if (ets.IsNew) GUI.color = Color.yellow;
                else if (!ets.IsKeyUnique) GUI.color = Color.red;

                if (!ets.IsNew && !ets.IsDeleted && ets.IsKeyUnique && ets.IsKeyEdited)
                    GUI.color = Color.cyan;

                EditorGUILayout.LabelField(ets.Key, _readOnlyFieldStyle);

                for (int j = 0; j < _data.Languages.Count; j++)
                {
                    if (!ets.IsNew && !ets.IsDeleted)
                        GUI.color = ets.IsEdited[j] ? Color.cyan : Color.white;

                    EditorGUILayout.LabelField(ets.Get(_data.Languages[j]), _readOnlyFieldStyle);
                }

                GUI.color = Color.white;

                EditorGUILayout.EndHorizontal();
                if (hasChanged) _data.TranslationStrings[i] = ets;
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Save file"))
                OnSave();

            if (GUILayout.Button("Generate Translations.cs"))
                LocalizationStringGenerator.Generate();

            if (GUILayout.Button("+ New language"))
                OnAddLanguage();
            EditorGUILayout.EndHorizontal();
        }

        private bool IsFiltered(in ETS ets)
        {
            if (_filter == 0 && !ets.Key.ToLower().Contains(_searchParameters))
                return true;
            if (_filter > 0 && !ets.Get(_data.Languages[_filter - 1]).ToLower().Contains(_searchParameters))
                return true;
            //if (_modificationFilter <= 0) return false;
            bool isEdited = ets.IsKeyEdited || ets.IsEdited.Contains(true);
            int filter = (int) _modificationFilter;
            switch (filter)
            {
                case 0:
                case 1 when isEdited:
                case 2 when ets.IsDeleted:
                case 3 when isEdited || ets.IsDeleted:
                case 4 when ets.IsNew:
                case 5 when isEdited || ets.IsNew:
                case 6 when ets.IsDeleted || ets.IsNew:
                case 7 when isEdited || ets.IsDeleted || ets.IsNew:
                case -1 when isEdited || ets.IsDeleted || ets.IsNew:
                    return false;
            }

            return true;
        }

        private void OnAddLanguage()
        {
            _data.AddLanguage(_defaultLanguage);
            InitializeFilters();
        }

        private void OnAddString(int index)
        {
            _data.TranslationStrings.Insert(index, new ETS("default_id", new List<string>(_data.Languages),
                new List<string>(_data.Languages), true));
        }

        private void OnLoadData()
        {
            _data = new DataHolder($"{Application.streamingAssetsPath}/Localization.csv");
            if (_data == null)
                Debug.LogError("Couldn't load the localization.csv file!");

            if (_data.HasNonUniqueKeys())
            {
                EditorUtility.DisplayDialog("Duplicate keys detected!",
                    "Translations have duplicate ids! Ids should be unique!", "Sorry...");
            }
        }

        private void OnSave()
        {
            if (_data.HasNonUniqueKeys())
            {
                EditorUtility.DisplayDialog("Duplicate keys detected!",
                    "Translations have duplicate ids! Ids should be unique!", "Sorry...");
                return;
            }

            string savePath =
                EditorUtility.OpenFolderPanel("Save translations to", Application.streamingAssetsPath, "Localization");

            if (string.IsNullOrEmpty(savePath)) return;

            string csv = "id";
            foreach (string language in _data.Languages)
                csv = $"{csv};{language}";

            csv = $"{csv}\n";

            foreach (ETS ets in _data.TranslationStrings)
            {
                if (ets.IsDeleted) continue;
                for (int i = 0; i < ets.Translations.Count; i++)
                {
                    string val = ets.Translations[i];
                    for (int j = val.Length - 1; j >= 0; j--)
                    {
                        if (val[j] == ';')
                            val = val.Insert(j, "\\");
                    }
                    ets.Translations[i] = val;
                }
                csv = $"{csv}{ets.Key};{string.Join(";", ets.Translations)}\n";
            }

            File.WriteAllText($"{savePath}/Temp_Localization.csv", csv);
            File.Replace($"{savePath}/Temp_Localization.csv", $"{savePath}/Localization.csv",
                $"{savePath}/Localization_backup.csv");

            OnLoadData();
        }

        private void OnEdit(int index)
        {
            ETSEditorWindow.ShowWindow(index, _data.TranslationStrings[index], Edited);
        }

        private void Edited(int index, ETS ets)
        {
            if (ets.IsNew && ets.IsDeleted)
            {
                _data.TranslationStrings.RemoveAt(index);
                return;
            }

            _data.TranslationStrings[index] = ets;

            if (_data.HasNonUniqueKeys())
            {
                EditorUtility.DisplayDialog("Duplicate keys detected!",
                    "Translations have duplicate ids! Ids should be unique!", "Sorry...");
            }
        }

    }
}