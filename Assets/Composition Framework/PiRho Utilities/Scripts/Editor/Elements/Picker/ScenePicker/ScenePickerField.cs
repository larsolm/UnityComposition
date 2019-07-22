using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class ScenePickerField : BaseField<string>
	{
		public static readonly string UssClassName = "pirho-scene-picker-field";
		public static readonly string LabelUssClassName = UssClassName + "__label";
		public static readonly string InputUssClassName = UssClassName + "__input";

		private ScenePickerControl _control;

		public ScenePickerField(string label, string path, Action onCreate) : base(label, null)
		{
			Setup(path, onCreate);
		}

		private void Setup(string value, Action onCreate)
		{
			_control = new ScenePickerControl(value, onCreate);
			_control.AddToClassList(InputUssClassName);
			_control.RegisterCallback<ChangeEvent<string>>(evt => base.value = evt.newValue);

			labelElement.AddToClassList(LabelUssClassName);

			this.SetVisualInput(_control);
			AddToClassList(UssClassName);
			SetValueWithoutNotify(value);
		}

		public override void SetValueWithoutNotify(string newValue)
		{
			base.SetValueWithoutNotify(newValue);
			_control.SetValueWithoutNotify(SceneHelper.GetSceneFromPath(newValue));
		}

		#region UXML Support

		public ScenePickerField() : base(null, null) { }

		public new class UxmlFactory : UxmlFactory<ScenePickerField, UxmlTraits> { }

		public new class UxmlTraits : BaseField<string>.UxmlTraits
		{
			private UxmlStringAttributeDescription _path = new UxmlStringAttributeDescription { name = "path" };

			public override void Init(VisualElement element, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(element, bag, cc);

				var field = element as ScenePickerField;
				var path = _path.GetValueFromBag(bag, cc);

				field.Setup(path, null);
			}
		}

		#endregion
	}
}
