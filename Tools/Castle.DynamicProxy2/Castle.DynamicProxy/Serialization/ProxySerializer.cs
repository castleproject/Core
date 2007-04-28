using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using Castle.Core.Interceptor;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Castle.DynamicProxy.Serialization
{
	/// <summary>
	/// Assists in serializing instances of the generated proxy types so that they can be deserialized via <see cref="ProxyObjectReference"/>.
	/// </summary>
	public class ProxySerializer
	{
		public static BinaryFormatter Formatter = new BinaryFormatter();

		public static void SerializeBaseProxyData (SerializationInfo info, object proxy, IInterceptor[] interceptors,
				string[] interfaceNames, Type baseType)
		{
			info.SetType (typeof (ProxyObjectReference));

			info.AddValue ("__interceptors", interceptors);
			info.AddValue ("__interfaces", interfaceNames);
			info.AddValue ("__baseType", baseType);
		}

		
		public static void SerializeInterfaceProxyData (SerializationInfo info, object target, int interfaceGeneratorType, string interfaceName)
		{
			info.AddValue ("__target", target);
			info.AddValue ("__interface_generator_type", interfaceGeneratorType);
			info.AddValue ("__theInterface", interfaceName);
		}

		public static void SerializeClassProxyData (SerializationInfo info, bool delegateToBase, Type targetType, object proxy)
		{
			info.AddValue ("__delegateToBase", delegateToBase);

			if (!delegateToBase)
			{
				MemberInfo[] members = FormatterServices.GetSerializableMembers (targetType);

				Indirection memberValues = new Indirection(FormatterServices.GetObjectData (proxy, members));
				// SubstitutIndirections (memberValues, proxy);
				info.AddValue ("__data", memberValues);
			}
		}

    /// <summary>
    /// Used to circumvent a serialization bug, where direct self references and directly held delegates are not deserialized correctly.
    /// </summary>
		[Serializable]
		public class Indirection
		{
			public readonly object IndirectedObject;

			public Indirection (object indirectedObject)
			{
				IndirectedObject = indirectedObject;
			}
		}
	}
}
