using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace PiRhoSoft.MonsterRpg.Engine
{
	[Serializable]
	public class WeightedTile
	{
		public int Weight = 1;
		public TileTransformInfo Info = new TileTransformInfo();
	}

	[Serializable]
	[CreateAssetMenu(fileName = nameof(WeightedRandomTile), menuName = "OoT2D/Tiles/Weighted Random Tile", order = 4)]
	public class WeightedRandomTile : TileBase
	{
		[Tooltip("The list of tiles this tile will choose to display randomly")]
		public List<WeightedTile> Tiles = new List<WeightedTile>();

		public override void GetTileData(Vector3Int location, ITilemap tileMap, ref TileData tileData)
		{
			base.GetTileData(location, tileMap, ref tileData);

			if (Tiles.Count > 0)
			{
				long hash = location.x;
				hash = hash + 0xabcd1234 + (hash << 15);
				hash = hash + 0x0987efab ^ (hash >> 11);
				hash ^= location.y;
				hash = hash + 0x46ac12fd + (hash << 7);
				hash = hash + 0xbe9730af ^ (hash << 11);
				UnityEngine.Random.InitState((int)hash);

				var totalWeight = 0;
				foreach (var tile in Tiles)
					totalWeight += tile.Weight;

				var randomWeight = UnityEngine.Random.Range(0, totalWeight);
				foreach (var tile in Tiles)
				{
					randomWeight -= tile.Weight;
					if (randomWeight < 0)
					{
						tileData.sprite = tile.Info.Sprite;
						tileData.transform = tile.Info.GetTransform();
						tileData.colliderType = Tile.ColliderType.None;
						tileData.flags = TileFlags.None;
						break;
					}
				}
			}
		}
	}
}