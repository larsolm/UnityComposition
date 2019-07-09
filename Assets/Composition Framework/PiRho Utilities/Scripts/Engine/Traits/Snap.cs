using UnityEngine;

namespace PiRhoSoft.Utilities.Engine
{
	public class SnapAttribute : PropertyTraitAttribute
	{
		private const int _order = int.MaxValue - 100;

		public float Number { get; private set; }
		public Vector4 Vector { get; private set; }
		public Bounds Bounds { get; private set; }

		public SnapAttribute(float snap) : base(_order)
		{
			Number = snap;
		}

		public SnapAttribute(int snap) : base(_order)
		{
			Number = snap;
		}

		public SnapAttribute(Vector2 snap) : base(_order)
		{
			Vector = snap;
		}

		public SnapAttribute(Vector2Int snap) : base(_order)
		{
			Vector = (Vector2)snap;
		}

		public SnapAttribute(Vector3 snap) : base(_order)
		{
			Vector = snap;
		}

		public SnapAttribute(Vector3Int snap) : base(_order)
		{
			Vector = (Vector3)snap;
		}

		public SnapAttribute(Vector4 snap) : base(_order)
		{
			Vector = snap;
		}

		public SnapAttribute(Rect snap) : base(_order)
		{
			Vector = new Vector4(snap.x, snap.y, snap.width, snap.height);
		}

		public SnapAttribute(RectInt snap) : base(_order)
		{
			Vector = new Vector4(snap.x, snap.y, snap.width, snap.height);
		}

		public SnapAttribute(Bounds snap) : base(_order)
		{
			Bounds = snap;
		}

		public SnapAttribute(BoundsInt snap) : base(_order)
		{
			Bounds = new Bounds(snap.center, snap.size);
		}
	}
}
