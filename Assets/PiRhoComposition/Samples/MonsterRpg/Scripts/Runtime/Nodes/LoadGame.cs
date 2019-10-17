using PiRhoSoft.Composition;
using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace PiRhoSoft.MonsterRpg
{
	[CreateGraphNodeMenu("Monster RPG/Load Game", 0)]
	public class LoadGame : GraphNode
	{
		private const string _invalidLoadError = "(MNLGIL) Failed to load save file {0}: {1}";

		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The filename to load")]
		public StringVariableSource Filename = new StringVariableSource();

		[Tooltip("A reference to the Player to load the data into")]
		[VariableConstraint(typeof(Player))]
		public VariableLookupReference Player = new VariableLookupReference();

		public override Color NodeColor => Colors.ExecutionLight;

		public override IEnumerator Run(IGraphRunner graph, IVariableMap variables)
		{
			if (variables.Resolve(this, Filename, out var filename) && variables.ResolveObject<Player>(this, Player, out var player))
			{
				var data = new SaveData();

				if (!string.IsNullOrEmpty(filename))
					Read(filename, data);

				player.Load(data);
			}

			graph.GoTo(Next, nameof(Next));
			yield break;
		}

		private void Read<T>(string filename, T data)
		{
			try
			{
				var path = SaveData.GetSavePath(filename);
				if (File.Exists(path))
				{
					var json = File.ReadAllText(path);
					JsonUtility.FromJsonOverwrite(json, data);
				}
			}
			catch (Exception e)
			{
				Debug.LogErrorFormat(this, _invalidLoadError, filename, e.Message);
			}
		}
	}
}
