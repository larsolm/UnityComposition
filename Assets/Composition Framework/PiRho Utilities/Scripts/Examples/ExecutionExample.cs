using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Soft/Examples/Execution")]
	public class ExecutionExample : MonoBehaviour
	{
		[Button(nameof(Clicked), Label = "Click")] [ChangeTrigger(nameof(Changed))]
		public bool Toggle;
		private void Clicked() => Debug.Log("Clicked");
		private void Changed(bool oldValue, bool newValue) => Debug.Log($"Changed from {oldValue} to {newValue}");
	}
}