using PiRhoSoft.UtilityEngine;
using System;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[Serializable]
	public class DependentObjectList : SerializedList<GameObject> { }

	[DisallowMultipleComponent]
	[HelpURL(Composition.DocumentationUrl + "interface-control")]
	[AddComponentMenu("PiRho Soft/Interface/Interface Control")]
	public class InterfaceControl : MonoBehaviour
	{
		[Tooltip("GameObjects in the scene that should be activated and deactivated with this control")]
		[ListDisplay(AllowCollapse = false, ShowEditButton = true, EmptyText = "Add items that need to be enabled along with this control")]
		public DependentObjectList DependentObjects = new DependentObjectList();

		public IVariableStore Variables { get; private set; }

		private int _activeCount = 0;

		public bool IsActive => _activeCount > 0;

		void Awake()
		{
			_activeCount = 0;
			Disable();
		}

		public void Activate()
		{
			if (_activeCount == 0)
			{
				Enable();
				Setup();
			}

			_activeCount++;
		}

		public void Deactivate()
		{
			_activeCount--;

			if (_activeCount == 0)
			{
				Teardown();
				Disable();
			}
		}

		public void UpdateBindings(IVariableStore variables, string group, BindingAnimationStatus status)
		{
			Variables = variables;

			UpdateBindings(group, status);
		}

		protected virtual void UpdateBindings(string group, BindingAnimationStatus status)
		{
			InterfaceBinding.UpdateBindings(gameObject, Variables, group, status);

			foreach (var obj in DependentObjects)
				InterfaceBinding.UpdateBindings(obj, Variables, group, status);
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
