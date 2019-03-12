using PiRhoSoft.UtilityEngine;
using System;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[Serializable]
	public class InterfaceControlDictionary : SerializedDictionary<string, InterfaceControl> { }

	[DisallowMultipleComponent]
	[RequireComponent(typeof(Canvas))]
	[AddComponentMenu("PiRho Soft/Interface/Interface")]
	[HelpURL(Composition.DocumentationUrl + "interface")]
	public class Interface : MonoBehaviour
	{
		[Tooltip("The name of the Interface to use when looking it up on the Interface Manager")]
		public string Name = "";

		[Tooltip("The name of the axis to use for left and right navigation")] public string HorizontalAxis = "Horizontal";
		[Tooltip("The name of the axis to use for up and down navigation")] public string VerticalAxis = "Vertical";
		[Tooltip("The name of the button to use to make selections")] public string AcceptButton = "Submit";
		[Tooltip("The name of the button to use to close menus")] public string CancelButton = "Cancel";

		[Tooltip("A list of Interface Controls that can be shown via string name")]
		[DictionaryDisplay(EmptyText = "Add Interface Controls that can be shown by name")]
		public InterfaceControlDictionary InterfaceControls = new InterfaceControlDictionary();

		public ButtonState Left { get; protected set; }
		public ButtonState Right { get; protected set; }
		public ButtonState Up { get; protected set; }
		public ButtonState Down { get; protected set; }
		public ButtonState Accept { get; protected set; }
		public ButtonState Cancel { get; protected set; }

		private Canvas _canvas;

		private int _activeCount = 0;

		void Awake()
		{
			_canvas = GetComponent<Canvas>();

			foreach (var control in InterfaceControls.Values)
				control.Initialize(this);

			if (_canvas.worldCamera)
				_canvas.worldCamera.gameObject.SetActive(false);
		}

		void OnEnable()
		{
			InterfaceManager.Instance.Add(this);
		}

		void OnDisable()
		{
			if (InterfaceManager.Exists)
				InterfaceManager.Instance.Remove(this);
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

		public virtual void UpdateInput()
		{
			Left = InputHelper.GetAxisState(HorizontalAxis, -0.25f);
			Right = InputHelper.GetAxisState(HorizontalAxis, 0.25f);
			Up = InputHelper.GetAxisState(VerticalAxis, 0.25f);
			Down = InputHelper.GetAxisState(VerticalAxis, -0.25f);
			Accept = InputHelper.GetButtonState(AcceptButton);
			Cancel = InputHelper.GetButtonState(CancelButton);
		}
	}
}
