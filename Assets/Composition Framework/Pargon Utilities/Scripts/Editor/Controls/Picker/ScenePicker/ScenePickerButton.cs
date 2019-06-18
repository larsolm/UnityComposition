using PiRhoSoft.PargonUtilities.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public class ScenePickerButton : BasePickerButton<SceneAsset>
	{
		private const string _styleSheetPath = "Assets/PargonUtilities/Scripts/Editor/Controls/Picker/ScenePicker/ScenePickerButton.uss";

		private class Factory : UxmlFactory<ScenePickerButton, Traits> { }

		private class Traits : UxmlTraits
		{
			private UxmlStringAttributeDescription _path = new UxmlStringAttributeDescription { name = "scene-path" };

			public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
			{
				get { yield break; }
			}

			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);

				var scenePicker = ve as ScenePickerButton;
				var path = _path.GetValueFromBag(bag, cc);
				
				scenePicker.Setup(path, null);
			}
		}

		private Image _load;
		private Image _create;

		private MessageBox _buildWarning;

		public void Setup(SerializedProperty property, Action onCreate)
		{
			if (property.propertyType == SerializedPropertyType.String)
			{
				Setup(property.stringValue, onCreate);
				BindToProperty(property);
			}
			else if (property.propertyType == SerializedPropertyType.Integer)
			{
				Setup(property.intValue, onCreate);
				BindToProperty(property);
			}
			else // SceneReference
			{
				var pathProperty = property.FindPropertyRelative(nameof(SceneReference.Path));
				Setup(pathProperty.stringValue, onCreate);
				BindToProperty(pathProperty);
			}
		}

		public void Setup(int buildIndex, Action onCreate)
		{
			var scene = GetSceneFromBuildIndex(buildIndex);
			Setup(scene, onCreate);
		}

		public void Setup(string path, Action onCreate)
		{
			var scene = GetSceneFromPath(path);
			Setup(scene, onCreate);
		}

		public void Setup(SceneAsset scene, Action onCreate)
		{
			ElementHelper.AddStyleSheet(this, _styleSheetPath);

			var picker = new ObjectPicker();
			picker.Setup(typeof(SceneAsset), scene);
			picker.OnSelected += selectedObject => value = selectedObject as SceneAsset;

			_load = new Image { image = Icon.Load.Content, tintColor = Color.black };
			_load.AddManipulator(new Clickable(Load));

			_create = new Image { image = Icon.Add.Content };
			_create.AddManipulator(new Clickable(() => Create(onCreate)));

			_buildWarning = new MessageBox();
			_buildWarning.Setup(MessageBoxType.Info, "This scene is not in the build settings. Add it now?");
			_buildWarning.Add(new Button(AddToBuild) { text = "Add" });

			Setup(picker, scene);

			Add(_load);
			Add(_create);
			Add(_buildWarning);
		}

		private void Load()
		{
			var scene = SceneManager.GetSceneByName(value.name);

			if (scene.isLoaded)
				EditorSceneManager.CloseScene(scene, true);
			else
				EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(value), OpenSceneMode.Additive);

			Refresh();
		}

		private void Create(Action onCreate)
		{
			var scene = SceneHelper.CreateScene(onCreate);
			if (scene.IsValid())
				value = GetSceneFromPath(scene.path);
		}

		private void AddToBuild()
		{
			var scene = new EditorBuildSettingsScene(AssetDatabase.GetAssetPath(value), true);
			EditorBuildSettings.scenes = EditorBuildSettings.scenes.Append(scene).ToArray();

			Refresh();
		}

		#region BindableValueElement Implementation

		protected override void SetValueToProperty(SerializedProperty property, SceneAsset value)
		{
			var path = AssetDatabase.GetAssetPath(value);

			if (property.propertyType == SerializedPropertyType.String)
				property.stringValue = path;
			else if (property.propertyType == SerializedPropertyType.Integer)
			{
				var buildIndex = SceneUtility.GetBuildIndexByScenePath(path);
				if (buildIndex < 0)
				{
					AddToBuild();
					buildIndex = SceneUtility.GetBuildIndexByScenePath(path);
				}

				property.intValue = buildIndex;
			}
		}

		protected override SceneAsset GetValueFromProperty(SerializedProperty property)
		{
			if (property.propertyType == SerializedPropertyType.String)
				return GetSceneFromPath(property.stringValue);
			else if (property.propertyType == SerializedPropertyType.Integer)
				return GetSceneFromBuildIndex(property.intValue);
			else
				return null;
		}

		private SceneAsset GetSceneFromPath(string path)
		{
			return AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
		}

		private SceneAsset GetSceneFromBuildIndex(int index)
		{
			var path = SceneUtility.GetScenePathByBuildIndex(index);
			return GetSceneFromPath(path);
		}

		protected override void Refresh()
		{
			var text = value == null ? "None (Scene)" : value.name;
			var icon = value == null ? null : AssetPreview.GetMiniTypeThumbnail(typeof(SceneAsset));

			SetLabel(icon, text);

			if (value)
			{
				var path = AssetDatabase.GetAssetPath(value);
				var scene = SceneManager.GetSceneByPath(path);
				var buildIndex = SceneUtility.GetBuildIndexByScenePath(path);

				_load.SetEnabled(!scene.isLoaded || SceneManager.sceneCount > 1);
				_load.image = scene.isLoaded ? Icon.Unload.Content : Icon.Load.Content;

				ElementHelper.SetVisible(_buildWarning, buildIndex < 0);
			}
			else
			{
				_load.image = Icon.Load.Content;
				_load.SetEnabled(false);

				ElementHelper.SetVisible(_buildWarning, false);
			}
		}

		#endregion
	}
}
