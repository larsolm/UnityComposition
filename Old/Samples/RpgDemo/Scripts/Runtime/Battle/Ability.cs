using PiRhoSoft.Composition;
using System;
using UnityEngine;

namespace PiRhoSoft.MonsterRpg
{
	public enum AbilityUseLocation
	{
		World,
		Battle
	}

	[Serializable]
	public class AbilityVariableSource : VariableSource<Ability> { }

	[CreateAssetMenu(menuName = "Monster RPG/Ability", fileName = nameof(Ability), order = 202)]
	public class Ability : SchemaVariableAsset
	{
		public virtual Move CreateMove(Creature creature)
		{
			var move = CreateInstance<Move>();
			move.Ability = this;
			move.name = name;
			move.Setup(creature);
			return move;
		}
	}
}
