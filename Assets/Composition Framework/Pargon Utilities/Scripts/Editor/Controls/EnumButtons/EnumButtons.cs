using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public class EnumButtons : BindableValueElement<Enum>
	{
		private const string _invalidTypeWarning = "(PICEBIT) Invalid type for EnumButtons: the type '{0}' must be an Enum type";
		private const string _invalidValueWarning = "(PICEBIV) Could not parse value of '{0}', because it isn't defined in the enum of type '{1}'";

		private const string _styleSheetPath = "Assets/PargonUtilities/Scripts/Editor/Controls/EnumButtons/EnumButtons.uss";

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

		public Type Type { get; private set; }
		public bool Flags { get; private set; }

		private UQueryState<Button> _buttons;

		public void Setup(Type type, bool flags, SerializedProperty property)
		{
			var initialValue = GetEnumFromInt(type, property.intValue);

			Setup(type, flags, initialValue);
			BindToProperty(property);
		}

		public void Setup(Type type, bool flags, Enum initialValue)
		{
			Type = type;
			Flags = flags;

			if (Type == null || !Type.IsEnum)
			{
				Debug.LogWarningFormat(_invalidTypeWarning, Type);
				return;
			}

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
						var current = GetIntFromEnum(Type, value);
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

						value = GetEnumFromInt(Type, current);
					}
					else
					{
						value = selected;
					}
				})
				{
					text = names[i],
					userData = values.GetValue(i)
				};

				Add(button);
			}

			_buttons = this.Query<Button>().Build();

			SetValueWithoutNotify(initialValue);
		}

		protected override void Refresh()
		{
			_buttons.ForEach(button =>
			{
				if (Flags)
				{
					var current = GetIntFromEnum(Type, value);
					var buttonValue = GetIntFromEnum(Type, button.userData as Enum);

					if ((buttonValue != 0 && (current & buttonValue) == buttonValue) || (current == 0 && buttonValue == 0))
						button.AddToClassList("active");
					else
						button.RemoveFromClassList("active");
				}
				else
				{
					if (value.Equals(button.userData as Enum))
						button.AddToClassList("active");
					else
						button.RemoveFromClassList("active");
				}
			});
		}

		protected override void SetValueToProperty(SerializedProperty property, Enum value)
		{
			property.intValue = GetIntFromEnum(Type, value);
		}

		protected override Enum GetValueFromProperty(SerializedProperty property)
		{
			return GetEnumFromInt(Type, property.intValue);
		}

		private Enum GetEnumFromInt(Type type, int value)
		{
			return Enum.ToObject(type, value) as Enum;
		}

		private int GetIntFromEnum(Type type, Enum value)
		{
			return (int)Enum.Parse(type, value.ToString());
		}
	}
}
