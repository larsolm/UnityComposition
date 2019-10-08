using PiRhoSoft.Composition;
using PiRhoSoft.Utilities;
using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace PiRhoSoft.MonsterRpg
{
	[Serializable]
	public class Roster : SerializedList<CreatureReference>, IResettableVariables, IVariableArray
	{
		public List<Creature> Creatures { get; private set; }

		public void Setup()
		{
			foreach (var reference in this)
				reference.Setup();
		}

		public void CreateCreatures(ITrainer trainer)
		{
			Creatures = new List<Creature>(Count);

			foreach (var reference in this)
				Creatures.Add(reference.CreateCreature(trainer));
		}

		public void DestroyCreatures()
		{
			if (Creatures != null)
			{
				foreach (var creature in Creatures)
				{
					creature.Teardown();
					Object.Destroy(creature);
				}

				Creatures.Clear();
			}
		}

		public void AddCreature(Creature creature)
		{
			Creatures.Add(creature);
		}

		public void RemoveCreature(Creature creature)
		{
			TakeCreature(creature);
			creature.Teardown();
			Object.Destroy(creature);
		}

		public void TakeCreature(Creature creature)
		{
			for (var i = 0; i < Creatures.Count; i++)
			{
				if (Creatures[i] == creature)
				{
					Creatures.RemoveAt(i);
					break;
				}
			}
		}

		#region IVariableReset Implementation

		public void ResetTag(string tag)
		{
			foreach (var creature in Creatures)
				creature.ResetTag(tag);
		}

		public void ResetVariables(IList<string> traits)
		{
			foreach (var creature in Creatures)
				creature.ResetVariables(traits);
		}

		public void ResetAll()
		{
			foreach (var creature in Creatures)
				creature.ResetAll();
		}

		#endregion

		#region IVariableList Implementation

		public int VariableCount => Creatures.Count;
		public Variable GetVariable(int index) => index >= 0 && index < Creatures.Count ? Variable.Object(Creatures[index]) : Variable.Empty;
		public SetVariableResult SetVariable(int index, Variable value) => SetVariableResult.ReadOnly;

		public SetVariableResult AddVariable(Variable value)
		{
			if (value.TryGetObject<Creature>(out var creature))
			{
				AddCreature(creature);
				return SetVariableResult.Success;
			}

			return SetVariableResult.TypeMismatch;
		}

		public SetVariableResult RemoveVariable(int index)
		{
			if (index >= 0 && index < Creatures.Count)
			{
				RemoveCreature(Creatures[index]);
				Creatures.RemoveAt(index);
				return SetVariableResult.Success;
			}

			return SetVariableResult.NotFound;
		}

		#endregion
	}
}
