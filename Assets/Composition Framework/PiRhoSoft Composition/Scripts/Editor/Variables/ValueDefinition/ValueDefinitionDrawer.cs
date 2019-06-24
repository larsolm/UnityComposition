using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PiRhoSoft.CompositionEngine;
using PiRhoSoft.PargonUtilities.Editor;
using PiRhoSoft.UtilityEditor;
using PiRhoSoft.UtilityEngine;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.CompositionEditor
{
	public class ValueDefinitionControl : PropertyControl
	{
		#region Drawer Interface

		private SerializedProperty _typeProperty;
		private SerializedProperty _constraintProperty;
		private SerializedProperty _objectsProperty;
		private SerializedProperty _isTypeLockedProperty;
		private SerializedProperty _isConstraintLockedProperty;

		private VariableConstraint _constraint;
		private List<Object> _objects;

		public override void Setup(SerializedProperty property, FieldInfo fieldInfo, PropertyAttribute attribute)
		{
			_typeProperty = property.FindPropertyRelative("_type");
			_constraintProperty = property.FindPropertyRelative("_constraint");
			_objectsProperty = property.FindPropertyRelative("_objects");
			_isTypeLockedProperty = property.FindPropertyRelative("_isTypeLocked");
			_isConstraintLockedProperty = property.FindPropertyRelative("_isConstraintLocked");

			var data = _constraintProperty.stringValue;
			_objects = new List<Object>();

			for (var i = 0; i < _objectsProperty.arraySize; i++)
				_objects.Add(_objectsProperty.GetArrayElementAtIndex(i).objectReferenceValue);

			_constraint = VariableHandler.LoadConstraint(ref data, ref _objects);
		}

		public override void Draw(Rect position, SerializedProperty property, GUIContent label)
		{
			var expanded = property.isExpanded;
			var definition = ValueDefinition.Create((VariableType)_typeProperty.enumValueIndex, _constraint, null, null, _isTypeLockedProperty.boolValue, _isConstraintLockedProperty.boolValue);
			definition = Draw(position, label, definition, VariableInitializerType.None, null, true, ref expanded);

			_typeProperty.enumValueIndex = (int)definition.Type;
			_constraint = definition.Constraint;
			_constraintProperty.stringValue = _constraint != null ? VariableHandler.SaveConstraint(definition.Type, _constraint, ref _objects) : string.Empty;

			if (_objects != null)
			{
				_objectsProperty.arraySize = _objects.Count;

				for (var i = 0; i < _objects.Count; i++)
					_objectsProperty.GetArrayElementAtIndex(i).objectReferenceValue = _objects[i];
			}
			else
			{
				_objectsProperty.arraySize = 0;
			}

			property.isExpanded = expanded;
		}

		#endregion
	}

	[CustomPropertyDrawer(typeof(ValueDefinition))]
	public class ValueDefinitionDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var container = ElementHelper.CreatePropertyContainer(property.displayName, ElementHelper.GetTooltip(fieldInfo));
			
			var typeProperty = property.FindPropertyRelative("_type");
			var constraintProperty = property.FindPropertyRelative("_constraint");
			var objectsProperty = property.FindPropertyRelative("_objects");
			var isTypeLockedProperty = property.FindPropertyRelative("_isTypeLocked");
			var isConstraintLockedProperty = property.FindPropertyRelative("_isConstraintLocked");

			var data = constraintProperty.stringValue;
			var objects = new List<Object>();

			for (var i = 0; i < objectsProperty.arraySize; i++)
				objects.Add(objectsProperty.GetArrayElementAtIndex(i).objectReferenceValue);

			var constraint = VariableHandler.LoadConstraint(ref data, ref objects);
			var definition = ValueDefinition.Create((VariableType)typeProperty.enumValueIndex, constraint, null, null, isTypeLockedProperty.boolValue, isConstraintLockedProperty.boolValue);

			var definitionElement = new ValueDefinitionElement(property);
			definitionElement.Setup(definition, VariableInitializerType.None, null);

			return container;
		}
	}
}
