using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public class EnumButtons : VisualElement, IBindableProperty<Enum>, IBindableObject<Enum>
	{
		private const string _invalidTypeWarning = "(PUCEBIT) Invalid type for EnumButtons: the type '{0}' must be an Enum type";
		private const string _invalidValueWarning = "(PUCEBIV) Could not parse value of '{0}', because it isn't defined in the enum of type '{1}'";

		private const string _styleSheetPath = Utilities.AssetPath + "Controls/EnumButtons/EnumButtons.uss";
		private const string _ussActive = "active";

		public Enum Value { get; private set; }
		public Type Type { get; private set; }
		public bool Flags { get; private set; }

		private Func<Enum> _getValue;
		private Action<Enum> _setValue;
		private UQueryState<Button> _buttons;

		public EnumButtons()
		{
		}

		public EnumButtons(SerializedProperty property)
		{
			ElementHelper.Bind(this, this, property);
		}

		public EnumButtons(Object owner, Func<Enum> getValue, Action<Enum> setValue)
		{
			ElementHelper.Bind(this, this, owner);

			_getValue = getValue;
			_setValue = setValue;
		}

		public void Setup(Type type, bool flags, Enum value)
		{
			if (type == null || !type.IsEnum)
			{
				Debug.LogWarningFormat(_invalidTypeWarning, type);
				return;
			}

			Value = value;
			Type = type;
			Flags = flags;

			ElementHelper.AddStyleSheet(this, _styleSheetPath);

			var names = Enum.GetNames(Type);
			var values = Enum.GetValues(Type);

			for (var i = 0; i < names.Length; i++)
			{
				var index = i;
				var button = new Button(() =>
				{
					var selected = values.GetValue(index) as Enum;

					if (Flags)
					{
						var current = GetIntFromEnum(Type, Value);
						var buttonValue = GetIntFromEnum(Type, selected);

						if ((buttonValue != 0 && (current & buttonValue) == buttonValue) || (current == 0 && buttonValue == 0))
						{
							if (buttonValue != ~0)
								current &= ~buttonValue;
						}
						else
						{
							if (buttonValue == 0)
								current = 0;
							else
								current |= buttonValue;
						}

						Value = GetEnumFromInt(Type, current);
					}
					else
					{
						Value = selected;
					}
				})
				{
					text = names[i],
					userData = values.GetValue(i)
				};

				Add(button);
			}

			_buttons = this.Query<Button>().Build();
		}

		public Enum GetValueFromElement(VisualElement element)
		{
			return Value;
		}

		public Enum GetValueFromProperty(SerializedProperty property)
		{
			return GetEnumFromInt(Type, property.intValue);
		}

		public Enum GetValueFromObject(Object owner)
		{
			return _getValue();
		}

		public void UpdateElement(Enum value, VisualElement element, SerializedProperty property)
		{
			UpdateElement(value);
		}

		public void UpdateElement(Enum value, VisualElement element, Object owner)
		{
			UpdateElement(value);
		}

		public void UpdateProperty(Enum value, VisualElement element, SerializedProperty property)
		{
			property.intValue = GetIntFromEnum(Type, value);
		}

		public void UpdateObject(Enum value, VisualElement element, Object owner)
		{
			_setValue(value);
		}

		public Enum GetEnumFromInt(Type type, int value)
		{
			return Enum.ToObject(type, value) as Enum;
		}

		public int GetIntFromEnum(Type type, Enum value)
		{
			return (int)Enum.Parse(type, value.ToString());
		}

		private void UpdateElement(Enum value)
		{
			_buttons.ForEach(button =>
			{
				if (Flags)
				{
					var current = GetIntFromEnum(Type, Value);
					var buttonValue = GetIntFromEnum(Type, button.userData as Enum);

					ElementHelper.ToggleClass(button, _ussActive, (buttonValue != 0 && (current & buttonValue) == buttonValue) || (current == 0 && buttonValue == 0));
				}
				else
				{
					ElementHelper.ToggleClass(button, _ussActive, Value.Equals(button.userData as Enum));
				}
			});
		}

		#region UXML

		public class Factory : UxmlFactory<EnumButtons, Traits> { }

		public class Traits : UxmlTraits
		{
			private UxmlStringAttributeDescription _type = new UxmlStringAttributeDescription { name = "type", use = UxmlAttributeDescription.Use.Required };
			private UxmlStringAttributeDescription _value = new UxmlStringAttributeDescription { name = "value" };
			private UxmlBoolAttributeDescription _flags = new UxmlBoolAttributeDescription { name = "flags" };

			public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
			{
				get { yield break; }
			}

			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);

				var enumButtons = ve as EnumButtons;
				var typeName = _type.GetValueFromBag(bag, cc);
				var flags = _flags.GetValueFromBag(bag, cc);
				var type = Type.GetType(typeName);

				if (type != null && type.IsEnum)
				{
					var stringValue = _value.GetValueFromBag(bag, cc);

					if (Enum.IsDefined(type, stringValue))
					{
						var value = Enum.Parse(type, stringValue) as Enum;
						enumButtons.Setup(type, flags, value);
					}
					else
					{
						Debug.LogErrorFormat(_invalidValueWarning, stringValue, type.Name);
						enumButtons.Setup(type, flags, Enum.GetValues(type).GetValue(0) as Enum);
					}
				}
				else
				{
					Debug.LogWarningFormat(_invalidTypeWarning, typeName);
				}
			}
		}

		#endregion
	}
}
