﻿using System;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[Serializable]
	public class BindingFormatter
	{
		public enum FormatType
		{
			None,
			Time,
			Number
		}

		public enum TimeFormatType
		{
			SecondsMilliseconds,
			MinutesSeconds,
			MinutesSecondsMilliseconds,
			HoursMinutes,
			Custom
		}

		public enum NumberFormatType
		{
			Percentage,
			Commas,
			Rounded,
			Decimal,
			Custom
		}

		[Tooltip("The format of the displayed string")]
		public string Format = "{0}";

		[Tooltip("The method to format the number by")]
		public FormatType Formatting = FormatType.None;

		[Tooltip("The way to format the time")]
		public TimeFormatType TimeFormatting = TimeFormatType.Custom;

		[Tooltip("The way to format the number")]
		public NumberFormatType NumberFormatting = NumberFormatType.Custom;

		[Tooltip("The format string to be used to format the number")]
		public string ValueFormat;

		public string GetFormattedString(float number)
		{
			var numberString = GetNumberString(number);
			return string.Format(Format, numberString);
		}

		public string GetFormattedString(int number)
		{
			var numberString = GetNumberString(number);
			return string.Format(Format, numberString);
		}

		private string GetNumberString(float number)
		{
			switch (Formatting)
			{
				case FormatType.Time:
				{
					var time = TimeSpan.FromSeconds(number);
					return time.ToString(ValueFormat);
				}
				case FormatType.Number:
				{
					return number.ToString(ValueFormat);
				}
				default:
				{
					return number.ToString();
				}
			}
		}

		private string GetNumberString(int number)
		{
			switch (Formatting)
			{
				case FormatType.Time:
				{
					var time = TimeSpan.FromSeconds(number);
					return time.ToString(ValueFormat);
				}
				case FormatType.Number:
				{
					return number.ToString(ValueFormat);
				}
				default:
				{
					return number.ToString();
				}
			}
		}
	}
}
