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
	using System.Runtime.Serialization;
	using System.ServiceModel;
	using System.ServiceModel.Description;
	using System.Threading;
	using Castle.Facilities.WcfIntegration.Internal;

	[Serializable]
	public class AsyncType : TypeDelegator, ISerializable
	{
		private Type[] interfaces;
		private readonly Type syncType;
		private readonly Dictionary<RuntimeMethodHandle, MethodInfo> beginMethods;
		private readonly Dictionary<RuntimeMethodHandle, MethodInfo> endMethods;
		private readonly Dictionary<RuntimeMethodHandle, BeginMethod> fakeBeginMethods;
		private readonly Dictionary<RuntimeMethodHandle, EndMethod> fakeEndMethods;
		private BeginMethod lastAccessedBeginMethod;

		private static readonly IDictionary<Type, AsyncType> typeToAsyncType = new Dictionary<Type, AsyncType>();
		private static ReaderWriterLock locker = new ReaderWriterLock();

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

			beginMethods = new Dictionary<RuntimeMethodHandle, MethodInfo>();
			endMethods = new Dictionary<RuntimeMethodHandle, MethodInfo>();
			fakeBeginMethods = new Dictionary<RuntimeMethodHandle, BeginMethod>();
			fakeEndMethods = new Dictionary<RuntimeMethodHandle, EndMethod>();

			CollectAsynchronousMethods();
		}

		protected AsyncType(SerializationInfo info, StreamingContext context)
			: this(GetBaseType(info))
		{
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("syncType", syncType.AssemblyQualifiedName);
		}

		public Type SyncType
		{
			get { return syncType; }
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			//TODO: review this piece if it needs some magic too
			return base.IsDefined(attributeType, inherit);
		}

		public override Type[] GetInterfaces()
		{
			return WcfUtils.SafeInitialize(ref interfaces, () =>
			{
				var effectiveInterfaces = syncType.GetInterfaces();
				for (int i = 0; i < effectiveInterfaces.Length; ++i)
				{
					effectiveInterfaces[i] = GetEffectiveType(effectiveInterfaces[i]);	
				}
				return effectiveInterfaces;
			});
		}

		public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
		{
			var methods = syncType.GetMethods(bindingAttr);
			return methods.Concat(fakeBeginMethods.Values.Cast<MethodInfo>())
				.Concat(fakeEndMethods.Values.Cast<MethodInfo>()).ToArray();
		}

		public override MemberInfo[] GetMember(string name, MemberTypes type, BindingFlags bindingAttr)
		{
			var baseResult = syncType.GetMember(name, bindingAttr);
			if ((type & MemberTypes.Method) != MemberTypes.Method)
			{
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
				return new MemberInfo[] {fakeEndMethods[lastAccessedBeginMethod.SyncMethod.MethodHandle]};
			}

			return baseResult.Union(fakeEndMethods.Values.Where(m =>
				m.SyncMethod.Name.Equals(potentialSyncMethodName, StringComparison.Ordinal)
				).Cast<MemberInfo>()).ToArray();
		}

		private bool IsMatchingEndMethodForLastAccessedBeginMethod(string potentialSyncMethodName)
		{
			return lastAccessedBeginMethod != null &&
				   lastAccessedBeginMethod.SyncMethod.Name.Equals(potentialSyncMethodName, StringComparison.Ordinal);
		}

		public MethodInfo GetBeginMethod(MethodInfo syncMethod)
		{
			if (syncMethod.DeclaringType == syncType)
			{
				BeginMethod beginMethod;
				if (fakeBeginMethods.TryGetValue(syncMethod.MethodHandle, out beginMethod))
					return beginMethod;
				return beginMethods[syncMethod.MethodHandle];
			}

			foreach (var asyncType in GetInterfaces().OfType<AsyncType>()
				.Where(i => i.SyncType == syncMethod.DeclaringType))
			{
				BeginMethod beginMethod;
				if (asyncType.fakeBeginMethods.TryGetValue(syncMethod.MethodHandle, out beginMethod))
					return beginMethod;
				return asyncType.beginMethods[syncMethod.MethodHandle];
			}

			return null;
		}

		public void PushLastAccessedBeginMethod(BeginMethod beginMethod)
		{
			lastAccessedBeginMethod = beginMethod;
		}

		public MethodInfo GetEndMethod(MethodInfo syncMethod)
		{
			if (syncMethod.DeclaringType == syncType)
			{
				EndMethod endMethod;
				if (fakeEndMethods.TryGetValue(syncMethod.MethodHandle, out endMethod))
					return endMethod;
				return endMethods[syncMethod.MethodHandle];
			}

			foreach (var asyncType in GetInterfaces().OfType<AsyncType>()
				.Where(i => i.SyncType == syncMethod.DeclaringType))
			{
				EndMethod endMethod;
				if (asyncType.fakeEndMethods.TryGetValue(syncMethod.MethodHandle, out endMethod))
					return endMethod;
				return asyncType.endMethods[syncMethod.MethodHandle];
			}

			return null;
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

		private void CollectAsynchronousMethods()
		{
			var contract = ContractDescription.GetContract(syncType);

			foreach (var operation in contract.Operations)
			{
				var syncMethod = operation.SyncMethod;

				if (syncMethod != null && syncMethod.DeclaringType == syncType)
				{
					if (operation.BeginMethod == null)
					{
						fakeBeginMethods.Add(syncMethod.MethodHandle, new BeginMethod(syncMethod, this));
						fakeEndMethods.Add(syncMethod.MethodHandle, new EndMethod(syncMethod, this));
					}
					else
					{
						beginMethods.Add(syncMethod.MethodHandle, operation.BeginMethod);
						endMethods.Add(syncMethod.MethodHandle, operation.EndMethod);
					}
				}
			}
		}

		private Type GetEffectiveType(Type asyncCandidateType)
		{
			if (asyncCandidateType != null && asyncCandidateType != typeof(object))
			{
				var serviceContract = asyncCandidateType.GetAttribute<ServiceContractAttribute>(false);
				if (serviceContract != null)
				{
					return GetAsyncType(asyncCandidateType);
				}
			}
			return asyncCandidateType;
		}

		private static Type GetBaseType(SerializationInfo information)
		{
			string typeName = information.GetString("syncType");
			return GetType(typeName);
		}
	}
}