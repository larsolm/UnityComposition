using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace PiRhoSoft.MonsterRpg.Engine
{
	[CreateAssetMenu(fileName = nameof(RuleTile), menuName = "OoT2D/Tiles/Rule Tile", order = 4)]
	public class RuleTile : TileBase
	{
		[Serializable]
		public class Rule
		{
			public NeighborType[] Neighbors = new NeighborType[_neighborCount];

			public TileTransformInfo Tile = new TileTransformInfo();

			[Tooltip("Whether this rule should display a different tile asset")]
			public bool UseReference = false;

			[Tooltip("The tile asset to display")]
			public TileBase Reference = null;
		}

		public enum NeighborType
		{
			Any,
			This,
			NotThis,
			Count
		}

		[Tooltip("The rule to follow when none of the other rules match")]
		public Rule DefaultRule = new Rule();

		[Tooltip("The neighbor tile types that this tile will check for neighbors against in adition to itself")]
		public List<TileBase> NeighborTypes = new List<TileBase>();

		[Tooltip("The rules for this tile to follow")]
		public List<Rule> Rules = new List<Rule>();

		private const int _neighborCount = 8;

		private TileBase[] _cachedNeighbors = new TileBase[_neighborCount];

		public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
		{
			var resolvedRule = DefaultRule;

			if (Rules.Count > 0)
			{
				TileBase[] neighbors = null;

				GetNeighbors(tilemap, position, ref neighbors);

				foreach (var rule in Rules)
				{
					if (RuleMatches(rule, neighbors, this))
					{
						resolvedRule = rule;
						break;
					}
				}
			}

			if (resolvedRule.UseReference)
			{
					resolvedRule.Reference?.GetTileData(position, tilemap, ref tileData);
			}
			else
			{
				tileData.transform = resolvedRule.Tile.GetTransform();
				tileData.sprite = resolvedRule.Tile.Sprite;
				tileData.colliderType = Tile.ColliderType.None;
				tileData.flags = TileFlags.None;
			}
		}

		public override bool GetTileAnimationData(Vector3Int position, ITilemap tilemap, ref TileAnimationData tileAnimationData)
		{
			if (Rules.Count > 0)
			{
				TileBase[] neighbors = null;

				GetNeighbors(tilemap, position, ref neighbors);

				foreach (var rule in Rules)
				{
					if (RuleMatches(rule, neighbors, this))
					{
						if (rule.UseReference)
							return rule.Reference.GetTileAnimationData(position, tilemap, ref tileAnimationData);

						return false;
					}
				}
			}

			return false;
		}

		private void GetNeighbors(ITilemap tilemap, Vector3Int position, ref TileBase[] neighbors)
		{
			if (neighbors != null)
				return;

			if (_cachedNeighbors == null || _cachedNeighbors.Length < _neighborCount)
				_cachedNeighbors = new TileBase[_neighborCount];

			var index = 0;
			for (var y = 1; y >= -1; y--)
			{
				for (var x = -1; x <= 1; x++)
				{
					if (x != 0 || y != 0)
						_cachedNeighbors[index++] = tilemap.GetTile(new Vector3Int(position.x + x, position.y + y, position.z));
				}
			}

			neighbors = _cachedNeighbors;
		}

		public override void RefreshTile(Vector3Int location, ITilemap tileMap)
		{
			if (Rules.Count > 0)
			{
				for (var y = -1; y <= 1; y++)
				{
					for (var x = -1; x <= 1; x++)
						base.RefreshTile(location + new Vector3Int(x, y, 0), tileMap);
				}
			}
			else
			{
				base.RefreshTile(location, tileMap);
			}
		}

		public int GetRuleIndex(Vector3Int position, ITilemap tilemap, TileBase referenceTile)
		{
			if (Rules.Count > 0)
			{
				TileBase[] neighbors = null;

				GetNeighbors(tilemap, position, ref neighbors);

				for (var i = 0; i < Rules.Count; i++)
				{
					if (RuleMatches(Rules[i], neighbors, referenceTile))
						return i;
				}
			}

			return -1;
		}

		private bool RuleMatches(Rule rule, TileBase[] neighbors, TileBase referenceTile)
		{
			for (var i = 0; i < neighbors.Length; i++)
			{
				var tile = neighbors[i];
				var type = rule.Neighbors[i];

				if (!RuleMatches(type, tile, referenceTile))
					return false;
			}

			return true;
		}

		private bool RuleMatches(NeighborType neighbor, TileBase tile, TileBase referenceTile)
		{
			switch (neighbor)
			{
				case NeighborType.This: return CheckNeighborType(tile, referenceTile);
				case NeighborType.NotThis: return !CheckNeighborType(tile, referenceTile);
			}

			return true;
		}

		private bool CheckNeighborType(TileBase tile, TileBase referenceTile)
		{
			if (tile == null)
				return false;

			if (tile == this || tile == referenceTile)
				return true;

			foreach (var type in NeighborTypes)
			{
				if (type == tile)
					return true;
			}

			return false;
		}
	}
}