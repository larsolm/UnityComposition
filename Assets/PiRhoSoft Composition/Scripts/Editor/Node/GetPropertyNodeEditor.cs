using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEditor;
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.CompositionEditor
{
	[CustomEditor(typeof(GetPropertyNode))]
	class GetPropertyNodeEditor : Editor
	{
		private static readonly Label _nextContent = new Label(typeof(GetPropertyNode), nameof(GetPropertyNode.Next));
		private static readonly Label _targetTypeContent = new Label(typeof(GetPropertyNode), nameof(GetPropertyNode.TargetTypeName));
		private static readonly Label _propertyContent = new Label(typeof(GetPropertyNode), nameof(GetPropertyNode.PropertyName));
		private static readonly Label _outputContent = new Label(typeof(GetPropertyNode), nameof(GetPropertyNode.Output));

		private GetPropertyNode _node;
		private string[] _propertyNames;
		private Type[] _propertyTypes;
		private SerializedProperty _targetProperty;

		void OnEnable()
		{
			_node = target as GetPropertyNode;
			_targetProperty = serializedObject.FindProperty(nameof(GetPropertyNode.Target));

			BuildPropertyList();
		}

		private void BuildPropertyList()
		{
			if (_node.TargetType != null)
			{
				var fields = _node.TargetType.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Where(field => VariableValue.GetType(field.FieldType) != VariableType.Other).ToArray(); // TODO: Filter this list by other unwanted types

				var properties = _node.TargetType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Where(property => property.GetMethod != null)
					.Where(property => VariableValue.GetType(property.PropertyType) != VariableType.Other)
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
			using (new UndoScope(_node, false))
				InstructionGraphNodeDrawer.Draw(_nextContent.Content, _node.Next);

			using (new UndoScope(serializedObject))
				EditorGUILayout.PropertyField(_targetProperty);

			using (new UndoScope(_node, false))
			{
				var selectedTargetType = TypePopupDrawer.Draw<Component>(_targetTypeContent.Content, _node.TargetType, false);
				if (selectedTargetType != _node.TargetType)
					SetTargetType(selectedTargetType);

				if (_node.TargetType != null)
				{
					var property = Array.IndexOf(_propertyNames, _node.PropertyName);
					var selectedProperty = EditorGUILayout.Popup(_propertyContent.Content, property, _propertyNames);

					if (selectedProperty != property)
						SetProperty(_propertyNames[selectedProperty], _propertyTypes[selectedProperty]);

					if (_node.PropertyType != null)
						VariableReferenceControl.Draw(_outputContent.Content, _node.Output);
				}
			}
		}

		private void SetTargetType(Type type)
		{
			_node.TargetType = type;
			_node.PropertyType = null;
			_node.PropertyName = null;
			_node.Field = null;
			_node.Property = null;

			BuildPropertyList();
		}

		private void SetProperty(string name, Type type)
		{
			_node.PropertyType = type;
			_node.PropertyName = name;
			_node.Field = _node.TargetType.GetField(name);
			_node.Property = _node.TargetType.GetProperty(name);
		}
	}
}
