using PiRhoSoft.Utilities;
using System;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[Serializable]
	public class DependentObjectList : SerializedList<GameObject> { }

	[DisallowMultipleComponent]
	[HelpURL(Composition.DocumentationUrl + "interface-control")]
	[AddComponentMenu("PiRho Soft/Interface/Interface Control")]
	public class InterfaceControl : MonoBehaviour
	{
		[Tooltip("GameObjects in the scene that should be activated and deactivated with this control")]
		[List(EmptyLabel = "Add items that need to be enabled along with this control")]
		public DependentObjectList DependentObjects = new DependentObjectList();

		public bool IsActive { get; private set; } = false;

		protected virtual void Awake()
		{
			Disable();
		}

		public void Activate()
		{
			if (!IsActive)
			{
				Enable();
				Setup();
			}

			IsActive = true;
		}

		public void Deactivate()
		{
			if (IsActive)
			{
				Teardown();
				Disable();
			}

			IsActive = false;
		}

		protected virtual void Setup()
		{
		}

		protected virtual void Teardown()
		{
		}

		private void Enable()
		{
			gameObject.SetActive(true);

			foreach (var obj in DependentObjects)
				obj.SetActive(true);
		}

		private void Disable()
		{
			gameObject.SetActive(false);

			foreach (var obj in DependentObjects)
				obj.SetActive(false);
		}
	}
}
