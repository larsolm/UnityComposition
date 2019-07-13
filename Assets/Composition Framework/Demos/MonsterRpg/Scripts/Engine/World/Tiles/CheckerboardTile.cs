using UnityEngine;
using UnityEngine.Tilemaps;

namespace PiRhoSoft.MonsterRpg.Engine
{
	[CreateAssetMenu(fileName = nameof(CheckerboardTile), menuName = "OoT2D/Tiles/Checkerboard Tile", order = 2)]
	public class CheckerboardTile : TileBase
	{
		[Tooltip("The sprite to use for the first squares")] public TileTransformInfo First;
		[Tooltip("The sprite to use for the second squares")] public TileTransformInfo Second;

		public override void GetTileData(Vector3Int position, ITilemap tileMap, ref TileData tileData)
		{
			var info = GetInfo(position);

			tileData.transform = info.GetTransform();
			tileData.color = Color.white;
			tileData.sprite = info.Sprite;
			tileData.colliderType = Tile.ColliderType.None;
		}

		public TileTransformInfo GetInfo(Vector3Int position)
		{
			if (position.y % 2 == 0)
				return position.x % 2 == 0 ? First : Second;
			else
				return position.x % 2 == 0 ? Second : First;
		}
	}
}
