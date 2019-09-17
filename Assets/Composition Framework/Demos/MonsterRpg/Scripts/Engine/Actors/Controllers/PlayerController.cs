using UnityEngine;
using UnityEngine.InputSystem;

namespace PiRhoSoft.MonsterRpg
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Player))]
	[RequireComponent(typeof(PlayerInput))]
	[AddComponentMenu("PiRho Soft/Monster RPG/Player Controller")]
	public class PlayerController : Controller
	{
		private Vector2 _move = Vector2.zero;

		void FixedUpdate()
		{
			UpdateMover(_move);
		}

		private void OnMove(InputValue value)
		{
			_move = value.Get<Vector2>();
		}

		private void OnInteract(InputValue value)
		{
			if (!IsFrozen)
			{
				var input = value.Get<float>();
				if (input > 0.0f)
					Player.Instance.Interact();
			}
		}
	}
}
