using UnityEngine;

namespace PiRhoSoft.Utilities
{
	[AddComponentMenu("PiRho Soft/Examples/Controls")]
	public class ControlsExample : MonoBehaviour
	{
		public enum TestEnum
		{
			Zero = 0x0,
			One = 0x1,
			Two = 0x2,
			Four = 0x4,
			Eight = 0x8
		}

		[TypePicker(typeof(MonoBehaviour), false)]
		public string Type;

		[ObjectPicker]
		public Object Object;

		[ScenePicker]
		public string Scene;

		[Popup(new int[] { 0, 1, 2, 3, 4 }, new string[] { "Zero", "One", "Two", "Three", "Four" })]
		public int IntPopup;

		[Popup(new float[] { 0.0f, 0.1f, 0.2f, 0.4f, 0.8f, 1.6f })]
		public float FloatPopup;

		[Popup(new string[] { "", "Hello", "Hola", "Bonjour" })]
		public string StringPopup;

		[ComboBox(new string[] { "One Fish", "Two Fish", "Red Fish", "Blue Fish" })]
		public string ComboBox;

		[Euler]
		public Quaternion Euler;

		[EnumButtons]
		public TestEnum Buttons;

		[EnumButtons(true)]
		public TestEnum Flags;
	}
}