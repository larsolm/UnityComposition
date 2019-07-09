using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Utilities.Editor
{
	public class Euler : VisualElement, IBindableProperty<Quaternion>, IBindableObject<Quaternion>
	{
		private const string _styleSheetPath = Utilities.AssetPath + "Controls/Euler/Euler.uss";

		private readonly Object _owner;
		private readonly Func<Quaternion> _getValue;
		private readonly Action<Quaternion> _setValue;

		private Vector3Field _field;

		public Euler()
		{
		}

		public Euler(SerializedProperty property)
		{
			ElementHelper.Bind(this, this, property);
		}

		public Euler(Object owner, Func<Quaternion> getValue, Action<Quaternion> setValue)
		{
			_getValue = getValue;
			_setValue = setValue;

			ElementHelper.Bind(this, this, owner);
		}

		public void Setup(Quaternion value)
		{
			ElementHelper.AddStyleSheet(this, _styleSheetPath);

			_field = new Vector3Field() { value = value.eulerAngles };

			Add(_field);
		}

		public Quaternion GetValueFromElement(VisualElement element)
		{
			return Quaternion.Euler(_field.value);
		}

		public Quaternion GetValueFromProperty(SerializedProperty property)
		{
			return property.quaternionValue;
		}

		public Quaternion GetValueFromObject(Object owner)
		{
			return _getValue();
		}

		public void UpdateElement(Quaternion value, VisualElement element, SerializedProperty property)
		{
			UpdateElement(value);
		}

		public void UpdateElement(Quaternion value, VisualElement element, Object owner)
		{
			UpdateElement(value);
		}

		public void UpdateProperty(Quaternion value, VisualElement element, SerializedProperty property)
		{
			property.quaternionValue = value;
		}

		public void UpdateObject(Quaternion value, VisualElement element, Object owner)
		{
			_setValue(value);
		}

		private void UpdateElement(Quaternion value)
		{
			_field.SetValueWithoutNotify(value.eulerAngles);
		}

		#region UXML

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

		#endregion
	}
}
