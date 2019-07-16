using PiRhoSoft.Utilities;
using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace PiRhoSoft.MonsterRpg
{
	[DisallowMultipleComponent]
	[AddComponentMenu("Monster RPG/World/Layer Sorting")]
	public class LayerSorting : MonoBehaviour
	{
		public static readonly int LayerCount = Enum.GetValues(typeof(CollisionLayer)).Length - 1;

		private const int _bottomSortOffset = -100; // mover shadows are at -10, so this needs to be less than that

		[Tooltip("The movement layer this object will sort on")]
		[EnumButtons]
		public CollisionLayer Layer = CollisionLayer.One;

		[Tooltip("Set this to make this object sort below all other objects on the same layer (useful for tilemaps)")]
		public bool ForceToBottom = false;

		private SortingGroup _sorting;
		private Renderer _renderer;

		public static int GetSortingOrder(CollisionLayer layer)
		{
			return (layer == CollisionLayer.All ? 0 : (int)layer) * 1000;
		}

		void Awake()
		{
			_sorting = GetComponent<SortingGroup>();
			_renderer = GetComponent<Renderer>();
		}

		void OnEnable()
		{
			var offset = ForceToBottom ? _bottomSortOffset : 0;
			var order = GetSortingOrder(Layer) + offset;

			if (_sorting)
				_sorting.sortingOrder = order;
			if (_renderer)
				_renderer.sortingOrder = order;

			gameObject.layer = LayerCollision.GetLayer(Layer);
		}
	}
}