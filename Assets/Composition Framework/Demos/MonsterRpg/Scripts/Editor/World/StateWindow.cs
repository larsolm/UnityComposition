using PiRhoSoft.Composition;
using PiRhoSoft.Utilities.Editor;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.MonsterRpg.Editor
{
	public class StateWindow : EditorWindow
	{
		private class StateWindowElement : VisualElement
		{
			public StateWindowElement(Object owner)
			{
				CreateMainScenePicker(owner);
				CreateLoadGraphPicker(owner);
				CreateSaveFilePicker();
				CreateSpawnZonePicker(owner);
			}

			private void CreateMainScenePicker(Object owner)
			{
				var mainSceneContainer = new FieldContainer("Main Scene", "The main scene to load when the game starts up");
				var mainScenePicker = new ScenePickerControl(SceneLoader.MainScenePreference.Value, () =>
				{
					var gameObject = new GameObject("World Manager");
					gameObject.AddComponent<WorldManager>();
					gameObject.AddComponent<AudioManager>();
				});

				mainScenePicker.RegisterCallback<ChangeEvent<string>>(evt => SceneLoader.MainScenePreference.Value = evt.newValue);
				mainSceneContainer.Add(mainScenePicker);
				Add(mainSceneContainer);
			}

			private void CreateLoadGraphPicker(Object owner)
			{
				var loadGraphContainer = new FieldContainer("Load Graph", "The graph to run when the game starts up");
				var graphPicker = new ObjectPickerControl(AssetDatabase.LoadAssetAtPath<Graph>(SceneLoader.LoadGraphPreference.Value), typeof(Graph));
				graphPicker.RegisterCallback<ChangeEvent<Object>>(evt =>
				{
					SceneLoader.LoadGraphPreference.Value = AssetDatabase.GetAssetPath(evt.newValue);
				});

				loadGraphContainer.Add(graphPicker);
				Add(loadGraphContainer);
			}

			private void CreateSaveFilePicker()
			{
				var fileContaier = new FieldContainer("Save File", "The save file to load when testing the game");

				var editingContainer = new VisualElement();
				var playingContainer = new VisualElement();

				editingContainer.Add(new Button(() =>
				{
					var path = EditorUtility.SaveFilePanel("Create Save File", Application.persistentDataPath, "Editor Save", "json");

					if (!string.IsNullOrEmpty(path))
					{
						File.WriteAllText(path, "{}");
						SceneLoader.FilePreference.Value = path;
					}
				})
				{
					text = "New",
					tooltip = "Create a new save file"
				});

				editingContainer.Add(new Button(() =>
				{
					var path = EditorUtility.OpenFilePanel("Open Save File", Application.persistentDataPath, "json");

					if (!string.IsNullOrEmpty(path))
						SceneLoader.FilePreference.Value = path;
				})
				{
					text = "Open",
					tooltip = "Open an existing save file"
				});

				editingContainer.Add(new Button(() =>
				{
					SceneLoader.FilePreference.Value = "";
				})
				{
					text = "Clear",
					tooltip = "Stop using this save file"
				});


				playingContainer.Add(new Button(() =>
				{
					WorldLoader.Instance.Save();
				})
				{
					text = "Save",
					tooltip = "Save the game"
				});

				fileContaier.Add(editingContainer);
				fileContaier.Add(playingContainer);

				EditorApplication.playModeStateChanged += state =>
				{
					editingContainer.SetDisplayed(state == PlayModeStateChange.EnteredEditMode || state == PlayModeStateChange.ExitingPlayMode);
					playingContainer.SetDisplayed(state == PlayModeStateChange.EnteredPlayMode || state == PlayModeStateChange.ExitingEditMode);
				};
			}

			private void CreateSpawnZonePicker(Object owner)
			{
				var spawnPicker = new FieldContainer("Starting Spawn", "The spawn to spawn at when testing");

				var textField = new TextField { value = SceneLoader.SpawnPreference.Value };
				textField.style.flexGrow = 1;
				textField.RegisterValueChangedCallback(evt => SceneLoader.SpawnPreference.Value = evt.newValue);

				var zoneContainer = new FieldContainer("Starting Zone", "The zone to start in when testing");

				var typeContainer = new VisualElement();
				typeContainer.style.flexGrow = 1;

				var zonePicker = new ObjectPickerField();//(owner, () => AssetDatabase.LoadAssetAtPath<Zone>(SceneLoader.ZonePreference.Value), zone => SceneLoader.ZonePreference.Value = AssetDatabase.GetAssetPath(zone));
				//zonePicker.Setup(typeof(Zone), SceneLoader.ZonePreference.Value);

				var buttons = new EnumButtonsField();//(owner, () => (SceneLoader.ZoneLoadType)SceneLoader.ZoneTypePreference.Value, value =>
				//{
				//	var type = (SceneLoader.ZoneLoadType)value;
				//	SceneLoader.ZoneTypePreference.Value = (int)type;
				//
				//	ElementHelper.SetVisible(zonePicker, type == SceneLoader.ZoneLoadType.Specific);
				//	ElementHelper.SetVisible(spawnPicker, type != SceneLoader.ZoneLoadType.Saved);
				//});

				zonePicker.SetDisplayed((SceneLoader.ZoneLoadType)SceneLoader.ZoneTypePreference.Value == SceneLoader.ZoneLoadType.Specific);
				spawnPicker.SetDisplayed((SceneLoader.ZoneLoadType)SceneLoader.ZoneTypePreference.Value != SceneLoader.ZoneLoadType.Saved);

				//buttons.Setup(typeof(SceneLoader.ZoneLoadType), false, (SceneLoader.ZoneLoadType)SceneLoader.ZoneTypePreference.Value);

				zoneContainer.Add(typeContainer);
				typeContainer.Add(buttons);
				typeContainer.Add(zonePicker);

				spawnPicker.Add(textField);
				Add(zoneContainer);
				Add(spawnPicker);
			}
		}

		private static readonly Icon _stateIcon = Icon.BuiltIn("SavePassive");

		[UnityEditor.MenuItem("Window/Monster RPG/State Manager")]
		public static void Open()
		{
			var window = GetWindow<StateWindow>("State Manager");
			window.titleContent.image = _stateIcon.Texture;
			window.Show();
		}

		private void OnEnable()
		{
			rootVisualElement.Add(new StateWindowElement(this));
			rootVisualElement.style.paddingTop = 5;
			rootVisualElement.style.paddingRight = 5;
			rootVisualElement.style.paddingBottom = 5;
			rootVisualElement.style.paddingLeft = 5;
		}
	}
}
