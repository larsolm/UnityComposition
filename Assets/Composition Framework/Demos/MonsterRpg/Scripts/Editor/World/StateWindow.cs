using PiRhoSoft.Composition;
using PiRhoSoft.Utilities;
using PiRhoSoft.Utilities.Editor;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using MenuItem = UnityEditor.MenuItem;

namespace PiRhoSoft.MonsterRpg.Editor
{
	public class StateWindow : EditorWindow
	{
		private static readonly GUIContent _mainSceneContent = new GUIContent("Main Scene", "The main scene to load when the game starts up");
		private static readonly GUIContent _loadGraphContent = new GUIContent("Load Graph", "The graph to run when the game starts up");
		private static readonly GUIContent _saveButton = new GUIContent("Save", "Save the game");
		private static readonly GUIContent _newButton = new GUIContent("New", "Create a new save file");
		private static readonly GUIContent _openButton = new GUIContent("Open", "Open an existing save file");
		private static readonly GUIContent _clearButton = new GUIContent("Clear", "Stop using a save file");
		private static readonly GUIContent _activeZoneOptionContent = new GUIContent("Active Zone in Editor");
		private static readonly GUIContent _savedZoneOptionContent = new GUIContent("Saved Zone");
		private static readonly GUIContent _startingZoneContent = new GUIContent("Starting Zone");
		private static readonly GUIContent _spawnContent = new GUIContent("Starting Spawn");

		private AssetList _zoneList;
		private GUIContent[] _zoneNames;
		private SceneReference _mainScene;
		private Graph _loadGraph;

		[MenuItem("Window/OoT2D/State Manager")]
		public static void Open()
		{
			GetWindow<StateWindow>("State Manager").Show();
		}

		void OnGUI()
		{
			EditorGUILayout.Space();
			DrawScenePicker();
			GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
			DrawGraphPicker();
			GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
			DrawFilePicker();
			GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
			DrawZonePicker();
			GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
			DrawSpawn();
		}

		private void OnEnable()
		{
			_mainScene = new SceneReference { Path = SceneLoader.MainScenePreference.Value };
			_mainScene.Setup(this);
			_loadGraph = AssetDatabase.LoadAssetAtPath<Graph>(SceneLoader.LoadGraphPreference.Value);
		}

		private void OnDisable()
		{
			_mainScene.Teardown();
		}

		private void DrawScenePicker()
		{
			SceneReferenceDrawer.Draw(_mainScene, _mainSceneContent, AssetLocation.Selectable, "Main", null);
			SceneLoader.MainScenePreference.Value = _mainScene.Path;
		}

		private void DrawGraphPicker()
		{
			_loadGraph = AssetDisplayDrawer.Draw(_loadGraphContent, _loadGraph, true, true, AssetLocation.Selectable, "LoadGraph");
			SceneLoader.LoadGraphPreference.Value = AssetDatabase.GetAssetPath(_loadGraph);
		}

		private void DrawFilePicker()
		{
			using (new EditorGUILayout.HorizontalScope())
			{
				EditorGUILayout.PrefixLabel("Save File");

				if (EditorApplication.isPlaying)
				{
					if (GUILayout.Button(_saveButton, GUILayout.MinWidth(20)))
						WorldLoader.Instance.Save();
				}
				else
				{
					EditorGUILayout.SelectableLabel(SceneLoader.FilePreference.Value, GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.MinWidth(5));

					if (GUILayout.Button(_newButton, GUILayout.MinWidth(20)))
					{
						var path = EditorUtility.SaveFilePanel("Create Save File", Application.persistentDataPath, "Editor Save", "json");

						if (!string.IsNullOrEmpty(path))
						{
							File.WriteAllText(path, "{}");
							SceneLoader.FilePreference.Value = path;

							if (SceneLoader.ZoneTypePreference.Value == SceneLoader.LoadSavedZone)
								SceneLoader.ZoneTypePreference.Value = SceneLoader.LoadActiveZone;
						}
					}

					if (GUILayout.Button(_openButton, GUILayout.MinWidth(20)))
					{
						var path = EditorUtility.OpenFilePanel("Open Save File", Application.persistentDataPath, "json");

						if (!string.IsNullOrEmpty(path))
							SceneLoader.FilePreference.Value = path;
					}

					if (GUILayout.Button(_clearButton, GUILayout.MinWidth(20)))
					{
						SceneLoader.FilePreference.Value = "";

						if (SceneLoader.ZoneTypePreference.Value == SceneLoader.LoadSavedZone)
							SceneLoader.ZoneTypePreference.Value = SceneLoader.LoadActiveZone;
					}
				}
			}
		}

		private void BuildZoneList()
		{
			var zoneList = AssetHelper.GetAssetList<Zone>();

			if (zoneList != _zoneList)
			{
				_zoneNames = new GUIContent[zoneList.Names.Length + 3];
				_zoneList = zoneList;

				Array.Copy(zoneList.Names, 0, _zoneNames, 3, zoneList.Names.Length);

				_zoneNames[0] = _activeZoneOptionContent;
				_zoneNames[1] = _savedZoneOptionContent;
				_zoneNames[2] = new GUIContent(string.Empty);
			}
		}

		private void DrawZonePicker()
		{
			BuildZoneList();

			var index = 0;

			if (SceneLoader.ZoneTypePreference.Value == SceneLoader.LoadActiveZone)
			{
				index = 0;
			}
			else if (SceneLoader.ZoneTypePreference.Value == SceneLoader.LoadSavedZone)
			{
				index = 1;
			}
			else
			{
				for (var i = 0; i < _zoneList.Assets.Count; i++)
				{
					if ((_zoneList.Assets[i] as Zone).Scene.Path == SceneLoader.ZonePreference.Value)
					{
						index = i + 3;
						break;
					}
				}
			}

			index = EditorGUILayout.Popup(_startingZoneContent, index, _zoneNames);

			if (index == 0)
			{
				SceneLoader.ZoneTypePreference.Value = SceneLoader.LoadActiveZone;
			}
			else if (index == 1)
			{
				SceneLoader.ZoneTypePreference.Value = SceneLoader.LoadSavedZone;
			}
			else
			{
				SceneLoader.ZoneTypePreference.Value = SceneLoader.LoadSpecificZone;
				SceneLoader.ZonePreference.Value = (_zoneList.Assets[index - 3] as Zone).Scene.Path;
			}
		}

		private void DrawSpawn()
		{
			if (SceneLoader.ZoneTypePreference.Value != SceneLoader.LoadSavedZone)
				SceneLoader.SpawnPreference.Value = EditorGUILayout.TextField(_spawnContent, SceneLoader.SpawnPreference.Value);
		}
	}
}
