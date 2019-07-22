using UnityEngine;

namespace PiRhoSoft.Utilities
{
	public class ScenePickerAttribute : PropertyAttribute
	{
		public string CreateMethod { get; private set; }

		public ScenePickerAttribute()
		{
			CreateMethod = string.Empty;
		}

		public ScenePickerAttribute(string createMethod)
		{
			CreateMethod = createMethod;
		}
	}
}
