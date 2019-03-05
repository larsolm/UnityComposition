using PiRhoSoft.UtilityEngine;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[DisallowMultipleComponent]
	[HelpURL(Composition.DocumentationUrl + "interface-manager")]
	[AddComponentMenu("Composition/Interface Manager")]
	public class InterfaceManager : SingletonBehaviour<InterfaceManager>
	{
		private const string _invalidAddError = "(CIMIA) Failed to add Interface: an Interface named {0} has already been added";
		private const string _invalidRemoveError = "(CIMIR) Failed to remove Interface: an Interface named {0} has not been added";

		[Tooltip("The name of the axis to use for left and right navigation")] public string HorizontalAxis = "Horizontal";
		[Tooltip("The name of the axis to use for up and down navigation")] public string VerticalAxis = "Vertical";
		[Tooltip("The name of the button to use to make selections")] public string AcceptButton = "Submit";
		[Tooltip("The name of the button to use to close menus")] public string CancelButton = "Cancel";
		[Tooltip("The name of the button to use to pause the game")] public string StartButton = "Pause";

		[Tooltip("The name of the key to use to make selections")] public KeyCode AcceptKey = KeyCode.Space;
		[Tooltip("The name of the key to use to close menus")] public KeyCode CancelKey = KeyCode.Escape;
		[Tooltip("The name of the key to use to pause the game")] public KeyCode StartKey = KeyCode.Tab;

		private Dictionary<string, Interface> _enabledInterfaces = new Dictionary<string, Interface>();
		private List<Interface> _focusedInterfaces = new List<Interface>();

		public Interface FocusedInterface => _focusedInterfaces.Count > 0 ? _focusedInterfaces[_focusedInterfaces.Count - 1] : null;

		public InterfaceType GetInterface<InterfaceType>(string name) where InterfaceType : Interface
		{
			return string.IsNullOrEmpty(name) ? FocusedInterface as InterfaceType : _enabledInterfaces.TryGetValue(name, out var ui) ? ui as InterfaceType : null;
		}

		public ButtonState Left { get; protected set; }
		public ButtonState Right { get; protected set; }
		public ButtonState Up { get; protected set; }
		public ButtonState Down { get; protected set; }
		public ButtonState Accept { get; protected set; }
		public ButtonState Cancel { get; protected set; }
		public ButtonState Start { get; protected set; }

		void Update()
		{
			UpdateInput();
			FocusedInterface?.UpdateInput();
		}

		void LateUpdate()
		{
			InputHelper.LateUpdate();
		}

		protected virtual void UpdateInput()
		{
			Left = InputHelper.GetAxisState(HorizontalAxis, -0.25f);
			Right = InputHelper.GetAxisState(HorizontalAxis, 0.25f);
			Up = InputHelper.GetAxisState(VerticalAxis, 0.25f);
			Down = InputHelper.GetAxisState(VerticalAxis, -0.25f);
			Accept = InputHelper.GetButtonState(AcceptKey, AcceptButton);
			Cancel = InputHelper.GetButtonState(CancelKey, CancelButton);
			Start = InputHelper.GetButtonState(StartKey, StartButton);
		}

		internal void Add(Interface ui)
		{
			if (!_enabledInterfaces.ContainsKey(ui.Name))
				_enabledInterfaces.Add(ui.Name, ui);
			else
				Debug.LogWarningFormat(ui, _invalidAddError, ui.Name);
		}

		internal void Remove(Interface ui)
		{
			if (!_enabledInterfaces.Remove(ui.Name))
				Debug.LogWarningFormat(ui, _invalidRemoveError, ui.Name);
		}

		internal void MakeFocused(Interface ui)
		{
			// if this interface already exists, move it to the end and make it the focused interface
			// no parity with RemoveFocus as interfaces can be focused and removed out of order
			_focusedInterfaces.Remove(ui);
			_focusedInterfaces.Add(ui);
		}

		internal void RemoveFocus(Interface ui)
		{
			_focusedInterfaces.Remove(ui);
		}
	}
}
