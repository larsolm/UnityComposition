using System;
using UnityEngine;

namespace PiRhoSoft.UtilityEngine
{
	[Flags]
	public enum AssetLocation
	{
		None,
		AssetRoot,
		Selectable
	}

	public class AssetDisplayAttribute : PropertyAttribute
	{
		public bool ShowNoneOption = true;
		public bool ShowEditButton = true;
		public AssetLocation SaveLocation = AssetLocation.None;
		public string DefaultName = null;
	}
}
