// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

	using Castle.MonoRail.Framework.Internal;

	/// <summary>
	/// Associates a rescue template with a <see cref="Controller"/> or an action 
	/// (method). The rescue is invoked in response to some exception during the 
	/// action processing.
	/// </summary>
	/// <remarks>
	/// The view must exist in the <c>rescues</c> folder in your view folder
	/// </remarks>
	[AttributeUsage(AttributeTargets.Class|AttributeTargets.Method, AllowMultiple=true), Serializable]
	public class RescueAttribute : Attribute, IRescueDescriptorBuilder
	{
		private readonly String viewName;
		private readonly Type exceptionType;
		
		/// <summary>
		/// Constructs a RescueAttribute with the template name.
		/// </summary>
		/// <param name="viewName">The view to use in the event of error</param>
		public RescueAttribute(String viewName) : this(viewName, typeof(Exception))
		{			
		}

		/// <summary>
		/// Constructs a RescueAttribute with the template name and exception type.
		/// </summary>
		/// <param name="viewName">The view to use in the event of error</param>
		/// <param name="exceptionType">The exception to match</param>
		public RescueAttribute(String viewName, Type exceptionType)
		{
			if (viewName == null || viewName.Length == 0)
			{
				throw new ArgumentNullException("viewName");
			}
			
			if (exceptionType != null && !typeof(Exception).IsAssignableFrom(exceptionType))
			{
				throw new ArgumentException("exceptionType must be a type assignable from Exception");
			}
			
			this.viewName = viewName;
			this.exceptionType = exceptionType;
		}

		/// <summary>
		/// Gets the view name to use
		/// </summary>
		public String ViewName
		{
			get { return viewName; }
		}
		
		/// <summary>
		/// Gets the exception type
		/// </summary>
		public Type ExceptionType
		{
			get { return exceptionType; }
		}

		/// <summary>
		/// <see cref="IRescueDescriptorBuilder"/> implementation. 
		/// Builds the rescue descriptors.
		/// </summary>
		/// <returns></returns>
		public RescueDescriptor[] BuildRescueDescriptors()
		{
			return new RescueDescriptor[] { new RescueDescriptor(viewName, exceptionType) };
		}
	}
}
