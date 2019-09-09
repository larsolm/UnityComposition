using UnityEngine;

namespace PiRhoSoft.MonsterRpg
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Mover))]
	[RequireComponent(typeof(Animator))]
	[HelpURL(MonsterRpg.DocumentationUrl + "mover-animator")]
	[AddComponentMenu("Monster RPG/Animation/Mover Animator")]
	public class MoverAnimator : MonoBehaviour
	{
		private static readonly int _horizontal = Animator.StringToHash("Horizontal");
		private static readonly int _vertical = Animator.StringToHash("Vertical");
		private static readonly int _moving = Animator.StringToHash("Moving");

		private Animator _animator;
		private Mover _mover;

		void Awake()
		{
			_mover = GetComponent<Mover>();
			_animator = GetComponent<Animator>();
		}

		void Update()
		{
			var direction = _mover.DirectionVector;

			_animator.SetFloat(_horizontal, Mathf.Clamp(direction.x, -1, 1));
			_animator.SetFloat(_vertical, Mathf.Clamp(direction.y, -1, 1));
			_animator.SetBool(_moving, _mover.Speed > 0);
		}
	}
}
