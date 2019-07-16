using PiRhoSoft.Composition;
using PiRhoSoft.Utilities.Editor;
using PiRhoSoft.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class VariableReferenceElement : TextField
	{
		private Object _owner;
		private VariableReference _reference;
		private AutocompleteSource _source;

		public void Setup(SerializedProperty property, AutocompleteSource source)
		{
			var reference = PropertyHelper.GetObject<VariableReference>(property);
			Setup(property.serializedObject.targetObject, reference, source);
		}

		public void Setup(Object owner, VariableReference reference, AutocompleteSource source)
		{
			_owner = owner;
			_reference = reference;
			_source = source;

			//ChangeHelper.Bind(this, _owner, () => text, () => _reference.Variable, value => SetValueWithoutNotify(value), value => _reference.Variable = value);

			style.flexGrow = 1;
		}
	}

	[CustomPropertyDrawer(typeof(VariableReference))]
	public class VariableReferenceDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var source = property.serializedObject.targetObject as IAutocompleteSource;
			var container = ElementHelper.CreatePropertyContainer(property.displayName, ElementHelper.GetTooltip(fieldInfo));
			var element = new VariableReferenceElement();

			element.Setup(property, source.AutocompleteSource);
			container.Add(element);

			return container;
		}
	}
}
