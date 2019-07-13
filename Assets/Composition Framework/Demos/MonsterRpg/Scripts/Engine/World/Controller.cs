using UnityEngine;

namespace PiRhoSoft.MonsterRpg.Engine
{
	[RequireComponent(typeof(Mover))]
	public abstract class Controller : MonoBehaviour
	{
		public Mover Mover { get; private set; }
		public bool IsFrozen => _frozenCount > 0;

		private int _frozenCount = 0;

		protected virtual void Awake()
		{
			Mover = GetComponent<Mover>();
		}

		public void Freeze()
		{
			_frozenCount++;
		}

		public void Thaw()
		{
			_frozenCount--;
		}

		protected void UpdateMover(float horizontal, float vertical)
		{
			if (IsFrozen)
				Mover.UpdateMove(0.0f, 0.0f);
			else
				Mover.UpdateMove(horizontal, vertical);
		}

		public virtual void Load(string saveData) { }
		public virtual string Save() => string.Empty;
	}
}
