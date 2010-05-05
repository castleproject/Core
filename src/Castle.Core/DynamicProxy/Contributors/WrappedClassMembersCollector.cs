// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.Contributors
{
	using System;
	using System.Reflection;
	using System.Runtime.CompilerServices;

	using Castle.DynamicProxy.Generators.Emitters;

	public class WrappedClassMembersCollector : ClassMembersCollector
	{
		public WrappedClassMembersCollector(Type type) : base(type)
		{
		}

		public override void CollectMembersToProxy(IProxyGenerationHook hook)
		{
			base.CollectMembersToProxy(hook);
			CollectFields(hook);
			// TODO: perhaps we should also look for nested classes...
		}

		private void CollectFields(IProxyGenerationHook hook)
		{
			var fields = TypeUtil.GetAllFields(type);
			foreach (var field in fields)
			{
				if(IsOKToBeOnProxy(field)) continue;

				hook.NonProxyableMemberNotification(type, field);
			}
		}

		protected virtual bool IsOKToBeOnProxy(FieldInfo field)
		{
			return IsGeneratedByTheCompiler(field);
		}

		protected bool IsGeneratedByTheCompiler(FieldInfo field)
		{
			// for example fields backing autoproperties
			return Attribute.IsDefined(field, typeof(CompilerGeneratedAttribute));
		}
	}
}