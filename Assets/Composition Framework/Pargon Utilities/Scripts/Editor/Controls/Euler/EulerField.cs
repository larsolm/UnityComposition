using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public class EulerField : BaseField<Quaternion>
	{
		private const string _invalidTypeError = "(PUEEFIT) invalid property for EulerField: '{0}' is type '{1}' rather than 'Quaternion'";

		public new static readonly string ussClassName = "pirho-euler-field";
		public new static readonly string labelUssClassName = ussClassName + "__label";
		public new static readonly string inputUssClassName = ussClassName + "__input";

		private SerializedProperty _property;

		public EulerField(SerializedProperty property, string label) : base(label, null)
		{
			_property = property;

			if (property.propertyType == SerializedPropertyType.Quaternion)
			{
				bindingPath = property.propertyPath;
				Setup(property.quaternionValue);
			}
			else
			{
				Debug.LogErrorFormat(_invalidTypeError, property.propertyPath, property.propertyType);
			}
		}

		private void Setup(Quaternion value)
		{
			var control = new EulerControl(value);

			AddToClassList(ussClassName);
			labelElement.AddToClassList(labelUssClassName);
			control.AddToClassList(inputUssClassName);

			this.SetVisualInput(control);
			this.RegisterValueChangedCallback(evt => control.SetValueWithoutNotify(evt.newValue));
			control.RegisterCallback<ChangeEvent<Quaternion>>(evt => base.value = evt.newValue);
		}

		#region UXML Support

		public EulerField() : base(null, null) {}

		public new class UxmlFactory : UxmlFactory<EulerField, UxmlTraits> { }

		public new class UxmlTraits : BaseField<Quaternion>.UxmlTraits
		{
			private UxmlFloatAttributeDescription _x = new UxmlFloatAttributeDescription { name = "x" };
			private UxmlFloatAttributeDescription _y = new UxmlFloatAttributeDescription { name = "y" };
			private UxmlFloatAttributeDescription _z = new UxmlFloatAttributeDescription { name = "z" };

			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);

				var euler = ve as EulerField;
				var x = _x.GetValueFromBag(bag, cc);
				var y = _y.GetValueFromBag(bag, cc);
				var z = _z.GetValueFromBag(bag, cc);

				euler.Setup(Quaternion.Euler(x, y, z));
			}
		}

		#endregion
	}
}