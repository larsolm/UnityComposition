using PiRhoSoft.Utilities.Editor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class ExpressionField : BaseField<string>
	{
		public static readonly string UssClassName = "pirho-expression-field";
		public static readonly string LabelUssClassName = UssClassName + "__label";
		public static readonly string InputUssClassName = UssClassName + "__input";

		private ExpressionControl _control;

		public ExpressionField(string label, Expression value, IAutocompleteItem autocomplete) : base(label, null)
		{
			Setup(value, autocomplete);
		}

		private void Setup(Expression value, IAutocompleteItem autocomplete)
		{
			_control = new ExpressionControl(value, autocomplete);
			_control.AddToClassList(InputUssClassName);
			_control.RegisterCallback<ChangeEvent<string>>(evt => base.value = evt.newValue);

			labelElement.AddToClassList(LabelUssClassName);

			this.SetVisualInput(_control);
			AddToClassList(UssClassName);
			SetValueWithoutNotify(value.Statement);
		}

		public override void SetValueWithoutNotify(string newValue)
		{
			base.SetValueWithoutNotify(newValue);
			_control.SetValueWithoutNotify(newValue);
		}

		protected override void ExecuteDefaultActionAtTarget(EventBase evt)
		{
			base.ExecuteDefaultActionAtTarget(evt);

			if (this.TryGetPropertyBindEvent(evt, out var property))
			{
				var textProperty = property.FindPropertyRelative(Expression.StatementField);
				if (textProperty != null)
					bindingPath = textProperty.propertyPath;
			}
		}
	}
}
