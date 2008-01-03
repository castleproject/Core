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

namespace Castle.MonoRail.Framework.Descriptors
{
	using System;
	using Castle.MonoRail.Framework;

	/// <summary>
	/// Represents the meta information and type of
	/// an implementation of <see cref="ITransformFilter"/>.
	/// </summary>
	public class TransformFilterDescriptor
	{
		private readonly Type transformFilterType;
		private readonly int executionOrder;
//		private TransformFilterAttribute attribute;

		/// <summary>
		/// Initializes a new instance of the <see cref="TransformFilterDescriptor"/> class.
		/// </summary>
		/// <param name="transformFilterType">Type of the transform filter.</param>
		/// <param name="executionOrder">The execution order.</param>
		/// <param name="attribute">The attribute.</param>
		public TransformFilterDescriptor(Type transformFilterType, int executionOrder, TransformFilterAttribute attribute)
		{
			this.transformFilterType = transformFilterType;
//			this.attribute = attribute;
			this.executionOrder = executionOrder;
		}

//		/// <summary>
//		/// Gets the attribute.
//		/// </summary>
//		/// <value>The attribute.</value>
//		public TransformFilterAttribute Attribute
//		{
//			get { return attribute; }
//		}

		/// <summary>
		/// Gets the type of the transform filter.
		/// </summary>
		/// <value>The type of the transform filter.</value>
		public Type TransformFilterType
		{
			get { return transformFilterType; }
		}

		/// <summary>
		/// Gets the execution order.
		/// </summary>
		/// <value>The execution order.</value>
		public int ExecutionOrder
		{
			get { return executionOrder; }
		}
	}
}
