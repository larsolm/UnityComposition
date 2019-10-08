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

		[Tooltip("The tag of the variables to save")]
		public StringVariableSource Tag = new StringVariableSource();

		public override Color NodeColor => Colors.ExecutionLight;

		public override IEnumerator Run(IGraphRunner graph, IVariableCollection variables)
		{
			if (variables.Resolve(this, Tag, out var tag))
			{
				var data = new SaveData { Game = new GameSaveData(), World = new WorldSaveData(), Player = new PlayerSaveData() };
				data.Game.PlayerSpawn.Direction = Player.Instance.Mover.MovementDirection;
				data.Game.PlayerSpawn.Position = Player.Instance.transform.position;
				data.Game.PlayerSpawn.Layer = Player.Instance.Mover.MovementLayer;

				Player.Instance.Save(data.Player, tag);
				var filename = WorldManager.Instance.Save(data.World, tag);

				Write(filename, data);
			}

			graph.GoTo(Next, nameof(Next));

			yield break;
		}

		private void Write<T>(string filename, T data)
		{
			try
			{
				var path = WorldManager.Instance.GetSavePath(filename);
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
