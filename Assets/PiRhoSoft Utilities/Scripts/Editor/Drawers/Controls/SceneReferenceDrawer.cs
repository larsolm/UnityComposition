using System;
using System.Reflection;
using PiRhoSoft.UtilityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PiRhoSoft.UtilityEditor
{
	public class SceneReferenceMaintainer : UnityEditor.AssetModificationProcessor
	{
		private static AssetDeleteResult OnWillDeleteAsset(string path, RemoveAssetOptions options)
		{
			SceneReference.SceneMoved?.Invoke(path, string.Empty);
			return AssetDeleteResult.DidNotDelete;
		}

		private static AssetMoveResult OnWillMoveAsset(string sourcePath, string destinationPath)
		{
			SceneReference.SceneMoved?.Invoke(sourcePath, destinationPath);
			return AssetMoveResult.DidNotMove;
		}
	}

	[CustomPropertyDrawer(typeof(SceneReference))]
	public class SceneReferenceDrawer : PropertyDrawer
	{
		public static readonly Label _loadSceneButton = new Label(Icon.BuiltIn(Icon.Load), "", "Load the selected scene");
		public static readonly Label _unloadSceneButton = new Label(Icon.BuiltIn(Icon.Unload), "", "Unload the selected scene");
		public static readonly Label _refreshScenesButton = new Label(Icon.BuiltIn(Icon.Refresh), "", "Refresh the list of scenes");

		private static SceneReference _temporary = new SceneReference();

		public static void Draw(SceneReference scene, GUIContent label, AssetLocation location, string newSceneName, Action creator)
		{
			var rect = EditorGUILayout.GetControlRect();
			Draw(rect, scene, label, location, newSceneName, creator);
		}

		public static void Draw(Rect position, SceneReference scene, GUIContent label, AssetLocation location, string newSceneName, Action creator)
		{
			var rect = EditorGUI.PrefixLabel(position, label);
			var refreshRect = RectHelper.TakeTrailingIcon(ref rect);

			if (GUI.Button(refreshRect, _refreshScenesButton.Content, GUIStyle.none))
				SceneHelper.RefreshLists();

			var list = SceneHelper.GetSceneList(true, location != AssetLocation.None);
			var index = list.GetIndex(scene.Path);

			if (index != 0 && scene.IsAssigned)
			{
				var loadRect = RectHelper.TakeTrailingIcon(ref rect);
				var s = scene.Scene;

				using (ColorScope.ContentColor(Color.black))
				{
					if (s.IsValid() && s.isLoaded)
					{
						if (GUI.Button(loadRect, _unloadSceneButton.Content, GUIStyle.none))
						{
							if (EditorSceneManager.SaveModifiedScenesIfUserWantsTo(new Scene[] { s }))
								EditorSceneManager.CloseScene(s, true);
						}
					}
					else
					{
						if (GUI.Button(loadRect, _loadSceneButton.Content, GUIStyle.none))
						{
							s = EditorSceneManager.OpenScene(scene.Path, OpenSceneMode.Additive);
							SceneManager.SetActiveScene(s);
						}
					}
				}
			}

			var thumbnail = AssetPreview.GetMiniTypeThumbnail(typeof(SceneAsset));
			var start = scene.Path.LastIndexOf('/') + 1;
			var popupLabel = index != 0 && scene.IsAssigned ? new GUIContent(scene.Path.Substring(start, scene.Path.Length - start - 6), thumbnail) : new GUIContent("None");

			var selection = SelectionPopup.Draw(rect, popupLabel, new SelectionState { Tab = 0, Index = index }, list.Tree);

			if (selection.Tab == 0 && selection.Index != index)
			{
				scene.Path = list.GetPath(selection.Index);
			}
			else if (selection.Tab == 1)
			{
				var newScene = SceneHelper.CreateScene(location, newSceneName, creator);
				scene.Path = newScene.path;
			}
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var attribute = TypeHelper.GetAttribute<SceneReferenceAttribute>(fieldInfo);
			var pathProperty = property.FindPropertyRelative(nameof(SceneReference.Path));

			label.tooltip = Label.GetTooltip(fieldInfo);
			_temporary.Path = pathProperty.stringValue;

			if (attribute != null)
			{
				var method = fieldInfo.DeclaringType.GetMethod(attribute.Creator, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				void creator() => method?.Invoke(method.IsStatic ? null : property.serializedObject.targetObject, null);

				Draw(position, _temporary, label, attribute.SaveLocation, attribute.DefaultName, creator);
			}
			else
			{
				Draw(position, _temporary, label, AssetLocation.None, null, null);
			}

			pathProperty.stringValue = _temporary.Path;
		}
	}
}
