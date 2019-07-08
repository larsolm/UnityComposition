using PiRhoSoft.PargonUtilities.Engine;
using UnityEngine;

namespace PiRhoSoft.PargonUtilities.Engine
{
	public abstract class Resource : ScriptableObject, ISerializationCallbackReceiver
	{
		public const string _invalidPathWarning = "(URIP) Invalid Resource location: the {0} at path {1} should be beneath a folder called 'Resources' so it can be loaded at runtime";

#pragma warning disable CS0414

		private static readonly string ResourcesFolder = "Resources/";
		private static readonly int FolderLength = "Resources/".Length;
		private static readonly int ExtensionLength = ".asset".Length;

#pragma warning restore CS0414

		[SerializeField] [ReadOnly] private string _path = "";

		public string Path => _path;

		#region ISerializationCallbackReceiver Implementation

		public void OnBeforeSerialize()
		{
#if UNITY_EDITOR
			// During runtime ScriptableObjects are only referenceable outside of scenes (e.g from a game save file) by
			// passing their path to Resources Load (and then only if the object is in a Resources folder).

			var path = UnityEditor.AssetDatabase.GetAssetPath(this);
			var index = path.IndexOf(ResourcesFolder);

			if (index < 0)
			{
				if (path != _path && !string.IsNullOrEmpty(path))
					Debug.LogWarningFormat(this, _invalidPathWarning, GetType().Name, path);

				_path = path;
			}
			else
			{
				// Unity merges all Resources folders so the path needed to look up the Resource is just the portion
				// beneath the Resources folder.

				_path = path.Substring(index + FolderLength, path.Length - index - FolderLength - ExtensionLength);
			}
#endif
		}

		public void OnAfterDeserialize()
		{
		}

		#endregion
	}
}
