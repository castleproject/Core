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
	using System.Globalization;
	using System.Reflection;
	using System.ServiceModel;

	[Serializable]
	public abstract class AsyncMethod : MethodInfo
	{
		private readonly RuntimeMethodHandle handle;

		protected AsyncMethod(MethodInfo syncMethod, AsyncType type)
		{
			if (syncMethod == null)
			{
				throw new ArgumentNullException("syncMethod");
			}

			if (type == null)
			{
				throw new ArgumentNullException("type");
			}

			if (type.SyncType != syncMethod.DeclaringType)
			{
				throw new ArgumentException("The given method is not defined on given type", "syncMethod");
			}

			VerifyContract(syncMethod);

			SyncMethod = syncMethod;
			AsyncType = type;
			handle = ObtainNewHandle(syncMethod);
		}

		public AsyncType AsyncType { get; private set; }

		public abstract override Type ReturnType { get; }

		public override ICustomAttributeProvider ReturnTypeCustomAttributes
		{
			get { return ReturnParameter; }
		}

		public abstract override string Name { get; }

		public override Type ReflectedType
		{
			get { throw new NotImplementedException(); }
		}

		public override RuntimeMethodHandle MethodHandle
		{
			get { return handle; }
		}

		public override MethodAttributes Attributes
		{
			get { throw new NotImplementedException(); }
		}

		public MethodInfo SyncMethod { get; private set; }

		public override ParameterInfo ReturnParameter
		{
			get { return new AsyncMethodParameter(ReturnType, this); }
		}

		protected void VerifyContract(MethodInfo syncMethod)
		{
			var attribute = syncMethod.GetAttribute<OperationContractAttribute>();

			if (attribute == null)
			{
				throw new ArgumentException(
					"The given method does not have OperationContractAttribute defined and " +
					"cannot be used with the asynchronous pattern.",
					"syncMethod");
			}

			if (attribute.AsyncPattern)
			{
				throw new ArgumentException(
					"The given method has OperationContractAttribute with AsyncPattern property set to true, " +
					"which suggests it already participating in an asynchronous pattern",
					"syncMethod");
			}
		}

		protected virtual RuntimeMethodHandle ObtainNewHandle(MethodInfo syncMethod)
		{
			//NOTE: this is an ugly hack, but it'll work - we only need this handle to be different than syncMethod' handle
			return HandleProvider.GetNextHandle();
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			throw new NotImplementedException();
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			throw new NotImplementedException();
		}

		public override MethodImplAttributes GetMethodImplementationFlags()
		{
			throw new NotImplementedException();
		}

		public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters,
									  CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		public override MethodInfo GetBaseDefinition()
		{
			throw new NotImplementedException();
		}
	}
}