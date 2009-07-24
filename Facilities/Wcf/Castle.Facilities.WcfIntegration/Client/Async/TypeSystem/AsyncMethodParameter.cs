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
	public class AsyncMethodParameter : ParameterInfo
	{
		private readonly Type type;
		private readonly AsyncMethod method;

		public AsyncMethodParameter(Type type, AsyncMethod method)
		{
			this.type = type;
			this.method = method;
		}

		public override MemberInfo Member
		{
			get { return method; }
		}

		public override Type ParameterType
		{
			get { return type; }
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			return new object[0];
		}
	}
}