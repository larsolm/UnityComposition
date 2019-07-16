using PiRhoSoft.Utilities;
using UnityEngine;
using UnityEngine.Rendering;

namespace PiRhoSoft.MonsterRpg
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Rigidbody2D))]
	public class Mover : MonoBehaviour
	{
		[Range(0.0f, 10.0f)]
		public float MoveSpeed = 5.0f;

		[EnumButtons]
		public CollisionLayer MovementLayer = CollisionLayer.One; // Need to change layer for collisions when this changes

		[EnumButtons]
		public MovementDirection MovementDirection;

		public float Speed => _velocity.sqrMagnitude;
		public Vector2 DirectionVector => Direction.GetVector(MovementDirection);

		protected Vector2 _velocity = Vector2.zero;

		private SortingGroup _sorting;
		private SpriteRenderer _renderer;
		private Rigidbody2D _body;

		void Awake()
		{
			if (MovementDirection == MovementDirection.None)
				FaceDirection(MovementDirection.Down);

			_sorting = GetComponent<SortingGroup>();
			_renderer = GetComponent<SpriteRenderer>();
			_body = GetComponent<Rigidbody2D>();

			gameObject.layer = LayerCollision.GetLayer(MovementLayer);
		}

		void LateUpdate()
		{
			var order = LayerSorting.GetSortingOrder(MovementLayer);

			if (_sorting)
				_sorting.sortingOrder = order;
			else if (_renderer)
				_renderer.sortingOrder = order;
		}

		public void WarpToPosition(Vector2 position, MovementDirection direction, CollisionLayer layer)
		{
			transform.position = position;

			FaceDirection(direction);

			if (layer != CollisionLayer.All)
				MovementLayer = layer;
		}

		public void FaceDirection(MovementDirection direction)
		{
			if (direction == MovementDirection.None)
				return;

			MovementDirection = direction;
		}

		public void UpdateMove(float horizontal, float vertical)
		{
			_velocity.Set(horizontal, vertical);
			_velocity.Normalize();
			_velocity *= MoveSpeed;

			UpdateMoveDirection(horizontal, vertical);
			Move();
		}

		private void Move()
		{
			_body.MovePosition(_body.position + (_velocity * Time.fixedDeltaTime));
		}

		private void UpdateMoveDirection(float horizontal, float vertical)
		{
			if (horizontal == 0.0f)
			{
				if (vertical != 0.0f)
					FaceDirection(vertical < 0.0f ? MovementDirection.Down : MovementDirection.Up);
			}
			else if (vertical == 0.0f)
			{
				if (horizontal != 0.0f)
					FaceDirection(horizontal < 0.0f ? MovementDirection.Left : MovementDirection.Right);
			}
			else
			{
				switch (MovementDirection)
				{
					case MovementDirection.Left: if (horizontal > 0) FaceDirection(MovementDirection.Right); break;
					case MovementDirection.Right: if (horizontal < 0) FaceDirection(MovementDirection.Left); break;
					case MovementDirection.Down: if (vertical > 0) FaceDirection(MovementDirection.Up); break;
					case MovementDirection.Up: if (vertical < 0) FaceDirection(MovementDirection.Down); break;
				}
			}
		}
	}
}