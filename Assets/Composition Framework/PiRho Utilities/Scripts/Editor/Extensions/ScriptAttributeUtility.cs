using System;
using System.Reflection;
using UnityEngine;

namespace PiRhoSoft.Utilities.Editor
{
	public static class ScriptAttributeUtility
	{
		private const string _changedInternalsError = "(PUSAUCI) failed to setup ScriptAttributeUtility: Unity internals have changed";

		private static string _scriptAttributeUtility = nameof(UnityEditor) + "." + nameof(ScriptAttributeUtility);
		private static MethodInfo _getDrawerTypeForType;
		private static object[] _type = new object[1];

		static ScriptAttributeUtility()
		{
			var type = typeof(UnityEditor.Editor).Assembly.GetType(_scriptAttributeUtility);
			_getDrawerTypeForType = type?.GetMethod(nameof(GetDrawerTypeForType), BindingFlags.Static | BindingFlags.NonPublic);
			var parameters = _getDrawerTypeForType?.GetParameters();

			if (_getDrawerTypeForType == null || _getDrawerTypeForType.ReturnType != typeof(Type) || parameters.Length != 1 || parameters[0].ParameterType != typeof(Type))
				Debug.LogError(_changedInternalsError);
		}

		public static Type GetDrawerTypeForType(Type type)
		{
			_type[0] = type;
			return _getDrawerTypeForType?.Invoke(null, _type) as Type;
		}
	}
}