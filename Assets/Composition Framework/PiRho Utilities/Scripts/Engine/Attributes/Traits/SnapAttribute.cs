using UnityEngine;

namespace PiRhoSoft.Utilities
{
	public class SnapAttribute : PropertyTraitAttribute
	{
		public const int Order = 0;

		public float Number { get; private set; }
		public Vector4 Vector { get; private set; }
		public Bounds Bounds { get; private set; }

		public SnapAttribute(float snap) : base(FieldPhase, Order)
		{
			Number = snap;
		}

		public SnapAttribute(int snap) : base(FieldPhase, Order)
		{
			Number = snap;
		}

		public SnapAttribute(Vector2 snap) : base(FieldPhase, Order)
		{
			Vector = snap;
		}

		public SnapAttribute(Vector2Int snap) : base(FieldPhase, Order)
		{
			Vector = (Vector2)snap;
		}

		public SnapAttribute(Vector3 snap) : base(FieldPhase, Order)
		{
			Vector = snap;
		}

		public SnapAttribute(Vector3Int snap) : base(FieldPhase, Order)
		{
			Vector = (Vector3)snap;
		}

		public SnapAttribute(Vector4 snap) : base(FieldPhase, Order)
		{
			Vector = snap;
		}

		public SnapAttribute(Rect snap) : base(FieldPhase, Order)
		{
			Vector = new Vector4(snap.x, snap.y, snap.width, snap.height);
		}

		public SnapAttribute(RectInt snap) : base(FieldPhase, Order)
		{
			Vector = new Vector4(snap.x, snap.y, snap.width, snap.height);
		}

		public SnapAttribute(Bounds snap) : base(FieldPhase, Order)
		{
			Bounds = snap;
		}

		public SnapAttribute(BoundsInt snap) : base(FieldPhase, Order)
		{
			Bounds = new Bounds(snap.center, snap.size);
		}
	}
}