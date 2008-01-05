// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Views.Brail.Macros
{
	/// <summary>
	/// we need to have a unique name, because of MR 371, nested components should not use the same name.
	/// </summary>
	public class ComponentNaming
	{
		public static string GetComponentNameFor(object obj)
		{
			return "component" + obj.GetHashCode();
		}

		public static string GetComponentContextName(object obj)
		{
			return "componentContext" + obj.GetHashCode();
		}

		public static string GetComponentFactoryName(object obj)
		{
			return "viewComponentFactory" + obj;
		}
	}
}