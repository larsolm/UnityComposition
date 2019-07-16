using PiRhoSoft.Composition;
using PiRhoSoft.Utilities.Editor;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using MenuItem = UnityEditor.MenuItem;
using Object = UnityEngine.Object;

namespace PiRhoSoft.MonsterRpg.Editor
{
	public class StateWindow : EditorWindow
	{
		private class StateWindowElement : VisualElement
		{
			private const string StylePath = Composition.Composition.StylePath;

			private AssetList _zoneList;
			private GUIContent[] _zoneNames;
			private SceneAsset _mainScene;
			private Graph _loadGraph;

			private VisualElement _editingContainer;
			private VisualElement _playingContainer;
			
			public StateWindowElement(Object owner)
			{
				EditorApplication.playModeStateChanged += PlayStateChanged;

				_mainScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(SceneLoader.MainScenePreference.Value);
				_loadGraph = AssetDatabase.LoadAssetAtPath<Graph>(SceneLoader.LoadGraphPreference.Value);

				CreateScenePicker(owner);
				CreateGraphPicker(owner);
				CreateFilePicker();
				CreateZonePicker();
				CreateSpawnPicker();
			}

			private void CreateScenePicker(Object owner)
			{
				var mainSceneContainer = ElementHelper.CreatePropertyContainer("Main Scene", "The main scene to load when the game starts up");

				var mainScenePicker = new ScenePicker(owner, () => _mainScene, scene =>
				{
					_mainScene = scene;
					SceneLoader.MainScenePreference.Value = AssetDatabase.GetAssetPath(_mainScene);
				});

				Add(mainSceneContainer);
			}

			private void CreateGraphPicker(Object owner)
			{
				var loadGraphContainer = ElementHelper.CreatePropertyContainer("Load Graph", "The graph to run when the game starts up");

				var graphPicker = new ObjectPicker(owner, () => _loadGraph, graph =>
				{
					_loadGraph = graph as Graph;
					SceneLoader.LoadGraphPreference.Value = AssetDatabase.GetAssetPath(_loadGraph);
				});

				Add(loadGraphContainer);
			}

			private void CreateFilePicker()
			{
				var fileContaier = ElementHelper.CreatePropertyContainer("Save File", "The save file to load when testing the game");

				_editingContainer = new VisualElement();
				_playingContainer = new VisualElement();

				_editingContainer.Add(new Button(() =>
				{
					var path = EditorUtility.SaveFilePanel("Create Save File", Application.persistentDataPath, "Editor Save", "json");

					if (!string.IsNullOrEmpty(path))
					{
						File.WriteAllText(path, "{}");
						SceneLoader.FilePreference.Value = path;

						if (SceneLoader.ZoneTypePreference.Value == SceneLoader.LoadSavedZone)
							SceneLoader.ZoneTypePreference.Value = SceneLoader.LoadActiveZone;
					}
				})
				{
					text = "New",
					tooltip = "Create a new save file"
				});

				_editingContainer.Add(new Button(() =>
				{
					var path = EditorUtility.OpenFilePanel("Open Save File", Application.persistentDataPath, "json");

					if (!string.IsNullOrEmpty(path))
						SceneLoader.FilePreference.Value = path;
				})
				{
					text = "Open",
					tooltip = "Open an existing save file"
				});

				_editingContainer.Add(new Button(() =>
				{
					SceneLoader.FilePreference.Value = "";

					if (SceneLoader.ZoneTypePreference.Value == SceneLoader.LoadSavedZone)
						SceneLoader.ZoneTypePreference.Value = SceneLoader.LoadActiveZone;
				})
				{
					text = "Clear",
					tooltip = "Stop using this save file"
				});


				_playingContainer.Add(new Button(() =>
				{
					WorldLoader.Instance.Save();
				})
				{
					text = "Save",
					tooltip = "Save the game"
				});

				fileContaier.Add(_editingContainer);
				fileContaier.Add(_playingContainer);
			}

			private void CreateZonePicker()
			{
				var zonePicker = ElementHelper.CreatePropertyContainer("Starting Zone", "The zone to start in when testing");
			}

			private void CreateSpawnPicker()
			{
				//if (SceneLoader.ZoneTypePreference.Value != SceneLoader.LoadSavedZone)

				var spawnPicker = ElementHelper.CreatePropertyContainer("Starting Spawn", "The spawn to spawn at when testing");

				var textField = new TextField { value = SceneLoader.SpawnPreference.Value };
				textField.RegisterValueChangedCallback(evt => SceneLoader.SpawnPreference.Value = evt.newValue);

				spawnPicker.Add(textField);
				Add(spawnPicker);
			}

			private void PlayStateChanged(PlayModeStateChange state)
			{
				if (state == PlayModeStateChange.EnteredPlayMode)
				{
					_editingContainer.style.display = DisplayStyle.None;
					_playingContainer.style.display = DisplayStyle.Flex;
				}
				else if (state == PlayModeStateChange.ExitingPlayMode)
				{
					_editingContainer.style.display = DisplayStyle.Flex;
					_playingContainer.style.display = DisplayStyle.None;
				}
			}
		}

		private static readonly GUIContent _activeZoneOptionContent = new GUIContent("Active Zone in Editor");
		private static readonly GUIContent _savedZoneOptionContent = new GUIContent("Saved Zone");

		[MenuItem("Window/OoT2D/State Manager")]
		public static void Open()
		{
			var window = GetWindow<StateWindow>("State Manager");
			window.rootVisualElement.Add(new StateWindowElement(window));
			window.Show();
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
	}
}
