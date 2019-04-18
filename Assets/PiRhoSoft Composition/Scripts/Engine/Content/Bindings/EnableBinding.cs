using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "enable-binding")]
	[AddComponentMenu("PiRho Soft/Bindings/Enable Binding")]
	public class EnableBinding : VariableBinding
	{
		private const string _invalidObjectWarning = "(CBEBIO) Failed to enable or disable object '{0}': the object must be a GameObject, Behaviour, or Renderer";

		[Tooltip("The GameObject, Behaviour, or Renderer to enable or disable based on Expression")]
		public Object Object;

		[Tooltip("The expression to run to determine if the refereneced component should be enabled")]
		public Expression Condition = new Expression();

		protected override void UpdateBinding(IVariableStore variables, BindingAnimationStatus status)
		{
			if (Object)
			{
				var active = false;

				try { Condition.Evaluate(variables).TryGetBool(out active); }
				catch { }

				if (Object is GameObject gameObject)
					gameObject.SetActive(active);
				else if (Object is Behaviour behaviour)
					behaviour.enabled = active;
				else if (Object is Renderer renderer)
					renderer.enabled = active;
				else
					Debug.LogWarningFormat(Object, _invalidObjectWarning, Object.name);
			}
		}
	}
}
