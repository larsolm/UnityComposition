﻿using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEditor;
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.CompositionEditor
{
	[CustomEditor(typeof(GetPropertyNode))]
	class GetPropertyNodeEditor : Editor
	{
		private static readonly GUIContent _targetTypeContent = new GUIContent("Object Type", "The Type of object to get the property of");
		private static readonly GUIContent _propertyContent = new GUIContent("Property", "The property to get the value of");

		private GetPropertyNode _node;
		private string[] _propertyNames;
		private string[] _propertyDisplays;
		private SerializedProperty _nameProperty;
		private SerializedProperty _nextProperty;
		private SerializedProperty _targetProperty;
		private SerializedProperty _outputProperty;

		void OnEnable()
		{
			_node = target as GetPropertyNode;
			_nameProperty = serializedObject.FindProperty(nameof(GetPropertyNode.Name));
			_nextProperty = serializedObject.FindProperty(nameof(GetPropertyNode.Next));
			_targetProperty = serializedObject.FindProperty(nameof(GetPropertyNode.Target));
			_outputProperty = serializedObject.FindProperty(nameof(GetPropertyNode.Output));

			BuildPropertyList();
		}

		private void BuildPropertyList()
		{
			if (_node.TargetType != null)
			{
				var fields = _node.TargetType.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Where(field => VariableValue.GetType(field.FieldType) != VariableType.Empty).ToArray(); // TODO: Filter this list by other unwanted types

				var properties = _node.TargetType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Where(property => property.GetMethod != null)
					.Where(property => VariableValue.GetType(property.PropertyType) != VariableType.Empty)
					.Where(property => property.GetCustomAttribute<ObsoleteAttribute>() == null).ToArray();

				_propertyNames = new string[fields.Length + properties.Length];
				_propertyDisplays = new string[fields.Length + properties.Length];

				var index = 0;

				foreach (var field in fields)
				{
					_propertyNames[index] = field.Name;
					_propertyDisplays[index++] = string.Format("{0} {1}", field.FieldType.Name, field.Name);
				}

				foreach (var property in properties)
				{
					_propertyNames[index] = property.Name;
					_propertyDisplays[index++] = string.Format("{0} {1}", property.PropertyType.Name, property.Name);
				}
			}
			else
			{
				_propertyNames = null;
			}
		}

		public override void OnInspectorGUI()
		{
			using (new UndoScope(serializedObject))
			{
				EditorGUILayout.PropertyField(_nameProperty);
				EditorGUILayout.PropertyField(_nextProperty);
				EditorGUILayout.PropertyField(_targetProperty);
			}

			var type = TypeDisplayDrawer.Draw<Object>(_targetTypeContent, _node.TargetType, false, true);

			if (type != _node.TargetType)
			{
				using (new UndoScope(_node, false))
				{
					_node.TargetType = type;
					_node.Field = null;
					_node.Property = null;

					BuildPropertyList();
				}
			}

			if (_node.TargetType != null)
			{
				using (new UndoScope(_node, false))
				{
					var property = _node.Property != null ? Array.IndexOf(_propertyNames, _node.Property.Name) : -1;
					var selectedProperty = EditorGUILayout.Popup(_propertyContent, property, _propertyDisplays);

					if (selectedProperty != property)
						SetProperty(_propertyNames[selectedProperty]);
				}

				if (_node.Property != null)
				{
					using (new UndoScope(serializedObject))
						EditorGUILayout.PropertyField(_outputProperty);
				}
			}
		}

		private void SetProperty(string name)
		{
			_node.Field = _node.TargetType.GetField(name);
			_node.Property = _node.TargetType.GetProperty(name);
		}
	}
}
