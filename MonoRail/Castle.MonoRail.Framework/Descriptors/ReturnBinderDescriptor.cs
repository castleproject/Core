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

namespace Castle.MonoRail.Framework.Descriptors
{
	using System;

	/// <summary>
	/// Pendent
	/// </summary>
	public class ReturnBinderDescriptor
	{
		private readonly Type returnType;
		private readonly IReturnBinder returnTypeBinder;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="ReturnBinderDescriptor"/> class.
		/// </summary>
		/// <param name="returnType">Type of the return.</param>
		/// <param name="returnTypeBinder">The return type binder.</param>
		public ReturnBinderDescriptor(Type returnType, IReturnBinder returnTypeBinder)
		{
			this.returnType = returnType;
			this.returnTypeBinder = returnTypeBinder;
		}

		/// <summary>
		/// Gets the type of the return.
		/// </summary>
		/// <value>The type of the return.</value>
		public Type ReturnType
		{
			get { return returnType; }
		}

		/// <summary>
		/// Gets or sets the return type binder.
		/// </summary>
		/// <value>The return type binder.</value>
		public IReturnBinder ReturnTypeBinder
		{
			get { return returnTypeBinder; }
		}
	}
}
