using PiRhoSoft.PargonUtilities.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public class ScenePicker : BasePickerButton<SceneAsset>
	{
		private class Picker : BasePicker<SceneAsset>
		{
			public void Setup(SceneAsset value)
			{
				var assets = AssetHelper.GetAssetList(typeof(SceneAsset));
				CreateTree(assets.Type.Name, assets.Paths, assets.Assets.Cast<SceneAsset>().ToList(), value, asset =>
				{
					var icon = AssetPreview.GetMiniThumbnail(asset);
					return icon == null && asset ? AssetPreview.GetMiniTypeThumbnail(asset.GetType()) : icon;
				});
			}
		}

		private const string _styleSheetPath = Utilities.AssetPath + "Controls/Picker/ScenePicker/ScenePicker.uss";

		public ScenePicker() { }
		public ScenePicker(SerializedProperty property) : base(property) { }
		public ScenePicker(Object owner, Func<SceneAsset> getValue, Action<SceneAsset> setValue) : base(owner, getValue, setValue) { }

		private Image _load;
		private Image _create;

		private MessageBox _buildWarning;

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

		public void Setup(SceneAsset value, Action onCreate)
		{
			ElementHelper.AddStyleSheet(this, _styleSheetPath);

			var picker = new Picker();
			picker.Setup(value);
			picker.OnSelected += selectedObject =>
			{
				Value = selectedObject as SceneAsset;
				ElementHelper.SendChangeEvent(this, value, Value);
			};

			_load = new Image { image = Icon.Load.Content, tintColor = Color.black };
			_load.AddManipulator(new Clickable(Load));

			_create = new Image { image = Icon.Add.Content };
			_create.AddManipulator(new Clickable(() => Create(onCreate)));

			_buildWarning = new MessageBox();
			_buildWarning.Setup(MessageBoxType.Info, "This scene is not in the build settings. Add it now?");
			_buildWarning.Add(new Button(AddToBuild) { text = "Add" });

			Setup(picker, value);

			Add(_load);
			Add(_create);
			Add(_buildWarning);
		}

		private void Load()
		{
			var scene = SceneManager.GetSceneByName(Value.name);

			if (scene.isLoaded)
				EditorSceneManager.CloseScene(scene, true);
			else
				EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(Value), OpenSceneMode.Additive);

			UpdateElement(Value);
		}

		private void Create(Action onCreate)
		{
			var scene = SceneHelper.CreateScene(onCreate);
			if (scene.IsValid())
				Value = GetSceneFromPath(scene.path);
		}

		private void AddToBuild()
		{
			var scene = new EditorBuildSettingsScene(AssetDatabase.GetAssetPath(Value), true);
			EditorBuildSettings.scenes = EditorBuildSettings.scenes.Append(scene).ToArray();

			UpdateElement(Value);
		}

		public override SceneAsset GetValueFromProperty(SerializedProperty property)
		{
			if (property.propertyType == SerializedPropertyType.String)
				return GetSceneFromPath(property.stringValue);
			else if (property.propertyType == SerializedPropertyType.Integer)
				return GetSceneFromBuildIndex(property.intValue);
			else
				return null;
		}

		public override void UpdateProperty(SceneAsset value, VisualElement element, SerializedProperty property)
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

		protected override void UpdateElement(SceneAsset value)
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

		private SceneAsset GetSceneFromPath(string path)
		{
			return AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
		}

		private SceneAsset GetSceneFromBuildIndex(int index)
		{
			var path = SceneUtility.GetScenePathByBuildIndex(index);
			return GetSceneFromPath(path);
		}

		#region UXML

		private class Factory : UxmlFactory<ScenePicker, Traits> { }

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

				var button = ve as ScenePicker;
				var path = _path.GetValueFromBag(bag, cc);

				button.Setup(path, null);
			}
		}

		#endregion
	}
}
