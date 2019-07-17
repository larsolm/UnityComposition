﻿using System;
using UnityEngine;

namespace PiRhoSoft.PargonUtilities.Engine
{
	public class TypePickerAttribute : PropertyAttribute
	{
		public Type BaseType { get; private set; }
		public bool ShowAbstract { get; private set; }

		public TypePickerAttribute(Type baseType, bool showAbstract)
		{
			BaseType = baseType;
			ShowAbstract = showAbstract;
		}
	}
}