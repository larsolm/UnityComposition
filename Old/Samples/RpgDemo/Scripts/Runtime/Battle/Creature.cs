using PiRhoSoft.Composition;
using PiRhoSoft.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.MonsterRpg
{
	[Serializable]
	public class CreatureSaveData
	{
		public string SpeciesPath = string.Empty;
		public string Name = string.Empty;
		public SerializedVariableDictionary Variables = new SerializedVariableDictionary();
		public SkillsDictionary Skills = new SkillsDictionary();
		public List<MoveSaveData> Moves = new List<MoveSaveData>();
	}

	[Serializable]
	public class CreatureReference : IVariableMap
	{
		private const string _missingSpeciesWarning = "(ECRMS) A CreatureReference on '{0}' has not been assigned a Species";

		public Creature Creature;
		public Species Species;
		public GraphCaller GenerateGraph = new GraphCaller();

		public void Setup()
		{
			if (Creature)
				Creature.Setup(null);
		}

		public Creature CreateCreature(Trainer trainer)
		{
			if (Creature && Creature.Species)
			{
				return Creature.Clone(trainer);
			}
			else if (Species)
			{
				var creature = Species.CreateCreature(trainer);

				if (GenerateGraph.Graph)
					CompositionManager.Instance.RunGraph(GenerateGraph, Variable.Object(Creature));

				return creature;
			}
			else
			{
				Debug.LogWarningFormat(_missingSpeciesWarning, (trainer as Object)?.name);
				return null;
			}
		}

		#region IVariableMap Implementation

		public Variable GetVariable(string name) => Creature && Creature.Species ? Creature.GetVariable(name) : Species ? Species.GetVariable(name) : Variable.Empty;
		public SetVariableResult SetVariable(string name, Variable value) => Creature && Creature.Species ? Creature.SetVariable(name, value) : Species ? Species.SetVariable(name, value) : SetVariableResult.NotFound;
		public IReadOnlyList<string> VariableNames => Creature && Creature.Species ? Creature.VariableNames : Species ? Species.VariableNames : VariableDictionary.EmptyNames;

		#endregion
	}

	public class Creature : SchemaVariableAsset
	{
		private const string _missingAbilityWarning = "(ECMA) The Ability '{0}' could not be found";
		private const string _deletedSpeciesWarning = "(ECDS) The Species for Creature '{0}' has been deleted";

		[Tooltip("The Species this creature is an instance of")]
		[ReadOnly]
		public Species Species;

		[Tooltip("The name of this creature")]
		public string Name;

		[Tooltip("The moves this creature has learned")]
		public MoveList Moves = new MoveList();

		private SkillsDictionary _skills = new SkillsDictionary();
		private List<int> _pendingSkills = new List<int>();

		public Trainer Trainer { get; private set; }

		public void Setup(Trainer trainer)
		{
			Trainer = trainer;
			Moves.Setup(this);
		}

		public void Teardown()
		{
			foreach (var move in Moves)
				Destroy(move);
		}

		public Creature Clone(Trainer trainer)
		{
			var creature = Species.CreateCreature(trainer);

			creature.Name = Name;
			//creature.Variables.CopyFrom(Variables, null);

			foreach (var move in Moves)
			{
				var m = move.Clone(this);
				creature.Moves.AddVariable(Variable.Object(move));
			}

			foreach (var skill in _skills)
				creature._skills.Add(skill.Key, skill.Value);

			return creature;
		}

		#region Skills

		public bool HasPendingSkill()
		{
			return _pendingSkills.Count > 0;
		}

		public Skill TakePendingSkill()
		{
			if (HasPendingSkill())
			{
				var index = _pendingSkills[0];
				_pendingSkills.RemoveAt(0);
				return Species.Skills[index];
			}
			else
			{
				return null;
			}
		}

		public List<int> TakePendingSkills()
		{
			if (HasPendingSkill())
			{
				var skills = _pendingSkills;
				_pendingSkills = new List<int>();
				return skills;
			}
			else
			{
				return null;
			}
		}

		public void TeachPendingSkills()
		{
			var skills = TakePendingSkills();

			if (skills != null)
			{
				foreach (var skill in skills)
					TeachSkill(skill);
			}
		}

		public void TeachSkill(int index)
		{
			var skill = Species.Skills[index];

			if (CanLearnSkill(skill))
				TeachSkill(skill);
		}

		public bool CanLearnSkill(Skill skill)
		{
			var learnCount = _skills.TryGetValue(skill.Name, out int count) ? count : 0;
			return skill.LearnLimit <= 0 || learnCount < skill.LearnLimit ? skill.Condition.Execute(this, this, VariableType.Bool).AsBool : false;
		}

		public void TeachSkill(Skill skill)
		{
			CompositionManager.Instance.RunGraph(skill.Graph, Variable.Object(this));
			SkillLearned(skill);
		}

		private void SkillLearned(Skill skill)
		{
			_skills[skill.Name] = _skills.TryGetValue(skill.Name, out int count) ? count + 1 : 1;
		}

		#endregion

		#region Persistence

		public static Creature Create(CreatureSaveData data, Trainer trainer)
		{
			var species = Resources.Load<Species>(data.SpeciesPath);

			if (species != null)
			{
				var creature = species.CreateCreature(trainer);
				creature.Load(data, null);
				return creature;
			}

			return null;
		}

		public static CreatureSaveData Save(Creature creature, string tag)
		{
			var data = new CreatureSaveData { SpeciesPath = creature.Species ? creature.Species.name : string.Empty };
			creature.Save(data, tag);
			return data;
		}

		private void Load(CreatureSaveData data, string tag)
		{
			Name = data.Name;
			_skills = data.Skills;
			//Variables.CopyFrom(data.Variables, tag);

			foreach (var moveData in data.Moves)
			{
				var move = Move.Create(this, moveData, tag);

				if (move != null)
					Moves.AddVariable(Variable.Object(move));
				else
					Debug.LogWarningFormat(this, _missingAbilityWarning, Name);
			}
		}

		private void Save(CreatureSaveData data, string tag)
		{
			data.Name = Name;
			data.Skills = _skills;
			//Variables.CopyTo(data.Variables, tag);

			foreach (var move in Moves)
				data.Moves.Add(Move.Save(move, tag));
		}

		#endregion
	}
}
