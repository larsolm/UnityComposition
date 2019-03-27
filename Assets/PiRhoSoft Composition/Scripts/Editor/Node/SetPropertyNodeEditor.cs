using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEditor;
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.CompositionEditor
{
	[CustomEditor(typeof(SetPropertyNode))]
	class SetPropertyNodeEditor : Editor
	{
		private static readonly GUIContent _targetTypeContent = new GUIContent("Component Type", "The Type of the component to set the property for");
		private static readonly GUIContent _propertyContent = new GUIContent("Property", "The name of the property to set");

		private SetPropertyNode _node;
		private string[] _propertyNames;
		private Type[] _propertyTypes;
		private SerializedProperty _nameProperty;
		private SerializedProperty _nextProperty;
		private SerializedProperty _targetProperty;
		private SerializedProperty _valueProperty;

		void OnEnable()
		{
			_node = target as SetPropertyNode;
			_nameProperty = serializedObject.FindProperty(nameof(SetPropertyNode.Name));
			_nextProperty = serializedObject.FindProperty(nameof(SetPropertyNode.Next));
			_targetProperty = serializedObject.FindProperty(nameof(SetPropertyNode.Target));
			_valueProperty = serializedObject.FindProperty(nameof(SetPropertyNode.Value));

			BuildPropertyList();
		}

		private void BuildPropertyList()
		{
			if (_node.TargetType != null)
			{
				var fields = _node.TargetType.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Where(field => VariableValue.GetType(field.FieldType) != VariableType.Empty).ToArray();

				var properties = _node.TargetType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Where(property => property.SetMethod != null)
					.Where(property => VariableValue.GetType(property.PropertyType) != VariableType.Empty)
					.Where(property => property.GetCustomAttribute<ObsoleteAttribute>() == null).ToArray();

				_propertyNames = new string[fields.Length + properties.Length];
				_propertyTypes = new Type[fields.Length + properties.Length];

				var index = 0;

				foreach (var field in fields)
				{
					_propertyNames[index] = field.Name;
					_propertyTypes[index++] = field.FieldType;
				}

				foreach (var property in properties)
				{
					_propertyNames[index] = property.Name;
					_propertyTypes[index++] = property.PropertyType;
				}
			}
			else
			{
				_propertyNames = null;
				_propertyTypes = null;
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

			using (new UndoScope(_node, false))
			{
				var selectedTargetType = TypePopupDrawer.Draw<Component>(_targetTypeContent, _node.TargetType, false);
				if (selectedTargetType != _node.TargetType)
					SetTargetType(selectedTargetType);

				if (_node.TargetType != null)
				{
					var property = Array.IndexOf(_propertyNames, _node.PropertyName);
					var selectedProperty = EditorGUILayout.Popup(_propertyContent, property, _propertyNames);

					if (selectedProperty != property)
						SetProperty(_propertyNames[selectedProperty], _propertyTypes[selectedProperty]);
				}
			}

			if (_node.TargetType != null && !string.IsNullOrEmpty(_node.PropertyName))
			{
				using (new UndoScope(serializedObject))
					EditorGUILayout.PropertyField(_valueProperty);
			}
		}

		private void SetTargetType(Type type)
		{
			_node.TargetType = type;
			_node.PropertyName = null;
			_node.Field = null;
			_node.Property = null;

			BuildPropertyList();
		}

		private void SetProperty(string name, Type type)
		{
			_node.PropertyName = name;
			_node.Field = _node.TargetType.GetField(name);
			_node.Property = _node.TargetType.GetProperty(name);

			var variableType = VariableValue.GetType(type);
			var definition = variableType == VariableType.Object ? VariableDefinition.Create(string.Empty, type) : VariableDefinition.Create(string.Empty, variableType);

			_node.Value = new VariableValueSource(variableType, definition);
		}
	}
}
