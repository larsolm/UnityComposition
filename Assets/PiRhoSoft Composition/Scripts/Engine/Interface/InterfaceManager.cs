using PiRhoSoft.UtilityEngine;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[DisallowMultipleComponent]
	[HelpURL(Composition.DocumentationUrl + "interface-manager")]
	public class InterfaceManager : GlobalBehaviour<InterfaceManager>
	{
		private const string _invalidAddError = "(CIMIA) Failed to add Interface: an Interface named {0} has already been added";
		private const string _invalidRemoveError = "(CIMIR) Failed to remove Interface: an Interface named {0} has not been added";

		private Dictionary<string, Interface> _enabledInterfaces = new Dictionary<string, Interface>();
		private List<Interface> _focusedInterfaces = new List<Interface>();

		public Interface FocusedInterface => _focusedInterfaces.Count > 0 ? _focusedInterfaces[_focusedInterfaces.Count - 1] : null;

		public InterfaceType GetInterface<InterfaceType>(string name) where InterfaceType : Interface
		{
			return string.IsNullOrEmpty(name) ? FocusedInterface as InterfaceType : _enabledInterfaces.TryGetValue(name, out var ui) ? ui as InterfaceType : null;
		}

		void Update()
		{
			if (FocusedInterface) // Don't null coalesce
				FocusedInterface.UpdateInput();
		}

		void LateUpdate()
		{
			InputHelper.LateUpdate();
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
