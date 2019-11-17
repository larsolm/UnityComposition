using System;
using System.Collections.Generic;

namespace PiRhoSoft.DocGen.Editor
{
	[Serializable]
	public class Settings
	{
		[Flags]
		public enum AccessLevel
		{
			Public = 0x1,
			Protected = 0x2,
			Private = 0x4,
			Internal = 0x8,
			All = ~0
		}

		[Flags]
		public enum DeclarationType
		{
			Behaviour = 0x1,
			Asset = 0x2,
			Abstract = 0x4,
			Interface = 0x8,
			Class = 0x10,
			Enum = 0x20,
			Struct = 0x40,
			Generated = 0x80,
			All = ~0
		}

		[Flags]
		public enum MemberType
		{
			Constructor = 0x1,
			Field = 0x2,
			Property = 0x4,
			Method = 0x8,
			All = ~0
		}

		[Serializable]
		public class ExternalUrl
		{
			public string Namespace;
			public string UrlFormat;
		}

		public string CodePath = "Assets/Scripts";
		public string OutputFile = "Documentation/api.json";

		public List<string> Assemblies = new List<string> { "Assembly-CSharp" };
		public List<string> Namespaces = new List<string>();
		public List<ExternalUrl> Urls = new List<ExternalUrl> { new ExternalUrl { Namespace = "Unity.*", UrlFormat = "https://docs.unity3d.com/ScriptReference/{Name}.html" }, new ExternalUrl { Namespace = "System.*", UrlFormat = "https://docs.microsoft.com/en-us/dotnet/api/{Id}" } };
		public AccessLevel Access = AccessLevel.All;
		public DeclarationType Declarations = DeclarationType.All;
		public MemberType Members = MemberType.All;
	}
}
