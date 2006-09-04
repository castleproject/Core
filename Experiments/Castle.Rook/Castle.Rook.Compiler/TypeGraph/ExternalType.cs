// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.Rook.Compiler.TypeGraph
{
	using System;
	using System.Reflection;


	public class ExternalType : TypeDefinition
	{
		private TypeDelegator delegator;

		public ExternalType(Type type)
		{
			delegator = new TypeDelegator(type);
		}

		public TypeDelegator Type
		{
			get { return delegator; }
		}

		public override String Name
		{
			get { return delegator.Name; }
		}

		public override String FullName
		{
			get { return delegator.FullName; }
		}

		public override bool HasInstanceMember(String name)
		{
			MemberInfo[] members = delegator.GetMember(name, BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic);
			return members.Length != 0;
		}

		public override bool HasStaticMember(String name)
		{
			MemberInfo[] members = delegator.GetMember(name, BindingFlags.Static|BindingFlags.Public|BindingFlags.NonPublic);
			return members.Length != 0;
		}
	}
}