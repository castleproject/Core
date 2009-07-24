// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.Facilities.WcfIntegration.Async.TypeSystem
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using System.Runtime.CompilerServices;
	using System.Runtime.Serialization;
	using System.ServiceModel;
	using System.Threading;

	[Serializable]
	public class AsyncType : TypeDelegator, ISerializable
	{
		private readonly Type syncType;
		private readonly IDictionary<RuntimeMethodHandle, BeginMethod> beginMethods;
		private readonly IDictionary<RuntimeMethodHandle, EndMethod> endMethods;
		private BeginMethod lastAccessedBeginMethod;

		private static readonly IDictionary<Type, AsyncType> typeToAsyncType = new Dictionary<Type, AsyncType>();
		private static ReaderWriterLock locker = new ReaderWriterLock();

		protected AsyncType(SerializationInfo info, StreamingContext context)
			: this(GetBaseType(info))
		{
		}

		private AsyncType(Type type) : base(type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}

			if (!type.IsInterface)
			{
				throw new ArgumentException("Interface type expected.", "type");
			}

			syncType = type;
			beginMethods = new Dictionary<RuntimeMethodHandle, BeginMethod>();
			endMethods = new Dictionary<RuntimeMethodHandle, EndMethod>();

			CollectAsynchronousMethods();
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("syncType", syncType.AssemblyQualifiedName);
		}

		public Type SyncType
		{
			get { return syncType; }
		}

		public static AsyncType GetAsyncType<T>()
		{
			return GetAsyncType(typeof(T));
		}

		public static AsyncType GetAsyncType(Type type)
		{
			AsyncType asyncType;

			try
			{
				locker.AcquireReaderLock(Timeout.Infinite);

				if (!typeToAsyncType.TryGetValue(type, out asyncType))
				{
					locker.UpgradeToWriterLock(Timeout.Infinite);

					if (!typeToAsyncType.TryGetValue(type, out asyncType))
					{
						asyncType = new AsyncType(type);
						typeToAsyncType.Add(type, asyncType);
					}
				}
			}
			finally
			{
				locker.ReleaseLock();
			}

			return asyncType;
		}

		public override Type BaseType
		{
			get
			{
				Type baseType = base.BaseType;
				//TODO: wrap it if it's a service contracts too
				return baseType;
			}
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			bool isDefined = base.IsDefined(attributeType, inherit);
			//TODO: review this piece if it needs some magic too
			return isDefined;
		}


		public override Type[] GetInterfaces()
		{
			Type[] interfaces = base.GetInterfaces();
			//TODO: wrap these if they are service contracts too
			return interfaces;
		}

		public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
		{
			var methods = syncType.GetMethods(bindingAttr);
			//NOTE: if we had generic co/contravariance in .NET 3.5 we wouldn't have to do casting
			//      we could even do ToArray stuff, but since we don't we have to do it the hard way.
			return methods.Concat(beginMethods.Values.Cast<MethodInfo>())
				.Concat(endMethods.Values.Cast<MethodInfo>()).ToArray();
		}

		private bool IsSyncOperation(MethodInfo method)
		{
			var operationContract = method.GetAttribute<OperationContractAttribute>();
			return (operationContract != null) && (operationContract.AsyncPattern == false);
		}

		public override MemberInfo[] GetMember(string name, MemberTypes type, BindingFlags bindingAttr)
		{
			var baseResult = syncType.GetMember(name, bindingAttr);
			if ((type & MemberTypes.Method) != MemberTypes.Method)
			{
				//Not looking for methods, nothing we can add here
				return baseResult;
			}

			//NOTE: since WCF only uses this method for looking up end methods we try to match only end methods as well
			if (!name.StartsWith("End", StringComparison.Ordinal))
			{
				return baseResult;
			}

			string potentialSyncMethodName = name.Substring("End".Length);
			if (IsMatchingEndMethodForLastAccessedBeginMethod(potentialSyncMethodName))
			{
				return new MemberInfo[] {endMethods[lastAccessedBeginMethod.SyncMethod.MethodHandle]};
			}

			return baseResult.Union(endMethods.Values.Where(m =>
				m.SyncMethod.Name.Equals(potentialSyncMethodName, StringComparison.Ordinal)
				).Cast<MemberInfo>()).ToArray();
		}

		private bool IsMatchingEndMethodForLastAccessedBeginMethod(string potentialSyncMethodName)
		{
			return lastAccessedBeginMethod != null &&
				   lastAccessedBeginMethod.SyncMethod.Name.Equals(potentialSyncMethodName, StringComparison.Ordinal);
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			object[] customAttributes = base.GetCustomAttributes(attributeType, inherit);
			return customAttributes;
		}

		public BeginMethod GetBeginMethod(MethodInfo syncMethod)
		{
			if (syncMethod == null)
			{
				throw new ArgumentNullException("syncMethod");
			}
			return beginMethods[syncMethod.MethodHandle];
		}

		public void PushLastAccessedBeginMethod(BeginMethod beginMethod)
		{
			if (beginMethod == null)
			{
				throw new ArgumentNullException("beginMethod");
			}
			lastAccessedBeginMethod = beginMethod;
		}

		public EndMethod GetEndMethod(MethodInfo syncMethod)
		{
			if (syncMethod == null)
			{
				throw new ArgumentNullException("syncMethod");
			}
			return endMethods[syncMethod.MethodHandle];
		}

		private void CollectAsynchronousMethods()
		{
			foreach (var method in syncType.GetMethods(
				BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
			{
				if (IsSyncOperation(method))
				{
					beginMethods.Add(method.MethodHandle, new BeginMethod(method, this));
					endMethods.Add(method.MethodHandle, new EndMethod(method, this));
				}
			}
		}

		private static Type GetBaseType(SerializationInfo information)
		{
			string typeName = information.GetString("syncType");
			return GetType(typeName);
		}
	}
}