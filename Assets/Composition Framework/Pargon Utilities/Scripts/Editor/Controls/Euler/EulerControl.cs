using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public class EulerControl : VisualElement
	{
		private const string _stylesheet = Utilities.AssetPath + "Controls/Euler/EulerControl.uss";

		public static readonly string ussClassName = "pirho-euler";

		public Quaternion Value => Quaternion.Euler(Field.value);
		public Vector3Field Field { get; private set; }

		public EulerControl(Quaternion value)
		{
			Field = new Vector3Field { value = value.eulerAngles };
			Add(Field);

			Field.RegisterCallback<ChangeEvent<Vector3>>(evt => ElementHelper.SendChangeEvent(this, Quaternion.Euler(evt.previousValue), Quaternion.Euler(evt.newValue)));

			AddToClassList(ussClassName);
			ElementHelper.AddStyleSheet(this, _stylesheet);
		}

		public void SetValueWithoutNotify(Quaternion value)
		{
			if (Value != value)
				Field.SetValueWithoutNotify(value.eulerAngles);
		}
	}
}
