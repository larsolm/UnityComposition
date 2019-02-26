using PiRhoSoft.UtilityEngine;
using System;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[Serializable]
	public class InterfaceControlDictionary : SerializedDictionary<string, InterfaceControl> { }

	[DisallowMultipleComponent]
	[RequireComponent(typeof(Canvas))]
	[AddComponentMenu("Composition/Interface/Interface")]
	public class Interface : MonoBehaviour
	{
		[Tooltip("The name of the Interface to use when looking it up on the Interface Manager")]
		public string Name = "";

		[Tooltip("A list of Interface Controls that can be shown via string name")]
		[DictionaryDisplay(EmptyText = "Add Interface Controls that can be shown by name")]
		public InterfaceControlDictionary InterfaceControls = new InterfaceControlDictionary();

		private Canvas _canvas;

		private int _activeCount = 0;

		void Awake()
		{
			_canvas = GetComponent<Canvas>();

			if (_canvas.worldCamera)
				_canvas.worldCamera.gameObject.SetActive(false);
		}

		void OnEnable()
		{
			InterfaceManager.Instance.Add(this);
		}

		void OnDisable()
		{
			InterfaceManager.Instance?.Remove(this);
		}

		public void Activate()
		{
			if (_activeCount == 0)
			{
				if (_canvas.worldCamera)
					_canvas.worldCamera.gameObject.SetActive(true);

				Setup();
			}

			InterfaceManager.Instance.MakeFocused(this);

			_activeCount++;
		}

		public void Deactivate()
		{
			_activeCount--;


			if (_activeCount == 0)
			{
				if (_canvas.worldCamera)
					_canvas.worldCamera.gameObject.SetActive(false);

				InterfaceManager.Instance.RemoveFocus(this);
				Teardown();
			}
		}

		public ControlType GetControl<ControlType>(string name) where ControlType : InterfaceControl
		{
			return InterfaceControls.TryGetValue(name, out var control) ? control as ControlType : null;
		}

		protected virtual void Setup()
		{
		}

		protected virtual void Teardown()
		{
		}

		protected internal virtual void UpdateInput()
		{
		}
	}
}
