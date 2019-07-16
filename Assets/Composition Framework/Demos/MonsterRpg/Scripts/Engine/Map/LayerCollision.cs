using PiRhoSoft.Utilities;
using UnityEngine;

namespace PiRhoSoft.MonsterRpg
{
	public enum CollisionLayer
	{
		One,
		Two,
		Three,
		Four,
		Five,
		All
	}

	[DisallowMultipleComponent]
	[AddComponentMenu("Monster RPG/World/Layer Collision")]
	public class LayerCollision : MonoBehaviour
	{
		private static int[] _layers;

		[Tooltip("The movement layer this object will collide on")]
		[EnumButtons]
		public CollisionLayer Layer = CollisionLayer.One;

		public static int GetLayer(CollisionLayer layer)
		{
			if (_layers == null)
			{
				_layers = new int[]
				{
					LayerMask.NameToLayer(CollisionLayer.One.ToString()),
					LayerMask.NameToLayer(CollisionLayer.Two.ToString()),
					LayerMask.NameToLayer(CollisionLayer.Three.ToString()),
					LayerMask.NameToLayer(CollisionLayer.Four.ToString()),
					LayerMask.NameToLayer(CollisionLayer.Five.ToString()),
					LayerMask.NameToLayer(CollisionLayer.All.ToString())
				};
			}

			return _layers[(int)layer];
		}

		void Awake()
		{
			gameObject.layer = _layers[(int)Layer];
		}
	}
}