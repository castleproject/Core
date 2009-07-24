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

	[Serializable]
	public class EndMethod : AsyncMethod
	{
		private readonly ParameterInfo[] parameters;

		public EndMethod(MethodInfo syncMethod, AsyncType type)
			: base(syncMethod, type)
		{
			parameters = ObtainParameters(syncMethod);
		}

		public override ParameterInfo[] GetParameters()
		{
			return parameters;
		}

		public override Type ReturnType
		{
			get { return SyncMethod.ReturnType; }
		}

		public override string Name
		{
			get { return "End" + SyncMethod.Name; }
		}

		public override Type DeclaringType
		{
			get { return AsyncType; }
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			return new object[0];
		}

		private ParameterInfo[] ObtainParameters(MethodInfo syncMethod)
		{
			ParameterInfo[] parameters = syncMethod.GetParameters();
			Array.Resize(ref parameters, parameters.Length + 1);
			parameters[parameters.Length - 1] = new AsyncMethodParameter(typeof(IAsyncResult), this);
			return parameters;
		}
	}
}