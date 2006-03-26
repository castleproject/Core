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

namespace Castle.MonoRail.Framework
{
	using System;

	/// <summary>
	/// Associates a helper class with the controller.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=true), Serializable]
	public class HelperAttribute : Attribute, IHelpersAttribute
	{
		private HelperItem[] helpers;

		public HelperAttribute(Type helperType)
			: this(helperType, null)
		{
		}

		public HelperAttribute(Type helperType, String name)
		{
			this.helpers = new HelperItem[] { new HelperItem(helperType, name) };
		}

		public HelperItem[] GetHelpers()
		{
			return helpers;
		}

		public Type HelperType
		{
			get { return helpers[0].HelperType; }
		}

		public String Name
		{
			get { return helpers[0].Key; }
		}
	}
}