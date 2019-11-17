using System;
using System.Collections.Generic;

namespace PiRhoSoft.DocGen.Editor
{
	[Serializable]
	public class Member : ApiEntry
	{
		public string TypeId;
		public List<Member> Members;
	}
}
