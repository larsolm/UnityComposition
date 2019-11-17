using PiRhoSoft.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.DocGen.Editor
{
	public class Generator
	{
		public static string RootPath = new DirectoryInfo(Application.dataPath).Parent.FullName;

		public enum State
		{
			Waiting,
			Starting,
			Running,
			Done,
			Error
		}

		private Settings _settings;
		private Dictionary<Type, string> _pathsByType;
		private Dictionary<string, ExternalEntry> _externalById;
		private Dictionary<Type, InternalEntry> _internalByType;
		private Dictionary<string, InternalEntry> _internalById;

		private Thread _thread;

		private class ThreadData
		{
			public Settings Settings;
			public Api Api;
			public Action<string, float> Progress;
		}

		public void Generate(Settings settings)
		{
			var api = new Api();
			Generate(settings, api, null);
		}

		public void Generate(Settings settings, Action<string, float> progress)
		{
			var data = new ThreadData
			{
				Settings = settings,
				Api = new Api(),
				Progress = progress
			};

			_thread = new Thread(GenerateThread);
			_thread.Start(data);
			
			progress?.Invoke("Generation Started", 0.01f);
		}

		private void GenerateThread(object data)
		{
			var threadData = data as ThreadData;
			Generate(threadData.Settings, threadData.Api, threadData.Progress);
		}

		private void Generate(Settings settings, Api api, Action<string, float> progress)
		{
			try
			{
				progress?.Invoke("Generating Api", 0.25f);

				var types = AppDomain.CurrentDomain.GetAssemblies()
					.Where(assembly => settings.Assemblies.Contains(assembly.GetName().Name))
					.SelectMany(assembly => assembly.GetTypes())
					.Where(type => IsTypeIncluded(type));

				_settings = settings;
				_pathsByType = IdentifyPaths(types);
				_externalById = new Dictionary<string, ExternalEntry>();
				_internalByType = types.ToDictionary(type => type, type => CreateEntry(type));
				_internalById = _internalByType.ToDictionary(type => type.Value.Id, type => type.Value);

				foreach (var ns in settings.Namespaces)
					_internalById.Add(GenerateId(ns), CreateEntry(ns));

				foreach (var entry in _internalByType)
					FillEntry(entry.Key, entry.Value);

				api.InternalEntries = _internalById.Select(entry => entry.Value).OrderBy(entry => entry.Id).ToList();
				api.ExternalEntries = _externalById.Select(reference => reference.Value).OrderBy(reference => reference.Id).ToList();

				_settings = null;
				_pathsByType = null;
				_externalById = null;
				_internalByType = null;
				_internalById = null;

				progress?.Invoke("Writing Output", 0.75f);

				var path = Path.GetFullPath(Path.Combine(RootPath, settings.OutputFile));
				var file = new FileInfo(path);
				var json = JsonUtility.ToJson(api, true);

				Directory.CreateDirectory(file.Directory.FullName);
				File.WriteAllText(path, json);

				progress?.Invoke("Complete", 1.0f);
			}
			catch (Exception e)
			{
				Debug.LogError($"Documentation generation failed: {e.Message}");
				progress?.Invoke("Failed", 1.0f);
			}
		}

		#region Inclusion

		private Dictionary<Type, string> IdentifyPaths(IEnumerable<Type> types)
		{
			Regex _namespaceExpression = new Regex(@"(namespace)\s([^\s]+)");
			Regex _entryExpression = new Regex(@"(class|struct|enum|interface)\s([^<\s]+(<|))");

			string GetLookup(Type type)
			{
				var tilde = type.Name.IndexOf('`');

				return tilde >= 0
					? type.Name.Substring(0, tilde) + '<'
					: type.Name;
			}

			var root = Path.GetFullPath(Path.Combine(RootPath, _settings.CodePath));
			var files = Directory.EnumerateFiles(root, "*.cs", SearchOption.AllDirectories);
			var lookup = types.ToLookup(type => GetLookup(type), type => type);
			var map = new Dictionary<Type, string>();

			foreach (var file in files)
			{
				var path = file.Substring(root.Length);
				var contents = File.ReadAllText(file);

				var namespaceMatch = _namespaceExpression.Match(contents);
				var ns = namespaceMatch.Success ? namespaceMatch.Groups[2].Value : string.Empty;

				var entryNames = new List<string>();
				var entryMatches = _entryExpression.Matches(contents);

				foreach (var match in entryMatches.OfType<Match>())
				{
					var name = match.Groups[2].Value;
					entryNames.Add(name);
				}

				foreach (var name in entryNames)
				{
					var possibilities = lookup[name];
					var count = types.Count();

					foreach (var type in possibilities)
					{
						if (type.IsGenericType != name.EndsWith("<"))
							continue;

						if (type.Namespace == ns && (!type.IsNested || entryNames.Contains(GetLookup(type.DeclaringType))))
						{
							if (!map.ContainsKey(type))
								map.Add(type, path);

							break;
						}
					}
				}
			}

			return map;
		}

		private bool IsTypeIncluded(Type type)
		{
			if (!_settings.Namespaces.Contains(type.Namespace))
				return false;

			var includeBehaviours = _settings.Declarations.HasFlag(Settings.DeclarationType.Behaviour);
			var includeAssets = _settings.Declarations.HasFlag(Settings.DeclarationType.Asset);
			var includeAbstracts = _settings.Declarations.HasFlag(Settings.DeclarationType.Abstract);
			var includeInterfaces = _settings.Declarations.HasFlag(Settings.DeclarationType.Interface);
			var includeClasses = _settings.Declarations.HasFlag(Settings.DeclarationType.Class);
			var includeEnums = _settings.Declarations.HasFlag(Settings.DeclarationType.Enum);
			var includeStructs = _settings.Declarations.HasFlag(Settings.DeclarationType.Struct);
			var includeGenerated = _settings.Declarations.HasFlag(Settings.DeclarationType.Generated);

			var isBehaviour = typeof(MonoBehaviour).IsAssignableFrom(type);
			var isAsset = typeof(ScriptableObject).IsAssignableFrom(type);
			var isAbstract = type.IsAbstract;
			var isInterface = type.IsInterface;
			var isClass = type.IsClass && !isAbstract && !isBehaviour && !isAsset;
			var isEnum = type.IsEnum;
			var isStruct = type.IsValueType && !isEnum;
			var isGenerated = type.GetTypeInfo().IsDefined(typeof(CompilerGeneratedAttribute), true);

			if (isBehaviour && !includeBehaviours) return false;
			if (isAsset && !includeAssets) return false;
			if (isAbstract && !includeAbstracts) return false;
			if (isInterface && !includeInterfaces) return false;
			if (isClass && !includeClasses) return false;
			if (isEnum && !includeEnums) return false;
			if (isStruct && !includeStructs) return false;
			if (isGenerated && !includeGenerated) return false;

			return IsIncluded(type.IsPublic, type.IsNestedPrivate, type.IsNestedFamily, type.IsNestedFamORAssem, type.IsNestedAssembly || (!type.IsNested && !type.IsPublic), type.IsNestedFamANDAssem);
		}

		private bool IsMemberIncluded(ConstructorInfo constructor)
		{
			return IsIncluded(constructor.IsPublic, constructor.IsPrivate, constructor.IsFamily, constructor.IsFamilyOrAssembly, constructor.IsAssembly, constructor.IsFamilyAndAssembly);
		}

		private bool IsMemberIncluded(FieldInfo field)
		{
			return IsIncluded(field.IsPublic, field.IsPrivate, field.IsFamily, field.IsFamilyOrAssembly, field.IsAssembly, field.IsFamilyAndAssembly);
		}

		private bool IsMemberIncluded(PropertyInfo property)
		{
			var method = property.GetGetMethod(true) ?? property.GetSetMethod(true);
			return IsMemberIncluded(method);
		}

		private bool IsMemberIncluded(MethodInfo method)
		{
			return IsIncluded(method.IsPublic, method.IsPrivate, method.IsFamily, method.IsFamilyOrAssembly, method.IsAssembly, method.IsFamilyAndAssembly);
		}

		private bool IsIncluded(bool isPublic, bool isPrivate, bool isProtected, bool isProtectedInternal, bool isInternal, bool isPrivateProtected)
		{
			var includePublic = _settings.Access.HasFlag(Settings.AccessLevel.Public);
			var includeProtected = _settings.Access.HasFlag(Settings.AccessLevel.Protected);
			var includePrivate = _settings.Access.HasFlag(Settings.AccessLevel.Private);
			var includeInternal = _settings.Access.HasFlag(Settings.AccessLevel.Internal);

			var protectedTest = isProtected || isProtectedInternal || (includeInternal && isPrivateProtected);
			var internalTest = isInternal || isProtectedInternal || (includeProtected && isPrivateProtected);

			if (isPublic && !includePublic) return false;
			if (isPrivate && !includePrivate) return false;
			if (protectedTest && !includeProtected) return false;
			if (internalTest && !includeInternal) return false;

			return true;
		}

		#endregion

		#region References

		private ExternalEntry GetExternal(Type type)
		{
			var id = GenerateId(type);

			if (!_externalById.TryGetValue(id, out var reference))
			{
				var owner = type.IsNested
					? GetExternal(type.DeclaringType)
					: GetExternal(type.Namespace);

				reference = new ExternalEntry
				{
					Id = id,
					OwnerId = owner.Id,
					Name = type.Name
				};

				AssignUrl(reference, type);
				_externalById.Add(id, reference);
			}

			return reference;
		}

		private void AssignUrl(ExternalEntry entry, Type type)
		{
			foreach (var external in _settings.Urls)
			{
				var expression = new Regex(external.Namespace);
				if (expression.IsMatch(type.Namespace))
				{
					entry.Url = external.UrlFormat
						.Replace("{Name}", entry.Name)
						.Replace("{Id}", entry.Id);

					break;
				}
			}
		}

		private ExternalEntry GetExternal(string ns)
		{
			if (ns == null)
				return null;

			var id = GenerateId(ns);

			if (!_externalById.TryGetValue(id, out var reference))
			{
				reference = new ExternalEntry
				{
					Id = id,
					OwnerId = string.Empty,
					Name = ns
				};

				AssignUrl(reference, ns);
				_externalById.Add(id, reference);
			}

			return reference;
		}

		private void AssignUrl(ExternalEntry entry, string ns)
		{
			foreach (var external in _settings.Urls)
			{
				var expression = new Regex(external.Namespace);
				if (expression.IsMatch(ns))
				{
					entry.Url = external.UrlFormat
						.Replace("{Name}", entry.Name)
						.Replace("{Id}", entry.Id);

					break;
				}
			}
		}

		#endregion

		#region Declarations

		private InternalEntry CreateEntry(Type type)
		{
			return new InternalEntry
			{
				Id = GetTypeId(type),
				File = _pathsByType.ContainsKey(type) ? _pathsByType[type] : string.Empty,
				Members = new List<Member>(),
				Bases = new List<Member>()
			};
		}

		private InternalEntry CreateEntry(string ns)
		{
			return new InternalEntry
			{
				Id = GenerateId(ns),
				OwnerId = string.Empty,
				Name = ns,
				File = string.Empty,
				ModifierEnum = Modifier.Namespace,
				Bases = null,
				Members = null
			};
		}

		private void FillEntry(Type type, InternalEntry entry)
		{
			Modifier GetKind()
			{
				if (type.IsInterface) return Modifier.Interface;
				else if (type.IsEnum) return Modifier.Enum;
				else if (type.IsValueType) return Modifier.Struct;
				else return Modifier.Class;
			}

			var kind = GetKind();
			var modifiers = GetModifiers(type);

			entry.OwnerId = type.IsNested ? GetTypeId(type.DeclaringType) : GetNamespaceId(type.Namespace);
			entry.Name = type.Name;
			entry.ModifierEnum = kind | modifiers;

			var binding = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

			switch (kind)
			{
				case Modifier.Class:
				case Modifier.Struct:
				{
					if (type.BaseType != null)
						AddBase(entry, type.BaseType);

					foreach (var i in type.GetInterfaces())
						AddInterface(entry, i);

					foreach (var generic in type.GetGenericArguments())
						AddGeneric(entry, generic);

					foreach (var constructor in type.GetConstructors(binding).Where(c => IsMemberIncluded(c)))
						AddConstructor(entry, constructor);

					foreach (var field in type.GetFields(binding).Where(c => IsMemberIncluded(c)))
						AddField(entry, field);

					foreach (var property in type.GetProperties(binding).Where(c => IsMemberIncluded(c)))
						AddProperty(entry, property);

					foreach (var method in type.GetMethods(binding).Where(c => IsMemberIncluded(c)))
						AddMethod(entry, method);

					break;
				}
				case Modifier.Interface:
				{
					foreach (var i in type.GetInterfaces())
						AddInterface(entry, i);

					foreach (var generic in type.GetGenericArguments())
						AddGeneric(entry, generic);

					foreach (var property in type.GetProperties(binding).Where(c => IsMemberIncluded(c)))
						AddProperty(entry, property);

					foreach (var method in type.GetMethods(binding).Where(c => IsMemberIncluded(c)))
						AddMethod(entry, method);

					break;
				}
				case Modifier.Enum:
				{
					AddBase(entry, type.GetEnumUnderlyingType());

					foreach (var value in type.GetEnumValues().OfType<Enum>())
						AddValue(entry, value);

					break;
				}
			}
		}

		#endregion

		#region Members

		private static Type[] _ignoredBases = new Type[]
		{
			typeof(object)
		};

		private void AddBase(InternalEntry entry, Type baseType)
		{
			if (_ignoredBases.Contains(baseType))
				return;

			var members = new List<Member>();

			foreach (var generic in baseType.GetGenericArguments())
				AddGeneric(members, generic);

			entry.Bases.Add(new Member
			{
				Name = baseType.Name,
				TypeId = GetTypeId(baseType),
				ModifierEnum = Modifier.Class,
				Members = members
			});
		}

		private static Type[] _ignoredInterfaces = new Type[]
		{
			typeof(_Attribute)
		};

		private void AddInterface(InternalEntry entry, Type i)
		{
			if (_ignoredInterfaces.Contains(i))
				return;

			var members = new List<Member>();

			foreach (var generic in i.GetGenericArguments())
				AddGeneric(members, generic);

			entry.Bases.Add(new Member
			{
				Name = i.Name,
				TypeId = GetTypeId(i),
				ModifierEnum = Modifier.Interface,
				Members = members
			});
		}

		private void AddGeneric(InternalEntry entry, Type generic)
		{
			AddGeneric(entry.Members, generic);
		}

		private void AddConstructor(InternalEntry entry, ConstructorInfo constructor)
		{
			var members = new List<Member>();

			foreach (var parameter in constructor.GetParameters())
				AddParameter(members, parameter);

			entry.Members.Add(new Member
			{
				Name = constructor.DeclaringType.Name,
				TypeId = string.Empty,
				ModifierEnum = Modifier.Constructor | GetModifiers(constructor),
				Members = members
			});
		}

		private void AddField(InternalEntry entry, FieldInfo field)
		{
			entry.Members.Add(new Member
			{
				Name = field.Name,
				TypeId = GetTypeId(field.FieldType),
				ModifierEnum = Modifier.Field | GetModifiers(field),
				Members = null
			});
		}

		private void AddProperty(InternalEntry entry, PropertyInfo property)
		{
			entry.Members.Add(new Member
			{
				Name = property.Name,
				TypeId = GetTypeId(property.PropertyType),
				ModifierEnum = Modifier.Property | GetModifiers(property),
				Members = null
			});
		}

		private void AddMethod(InternalEntry entry, MethodInfo method)
		{
			var members = new List<Member>();

			foreach (var generic in method.GetGenericArguments())
				AddGeneric(members, generic);

			foreach (var parameter in method.GetParameters())
				AddParameter(members, parameter);

			entry.Members.Add(new Member
			{
				Name = method.Name,
				TypeId = GetTypeId(method.ReturnType),
				ModifierEnum = Modifier.Method | GetModifiers(method),
				Members = members
			});
		}

		private void AddValue(InternalEntry entry, Enum value)
		{
			entry.Members.Add(new Member
			{
				Name = value.ToString(),
				TypeId = GetTypeId(value.GetType().GetEnumUnderlyingType()),
				ModifierEnum = Modifier.Field | Modifier.Public | Modifier.Const,
				Members = null
			});
		}

		private void AddGeneric(List<Member> members, Type generic)
		{
			// TODO: constraints - generic.GetGenericParameterConstraints() and generic.GenericParameterAttributes

			members.Add(new Member
			{
				Name = generic.Name,
				TypeId = string.Empty,
				ModifierEnum = Modifier.Template,
				Members = null
			});
		}

		private void AddParameter(List<Member> members, ParameterInfo parameter)
		{
			var value = new List<Member>();

			if (parameter.HasDefaultValue)
			{
				value = new List<Member>();

				value.Add(new Member
				{
					Name = parameter.RawDefaultValue?.ToString() ?? "null"
				});
			}

			members.Add(new Member
			{
				Name = parameter.Name,
				TypeId = GetTypeId(parameter.ParameterType),
				ModifierEnum = Modifier.Parameter | GetModifiers(parameter),
				Members = value
			});
		}

		#endregion

		#region Ids

		private string GetTypeId(Type type)
		{
			var isArray = type.IsArray;

			if (isArray)
				type = type.GetElementType();

			var id = IsTypeIncluded(type)
				? GenerateId(type)
				: GetExternal(type).Id;

			if (isArray)
				id += "[]";

			return id;
		}

		private string GetNamespaceId(string name)
		{
			return _settings.Namespaces.Contains(name)
				? GenerateId(name)
				: GetExternal(name).Id;
		}

		private string GenerateId(Type type)
		{
			var id = GenerateId(type.Name);

			if (type.IsNested)
				id = GetTypeId(type.DeclaringType) + "." + id;
			else
				id = GetNamespaceId(type.Namespace) + "." + id;

			return id;
		}

		private string GenerateId(string name)
		{
			return name
				.Replace('`', '-')
				.ToLowerInvariant();
		}

		#endregion

		#region Modifiers

		private Modifier GetModifiers(Type type)
		{
			var modifiers = Modifier.None;

			if (type.IsAbstract && type.IsSealed) modifiers |= Modifier.Static;
			if (type.IsAbstract && !type.IsSealed) modifiers |= Modifier.Abstract;
			if (type.BaseType != null) modifiers |= Modifier.Override;
			if (typeof(Object).IsAssignableFrom(type) || type.HasAttribute<SerializableAttribute>()) modifiers |= Modifier.Serializable;
			if (type.IsPublic) modifiers |= Modifier.Public;
			if (type.IsNestedFamily || type.IsNestedFamORAssem || type.IsNestedFamANDAssem) modifiers |= Modifier.Protected;
			if (type.IsNestedPrivate || type.IsNestedFamANDAssem) modifiers |= Modifier.Private;
			if (type.IsNestedAssembly || type.IsNestedFamORAssem) modifiers |= Modifier.Internal;
			if (typeof(MonoBehaviour).IsAssignableFrom(type)) modifiers |= Modifier.Behaviour;
			if (typeof(ScriptableObject).IsAssignableFrom(type)) modifiers |= Modifier.Asset;

			return modifiers;
		}

		private Modifier GetModifiers(ConstructorInfo constructor)
		{
			var modifiers = Modifier.None;

			if (constructor.IsStatic) modifiers |= Modifier.Static;
			if (constructor.IsPublic) modifiers |= Modifier.Public;
			if (constructor.IsFamily || constructor.IsFamilyOrAssembly || constructor.IsFamilyAndAssembly) modifiers |= Modifier.Protected;
			if (constructor.IsPrivate || constructor.IsFamilyAndAssembly) modifiers |= Modifier.Private;
			if (constructor.IsAssembly || constructor.IsFamilyOrAssembly) modifiers |= Modifier.Internal;

			return modifiers;
		}

		private Modifier GetModifiers(FieldInfo field)
		{
			var modifiers = Modifier.None;

			if (field.IsStatic) modifiers |= Modifier.Static;
			if (field.IsLiteral) modifiers |= Modifier.Const;
			if (field.IsInitOnly) modifiers |= Modifier.ReadOnly;

			if (field.IsPublic) modifiers |= Modifier.Public;
			if (field.IsFamily || field.IsFamilyOrAssembly || field.IsFamilyAndAssembly) modifiers |= Modifier.Protected;
			if (field.IsPrivate || field.IsFamilyAndAssembly) modifiers |= Modifier.Private;
			if (field.IsAssembly || field.IsFamilyOrAssembly) modifiers |= Modifier.Internal;
			if (field.IsSerializable()) modifiers |= Modifier.Serializable;

			return modifiers;
		}

		private Modifier GetModifiers(PropertyInfo property)
		{
			var setMethod = property.GetSetMethod(true);
			var method = property.GetGetMethod(true) ?? setMethod;
			var modifiers = GetModifiers(method);

			if (setMethod == null || !IsMemberIncluded(setMethod)) modifiers |= Modifier.ReadOnly;

			return modifiers;
		}

		private Modifier GetModifiers(MethodInfo method)
		{
			var modifiers = Modifier.None;

			if (method.IsStatic) modifiers |= Modifier.Static;
			if (method.IsAbstract) modifiers|= Modifier.Abstract;
			if (method.IsVirtual && !method.IsFinal) modifiers |= Modifier.Virtual;
			if (method.DeclaringType == method.ReflectedType && !method.IsAbstract) modifiers |= Modifier.Override;

			if (method.IsPublic) modifiers |= Modifier.Public;
			if (method.IsFamily || method.IsFamilyOrAssembly || method.IsFamilyAndAssembly) modifiers |= Modifier.Protected;
			if (method.IsPrivate || method.IsFamilyAndAssembly) modifiers |= Modifier.Private;
			if (method.IsAssembly || method.IsFamilyOrAssembly) modifiers |= Modifier.Internal;

			return modifiers;
		}

		private Modifier GetModifiers(ParameterInfo parameter)
		{
			var modifiers = Modifier.None;

			if (parameter.ParameterType.IsByRef && parameter.IsOut) modifiers |= Modifier.Output;
			if (parameter.ParameterType.IsByRef && !parameter.IsOut) modifiers |= Modifier.Reference;

			return modifiers;
		}

		#endregion
	}
}
