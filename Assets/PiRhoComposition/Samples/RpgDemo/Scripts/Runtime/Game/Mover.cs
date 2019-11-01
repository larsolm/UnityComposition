using PiRhoSoft.Utilities;
using UnityEngine;

namespace PiRhoSoft.MonsterRpg
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(Rigidbody2D))]
	public class Mover : MonoBehaviour
	{
		private static readonly int _horizontal = Animator.StringToHash("Horizontal");
		private static readonly int _vertical = Animator.StringToHash("Vertical");
		private static readonly int _moving = Animator.StringToHash("Moving");

		[Tooltip("The speed at which the mover moves")]
		[Range(0.0f, 10.0f)]
		[CustomLabel("Move Speed (units/second)")]
		public float MoveSpeed = 5.0f;

		[Tooltip("The direction this mover begins facing")]
		[EnumButtons]
		public MovementDirection MovementDirection = MovementDirection.Down;

		public bool IsFrozen => _frozenCount > 0;

		private Vector2 _velocity = Vector2.zero;
		private int _frozenCount = 0;

		private Animator _animator;
		private Rigidbody2D _body;

		void Awake()
		{
			_body = GetComponent<Rigidbody2D>();
			_animator = GetComponent<Animator>();
		}

		void Update()
		{
			var direction = Direction.GetVector(MovementDirection);

			_animator.SetFloat(_horizontal, Mathf.Clamp(direction.x, -1, 1));
			_animator.SetFloat(_vertical, Mathf.Clamp(direction.y, -1, 1));
			_animator.SetBool(_moving, _velocity.sqrMagnitude > 0);
		}

		public void Freeze()
		{
			_frozenCount++;
		}

		public void Thaw()
		{
			_frozenCount--;
		}

		public void WarpToPosition(Vector2 position, MovementDirection direction)
		{
			transform.position = position;

			FaceDirection(direction);
		}

		public void FaceDirection(MovementDirection direction)
		{
			MovementDirection = direction;
		}

		public void UpdateMove(Vector2 move)
		{
			if (IsFrozen)
				move = Vector2.zero;

			_velocity = move.normalized * MoveSpeed;

			UpdateMoveDirection(move);
			Move();
		}

		private void Move()
		{
			_body.MovePosition(_body.position + (_velocity * Time.fixedDeltaTime));
		}

		private void UpdateMoveDirection(Vector2 move)
		{
			if (move.x == 0.0f)
			{
				if (move.y != 0.0f)
					FaceDirection(move.y < 0.0f ? MovementDirection.Down : MovementDirection.Up);
			}
			else if (move.y == 0.0f)
			{
				if (move.x != 0.0f)
					FaceDirection(move.x < 0.0f ? MovementDirection.Left : MovementDirection.Right);
			}
			else
			{
				switch (MovementDirection)
				{
					case MovementDirection.Left: if (move.x > 0) FaceDirection(MovementDirection.Right); break;
					case MovementDirection.Right: if (move.x < 0) FaceDirection(MovementDirection.Left); break;
					case MovementDirection.Down: if (move.y > 0) FaceDirection(MovementDirection.Up); break;
					case MovementDirection.Up: if (move.y < 0) FaceDirection(MovementDirection.Down); break;
				}
			}
		}
	}
}