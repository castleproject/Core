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

	public interface IRescuesAttribute
	{
		/// <summary>
		/// Gets the list of rescues to be applied.
		/// This list should be fixed, as this method will only be called once,
		/// when creating the controller and action descriptors.
		/// </summary>
		RescueItem[] GetRescues();
	}

	public sealed class RescueItem
	{
		private readonly string viewName;
		private readonly Type exceptionType;

		public RescueItem(string viewName, Type exceptionType)
		{
			this.viewName = viewName;
			this.exceptionType = exceptionType;

			if (viewName == null || viewName.Length == 0)
				throw new ArgumentNullException("viewName");

			if (exceptionType != null && !typeof(Exception).IsAssignableFrom(exceptionType))
				throw new ArgumentException("exceptionType must be a type assignable from Exception");
		}

		public RescueItem(string viewName)
			: this(viewName, null)
		{
		}

		public string ViewName
		{
			get { return viewName; }
		}

		public Type ExceptionType
		{
			get { return exceptionType; }
		}
	}
}