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

namespace Castle.Components.Validator
{
	using System;

	/// <summary>
	/// Marker attribute for a method that is executed by the <see cref="SelfValidationContributor"/>
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class ValidateSelfAttribute : Attribute
	{
		private RunWhen _runWhen = RunWhen.Everytime;
		private int _executionOrder;

		/// <summary>
		/// Gets or sets when this validation is run.
		/// </summary>
		/// <value>The run when.</value>
		public RunWhen RunWhen
		{
			get { return _runWhen; }
			set { _runWhen = value; }
		}

		/// <summary>
		/// Gets or sets the execution order.
		/// </summary>
		/// <value>The execution order.</value>
		public int ExecutionOrder
		{
			get { return _executionOrder; }
			set { _executionOrder = value; }
		}
	}
}
