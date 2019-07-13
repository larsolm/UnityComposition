using OoT2D;
using PiRhoSoft.UtilityEditor;
using UnityEditor;
using UnityEngine;

namespace OoT2DEditor
{
	[CustomEditor(typeof(CheckerboardTile))]
	public class CheckerboardTileEditor : Editor
	{
		private static readonly Label _firstContent = new Label(typeof(CheckerboardTile), nameof(CheckerboardTile.First));
		private static readonly Label _secondContent = new Label(typeof(CheckerboardTile), nameof(CheckerboardTile.Second));

		private CheckerboardTile _checkerboardTile;

		void OnEnable()
		{
			_checkerboardTile = target as CheckerboardTile;
		}

		public override void OnInspectorGUI()
		{
			using (new UndoScope(_checkerboardTile, false))
			{
				EditorGUILayout.LabelField(_firstContent.Content);
				_checkerboardTile.First = TileTransformInfoControl.Draw(GUIContent.none, _checkerboardTile.First);
				EditorGUILayout.LabelField(_secondContent.Content);
				_checkerboardTile.Second = TileTransformInfoControl.Draw(GUIContent.none, _checkerboardTile.Second);
			}
		}
	}
}