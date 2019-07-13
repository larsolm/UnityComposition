using OoT2D;
using PiRhoSoft.UtilityEditor;
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace OoT2DEditor
{
	[CustomEditor(typeof(RuleTile))]
	public class RuleTileEditor : Editor
	{
		private static readonly Label _createRulesButton = new Label(Icon.BuiltIn(Icon.Edit), tooltip: "Create or set a rule for each sprite in the default tile's sprite sheet");
		private static readonly Label _addRuleButton = new Label(Icon.BuiltIn(Icon.Add), tooltip: "Add a rule to the list");
		private static readonly Label _removeRuleButton = new Label(Icon.BuiltIn(Icon.Remove), tooltip: "Remove this rule");
		private static readonly Label _addNeighborTypeButton = new Label(Icon.BuiltIn(Icon.Add), tooltip: "Add a neightbor type to the list");
		private static readonly Label _removeNeightborTypeButton = new Label(Icon.BuiltIn(Icon.Remove), tooltip: "Remove this neighbor type");
		private static readonly Label _rulesContent = new Label(typeof(RuleTile), nameof(RuleTile.Rules));
		private static readonly Label _neighborTypesContent = new Label(typeof(RuleTile), nameof(RuleTile.NeighborTypes));
		private static readonly GUIContent _neighborTypesEmptyContent = new GUIContent("Add neighboring tile types to test positive for");
		private static readonly Label _defaultRuleContent = new Label(typeof(RuleTile), nameof(RuleTile.DefaultRule));
		private static readonly Label _isReferenceContent = new Label(typeof(RuleTile.Rule), nameof(RuleTile.Rule.UseReference));
		private static readonly GUIContent _referenceContent = new GUIContent("", Label.GetTooltip(typeof(RuleTile.Rule), nameof(RuleTile.Rule.Reference)));
		private static readonly GUIContent _thisContent = new GUIContent("", "The adjacent tile must be this tile or one the neighbor types");
		private static readonly GUIContent _notThisContent = new GUIContent("", "The adjacent tile is not this tile");
		private static readonly GUIContent _anyContent = new GUIContent("", "The adjacent tile can be anything");

		private const string _xIconData = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAABoSURBVDhPnY3BDcAgDAOZhS14dP1O0x2C/LBEgiNSHvfwyZabmV0jZRUpq2zi6f0DJwdcQOEdwwDLypF0zHLMa9+NQRxkQ+ACOT2STVw/q8eY1346ZlE54sYAhVhSDrjwFymrSFnD2gTZpls2OvFUHAAAAABJRU5ErkJggg==";
		private const string _arrow0Data = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAACYSURBVDhPzZExDoQwDATzE4oU4QXXcgUFj+YxtETwgpMwXuFcwMFSRMVKKwzZcWzhiMg91jtg34XIntkre5EaT7yjjhI9pOD5Mw5k2X/DdUwFr3cQ7Pu23E/BiwXyWSOxrNqx+ewnsayam5OLBtbOGPUM/r93YZL4/dhpR/amwByGFBz170gNChA6w5bQQMqramBTgJ+Z3A58WuWejPCaHQAAAABJRU5ErkJggg==";
		private const string _arrow1Data = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAABqSURBVDhPxYzBDYAgEATpxYcd+PVr0fZ2siZrjmMhFz6STIiDs8XMlpEyi5RkO/d66TcgJUB43JfNBqRkSEYDnYjhbKD5GIUkDqRDwoH3+NgTAw+bL/aoOP4DOgH+iwECEt+IlFmkzGHlAYKAWF9R8zUnAAAAAElFTkSuQmCC";
		private const string _arrow2Data = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAAC0SURBVDhPjVE5EsIwDMxPKFKYF9CagoJH8xhaMskLmEGsjOSRkBzYmU2s9a58TUQUmCH1BWEHweuKP+D8tphrWcAHuIGrjPnPNY8X2+DzEWE+FzrdrkNyg2YGNNfRGlyOaZDJOxBrDhgOowaYW8UW0Vau5ZkFmXbbDr+CzOHKmLinAXMEePyZ9dZkZR+s5QX2O8DY3zZ/sgYcdDqeEVp8516o0QQV1qeMwg6C91toYoLoo+kNt/tpKQEVvFQAAAAASUVORK5CYII=";
		private const string _arrow3Data = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAAB2SURBVDhPzY1LCoAwEEPnLi48gW5d6p31bH5SMhp0Cq0g+CCLxrzRPqMZ2pRqKG4IqzJc7JepTlbRZXYpWTg4RZE1XAso8VHFKNhQuTjKtZvHUNCEMogO4K3BhvMn9wP4EzoPZ3n0AGTW5fiBVzLAAYTP32C2Ay3agtu9V/9PAAAAAElFTkSuQmCC";
		private const string _arrow5Data = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAABqSURBVDhPnY3BCYBADASvFx924NevRdvbyoLBmNuDJQMDGjNxAFhK1DyUQ9fvobCdO+j7+sOKj/uSB+xYHZAxl7IR1wNTXJeVcaAVU+614uWfCT9mVUhknMlxDokd15BYsQrJFHeUQ0+MB5ErsPi/6hO1AAAAAElFTkSuQmCC";
		private const string _arrow6Data = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAACaSURBVDhPxZExEkAwEEVzE4UiTqClUDi0w2hlOIEZsV82xCZmQuPPfFn8t1mirLWf7S5flQOXjd64vCuEKWTKVt+6AayH3tIa7yLg6Qh2FcKFB72jBgJeziA1CMHzeaNHjkfwnAK86f3KUafU2ClHIJSzs/8HHLv09M3SaMCxS7ljw/IYJWzQABOQZ66x4h614ahTCL/WT7BSO51b5Z5hSx88AAAAAElFTkSuQmCC";
		private const string _arrow7Data = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAABQSURBVDhPYxh8QNle/T8U/4MKEQdAmsz2eICx6W530gygr2aQBmSMphkZYxqErAEXxusKfAYQ7XyyNMIAsgEkaYQBkAFkaYQBsjXSGDAwAAD193z4luKPrAAAAABJRU5ErkJggg==";
		private const string _arrow8Data = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAACYSURBVDhPxZE9DoAwCIW9iUOHegJXHRw8tIdx1egJTMSHAeMPaHSR5KVQ+KCkCRF91mdz4VDEWVzXTBgg5U1N5wahjHzXS3iFFVRxAygNVaZxJ6VHGIl2D6oUXP0ijlJuTp724FnID1Lq7uw2QM5+thoKth0N+GGyA7IA3+yM77Ag1e2zkey5gCdAg/h8csy+/89v7E+YkgUntOWeVt2SfAAAAABJRU5ErkJggg==";
		private const string _flipXData = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAAOwQAADsEBuJFr7QAAABh0RVh0U29mdHdhcmUAcGFpbnQubmV0IDQuMC41ZYUyZQAAAG1JREFUOE+lj9ENwCAIRB2IFdyRfRiuDSaXAF4MrR9P5eRhHGb2Gxp2oaEjIovTXSrAnPNx6hlgyCZ7o6omOdYOldGIZhAziEmOTSfigLV0RYAB9y9f/7kO8L3WUaQyhCgz0dmCL9CwCw172HgBeyG6oloC8fAAAAAASUVORK5CYII=";
		private const string _flipYData = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAAOwgAADsIBFShKgAAAABh0RVh0U29mdHdhcmUAcGFpbnQubmV0IDQuMC41ZYUyZQAAAG9JREFUOE+djckNACEMAykoLdAjHbPyw1IOJ0L7mAejjFlm9hspyd77Kk+kBAjPOXcakJIh6QaKyOE0EB5dSPJAiUmOiL8PMVGxugsP/0OOib8vsY8yYwy6gRyC8CB5QIWgCMKBLgRSkikEUr5h6wOPWfMoCYILdgAAAABJRU5ErkJggg==";
		private const string _rotatedData = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAAOwQAADsEBuJFr7QAAABh0RVh0U29mdHdhcmUAcGFpbnQubmV0IDQuMC41ZYUyZQAAAHdJREFUOE+djssNwCAMQxmIFdgx+2S4Vj4YxWlQgcOT8nuG5u5C732Sd3lfLlmPMR4QhXgrTQaimUlA3EtD+CJlBuQ7aUAUMjEAv9gWCQNEPhHJUkYfZ1kEpcxDzioRzGIlr0Qwi0r+Q5rTgM+AAVcygHgt7+HtBZs/2QVWP8ahAAAAAElFTkSuQmCC";

		private static Texture2D[] _textures;
		public static Texture2D[] Textures
		{
			get
			{
				if (_textures == null)
				{
					_textures = new Texture2D[12] {
						Base64ToTexture(_arrow0Data), // 0 Top Left
						Base64ToTexture(_arrow1Data), // 1 Top Center
						Base64ToTexture(_arrow2Data), // 2 Top Right
						Base64ToTexture(_arrow3Data), // 3 Left
						Base64ToTexture(_rotatedData), // 4 Center
						Base64ToTexture(_arrow5Data), // 5 Right
						Base64ToTexture(_arrow6Data), // 6 Bottom Left
						Base64ToTexture(_arrow7Data), // 7 Bottom Center
						Base64ToTexture(_arrow8Data), // 8 Bottom Right
						Base64ToTexture(_xIconData), // 9
						Base64ToTexture(_flipXData), // 10
						Base64ToTexture(_flipYData), // 11
					};
				}
				return _textures;
			}
		}

		private static Texture2D Base64ToTexture(string base64)
		{
			Texture2D texture = new Texture2D(1, 1);
			texture.hideFlags = HideFlags.HideAndDontSave;
			texture.LoadImage(Convert.FromBase64String(base64));
			return texture;
		}

		private RuleTile _ruleTile;

		private ObjectListControl _rulesControl = new ObjectListControl();
		private ObjectListControl _neighborTypesControl = new ObjectListControl();

		private bool _needsUpdate;

		void OnEnable()
		{
			_ruleTile = target as RuleTile;

			_neighborTypesControl.Setup(_ruleTile.NeighborTypes)
				.MakeDrawable(DrawNeighbor)
				.MakeAddable(_addNeighborTypeButton, OnAddNeighbor)
				.MakeRemovable(_removeNeightborTypeButton, OnRemoveNeighbor)
				.MakeEmptyLabel(_neighborTypesEmptyContent);

			_rulesControl.Setup(_ruleTile.Rules)
				.MakeDrawable(DrawRule)
				.MakeAddable(_addRuleButton, OnAddRule)
				.MakeRemovable(_removeRuleButton, OnRemoveRule)
				.MakeReorderable(OnReorder)
				.MakeCustomHeight(GetElementHeight)
				.MakeHeaderButton(_createRulesButton, CreateRules, Color.white);
		}

		public override void OnInspectorGUI()
		{
			_needsUpdate = false;

			using (var changes = new EditorGUI.ChangeCheckScope())
			{
				using (new UndoScope(_ruleTile, false))
				{
					DrawDefaultRule();

					EditorGUILayout.Space();

					_neighborTypesControl.Draw(_neighborTypesContent.Content);
					_rulesControl.Draw(_rulesContent.Content);

					if (changes.changed)
						_needsUpdate = true;
				}
			}

			if (_needsUpdate)
			{
				EditorUtility.SetDirty(_ruleTile);
				SceneView.RepaintAll();
			}
		}

		private void DrawNeighbor(Rect rect, IList list, int index)
		{
			_ruleTile.NeighborTypes[index] = EditorGUI.ObjectField(rect, _ruleTile.NeighborTypes[index], typeof(TileBase), false) as TileBase;
		}

		private float GetElementHeight(int index)
		{
			return EditorGUIUtility.singleLineHeight * 4 + RectHelper.VerticalSpace * 3;
		}

		private void DrawRule(Rect rect, IList list, int index)
		{
			var rule = _ruleTile.Rules[index];

			var width = EditorGUIUtility.singleLineHeight * 3 + RectHelper.VerticalSpace * 2;
			var referenceRect = RectHelper.TakeLine(ref rect);
			var isReferenceRect = RectHelper.TakeWidth(ref referenceRect, referenceRect.width * 0.5f - RectHelper.HorizontalSpace);
			RectHelper.TakeHorizontalSpace(ref referenceRect);

			var matrixRect = RectHelper.TakeWidth(ref rect, width);
			RectHelper.TakeHorizontalSpace(ref rect);

			DrawRuleMatrix(_ruleTile, matrixRect, rule);

			rule.UseReference = EditorGUI.Toggle(isReferenceRect, _isReferenceContent.Content, rule.UseReference);

			if (rule.UseReference)
			{
				rule.Tile.Sprite = null;
				rule.Reference = EditorGUI.ObjectField(referenceRect, _referenceContent, rule.Reference, typeof(TileBase), false) as TileBase;

				if (rule.Reference == _ruleTile)
					rule.Reference = null;
			}
			else
			{
				rule.Tile = TileTransformInfoControl.Draw(rect, null, rule.Tile);
				rule.Reference = null;
			}
		}

		private void DrawDefaultRule()
		{
			EditorGUILayout.LabelField(_defaultRuleContent.Content);

			_ruleTile.DefaultRule.UseReference = EditorGUILayout.Toggle(_isReferenceContent.Content, _ruleTile.DefaultRule.UseReference);

			if (_ruleTile.DefaultRule.UseReference)
			{
				_ruleTile.DefaultRule.Tile.Sprite = null;
				_ruleTile.DefaultRule.Reference = EditorGUILayout.ObjectField(_referenceContent, _ruleTile.DefaultRule.Reference, typeof(TileBase), false) as TileBase;

				if (_ruleTile.DefaultRule.Reference == _ruleTile)
					_ruleTile.DefaultRule.Reference = null;
			}
			else
			{
				_ruleTile.DefaultRule.Tile = TileTransformInfoControl.Draw(null, _ruleTile.DefaultRule.Tile);
				_ruleTile.DefaultRule.Reference = null;
			}
		}

		private void CreateRules(Rect rect)
		{
			if (!_ruleTile.DefaultRule.UseReference && _ruleTile.DefaultRule.Tile.Sprite)
			{
				var path = AssetDatabase.GetAssetPath(_ruleTile.DefaultRule.Tile.Sprite);
				var children = AssetDatabase.LoadAllAssetsAtPath(path);

				var count = 0;
				foreach (var child in children)
				{
					if (child is Sprite sprite)
					{
						count++;

						if (count > _ruleTile.Rules.Count)
							_ruleTile.Rules.Add(new RuleTile.Rule());

						var rule = _ruleTile.Rules[count - 1];
						rule.UseReference = false;
						rule.Reference = null;
						rule.Tile.Sprite = sprite;
					}
				}

				_needsUpdate = true;
			}
		}

		private void OnAddRule(IList list)
		{
			_ruleTile.Rules.Add(new RuleTile.Rule());
			_needsUpdate = true;
		}


		private void OnAddNeighbor(IList list)
		{
			_neighborTypesControl.DoDefaultAdd();
			_needsUpdate = true;
		}

		private void OnRemoveRule(IList list, int index)
		{
			_rulesControl.DoDefaultRemove(index);
			_needsUpdate = true;
		}

		private void OnRemoveNeighbor(IList list, int index)
		{
			_neighborTypesControl.DoDefaultRemove(index);
			_needsUpdate = true;
		}

		private void OnReorder(int oldIndex, int newIndex)
		{
			_needsUpdate = true;
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
				HandleHelper.DrawLine(new Vector2(left, rect.yMin), new Vector2(left, rect.yMax), color);
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

						if (Event.current.type == EventType.MouseDown && r.Contains(Event.current.mousePosition))
						{
							if (Event.current.button == 0)
							{
								rule.Neighbors[index] = (RuleTile.NeighborType)Mathf.Repeat((int)currentRule + 1, (int)RuleTile.NeighborType.Count);

								GUI.changed = true;
								Event.current.Use();
							}
						}

						index++;
					}
					else
					{
						if (!rule.UseReference)
						{
							if (rule.Tile.FlipHorizontal)
								GUI.DrawTexture(r, Textures[10]);

							if (rule.Tile.FlipVertical)
								GUI.DrawTexture(r, Textures[11]);

							if (rule.Tile.Rotation != 0)
								GUI.DrawTexture(r, Textures[5]);
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
				case RuleTile.NeighborType.This: GUI.DrawTexture(rect, Textures[pos.y * 3 + pos.x]); break;
				case RuleTile.NeighborType.NotThis: GUI.DrawTexture(rect, Textures[9]); break;
			}
		}
	}
}
