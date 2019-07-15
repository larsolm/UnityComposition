using System.Reflection;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public static class BaseFieldExtensions
	{
		public static readonly string UssClassName = BaseField<int>.ussClassName;

		#region Visual Input

		private const string _visualInputProperty = "visualInput";

		public static VisualElement GetVisualInput<T>(this BaseField<T> field)
		{
			return GetVisualInputProperty<T>().GetValue(field) as VisualElement;
		}

		public static void SetVisualInput<T>(this BaseField<T> field, VisualElement element)
		{
			GetVisualInputProperty<T>().SetValue(field, element);
		}

		private static PropertyInfo GetVisualInputProperty<T>()
		{
			return typeof(BaseField<T>).GetProperty(_visualInputProperty, BindingFlags.Instance | BindingFlags.NonPublic);
		}

		#endregion
	}
}