// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Extensions.ExceptionChaining
{
	using System;
	using System.Collections;
	using System.Reflection;
	using System.Xml;

	/// <summary>
	/// This class expects to be configured with exclude types that detail the types of 
	/// exceptions to be ignored.
	/// 
	/// <code>
	///     <exclude type="System.Security.SecurityException, mscrolib" />
	/// </code>
	/// </summary>
	public class FilteredExceptionHandler : AbstractExceptionHandler, IConfigurableHandler
	{
		private ArrayList excludedTypes = new ArrayList();

		/// <summary>
		/// Implementors should check for known attributes and child nodes
		/// within the <c>exceptionHandlerNode</c>
		/// </summary>
		/// <param name="exceptionHandlerNode">The Xml node
		/// that represents this handler on the configuration file</param>
		public void Configure(XmlNode exceptionHandlerNode)
		{
			XmlNodeList excludeNodes = exceptionHandlerNode.SelectNodes("exclude");
			foreach (XmlNode excludeNode in excludeNodes)
			{
				string excludedType = excludeNode.Attributes["type"].Value;
				excludedTypes.Add(Type.GetType(excludedType, true));
			}
		}

		/// <summary>
		/// Implementors should perform the action
		/// on the exception. Note that the exception
		/// is available in <see cref="IRailsEngineContext.LastException"/>
		/// </summary>
		/// <param name="context"></param>
		public override void Process(IRailsEngineContext context)
		{
			Exception ex = context.LastException is TargetInvocationException
			               	? context.LastException.InnerException
			               	: context.LastException;
			if (!excludedTypes.Contains(ex.GetType()))
			{
				InvokeNext(context);
			}
		}
	}
}