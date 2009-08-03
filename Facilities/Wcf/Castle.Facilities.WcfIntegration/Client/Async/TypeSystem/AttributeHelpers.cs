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
	using System.Linq;
	using System.Reflection;

	public static class AttributeHelpers
	{
		public static TAttribute GetAttribute<TAttribute>(this ICustomAttributeProvider provider)
			where TAttribute:Attribute
		{
			return GetAttribute<TAttribute>(provider, true);
		}

		public static TAttribute GetAttribute<TAttribute>(this ICustomAttributeProvider provider, bool inherit) 
			where TAttribute : Attribute
		{
			return (TAttribute)provider.GetCustomAttributes(typeof(TAttribute), inherit).FirstOrDefault();
		}
	}
}