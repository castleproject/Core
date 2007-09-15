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

namespace Castle.MonoRail.Framework.Internal
{
	using System;

	/// <summary>
	/// Represents a rescue configuration
	/// </summary>
	public class RescueDescriptor
	{
		private readonly string viewName;
		private readonly Type exceptionType;

		/// <summary>
		/// Initializes a new instance of the <see cref="RescueDescriptor"/> class.
		/// </summary>
		/// <param name="viewName">Name of the rescue view.</param>
		/// <param name="exceptionType">Type of the exception it is associated with.</param>
		public RescueDescriptor(string viewName, Type exceptionType)
		{
			this.viewName = viewName;
			this.exceptionType = exceptionType;
		}

		/// <summary>
		/// Gets the name of the rescue view.
		/// </summary>
		/// <value>The name of the view.</value>
		public string ViewName
		{
			get { return viewName; }
		}

		/// <summary>
		/// Gets the type of the exception this rescue is associated with.
		/// </summary>
		/// <value>The type of the exception.</value>
		public Type ExceptionType
		{
			get { return exceptionType; }
		}
	}
}
