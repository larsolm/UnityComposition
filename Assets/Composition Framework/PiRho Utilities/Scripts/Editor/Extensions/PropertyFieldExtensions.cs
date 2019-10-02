using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public static class PropertyFieldExtensions
	{
		public static void SetLabel(this PropertyField field, string label)
		{
			var baseField = field.Q(className: BaseFieldExtensions.UssClassName);
			BaseFieldExtensions.SetLabel(baseField, label);
		}
	}
}