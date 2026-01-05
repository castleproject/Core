// Copyright 2004-2026 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.Generators
{
	using System;
	using System.Diagnostics;
	using System.Reflection;
	using System.Text;

	using Castle.DynamicProxy.Internal;

	internal abstract class MetaTypeElement
	{
		private readonly MemberInfo member;
		private string name;

		protected MetaTypeElement(MemberInfo member)
		{
			this.member = member;
			this.name = member.Name;
		}

		public bool CanBeImplementedExplicitly
		{
			get { return member.DeclaringType?.IsInterface ?? false; }
		}

		public string Name
		{
			get { return name; }
		}

		protected MemberInfo Member
		{
			get { return member; }
		}

		public abstract void SwitchToExplicitImplementation();

		protected void SwitchToExplicitImplementationName()
		{
			var name = member.Name;
			var declaringType = member.DeclaringType;

			if (declaringType.IsGenericType || declaringType.IsNested)
			{
				var builder = new StringBuilder();
				builder.AppendNamespaceQualifiedNameOf(declaringType).Append('.').Append(name);
				this.name = builder.ToString();
			}
			else
			{
				var ns = declaringType.Namespace;
				if (string.IsNullOrEmpty(ns))
				{
					this.name = string.Concat(declaringType.Name, ".", name);
				}
				else
				{
					this.name = string.Concat(ns, ".", declaringType.Name, ".", name);
				}
			}
		}
	}
}