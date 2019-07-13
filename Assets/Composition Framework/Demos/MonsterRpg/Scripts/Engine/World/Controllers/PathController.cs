using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[HelpURL(MonsterRpg.DocumentationUrl + "path-controller")]
	[AddComponentMenu("PiRho Soft/Controllers/Path Controller")]
	public class PathController : Controller
	{
		[Tooltip("Whether to start this path when the controller awakes or not")]
		public bool BeginOnAwake = true;

		[Tooltip("The path the mover will travel through")]
		public Path Path = new Path();

		private PathState _state = new PathState();

		void Start()
		{
			if (BeginOnAwake)
				StartPath();
		}

		public void StartPath()
		{
			_state.Start(Path, Mover);
		}

		#region Persistence

		internal override void Load(string saveData)
		{
			if (BeginOnAwake)
				_state.Load(saveData);
		}

		internal override string Save()
		{
			return BeginOnAwake ? _state.Save() : "";
		}

		#endregion
	}
}
