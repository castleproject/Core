using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization.Formatters.Binary;
using Castle.DynamicProxy.Generators;

namespace Castle.DynamicProxy
{
	/// <summary>
	/// Applied to the assemblies saved by <see cref="ModuleScope"/> in order to persist the cache data included in the persisted assembly.
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
	[CLSCompliant(false)]
	public class CacheMappingsAttribute : Attribute
	{
		private static readonly ConstructorInfo constructor =
			typeof (CacheMappingsAttribute).GetConstructor(new Type[] {typeof (byte[])});

		public static void ApplyTo(AssemblyBuilder assemblyBuilder, Dictionary<CacheKey, string> mappings)
		{
			using (MemoryStream stream = new MemoryStream())
			{
				BinaryFormatter formatter = new BinaryFormatter();
				formatter.Serialize(stream, mappings);
				byte[] bytes = stream.ToArray();
				CustomAttributeBuilder attributeBuilder = new CustomAttributeBuilder(constructor, new object[] {bytes});
				assemblyBuilder.SetCustomAttribute(attributeBuilder);
			}
		}

		private readonly byte[] _serializedCacheMappings;

		public CacheMappingsAttribute(byte[] serializedCacheMappings)
		{
			_serializedCacheMappings = serializedCacheMappings;
		}

		public byte[] SerializedCacheMappings
		{
			get { return _serializedCacheMappings; }
		}

		public Dictionary<CacheKey, string> GetDeserializedMappings()
		{
			using (MemoryStream stream = new MemoryStream(SerializedCacheMappings))
			{
				BinaryFormatter formatter = new BinaryFormatter();
				return (Dictionary<CacheKey, string>) formatter.Deserialize(stream);
			}
		}
	}
}