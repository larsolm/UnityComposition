using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEditor;
using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace PiRhoSoft.CompositionEditor
{
	[CustomEditor(typeof(SetPropertyNode))]
	class SetPropertyNodeEditor : Editor
	{
		private static readonly Label _nextContent = new Label(typeof(SetPropertyNode), nameof(SetPropertyNode.Next));
		private static readonly Label _targetContent = new Label(typeof(SetPropertyNode), nameof(SetPropertyNode.Target));
		private static readonly Label _componentTypeContent = new Label(typeof(SetPropertyNode), nameof(SetPropertyNode.ComponentType));
		private static readonly Label _propertyTypeContent = new Label(typeof(SetPropertyNode), nameof(SetPropertyNode.PropertyType));
		private static readonly Label _valueContent = new Label(typeof(SetPropertyNode), nameof(SetPropertyNode.Value));
		private static readonly Label _valueReferenceContent = new Label(typeof(SetPropertyNode), nameof(SetPropertyNode.ValueReference));
		private static readonly Label _sourceTypeContent = new Label(typeof(SetPropertyNode), nameof(SetPropertyNode.SourceType));
		private static readonly List<Type> _componentTypes;

		private SetPropertyNode _node;
		private string[] _propertyNames;
		private Type[] _propertyTypes;
		private VariableType _variableType;

		static SetPropertyNodeEditor()
		{
			_componentTypes = TypeHelper.ListDerivedTypes<Component>();
		}

		void OnEnable()
		{
			_node = target as SetPropertyNode;

			BuildPropertyList();
		}

		private void BuildPropertyList()
		{
			if (_node.ComponentType != null)
			{
				var fields = _node.ComponentType.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
				var properties = _node.ComponentType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(property => property.SetMethod != null && property.GetCustomAttribute<ObsoleteAttribute>() == null).ToArray();

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
		}

		public override void OnInspectorGUI()
		{
			using (new UndoScope(_node, false))
			{
				InstructionGraphNodeDrawer.Draw(_nextContent.Content, _node.Next);
				VariableReferenceControl.Draw(_targetContent.Content, _node.Target);

				var selectedComponentType = TypePopupDrawer.Draw<Component>(_componentTypeContent.Content, _node.ComponentType);
				if (selectedComponentType != _node.ComponentType)
					SetComponentType(selectedComponentType);

				if (_node.ComponentType != null)
				{
					var propertyType = Array.IndexOf(_propertyNames, _node.PropertyName);
					var selectedPropertyType = EditorGUILayout.Popup(_propertyTypeContent.Content, propertyType, _propertyNames);

					if (selectedPropertyType != propertyType)
						SetPropertyType(_propertyNames[selectedPropertyType], _propertyTypes[selectedPropertyType]);

					if (_node.PropertyType != null)
					{
						if (_variableType == VariableType.Raw)
							_node.SourceType = VariableSourceType.Reference;
						else
							_node.SourceType = (VariableSourceType)EnumButtonsDrawer.Draw(_sourceTypeContent.Content, (int)_node.SourceType, typeof(VariableSourceType), 2, 50);

						if (_node.SourceType == VariableSourceType.Reference)
							VariableReferenceControl.Draw(_valueReferenceContent.Content, _node.ValueReference);
						else if (_node.SourceType == VariableSourceType.Value) // special case VariableType.Object here so we get a constrained object picker
							_node.Value = VariableValueDrawer.Draw(_valueContent.Content, _node.Value, _variableType == VariableType.Object ? VariableDefinition.Create("", _node.PropertyType) : VariableDefinition.Create("", _variableType));
					}
				}
			}
		}

		private void SetComponentType(Type type)
		{
			_node.ComponentType = type;
			_node.PropertyType = null;
			_node.PropertyName = null;
			_node.Field = null;
			_node.Property = null;
			_node.Value = VariableValue.Empty;

			BuildPropertyList();
		}

		private void SetPropertyType(string name, Type type)
		{
			_node.PropertyType = type;
			_node.PropertyName = name;
			_node.Field = _node.ComponentType.GetField(name);
			_node.Property = _node.ComponentType.GetProperty(name);
			_node.Value = VariableValue.Empty;
			_variableType = VariableValue.GetType(type);
		}
	}
}
