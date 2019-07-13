using OoT2D;
using PiRhoSoft.UtilityEditor;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace OoT2DEditor
{
	[CustomEditor(typeof(RuleOverrideTile))]
	class RuleOverrideTileEditor : Editor
	{
		private static readonly Label _createRulesIcon = new Label(Icon.BuiltIn(Icon.Edit), tooltip: "Set the sprite for each rule for each sprite in the default tile's sprite sheet");
		private static readonly Label _overrideTileContent = new Label(typeof(RuleOverrideTile), nameof(RuleOverrideTile.OverrideTile));
		private static readonly Label _defaultRuleContent = new Label(typeof(RuleOverrideTile), nameof(RuleOverrideTile.DefaultRule));
		private static readonly Label _rulesContent = new Label(typeof(RuleOverrideTile), nameof(RuleOverrideTile.Rules));
		private static readonly Label _isReferenceContent = new Label(typeof(RuleOverrideTile.Rule), nameof(RuleOverrideTile.Rule.UseReference));
		private static readonly GUIContent _referenceContent = new GUIContent("", Label.GetTooltip(typeof(RuleOverrideTile.Rule), nameof(RuleOverrideTile.Rule.Reference)));

		private RuleOverrideTile _ruleOverrideTile;
		private ObjectListControl _rulesControl = new ObjectListControl();

		private bool _needsUpdate;

		void OnEnable()
		{
			_ruleOverrideTile = target as RuleOverrideTile;

			UpdateOverrides();
		}

		public override void OnInspectorGUI()
		{
			_needsUpdate = false;

			using (var changes = new EditorGUI.ChangeCheckScope())
			{
				using (new UndoScope(_ruleOverrideTile, false))
				{
					var overrideTile = EditorGUILayout.ObjectField(_overrideTileContent.Content, _ruleOverrideTile.OverrideTile, typeof(RuleTile), false) as RuleTile;
					if (overrideTile != _ruleOverrideTile.OverrideTile)
					{
						_ruleOverrideTile.OverrideTile = overrideTile;
						UpdateOverrides();
					}

					DrawDefaultRule();

					EditorGUILayout.Space();

					_rulesControl.Draw(_rulesContent.Content);

					if (changes.changed)
						_needsUpdate = true;
				}
			}

			if (_needsUpdate)
			{
				EditorUtility.SetDirty(_ruleOverrideTile);
				SceneView.RepaintAll();
			}
		}

		private void UpdateOverrides()
		{
			if (_ruleOverrideTile.OverrideTile)
			{
				if (_ruleOverrideTile.OverrideTile.Rules.Count > _ruleOverrideTile.Rules.Count)
					_ruleOverrideTile.Rules.AddRange(Enumerable.Repeat(new RuleOverrideTile.Rule(), _ruleOverrideTile.OverrideTile.Rules.Count - _ruleOverrideTile.Rules.Count));
				else
					_ruleOverrideTile.Rules.RemoveRange(_ruleOverrideTile.OverrideTile.Rules.Count, _ruleOverrideTile.Rules.Count - _ruleOverrideTile.OverrideTile.Rules.Count);
			}
			else
			{
				_ruleOverrideTile.Rules.Clear();
			}

			_rulesControl = new ObjectListControl();
			_rulesControl.Setup(_ruleOverrideTile.Rules)
				.MakeDrawable(DrawRule)
				.MakeCustomHeight(GetElementHeight)
				.MakeHeaderButton(_createRulesIcon, CreateRules, Color.white);
		}

		private float GetElementHeight(int index)
		{
			return EditorGUIUtility.singleLineHeight * 4 + RectHelper.VerticalSpace * 3;
		}

		private void DrawDefaultRule()
		{
			EditorGUILayout.LabelField(_defaultRuleContent.Content);

			_ruleOverrideTile.DefaultRule.UseReference = EditorGUILayout.Toggle(_isReferenceContent.Content, _ruleOverrideTile.DefaultRule.UseReference);

			if (_ruleOverrideTile.DefaultRule.UseReference)
			{
				_ruleOverrideTile.DefaultRule.Tile.Sprite = null;
				_ruleOverrideTile.DefaultRule.Reference = EditorGUILayout.ObjectField(_referenceContent, _ruleOverrideTile.DefaultRule.Reference, typeof(TileBase), false) as TileBase;

				if (_ruleOverrideTile.DefaultRule.Reference == _ruleOverrideTile)
					_ruleOverrideTile.DefaultRule.Reference = null;
			}
			else
			{
				_ruleOverrideTile.DefaultRule.Tile = TileTransformInfoControl.Draw(null, _ruleOverrideTile.DefaultRule.Tile);
				_ruleOverrideTile.DefaultRule.Reference = null;
			}
		}

		private void DrawRule(Rect rect, IList list, int index)
		{
			var rule = _ruleOverrideTile.Rules[index];
			var overrideRule = _ruleOverrideTile.OverrideTile.Rules[index];

			var width = EditorGUIUtility.singleLineHeight * 3 + RectHelper.VerticalSpace * 2;
			var referenceRect = RectHelper.TakeLine(ref rect);
			var isReferenceRect = RectHelper.TakeWidth(ref referenceRect, referenceRect.width * 0.5f - RectHelper.HorizontalSpace);
			RectHelper.TakeHorizontalSpace(ref referenceRect);

			var matrixRect = RectHelper.TakeWidth(ref rect, width);
			RectHelper.TakeHorizontalSpace(ref rect);

			DrawRuleMatrix(_ruleOverrideTile.OverrideTile, matrixRect, overrideRule);

			rule.UseReference = EditorGUI.Toggle(isReferenceRect, _isReferenceContent.Content, rule.UseReference);

			if (rule.UseReference)
			{
				rule.Tile.Sprite = null;
				rule.Reference = EditorGUI.ObjectField(referenceRect, _referenceContent, rule.Reference, typeof(TileBase), false) as TileBase;

				if (rule.Reference == _ruleOverrideTile)
					rule.Reference = null;
			}
			else
			{
				rule.Tile = TileTransformInfoControl.Draw(rect, null, rule.Tile);
				rule.Reference = null;
			}
		}

		private void DrawRuleMatrix(RuleTile tile, Rect rect, RuleTile.Rule rule)
		{
			var index = 0;
			var w = rect.width / 3;
			var h = rect.height / 3;
			var color = new Color(0, 0, 0, 0.2f);

			for (var y = 0; y <= 3; y++)
			{
				var top = rect.yMin + y * h;
				HandleHelper.DrawLine(new Vector2(rect.xMin, top), new Vector2(rect.xMax, top), color);
			}

			for (var x = 0; x <= 3; x++)
			{
				var left = rect.xMin + x * w;
				HandleHelper.DrawLine(new Vector3(left, rect.yMin), new Vector3(left, rect.yMax), color);
			}

			for (var y = 0; y <= 2; y++)
			{
				for (var x = 0; x <= 2; x++)
				{
					var r = new Rect(rect.xMin + x * w, rect.yMin + y * h, w - 1, h - 1);

					if (x != 1 || y != 1)
					{
						var currentRule = rule.Neighbors[index];

						DrawRule(tile, r, new Vector2Int(x, y), currentRule);

						index++;
					}
					else
					{
						if (!rule.UseReference)
						{
							if (rule.Tile.FlipHorizontal)
								GUI.DrawTexture(r, RuleTileEditor.Textures[10]);

							if (rule.Tile.FlipVertical)
								GUI.DrawTexture(r, RuleTileEditor.Textures[11]);

							if (rule.Tile.Rotation != 0)
								GUI.DrawTexture(r, RuleTileEditor.Textures[5]);
						}
					}
				}
			}
		}

		private void DrawRule(RuleTile tile, Rect rect, Vector2Int pos, RuleTile.NeighborType neighbor)
		{
			switch (neighbor)
			{
				case RuleTile.NeighborType.Any: break;
				case RuleTile.NeighborType.This: GUI.DrawTexture(rect, RuleTileEditor.Textures[pos.y * 3 + pos.x]); break;
				case RuleTile.NeighborType.NotThis: GUI.DrawTexture(rect, RuleTileEditor.Textures[9]); break;
			}
		}

		private void CreateRules(Rect rect)
		{
			if (!_ruleOverrideTile.DefaultRule.UseReference && _ruleOverrideTile.DefaultRule.Tile.Sprite)
			{
				var path = AssetDatabase.GetAssetPath(_ruleOverrideTile.DefaultRule.Tile.Sprite);
				var children = AssetDatabase.LoadAllAssetsAtPath(path);

				var count = 0;
				foreach (var child in children)
				{
					if (child is Sprite sprite)
					{
						count++;

						if (count > _ruleOverrideTile.Rules.Count())
							break;

						var rule = _ruleOverrideTile.Rules[count - 1];
						rule.UseReference = false;
						rule.Reference = null;
						rule.Tile.Sprite = sprite;
					}
				}

				_needsUpdate = true;
			}
		}
	}
}