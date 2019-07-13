using PiRhoSoft.Composition.Engine;
using PiRhoSoft.Utilities.Engine;
using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace PiRhoSoft.MonsterRpg.Engine
{
	[Serializable]
	public class Roster : SerializedList<CreatureReference>, IVariableReset, IVariableList
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

		#endregion

		#region IVariableList Implementation

		public VariableValue GetVariable(int index) => index >= 0 && index < Creatures.Count ? VariableValue.CreateReference(Creatures[index]) : VariableValue.Empty;
		public SetVariableResult SetVariable(int index, VariableValue value) => SetVariableResult.ReadOnly;
		public SetVariableResult AddVariable(VariableValue value)
		{
			if (value.TryGetReference<Creature>(out var creature))
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
