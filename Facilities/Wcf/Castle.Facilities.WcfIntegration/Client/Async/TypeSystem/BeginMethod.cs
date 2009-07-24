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
	using System.Reflection;
	using System.ServiceModel;
	using System.ServiceModel.Description;

	[Serializable]
	public class BeginMethod : AsyncMethod
	{
		private readonly ParameterInfo[] parameters;

		public BeginMethod(MethodInfo syncMethod, AsyncType type) 
			: base(syncMethod, type)
		{
			parameters = ObtainParameters(syncMethod);
		}

		public override string Name
		{
			get { return "Begin" + SyncMethod.Name; }
		}

		public override Type ReturnType
		{
			get { return typeof(IAsyncResult); }
		}

		public override Type DeclaringType
		{
			get
			{
				//NOTE: it's an ugly hack so that we can match end methods to begin methods,
				//      when there are many overloads of sync method (when name is not enough)
				AsyncType.PushLastAccessedBeginMethod(this);
				return AsyncType;
			}
		}

		private ParameterInfo[] ObtainParameters(MethodInfo syncMethod)
		{
			var parameters = syncMethod.GetParameters();
			Array.Resize(ref parameters, parameters.Length + 2);
			parameters[parameters.Length - 2] = new AsyncMethodParameter(typeof(AsyncCallback), this);
			parameters[parameters.Length - 1] = new AsyncMethodParameter(typeof(object), this);
			return parameters;
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			//NOTE: Do we have to take 'inherit' parameter into account here somehow?
			if (attributeType == typeof(OperationContractAttribute))
			{
				var operationContract = SyncMethod.GetAttribute<OperationContractAttribute>();
				operationContract.AsyncPattern = true;
				return new[] { operationContract };
			}

			if (typeof(IOperationBehavior).IsAssignableFrom(attributeType) ||
				attributeType == typeof(ServiceKnownTypeAttribute) ||
				attributeType == typeof(FaultContractAttribute))
			{
				return new object[0];
			}

			return SyncMethod.GetCustomAttributes(attributeType, inherit);
		}

		public override ParameterInfo[] GetParameters()
		{
			return parameters;
		}
	}
}