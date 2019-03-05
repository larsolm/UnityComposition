using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[RequireComponent(typeof(InterfaceControl))]
	[HelpURL(Composition.DocumentationUrl + "binding-updater")]
	[AddComponentMenu("PiRho Soft/Interface/Binding Updater")]
	public class BindingUpdater : MonoBehaviour
	{
		[Tooltip("The binding group to automatically update (update all if empty)")]
		public string Group;

		private InterfaceControl _control;

		void Awake()
		{
			_control = GetComponent<InterfaceControl>();
		}

		void Update()
		{
			if (_control.Variables != null)
				_control.UpdateBindings(_control.Variables, Group, null);
		}
	}
}