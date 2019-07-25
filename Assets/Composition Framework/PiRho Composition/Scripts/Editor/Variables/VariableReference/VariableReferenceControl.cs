using PiRhoSoft.Utilities;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class VariableReferenceControl : VisualElement
	{
		public VariableReference Value { get; private set; }
		public AutocompleteSource Source { get; private set; }

		private TextField _text;

		public VariableReferenceControl(VariableReference value, AutocompleteSource source)
		{
			Value = value;
			Source = source;

			_text = new TextField { value = Value.Variable };
			_text.RegisterValueChangedCallback(evt =>
			{
				Value.Variable = evt.newValue;
			});
		}

		public void SetValueWithoutNotify(VariableReference value)
		{
			Value = value;
			_text.SetValueWithoutNotify(value.Variable);
		}
	}
}
