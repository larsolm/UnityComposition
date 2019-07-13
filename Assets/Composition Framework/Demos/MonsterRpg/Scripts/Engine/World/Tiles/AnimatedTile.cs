using PiRhoSoft.UtilityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace PiRhoSoft.MonsterRpg.Engine
{
	[CreateAssetMenu(fileName = nameof(AnimatedTile), menuName = "OoT2D/Tiles/Animated Tile", order = 1)]
	public class AnimatedTile : TileBase
	{
		[Tooltip("Whether the start time of this animation should be randomized")]
		public bool RandomizeStart = false;

		[Tooltip("The start time of this animation")]
		[Range(0, 1)]
		public float AnimationStartTime = 0.0f;

		[Tooltip("The minimum and maximum speed of this animation")]
		public float AnimationSpeed = 1.0f;
		public float AnimationSpeedMaximum = 1.0f;

		[Tooltip("The sequence of tile infos this tile will animate through")]
		public List<TileTransformInfo> Tiles = new List<TileTransformInfo>();

		public override void GetTileData(Vector3Int location, ITilemap tileMap, ref TileData tileData)
		{
			var tile = Tiles.FirstOrDefault();

			tileData.transform = tile.GetTransform();
			tileData.color = Color.white;
			tileData.sprite = tile.Sprite;
			tileData.colliderType = Tile.ColliderType.None;
		}

		public override bool GetTileAnimationData(Vector3Int location, ITilemap tileMap, ref TileAnimationData tileAnimationData)
		{
			tileAnimationData.animatedSprites = Tiles.Select(tile => tile.Sprite).ToArray();
			tileAnimationData.animationSpeed = GetSpeed();
			tileAnimationData.animationStartTime = GetStart();
			return true;
		}

		private float GetSpeed()
		{
			return Random.Range(AnimationSpeed, AnimationSpeedMaximum);
		}

		private float GetStart()
		{
			return RandomizeStart ? Random.Range(0.0f, 1.0f) : AnimationStartTime;
		}
	}
}