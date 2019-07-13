using OoT2D;
using PiRhoSoft.UtilityEditor;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace OoT2DEditor
{
	[CustomEditor(typeof(AnimatedTile))]
	public class AnimatedTileEditor : Editor
	{
		private static readonly Label _createTilesIcon = new Label(Icon.BuiltIn(Icon.Edit), tooltip: "Add all the sprites from the same sheet as the first sprite");
		private static readonly Label _addTileButton = new Label(Icon.BuiltIn(Icon.Add), tooltip: "Add a tile to the list");
		private static readonly Label _removeTileButton = new Label(Icon.BuiltIn(Icon.Remove), tooltip: "Remove this tile from the list");
		private static readonly Label _tilesContent = new Label(typeof(AnimatedTile), nameof(AnimatedTile.Tiles));

		private AnimatedTile _animatedTile;
		private SerializedProperty _randomizeStartProperty;
		private SerializedProperty _animationStartProperty;
		private SerializedProperty _animationSpeedProperty;

		private PropertyListControl _tiles = new PropertyListControl();

		void OnEnable()
		{
			_animatedTile = target as AnimatedTile;

			_randomizeStartProperty = serializedObject.FindProperty(nameof(AnimatedTile.RandomizeStart));
			_animationSpeedProperty = serializedObject.FindProperty(nameof(AnimatedTile.AnimationSpeed));
			_animationStartProperty = serializedObject.FindProperty(nameof(AnimatedTile.AnimationStartTime));

			_tiles.Setup(serializedObject.FindProperty(nameof(_animatedTile.Tiles)))
				.MakeAddable(_addTileButton)
				.MakeRemovable(_removeTileButton)
				.MakeHeaderButton(_createTilesIcon, CreateTiles, Color.white)
				.MakeCustomHeight(GetHeight)
				.MakeReorderable();
		}

		public override void OnInspectorGUI()
		{
			using (new UndoScope(serializedObject))
			{
				EditorGUILayout.PropertyField(_randomizeStartProperty);

				if (!_randomizeStartProperty.boolValue)
					EditorGUILayout.PropertyField(_animationStartProperty);

				EditorGUILayout.PropertyField(_animationSpeedProperty);

				_tiles.Draw(_tilesContent.Content);
			}
		}

		private float GetHeight(int index)
		{
			return TileTransformInfoControl.Height;
		}

		private void CreateTiles(Rect rect)
		{
			var reference = _animatedTile.Tiles.FirstOrDefault().Sprite;
			if (reference)
			{
				var path = AssetDatabase.GetAssetPath(reference.texture);
				var children = AssetDatabase.LoadAllAssetsAtPath(path);

				_animatedTile.Tiles.Clear();

				foreach (var child in children)
				{
					if (child is Sprite sprite)
						_animatedTile.Tiles.Add(new TileTransformInfo { Sprite = sprite });
				}
			}
		}
	}
}