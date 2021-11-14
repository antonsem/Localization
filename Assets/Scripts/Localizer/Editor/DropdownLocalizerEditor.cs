using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityPickers;

namespace Localization
{
	[CustomEditor(typeof(DropdownLocalizer))]
	public class DropdownLocalizerEditor : Editor
	{
		private IEnumerable<Enum> _displayOrder;
		private List<string> _tempList;
		private Type _type;
		private FieldInfo _structFields;
		private FieldInfo _dropdown;
		private FieldInfo _label;
		private Array _translations;
		private GUIStyle _labelStyle;
		private bool _stylesInitialized = false;

		private void OnEnable()
		{
			_type = typeof(DropdownLocalizer);
			_dropdown = _type.GetField("dropdown", BindingFlags.NonPublic | BindingFlags.Instance);
			_label = _type.GetField("label", BindingFlags.NonPublic | BindingFlags.Instance);
			_structFields = _type.GetField("defaultStrings",
				BindingFlags.NonPublic | BindingFlags.Instance);
			_displayOrder = GetValues(typeof(Translation));
		}

		private void SetStyles()
		{
			_labelStyle = new GUIStyle(GUI.skin.label)
				{ fixedWidth = 20, stretchWidth = false, alignment = TextAnchor.MiddleRight };
			_stylesInitialized = true;
		}

		public override void OnInspectorGUI()
		{
			if (GUILayout.Button("Reset"))
			{
				MethodInfo dynMethod = _type.GetMethod("Reset", BindingFlags.NonPublic | BindingFlags.Instance);
				dynMethod.Invoke(target, null);
				_dropdown = _type.GetField("dropdown", BindingFlags.NonPublic | BindingFlags.Instance);
				_translations = null;
			}

			EditorGUILayout.Space(25);

			if (_dropdown.GetValue(target) == null)
			{
				EditorGUILayout.HelpBox(
					"The TMPro.TMP_Dropdown is not set! This component requires it to function...",
					MessageType.Error);
				return;
			}

			if (!_stylesInitialized)
				SetStyles();

			if (_translations == null || _translations.Length == 0)
				_translations = (Array)_structFields.GetValue(target);

			TextMeshProUGUI labelObj = _label.GetValue(target) as TextMeshProUGUI;
			if (!labelObj)
				EditorGUILayout.HelpBox("Label should be set! Localizer won't work otherwise...", MessageType.Error);

			EditorGUI.BeginChangeCheck();
			labelObj = EditorGUILayout.ObjectField("Label", labelObj, typeof(TextMeshProUGUI), true) as TextMeshProUGUI;
			if (EditorGUI.EndChangeCheck())
				_label.SetValue(target, labelObj);

			EditorGUILayout.Separator();

			int optionsCount = (_dropdown.GetValue(target) as TMP_Dropdown).options.Count;

			_tempList = new List<string>(_translations.Length);

			foreach (string translation in _translations)
				_tempList.Add(translation);

			bool hasChanged = false;

			if (optionsCount > _tempList.Count)
			{
				int count = _tempList.Count;
				for (int i = 0; i < optionsCount - count; i++)
					_tempList.Add(((Translation)0).ToString());

				hasChanged = true;
			}


			for (int i = 0; i < _tempList.Count; i++)
			{
				if (i >= optionsCount)
				{
					_tempList.RemoveAt(i);
					hasChanged = true;
					continue;
				}

				EditorGUILayout.BeginHorizontal();
				GUILayout.Label($"{(i + 1).ToString()}. ", _labelStyle);
				int i1 = i;
				EnumPicker.Button(_tempList[i], () => _displayOrder,
					val => {
						_tempList[i1] = ((Translation)val).ToString();
						UpdateValues(_tempList.ToArray());
					});

				EditorGUILayout.EndHorizontal();
			}

			if (!hasChanged) return;
			UpdateValues(_tempList.ToArray());
		}

		private void UpdateValues(IEnumerable list)
		{
			_structFields.SetValue(target, list);
			EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
			_translations = (Array)_structFields.GetValue(target);
			EditorUtility.SetDirty(target);
		}

		private static IEnumerable<Enum> GetValues(Type enumType)
		{
			return enumType.IsEnum
				? Enum.GetValues(enumType).Cast<Enum>()
				: Enumerable.Empty<Enum>();
		}
	}
}