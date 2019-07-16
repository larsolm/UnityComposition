using PiRhoSoft.Composition;
using PiRhoSoft.Utilities;
using UnityEngine;

namespace PiRhoSoft.MonsterRpg
{
	public interface ITrainer : IVariableStore
	{
		string Name { get; }
		Roster Roster { get; }
	}

	[ExecuteInEditMode] // for OnEnable initialization of Roster, and Inventory
	[DisallowMultipleComponent]
	[HelpURL(MonsterRpg.DocumentationUrl + "trainer")]
	[AddComponentMenu("PiRho Soft/Ecosystem/Trainer")]
	public class Trainer : VariableSetComponent, ITrainer
	{
		[Tooltip("The creatures this trainer has")] [SerializeField] public Roster _roster = new Roster();

		protected override void OnEnable()
		{
			_roster.Setup();

			if (ApplicationHelper.IsPlaying)
				_roster.CreateCreatures(this);
		}

		public void OnDisable()
		{
			if (ApplicationHelper.IsPlaying)
				_roster.DestroyCreatures();
		}

		#region ITrainer Implementation
		
		public Roster Roster => _roster;

		public string Name
		{
			get
			{
				var player = GetComponent<Player>();
				if (player)
					return player.Name;

				var npc = GetComponent<Npc>();
				if (npc)
					return npc.Name;

				return string.Empty;
			}
		}

		#endregion
	}
}
