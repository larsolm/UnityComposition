using System;
using UnityEngine;

namespace PiRhoSoft.MonsterRpg.Engine
{
	[Serializable]
	public struct TileTransformInfo
	{
		public static readonly int[] Rotations = new int[4] { 0, 90, 180, 270 };

		[Tooltip("The sprite for this tile")]
		public Sprite Sprite;

		[Tooltip("The rotation that this sprite will have")]
		public int Rotation;

		[Tooltip("Whether this sprite will be flipped horizontally")]
		public bool FlipHorizontal;

		[Tooltip("Whether this sprite will be flipped vertically")]
		public bool FlipVertical;

		public Matrix4x4 GetTransform()
		{
			return Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, -Rotations[Rotation]), new Vector3(FlipHorizontal ? -1 : 1, FlipVertical ? -1 : 1, 1));
		}
	}
}
