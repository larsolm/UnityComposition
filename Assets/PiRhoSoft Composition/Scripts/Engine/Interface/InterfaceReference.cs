using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.CompositionEngine
{
	[Serializable]
	public class InterfaceReference
	{
		private const string _missingInterfaceWarning = "(CIRMI) Unable to find interface for {0}: the interface '{1}' could not be found";
		private const string _missingControlWarning = "(CIRMC) Unable to find interface control for {0}: the interface '{1}' does not have a control named '{2}'";
		private const string _invalidInterfaceTypeWarning = "(CIRIIT) Unable to find interface for {0}: the interface '{1}' is not a {1}";
		private const string _invalidControlTypeWarning = "(CIRICT) Unable to find interface control for {0}: the interface control '{1}' is not a {2}";

		[Tooltip("The name of the interface to display (if empty, the active Interface will be used")]
		public string InterfaceName = "InterfaceName";

		[Tooltip("The name of the control to use")]
		public string ControlName = "ControlName";

		public void Activate(Object context)
		{
			Activate<InterfaceControl>(context);
		}

		public ControlType Activate<ControlType>(Object context) where ControlType : InterfaceControl
		{
			var ui = GetInterface<Interface>(context);
			var control = GetControl<ControlType>(ui, context);

			if (ui)
				ui.Activate();

			if (control)
				control.Activate();

			return control;
		}

		public void Deactivate(Object context)
		{
			var ui = GetInterface<Interface>(context);
			var control = GetControl<InterfaceControl>(ui, context);

			if (control)
				control.Deactivate();

			if (ui)
				ui.Deactivate();
		}
	
		public InterfaceType GetInterface<InterfaceType>(Object context) where InterfaceType : Interface
		{
			var ui = InterfaceManager.Instance.GetInterface<Interface>(InterfaceName);

			if (ui is InterfaceType typedUi)
				return typedUi;
			else if (ui && !string.IsNullOrEmpty(InterfaceName))
				Debug.LogWarningFormat(context, _missingInterfaceWarning, context.name, InterfaceName);
			else
				Debug.LogWarningFormat(context, _invalidInterfaceTypeWarning, context.name, InterfaceName, typeof(InterfaceType).Name);

			return null;
		}

		public ControlType GetControl<ControlType>(Object context) where ControlType : InterfaceControl
		{
			var ui = GetInterface<Interface>(context);
			return ui ? GetControl<ControlType>(ui, context) : null;
		}

		private ControlType GetControl<ControlType>(Interface ui, Object context) where ControlType : InterfaceControl
		{
			if (ui)
			{
				var control = ui.GetControl<InterfaceControl>(ControlName);

				if (control is ControlType typedControl)
					return typedControl;
				else if (control == null)
					Debug.LogWarningFormat(context, _missingControlWarning, context.name, InterfaceName, ControlName);
				else
					Debug.LogWarningFormat(context, _invalidControlTypeWarning, context.name, ControlName, typeof(ControlType).Name);
			}

			return null;
		}
	}
}
