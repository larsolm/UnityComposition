using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public static class BaseFieldExtensions
	{
		private const string _changedInternalsError = "(PUBFECI) failed to setup BaseField: Unity internals have changed";
		private const string _labelProperty = "label";
		private const string _visualInputProperty = "visualInput";

		public static readonly string UssClassName = BaseField<int>.ussClassName;
		public static readonly string LabelUssClassName = BaseField<int>.labelUssClassName;
		public static readonly string NoLabelVariantUssClassName = BaseField<int>.noLabelVariantUssClassName;

		public static void SetLabel(VisualElement element, string label)
		{
			// label is public but this allows access without knowing the generic type of the BaseField

			GetProperty(element.GetType(), _labelProperty, BindingFlags.Instance | BindingFlags.Public).SetValue(element, label);
		}

		public static VisualElement GetVisualInput<T>(this BaseField<T> field)
		{
			return GetProperty<T>(_visualInputProperty, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(field) as VisualElement;
		}

		public static void SetVisualInput<T>(this BaseField<T> field, VisualElement element)
		{
			GetProperty<T>(_visualInputProperty, BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(field, element);
		}

		#region Lookup

		// can't do lookups in a static constructor since they are dependent on the generic type

		private static PropertyInfo GetProperty<T>(string name, BindingFlags flags)
		{
			return GetProperty(typeof(BaseField<T>), name, flags);
		}

		private static PropertyInfo GetProperty(Type type, string name, BindingFlags flags)
		{
			var property = type.GetProperty(name, flags);

			if (property == null)
				Debug.LogError(_changedInternalsError);

			return property;
		}

		#endregion
	}
}