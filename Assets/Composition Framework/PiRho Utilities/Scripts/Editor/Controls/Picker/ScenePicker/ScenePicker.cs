using PiRhoSoft.PargonUtilities.Engine;
using System;
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
		private class SceneProvider : PickerProvider<SceneAsset> { }

		private const string _styleSheetPath = Utilities.AssetPath + "Controls/Picker/ScenePicker/ScenePicker.uss";

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

			_load = new Image { image = Icon.Load.Content, tintColor = Color.black };
			_load.AddManipulator(new Clickable(Load));

			_create = new Image { image = Icon.Add.Content };
			_create.AddManipulator(new Clickable(() => Create(onCreate)));

			_buildWarning = new MessageBox(MessageBoxType.Info, "This scene is not in the build settings. Add it now?");
			_buildWarning.Add(new Button(AddToBuild) { text = "Add" });
	
			var assets = AssetHelper.GetAssetList(typeof(SceneAsset));
			var provider = ScriptableObject.CreateInstance<SceneProvider>();
			provider.Setup(assets.Type.Name, assets.Paths.Prepend("None").ToList(), assets.Assets.Prepend(null).Cast<SceneAsset>().ToList(), asset =>
			{
				var icon = AssetPreview.GetMiniThumbnail(asset);
				return icon == null && asset ? AssetPreview.GetMiniTypeThumbnail(asset.GetType()) : icon;
			},
			selectedObject => ElementHelper.SendChangeEvent(this, value, selectedObject));

			Setup(provider, value);

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

			Refresh();
		}

		private void Create(Action onCreate)
		{
			var scene = SceneHelper.CreateScene(onCreate);
			if (scene.IsValid())
				ElementHelper.SendChangeEvent(this, Value, GetSceneFromPath(scene.path));
		}

		private void AddToBuild()
		{
			var scene = new EditorBuildSettingsScene(AssetDatabase.GetAssetPath(Value), true);
			EditorBuildSettings.scenes = EditorBuildSettings.scenes.Append(scene).ToArray();

			Refresh();
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

		protected override void Refresh()
		{
			var text = Value == null ? "None (Scene)" : Value.name;
			var icon = Value == null ? null : AssetPreview.GetMiniTypeThumbnail(typeof(SceneAsset));

			SetLabel(icon, text);

			if (Value)
			{
				var path = AssetDatabase.GetAssetPath(Value);
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
	}
}
