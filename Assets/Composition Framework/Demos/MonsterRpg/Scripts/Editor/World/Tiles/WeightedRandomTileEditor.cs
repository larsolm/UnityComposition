using OoT2D;
using PiRhoSoft.UtilityEditor;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace OoT2DEditor
{
	[CustomEditor(typeof(WeightedRandomTile))]
	public class WeightedRandomTileEditor : Editor
	{
		private static readonly Label _createTilesButton = new Label(Icon.BuiltIn(Icon.Edit), tooltip: "Add all the sprites from the same sheet as the first sprite");
		private static readonly Label _addTileButton = new Label(Icon.BuiltIn(Icon.Add), tooltip: "Add a tile to the list");
		private static readonly Label _removeTileButton = new Label(Icon.BuiltIn(Icon.Remove), tooltip: "Remove this tile from the list");
		private static readonly Label _tilesContent = new Label(typeof(WeightedRandomTile), nameof(WeightedRandomTile.Tiles));

		private WeightedRandomTile _weightedRandomTile;
		private PropertyListControl _tiles = new PropertyListControl();

		void OnEnable()
		{
			_weightedRandomTile = target as WeightedRandomTile;
			
			_tiles.Setup(serializedObject.FindProperty(nameof(_weightedRandomTile.Tiles)))
				.MakeDrawable(DrawTile)
				.MakeAddable(_addTileButton)
				.MakeRemovable(_removeTileButton)
				.MakeHeaderButton(_createTilesButton, CreateTiles, Color.white)
				.MakeCustomHeight(GetHeight)
				.MakeReorderable();
		}

		public override void OnInspectorGUI()
		{
			using (new UndoScope(serializedObject))
				_tiles.Draw(_tilesContent.Content);
		}

		private void DrawTile(Rect rect, SerializedProperty property, int index)
		{
			var element = property.GetArrayElementAtIndex(index);
			var weight = element.FindPropertyRelative(nameof(WeightedTile.Weight));
			var info = element.FindPropertyRelative(nameof(WeightedTile.Info));

			var weightRect = RectHelper.TakeLine(ref rect);

			EditorGUI.PropertyField(weightRect, weight);
			EditorGUI.PropertyField(rect, info, GUIContent.none);
		}

		private float GetHeight(int index)
		{
			return TileTransformInfoControl.Height + RectHelper.LineHeight;
		}

		private void CreateTiles(Rect rect)
		{
			var reference = _weightedRandomTile.Tiles.FirstOrDefault().Info.Sprite;
			if (reference)
			{
				var path = AssetDatabase.GetAssetPath(reference.texture);
				var children = AssetDatabase.LoadAllAssetsAtPath(path);

				_weightedRandomTile.Tiles.Clear();

				foreach (var child in children)
				{
					if (child is Sprite sprite)
						_weightedRandomTile.Tiles.Add(new WeightedTile { Info = new TileTransformInfo { Sprite = sprite } });
				}
			}
		}
	}
}