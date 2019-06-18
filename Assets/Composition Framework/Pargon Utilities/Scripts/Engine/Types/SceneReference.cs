﻿using System;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace PiRhoSoft.PargonUtilities.Engine
{
	[Serializable]
	public class SceneReference
	{
		public string Path;

		public bool IsAssigned => !string.IsNullOrEmpty(Path);
		public bool IsLoaded => Scene.IsValid();
		public Scene Scene => SceneManager.GetSceneByPath(Path);
		public int Index => SceneUtility.GetBuildIndexByScenePath(Path);

		#region Editor Support

#if UNITY_EDITOR

		public static Action<string, string> SceneMoved;

		private Object _owner;

		public void Setup(Object owner)
		{
			_owner = owner;
			SceneMoved += OnSceneMoved;
		}

		public void Teardown()
		{
			SceneMoved -= OnSceneMoved;
		}

		private void OnSceneMoved(string from, string to)
		{
			if (Path == from)
			{
				Path = to;

				if (_owner)
					UnityEditor.EditorUtility.SetDirty(_owner);
			}
		}

#else

		public void Setup(Object owner) { }
		public void Teardown() { }

#endif

		#endregion
	}
}
