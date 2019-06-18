using System;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.PargonUtilities.Engine
{
	public class SerializedList<T>
	{
		public List<T> Items;
	}

	public class ListAttribute : PropertyAttribute
	{
		public ListAttribute()
		{
			order = int.MaxValue - 100;
		}
	}

	public class Examples : MonoBehaviour
	{
		public enum TestEnum
		{
			One,
			Two,
			BuckleMyShoe,
			Three,
			Four
		}

		[Flags]
		public enum TestFlags
		{
			Zero = 0,
			One = 1,
			Two = 1 << 1,
			Four = 1 << 2,
			Eight = 1 << 3
		}

		[Serializable] public class TestEnumList : SerializedList<TestEnum> { }

		[EnumButtons]
		public TestEnum EnumButton;

		[EnumButtons(true)]
		public TestFlags EnumFlags;

		public bool Testing;

		[Conditional(nameof(EnumButton), (int)TestEnum.One, false)]
		public string TestOne;

		[Conditional(nameof(Testing), false)]
		public string TestTwo;

		[TypePicker(typeof(Component), true)]
		public string TypePicker;

		[ObjectPicker]
		public ScriptableObject ObjectPicker;

		[Required(MessageBoxType.Warning, "String Object must be specified")]
		[ObjectPicker(typeof(ScriptableObject))]
		public string StringObject;

		[ScenePicker(nameof(CreateScene))]
		public SceneReference ScenePicker;

		[ScenePicker(nameof(CreateScene))]
		public string StringScene;

		[ScenePicker(nameof(CreateScene))]
		public int IntScene;

		[Minimum(10)]
		[Conditional(nameof(IsVisible))]
		public int MinimumInt;

		[Minimum(5)]
		public float MinimumFloat;

		[Maximum(10)]
		public int MaximumInt;

		[Minimum(5)]
		[Maximum(10)]
		[Snap(0.5f)]
		[CustomLabel("Ranged Float (5 - 10)")]
		public float RangedFloat;

		[Required(MessageBoxType.Warning, "String is required")]
		public string RequiredString;

		[ReadOnly]
		public int Disabled = 100;

		[Placeholder("(placeholder)")]
		public string Placeholder;

		[Euler]
		public Quaternion Euler;

		[Validate(nameof(Validation), MessageBoxType.Error, "Must be greater than 0")]
		[CustomLabel("Validated (> 0)")]
		public float Validated = 0.0f;

		[ChangeTrigger(nameof(Changed))]
		public float Changes = 0.0f;

		[List]
		public TestEnumList Enums = new TestEnumList();

		[Dropdown(new string[] { "Zero", "One", "Two" }, new int[] { 0, 1, 2 })]
		public int DropdownInt;
	
		[MaximumLength(4)]
		public string Length4;

		private void OnEnable()
		{
			ScenePicker.Setup(this);
		}

		private void OnDisable()
		{
			ScenePicker.Teardown();
		}

		public static void CreateScene()
		{
		}

		private bool IsVisible()
		{
			return MinimumFloat > 6;
		}

		private bool Validation()
		{
			return Validated > 0;
		}

		private void Changed()
		{
			Debug.Log("Changed");
		}

		private string TestLabel()
		{
			return "TestLabel";
		}
	}
}