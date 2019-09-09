using PiRhoSoft.Composition;
using PiRhoSoft.Utilities;
using UnityEngine;

namespace PiRhoSoft.MonsterRpg
{
	public interface ITrainer : IVariableCollection
	{
		string Name { get; }
		Roster Roster { get; }
	}

	[ExecuteInEditMode] // for OnEnable initialization of Roster
	[DisallowMultipleComponent]
	[HelpURL(MonsterRpg.DocumentationUrl + "trainer")]
	[AddComponentMenu("PiRho Soft/Ecosystem/Trainer")]
	public class Trainer : VariableSetComponent, ITrainer
	{
		[Tooltip("The name of this trainer")] [SerializeField] public string _name = string.Empty;
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
		public string Name => _name;

		#endregion
	}
}
