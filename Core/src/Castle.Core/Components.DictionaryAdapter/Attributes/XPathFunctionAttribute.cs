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

namespace Castle.Components.DictionaryAdapter
{
#if !SILVERLIGHT
	using System;
	using System.Xml.Xsl;

	[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Property, AllowMultiple = true)]
	public class XPathFunctionAttribute : Attribute
	{
		protected XPathFunctionAttribute(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("Name cannot be empty", "name");
			}
			Name = name;
		}

		public XPathFunctionAttribute(string name, Type functionType) : this(name)
		{
			if (typeof(IXsltContextFunction).IsAssignableFrom(functionType) == false)
			{
				throw new ArgumentException("The functionType does not implement IXsltContextFunction");
			}

			var defaultCtor = functionType.GetConstructor(Type.EmptyTypes);
			if (defaultCtor == null)
			{
				throw new ArgumentException("The functionType does not have a parameterless constructor");
			}
			Function = (IXsltContextFunction)Activator.CreateInstance(functionType);
		}

		public string Name { get; private set; }

		public IXsltContextFunction Function { get; protected set; }

		public string Prefix { get; set; }
	}
#endif
}