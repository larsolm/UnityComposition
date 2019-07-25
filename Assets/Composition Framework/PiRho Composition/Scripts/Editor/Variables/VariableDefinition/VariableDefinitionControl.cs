using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class VariableDefinitionControl : VisualElement
	{
		public VariableDefinition Value { get; private set; }

		public VariableDefinitionControl(VariableDefinition value, VariableInitializerType initializer, TagList tags, bool showConstraintLabel)
		{
			Value = Value;

			Refresh();
		}

		public void SetValueWithoutNotify(VariableDefinition value)
		{
			Value = value;
			Refresh();
		}

		private void Refresh()
		{
		}
	}
}
