using System;
using System.Collections.Generic;
using System.IO;
using Object = UnityEngine.Object;

namespace PiRhoSoft.CompositionEngine
{
	public struct Variable
	{
		public string Name { get; private set; }
		public VariableValue Value { get; private set; }

		public static Variable Empty => Create(string.Empty, VariableValue.Empty);

		public static Variable Create(string name, VariableValue value)
		{
			return new Variable
			{
				Name = name,
				Value = value
			};
		}

		#region Persistence

		// TODO: needs error reporting

		public static void Save(Variable variable, ref string data, ref List<Object> objects)
		{
			objects = new List<Object>();
			data = variable.Write(objects);
		}

		public static void Save(IList<Variable> variables, ref List<string> data, ref List<Object> objects)
		{
			data = new List<string>();
			objects = new List<Object>();

			foreach (var variable in variables)
			{
				var variableData = variable.Write(objects);
				data.Add(variableData);
			}
		}

		public static void Load(Variable variable, ref string data, ref List<Object> objects)
		{
			variable.Read(data, objects);

			data = null;
			objects = null;
		}

		public static void Load(IList<Variable> variables, ref List<string> data, ref List<Object> objects)
		{
			variables.Clear();

			foreach (var variableData in data)
			{
				var variable = Empty;
				variable.Read(variableData, objects);
				variables.Add(variable);
			}

			data = null;
			objects = null;
		}

		private string Write(List<Object> objects)
		{
			using (var stream = new MemoryStream())
			{
				using (var writer = new BinaryWriter(stream))
				{
					writer.Write(Name);
					Value.Write(writer, objects);
				}

				return Convert.ToBase64String(stream.ToArray());
			}
		}

		private bool Read(string data, List<Object> objects)
		{
			try
			{
				var bytes = Convert.FromBase64String(data);

				using (var stream = new MemoryStream(bytes))
				{
					using (var reader = new BinaryReader(stream))
					{
						Name = reader.ReadString();

						// Make a temporary copy because 'ref this' doesn't work because Value copies the backing field
						var value = VariableValue.Empty;
						value.Read(reader, objects);
						Value = value;
					}
				}

				return true;
			}
			catch
			{
				return false;
			}
		}

		#endregion
	}
}
