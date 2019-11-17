using System;
using System.Collections.Generic;

namespace PiRhoSoft.DocGen.Editor
{
	[Serializable]
	public class Api
	{
		public List<InternalEntry> InternalEntries;
		public List<ExternalEntry> ExternalEntries;
	}
}
