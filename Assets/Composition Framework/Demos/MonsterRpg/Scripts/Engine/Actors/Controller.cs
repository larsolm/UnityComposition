using UnityEngine;

namespace PiRhoSoft.MonsterRpg
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

		protected void UpdateMover(Vector2 move)
		{
			if (IsFrozen)
				Mover.UpdateMove(Vector2.zero);
			else
				Mover.UpdateMove(move);
		}
	}
}
