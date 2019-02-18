using System;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[Serializable]
	public class InterfaceReference
	{
		private const string _missingInterfaceNameWarning = "(IIRMIN) Unable to find interface {0}: the interface could not be found";
		private const string _missingControlNameWarning = "(IIRMCN) Unable to find interface control: the interface {0} does not have a control named {1}";
		private const string _invalidInterfaceTypeWarning = "(IIRMIIT) Unable to find interface {0}: the interface is not a {1}";
		private const string _invalidControlTypeWarning = "(IIRMICT) Unable to find interface control {0}: the interface control is not a {1}";

		[Tooltip("The name of the interface to display (if empty, the active Interface will be used")]
		public string InterfaceName;

		[Tooltip("The name of the control to use")]
		public string ControlName;

		public void Activate()
		{
			Activate<InterfaceControl>();
		}

		public ControlType Activate<ControlType>() where ControlType : InterfaceControl
		{
			var ui = GetInterface<Interface>();
			var control = GetControl<ControlType>(ui);

			if (ui)
				ui.Activate();

			if (control)
				control.Activate();

			return control;
		}

		public void Deactivate()
		{
			var ui = GetInterface<Interface>();
			var control = GetControl<InterfaceControl>(ui);

			if (control)
				control.Deactivate();

			if (ui)
				ui.Deactivate();
		}
	
		public InterfaceType GetInterface<InterfaceType>() where InterfaceType : Interface
		{
			var ui = InterfaceManager.Instance.GetInterface<Interface>(InterfaceName);

			if (ui is InterfaceType typedUi)
				return typedUi;
			else if (ui && !string.IsNullOrEmpty(InterfaceName))
				Debug.LogWarningFormat(_missingInterfaceNameWarning, InterfaceName);
			else
				Debug.LogWarningFormat(_invalidInterfaceTypeWarning, InterfaceName, typeof(Type).Name);

			return null;
		}

		public ControlType GetControl<ControlType>() where ControlType : InterfaceControl
		{
			var ui = GetInterface<Interface>();
			return ui ? GetControl<ControlType>(ui) : null;
		}

		private ControlType GetControl<ControlType>(Interface ui) where ControlType : InterfaceControl
		{
			if (ui)
			{
				var control = ui.GetControl<InterfaceControl>(ControlName);

				if (control is ControlType typedControl)
					return typedControl;
				else if (control == null)
					Debug.LogWarningFormat(_missingControlNameWarning, InterfaceName, ControlName);
				else
					Debug.LogWarningFormat(_invalidControlTypeWarning, ControlName, typeof(Type).Name);
			}

			return null;
		}
	}
}
