using PiRhoSoft.Composition;
using PiRhoSoft.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.MonsterRpg
{
	[Serializable]
	public class MoveSaveData
	{
		public string AbilityPath = string.Empty;
		public string Name = string.Empty;
		public VariableStore Variables = new VariableStore();
	}

	[Serializable]
	public class MoveList : VariableList, IResettableVariables, IEnumerable<Move>
	{
		public void Setup(Creature creature)
		{
			foreach (var value in Values)
			{
				if (value.TryGetObject<Move>(out var move))
					move.Setup(creature);
			}
		}

		public void ResetTag(string tag)
		{
			foreach (var value in Values)
			{
				if (value.TryGetObject<Move>(out var move))
					move.ResetTag(tag);
			}
		}

		public void ResetVariables(IList<string> traits)
		{
			foreach (var value in Values)
			{
				if (value.TryGetObject<Move>(out var move))
					move.ResetVariables(traits);
			}
		}

		public void ResetAll()
		{
			foreach (var value in Values)
			{
				if (value.TryGetObject<Move>(out var move))
					move.ResetAll();
			}
		}

		IEnumerator<Move> IEnumerable<Move>.GetEnumerator()
		{
			for (var i = 0; i < VariableCount; i++)
				yield return Values[i].AsObject as Move;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			for (var i = 0; i < VariableCount; i++)
				yield return Values[i].AsObject;
		}
	}

	[HelpURL(MonsterRpg.DocumentationUrl + "move")]
	public class Move : VariableSetAsset
	{
		private const string _deletedAbilityWarning = "(EMDA) The Ability for Move '{0}' has been deleted";

		[Tooltip("The Ability this move is an instance of")] [ReadOnly] public Ability Ability;
		[Tooltip("The creature that has this move")] [ReadOnly] public Creature Creature;

		[MappedVariable] public string Name => Ability.Name;

		public void Setup(Creature creature)
		{
			if (Ability == null)
				Debug.LogWarningFormat(this, _deletedAbilityWarning, Name);

			Creature = creature;
		}

		public Move Clone(Creature creature)
		{
			var move = Ability.CreateMove(creature);
			move.Variables.CopyFrom(Variables, null);
			return move;
		}

		#region Persistence

		public static Move Create(Creature creature, MoveSaveData data, string tag)
		{
			var ability = Resources.Load<Ability>(data.AbilityPath);

			if (ability != null)
			{
				var move = ability.CreateMove(creature);
				move.Load(data, tag);
				return move;
			}

			return null;
		}

		public static MoveSaveData Save(Move move, string tag)
		{
			var data = new MoveSaveData { AbilityPath = move.Ability ? move.Ability.Path : string.Empty };
			move.Save(data, tag);
			return data;
		}

		public void Load(MoveSaveData data, string tag)
		{
			Variables.CopyFrom(data.Variables, tag);
		}

		public void Save(MoveSaveData data, string tag)
		{
			Variables.CopyTo(data.Variables, tag);
		}

		#endregion
	}
}
