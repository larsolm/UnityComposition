using PiRhoSoft.Utilities.Engine;
using UnityEngine;
using UnityEngine.Rendering;

namespace PiRhoSoft.MonsterRpg.Engine
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

		public void FaceDirection(MovementDirection direction)
		{
			if (direction == MovementDirection.None)
				return;

			MovementDirection = direction;
		}

		public virtual void UpdateMove(float horizontal, float vertical)
		{
			_velocity.Set(horizontal, vertical);
			_velocity.Normalize();
			_velocity *= MoveSpeed;

			Move();
		}

		public void WarpToPosition(Vector2 position, MovementDirection direction, CollisionLayer layer)
		{
			transform.position = position;

			FaceDirection(direction);

			if (layer != CollisionLayer.All)
				MovementLayer = layer;
		}

		protected void Move()
		{
			_body.MovePosition(_body.position + (_velocity * Time.fixedDeltaTime));
		}
	}
}