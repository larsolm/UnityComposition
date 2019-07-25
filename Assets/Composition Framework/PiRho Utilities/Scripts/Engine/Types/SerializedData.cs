using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Utilities
{
	public interface ISerializableData
	{
		void Save(BinaryWriter writer, SerializedData data);
		void Load(BinaryReader reader, SerializedData data);
	}

	[Serializable]
	public class SerializedData
	{
		public int Version;
		public string Data;
		public List<Object> References;

		public void Save(ISerializableData data, int version)
		{
			Reset(version);

			if (data != null)
			{
				using (var stream = new MemoryStream())
				{
					using (var writer = new BinaryWriter(stream))
					{
						writer.Write(Version);
						SaveObject(writer, data);
					}

					Data = GetString(stream.ToArray());
				}
			}
			else
			{
				Data = string.Empty;
			}
		}

		public void Load<T>(out T data) where T : class, ISerializableData
		{
			var bytes = GetBytes(Data);

			if (bytes != null)
			{
				using (var stream = new MemoryStream(bytes))
				{
					using (var reader = new BinaryReader(stream))
					{
						Version = reader.ReadInt32();
						data = LoadObject<T>(reader);
					}
				}
			}
			else
			{
				data = null;
			}

			Reset(-1);
		}

		public void SaveInstance(ISerializableData data, int version)
		{
			Reset(version);

			using (var stream = new MemoryStream())
			{
				using (var writer = new BinaryWriter(stream))
				{
					writer.Write(Version);
					data.Save(writer, this);
				}

				Data = GetString(stream.ToArray());
			}
		}

		public void LoadInstance(ISerializableData data)
		{
			var bytes = GetBytes(Data);

			if (bytes != null)
			{
				using (var stream = new MemoryStream(bytes))
				{
					using (var reader = new BinaryReader(stream))
					{
						Version = reader.ReadInt32();
						data.Load(reader, this);
					}
				}
			}

			Reset(-1);
		}

		public void SaveReference(BinaryWriter writer, Object obj)
		{
			if (References == null)
				References = new List<Object>();

			writer.Write(References.Count);
			References.Add(obj);
		}

		public Object LoadReference(BinaryReader reader)
		{
			var index = reader.ReadInt32();
			return References != null && index < References.Count ? References[index] : null;
		}

		public void SaveObject<T>(BinaryWriter writer, T obj)
		{
			writer.Write(obj != null);
			writer.Write(obj is ISerializableData);

			if (obj != null)
			{
				SaveType(writer, obj.GetType());

				if (obj is ISerializableData data)
				{
					data.Save(writer, this);
				}
				else
				{
					var json = JsonUtility.ToJson(obj);
					writer.Write(json);
				}
			}
		}

		public T LoadObject<T>(BinaryReader reader)
		{
			var isValid = reader.ReadBoolean();
			var isData = reader.ReadBoolean();

			if (isValid)
			{
				var type = LoadType(reader);
				var obj = default(T);

				try { obj = (T)Activator.CreateInstance(type); }
				catch { }

				if (isData)
				{
					if (obj is ISerializableData data)
						data.Load(reader, this);
				}
				else
				{
					var json = reader.ReadString();

					if (obj != null)
						JsonUtility.FromJsonOverwrite(json, obj);
				}

				return obj;
			}

			return default;
		}

		public void SaveType(BinaryWriter writer, Type type)
		{
			var name = type != null ? type.AssemblyQualifiedName : string.Empty;
			writer.Write(name);
		}

		public Type LoadType(BinaryReader reader)
		{
			var name = reader.ReadString();

			if (!string.IsNullOrEmpty(name))
			{
				try { return Type.GetType(name); }
				catch { }
			}

			return null;
		}

		public void SaveEnum(BinaryWriter writer, Enum e)
		{
			// Saving as a string is the simplest way of handling enums with non Int32 underlying type. It also allows
			// reordering/adding/removing of enum values without affecting saved data.

			SaveType(writer, e.GetType());
			writer.Write(e.ToString());
		}

		public Enum LoadEnum(BinaryReader reader)
		{
			var type = LoadType(reader);
			var name = reader.ReadString();

			try { return (Enum)Enum.Parse(type, name); }
			catch { }

			return null;
		}

		private void Reset(int version)
		{
			Version = version;
			Data = null;
			References = null;
		}

		private static byte[] GetBytes(string data)
		{
			try { return Convert.FromBase64String(data); }
			catch (FormatException) { return null; }
		}

		private static string GetString(byte[] bytes)
		{
			return Convert.ToBase64String(bytes);
		}
	}
}