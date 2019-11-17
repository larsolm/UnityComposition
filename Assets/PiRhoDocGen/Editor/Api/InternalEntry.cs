using System;
using System.Collections.Generic;

namespace PiRhoSoft.DocGen.Editor
{
	[Serializable]
	public class InternalEntry : ApiEntry
	{
		public string File;
		public List<Member> Bases;
		public List<Member> Members;
	}
}
