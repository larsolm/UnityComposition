using PiRhoSoft.Composition;
using PiRhoSoft.Utilities;
using UnityEngine;

namespace PiRhoSoft.MonsterRpg
{
	[ExecuteInEditMode] // for OnEnable initialization of Roster
	[DisallowMultipleComponent]
	[AddComponentMenu("Monster RPG/Trainer")]
	public class Trainer : SchemaVariableComponent
	{
		[Tooltip("The name of this trainer")]
		public string Name = string.Empty;

		[Tooltip("The creatures this trainer has")]
		public Roster Roster = new Roster();

		protected void OnEnable()
		{
			Roster.Setup();

			if (ApplicationHelper.IsPlaying)
				Roster.CreateCreatures(this);
		}

		public void OnDisable()
		{
			if (ApplicationHelper.IsPlaying)
				Roster.DestroyCreatures();
		}
	}
}
