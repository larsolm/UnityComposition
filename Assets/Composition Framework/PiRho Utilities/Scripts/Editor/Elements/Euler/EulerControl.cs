using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public class EulerControl : VisualElement
	{
		public const string Stylesheet = "Euler/EulerStyle.uss";
		public static readonly string UssClassName = "pirho-euler";

		public Quaternion Value => Quaternion.Euler(Field.value);
		public Vector3Field Field { get; private set; }

		public EulerControl(Quaternion value)
		{
			Field = new Vector3Field { value = value.eulerAngles };
			Add(Field);

			Field.RegisterCallback<ChangeEvent<Vector3>>(evt => this.SendChangeEvent(Quaternion.Euler(evt.previousValue), Quaternion.Euler(evt.newValue)));

			AddToClassList(UssClassName);
			this.AddStyleSheet(Utilities.ElementsPath, Stylesheet);
		}

		public void SetValueWithoutNotify(Quaternion value)
		{
			if (Value != value)
				Field.SetValueWithoutNotify(value.eulerAngles);
		}
	}
}