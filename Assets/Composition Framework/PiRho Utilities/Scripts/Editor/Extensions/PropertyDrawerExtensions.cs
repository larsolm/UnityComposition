using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.Utilities.Editor
{
	public static class PropertyDrawerExtensions
	{
		private const string _changedInternalsError = "(PUPDCI) failed to setup PropertyDrawer: Unity internals have changed";

		private static FieldInfo m_FieldInfo;
		private static FieldInfo m_Attribute;

		static PropertyDrawerExtensions()
		{
			var type = typeof(PropertyDrawer);
			m_FieldInfo = type.GetField(nameof(m_FieldInfo), BindingFlags.Instance | BindingFlags.NonPublic);
			m_Attribute = type.GetField(nameof(m_Attribute), BindingFlags.Instance | BindingFlags.NonPublic);

			if (m_FieldInfo == null || m_FieldInfo.FieldType != typeof(FieldInfo) || m_Attribute == null || m_Attribute.FieldType != typeof(PropertyAttribute))
				Debug.LogError(_changedInternalsError);
		}

		public static void SetFieldInfo(this PropertyDrawer drawer, FieldInfo value)
		{
			m_FieldInfo?.SetValue(drawer, value);
		}

		public static void SetAttribute(this PropertyDrawer drawer, PropertyAttribute value)
		{
			m_Attribute?.SetValue(drawer, value);
		}
	}
}