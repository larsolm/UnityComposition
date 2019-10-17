using PiRhoSoft.Composition;
using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace PiRhoSoft.MonsterRpg
{
	[CreateGraphNodeMenu("Monster RPG/Save Game", 1)]
	public class SaveGame : GraphNode
	{
		private const string _invalidSaveError = "(MNSGIS) Failed to write save file {0}: {1}";

		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The filename to save to")]
		public StringVariableSource Filename = new StringVariableSource();

		[Tooltip("A reference to the Player to save the data to")]
		[VariableConstraint(typeof(Player))]
		public VariableLookupReference Player = new VariableLookupReference();

		public override Color NodeColor => Colors.ExecutionLight;

		public override IEnumerator Run(IGraphRunner graph, IVariableMap variables)
		{
			if (variables.Resolve(this, Filename, out var filename) && variables.ResolveObject<Player>(this, Player, out var player))
			{
				var data = new SaveData();

				player.Save(data);
				Write(filename, data);
			}

			graph.GoTo(Next, nameof(Next));

			yield break;
		}

		private void Write<T>(string filename, T data)
		{
			try
			{
				var path = SaveData.GetSavePath(filename);
				var json = JsonUtility.ToJson(data, true);
				File.WriteAllText(path, json);
			}
			catch (Exception e)
			{
				Debug.LogErrorFormat(this, _invalidSaveError, filename, e.Message);
			}
		}
	}
}
