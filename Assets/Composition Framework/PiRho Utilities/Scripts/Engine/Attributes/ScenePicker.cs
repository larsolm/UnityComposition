﻿using UnityEngine;

namespace PiRhoSoft.Utilities
{
	public class ScenePickerAttribute : PropertyAttribute
	{
		public string CreateMethod { get; private set; }

		public ScenePickerAttribute(string createMethod = null)
		{
			CreateMethod = createMethod;
		}
	}
}
