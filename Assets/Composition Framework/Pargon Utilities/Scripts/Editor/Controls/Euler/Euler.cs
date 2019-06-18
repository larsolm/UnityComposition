using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public class Euler : BindableValueElement<Quaternion>
	{
		private const string _styleSheetPath = "Assets/PargonUtilities/Scripts/Editor/Controls/Euler/Euler.uss";

		public class Factory : UxmlFactory<Euler, Traits> { }

		public class Traits : UxmlTraits
		{
			private UxmlFloatAttributeDescription _x = new UxmlFloatAttributeDescription { name = "x" };
			private UxmlFloatAttributeDescription _y = new UxmlFloatAttributeDescription { name = "y" };
			private UxmlFloatAttributeDescription _z = new UxmlFloatAttributeDescription { name = "z" };

			public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
			{
				get { yield break; }
			}

			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);

				var euler = ve as Euler;
				var x = _x.GetValueFromBag(bag, cc);
				var y = _y.GetValueFromBag(bag, cc);
				var z = _z.GetValueFromBag(bag, cc);

				euler.Setup(Quaternion.Euler(x, y, z));
			}
		}

		private Vector3Field _field;

		public void Setup(SerializedProperty property)
		{
			Setup(property.quaternionValue);
			BindToProperty(property);
		}

		public void Setup(Quaternion initialValue)
		{
			ElementHelper.AddStyleSheet(this, _styleSheetPath);

			_field = new Vector3Field() { value = initialValue.eulerAngles };
			_field.RegisterValueChangedCallback(e => value = Quaternion.Euler(e.newValue));

			Add(_field);

			SetValueWithoutNotify(initialValue);
		}

		protected override void Refresh()
		{
			_field.SetValueWithoutNotify(value.eulerAngles);
		}

		protected override void SetValueToProperty(SerializedProperty property, Quaternion value)
		{
			property.quaternionValue = value;
		}

		protected override Quaternion GetValueFromProperty(SerializedProperty property)
		{
			return property.quaternionValue;
		}
	}
}
