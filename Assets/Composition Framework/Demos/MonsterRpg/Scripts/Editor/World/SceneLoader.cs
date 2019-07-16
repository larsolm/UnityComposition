using PiRhoSoft.Composition;
using PiRhoSoft.MonsterRpg;
using PiRhoSoft.Utilities.Editor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace PiRhoSoft.MonsterRpg.Editor
{
	[Serializable]
	public class SceneLoaderState
	{
		public SceneState Scenes;
		public bool LoadWorld;
		public string MainScene;
		public string LoadGraph;
		public string StartZone;
		public string StartSpawn;
	}

	[InitializeOnLoad]
	static class SceneLoader
	{
		public static readonly JsonPreference<SceneLoaderState> StatePreference = new JsonPreference<SceneLoaderState>("OoT2D.SceneLoader.State");
		public static readonly StringPreference MainScenePreference = new StringPreference("OoT2D.SceneLoader.MainScene", "Main");
		public static readonly StringPreference LoadGraphPreference = new StringPreference("OoT2D.SceneLoader.LoadGraph", "LoadGraph");
		public static readonly StringPreference FilePreference = new StringPreference("OoT2D.SceneLoader.File", "");
		public static readonly StringPreference ZonePreference = new StringPreference("OoT2D.SceneLoader.Zone", "");
		public static readonly StringPreference SpawnPreference = new StringPreference("OoT2D.SceneLoader.Spawn", "");
		public static readonly IntPreference ZoneTypePreference = new IntPreference("OoT2D.SceneLoader.ZoneType", 0);

		public const int LoadActiveZone = 0;
		public const int LoadSavedZone = 1;
		public const int LoadSpecificZone = 2;

		private const string _mainSceneNotSetError = "A Main Scene was not set";
		private const string _noZonesError = "The World does not have any zones";

		private static readonly GUIContent _failedToLoadContent = new GUIContent("Failed to load World");

		static SceneLoader()
		{
			EditorApplication.playModeStateChanged += OnPlayModeChanged;
		}

		private static void OnPlayModeChanged(PlayModeStateChange state)
		{
			switch (state)
			{
				case PlayModeStateChange.ExitingEditMode: PlayModeStarting(); break;
				case PlayModeStateChange.EnteredPlayMode: PlayModeStarted(); break;
				case PlayModeStateChange.ExitingPlayMode: PlayModeEnding(); break;
				case PlayModeStateChange.EnteredEditMode: PlayModeEnded(); break;
			}
		}

		private static void PlayModeStarting()
		{
			if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
			{
				var state = GetCurrentState();

				if (state.LoadWorld)
					EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

				StatePreference.Value = state;
			}
			else
			{
				EditorApplication.isPlaying = false;
			}
		}

		private static void PlayModeEnded()
		{
			SceneHelper.RestoreState(StatePreference.Value.Scenes);
		}

		private static void PlayModeStarted()
		{
			var state = StatePreference.Value;
			if (state.LoadWorld)
			{
				if (string.IsNullOrEmpty(state.MainScene))
				{
					PlayModeFailed(_mainSceneNotSetError);
				}
				else if (string.IsNullOrEmpty(state.StartZone))
				{
					PlayModeFailed(_noZonesError);
				}
				else
				{
					SceneManager.LoadScene(state.MainScene);

					var graph = AssetDatabase.LoadAssetAtPath<Graph>(state.LoadGraph);
					var zone = AssetDatabase.LoadAssetAtPath<Zone>(state.StartZone);

					var result = WorldLoader.Instance.Load(new VariableSet(), state.StartZone, state.StartSpawn, FilePreference.Value);

					CompositionManager.Instance.RunGraph(graph, VariableValue.CreateReference(zone));

					result.OnError = PlayModeFailed;
				}
			}
		}

		private static void PlayModeFailed(string message)
		{
			EditorApplication.isPlaying = false;

			if (Resources.FindObjectsOfTypeAll<SceneView>().Length > 0)
				EditorWindow.GetWindow<SceneView>().ShowNotification(_failedToLoadContent);

			Debug.Log(message);
		}

		private static void PlayModeEnding()
		{
		}

		public static SceneLoaderState GetCurrentState()
		{
			var state = new SceneLoaderState
			{
				Scenes = SceneHelper.CaptureState(),
				MainScene = MainScenePreference.Value,
				LoadGraph = LoadGraphPreference.Value,
				StartSpawn = SpawnPreference.Value,
			};

			if (ZoneTypePreference.Value == LoadActiveZone)
			{
				var zone = FindZone();
				if (zone == null)
				{
					var manager = FindWorldManager();

					if (manager != null && manager.World != null)
						zone = manager.World.Zones.Count > 0 ? manager.World.Zones[0] : null;
				}

				if (zone != null)
				{
					state.LoadWorld = true;
					state.StartZone = zone.name;
				}
			}
			else if (ZoneTypePreference.Value == LoadSpecificZone)
			{
				var zones = AssetHelper.ListAssets<Zone>();
				var zone = GetZone(zones, ZonePreference.Value);

				if (zone != null)
				{
					state.LoadWorld = true;
					state.StartZone = zone.name;
				}
			}
			else
			{
				state.LoadWorld = ZoneTypePreference.Value == LoadSavedZone;
			}

			return state;
		}

		private static WorldManager FindWorldManager()
		{
			var manager = Object.FindObjectsOfType<WorldManager>();
			return manager != null && manager.Length > 0 ? manager[0] : null;
		}

		private static Zone FindZone()
		{
			var zones = AssetHelper.ListAssets<Zone>();
			var scene = SceneManager.GetActiveScene();
			var zone = GetZone(zones, scene.path);

			if (zone != null)
				return zone;

			for (var i = 0; i < SceneManager.sceneCount; i++)
			{
				scene = SceneManager.GetSceneAt(i);
				zone = GetZone(zones, scene.path);

				if (zone != null)
					return zone;
			}

			return null;
		}

		private static Zone GetZone(List<Zone> zones, string scenePath)
		{
			foreach (var zone in zones)
			{
				if (zone.Scene.Path == scenePath)
					return zone;
			}

			return null;
		}
	}
}
