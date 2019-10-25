using PiRhoSoft.Utilities.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class ExpressionField : BaseField<string>
	{
		public const string UssClassName = "pirho-expression-field";
		public const string LabelUssClassName = UssClassName + "__label";
		public const string InputUssClassName = UssClassName + "__input";

		public ExpressionControl Control { get; private set; }

		public ExpressionField(SerializedProperty property, IAutocompleteItem autocomplete) : base(property.displayName, null)
		{
			Setup(property.GetObject<Expression>(), autocomplete);

			var statementProperty = property.FindPropertyRelative(Expression.StatementField);
			this.ConfigureProperty(statementProperty);
			this.SetLabel(property.displayName);
		}

		private void Setup(Expression value, IAutocompleteItem autocomplete)
		{
			Control = new ExpressionControl(value, autocomplete);
			Control.AddToClassList(InputUssClassName);
			Control.RegisterCallback<ChangeEvent<string>>(evt => base.value = evt.newValue);

			labelElement.AddToClassList(LabelUssClassName);

			this.SetVisualInput(Control);
			AddToClassList(UssClassName);
			SetValueWithoutNotify(value.Statement);
		}

		public override void SetValueWithoutNotify(string newValue)
		{
			base.SetValueWithoutNotify(newValue);
			Control.SetValueWithoutNotify(newValue);
		}
	}
}
