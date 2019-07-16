using PiRhoSoft.Composition;
using PiRhoSoft.Utilities;
using System;
using UnityEngine;

namespace PiRhoSoft.MonsterRpg
{
	[Serializable]
	public class SkillsDictionary : SerializedDictionary<string, int> { }

	[Serializable]
	public class SkillList : SerializedList<Skill>
	{
		public Skill Find(string name)
		{
			foreach (var skill in this)
			{
				if (skill.Name == name)
					return skill;
			}

			return null;
		}
	}

	[Serializable]
	public class Skill
	{
		[Tooltip("The name of this Skill")]
		public string Name;

		[Tooltip("The maximum number of times a Creature can learn this Skill")]
		public int LearnLimit = 1;
		
		[Tooltip("The condition that must be met in order for a Creature to learn this Skill")]
		public Expression Condition = new Expression();

		[Tooltip("The instruction to run when teaching this Skill")]
		public GraphCaller Graph = new GraphCaller();
	}
}
