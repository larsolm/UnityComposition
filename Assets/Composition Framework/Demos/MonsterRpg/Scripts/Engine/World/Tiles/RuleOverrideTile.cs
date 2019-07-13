using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace PiRhoSoft.MonsterRpg.Engine
{
	[CreateAssetMenu(fileName = nameof(RuleOverrideTile), menuName = "OoT2D/Tiles/Rule Override Tile", order = 5)]
	public class RuleOverrideTile : TileBase
	{
		[Serializable]
		public class Rule
		{
			public TileTransformInfo Tile = new TileTransformInfo();

			[Tooltip("Whether this override should display a different tile asset")]
			public bool UseReference;

			[Tooltip("The tile asset to display")]
			public TileBase Reference = null;
		}

		[Tooltip("The RuleTile whose rules this tile should override")]
		public RuleTile OverrideTile;

		[Tooltip("The default rule to display if no other rules match")]
		public Rule DefaultRule = new Rule();

		[Tooltip("The rules to display instead of the overriden tile's rules")]
		public List<Rule> Rules = new List<Rule>();

		public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
		{
			var resolvedRule = DefaultRule;

			if (OverrideTile)
			{
				var index = OverrideTile.GetRuleIndex(position, tilemap, this);
				if (index >= 0 && index < Rules.Count)
					resolvedRule = Rules[index];
			}

			if (resolvedRule.UseReference)
			{
				resolvedRule.Reference.GetTileData(position, tilemap, ref tileData);
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
			var rule = DefaultRule;

			if (OverrideTile)
			{
				var index = OverrideTile.GetRuleIndex(position, tilemap, this);
				if (index >= 0 && index < Rules.Count)
					rule = Rules[index];
			}

			if (rule.UseReference)
				return rule.Reference.GetTileAnimationData(position, tilemap, ref tileAnimationData);

			return false;
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
	}
}