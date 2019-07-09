﻿using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public class EnumButtonsControl : VisualElement
	{
		private const string _invalidTypeWarning = "(PUEBCIT) failed to set value on EnumButtonsControl: attempted to set a value with type '{0}' but the control is using type '{1}'";

		private const string _stylesheet = Utilities.AssetPath + "Controls/EnumButtons/EnumButtons.uss";

		public static readonly string ussClassName = "pirho-enum-buttons";
		public static readonly string buttonUssClassName = "pirho-enum-buttons__button";
		public static readonly string activeButtonUssClassName = buttonUssClassName + "--active";

		public Enum Value { get; private set; }
		public Type Type { get; private set; }
		public bool UseFlags { get; private set; }

		private string[] _names;
		private Array _values;
		private UQueryState<Button> _buttons;

		public EnumButtonsControl(Enum value, bool? useFlags = null)
		{
			Value = value;
			Type = value.GetType();
			UseFlags = useFlags.HasValue ? useFlags.Value : TypeHelper.HasAttribute<FlagsAttribute>(Type);

			_names = Enum.GetNames(Type);
			_values = Enum.GetValues(Type);
			_buttons = this.Query<Button>().Build();

			Rebuild();

			AddToClassList(ussClassName);
			ElementHelper.AddStyleSheet(this, _stylesheet);
		}

		public void SetValueWithoutNotify(Enum value)
		{
			if (Type != value.GetType())
			{
				Debug.LogWarningFormat(_invalidTypeWarning, value.GetType().Name, Type.Name);
			}
			else if (!Value.Equals(value))
			{
				Value = value;
				Refresh();
			}
		}

		private void Rebuild()
		{
			for (var i = 0; i < _names.Length; i++)
			{
				var index = i;
				var button = new Button(() => Toggle(index))
				{
					text = _names[i],
					userData = _values.GetValue(i)
				};

				button.AddToClassList(buttonUssClassName);
				Add(button);
			}

			Refresh();
		}

		private void Refresh()
		{
			_buttons.ForEach(button =>
			{
				if (UseFlags)
				{
					var current = GetIntFromEnum(Type, Value);
					var buttonValue = GetIntFromEnum(Type, button.userData as Enum);

					ElementHelper.ToggleClass(button, activeButtonUssClassName, (buttonValue != 0 && (current & buttonValue) == buttonValue) || (current == 0 && buttonValue == 0));
				}
				else
				{
					ElementHelper.ToggleClass(button, activeButtonUssClassName, Value.Equals(button.userData as Enum));
				}
			});
		}

		private void Toggle(int index)
		{
			var value = Value;
			var selected = _values.GetValue(index) as Enum;

			if (UseFlags)
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

				value = GetEnumFromInt(Type, current);
			}
			else
			{
				value = selected;
			}

			using (ChangeEvent<Enum> changeEvent = ChangeEvent<Enum>.GetPooled(Value, value))
			{
				changeEvent.target = this;
				SetValueWithoutNotify(value);
				SendEvent(changeEvent);
			}
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