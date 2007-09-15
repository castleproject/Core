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

namespace Castle.MonoRail.Framework
{
	using System;
	using System.IO;
	using Castle.MonoRail.Framework.Internal;

	/// <summary>
	/// Decorates an action associating a <see cref="IFilter"/>
	/// implementation with it. More than one can be associated.
	/// </summary>
	/// <remarks>
	/// If more than one filter is associate with an action (or controller and 
	/// action), the order of execution cannot be predicted. In this case
	/// use <see cref="ExecutionOrder"/> to define the order of execution.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true), Serializable]
	public class TransformFilterAttribute : Attribute, ITransformFilterDescriptorBuilder
	{
		private readonly Type transformFilterType;
		private int executionOrder = Int32.MaxValue;
		
		/// <summary>
		/// Constructs a TransformFilterAttribute associating the transformfilter type.
		/// </summary>
		/// <param name="transformFilterType">The transformfilter implementation</param>
		public TransformFilterAttribute(Type transformFilterType)
		{
			if (!typeof(ITransformFilter).IsAssignableFrom(transformFilterType))
			{
				throw new ArgumentException("The specified type does not implement ITransformFilter");
			}

			if (!typeof(Stream).IsAssignableFrom(transformFilterType))
			{
				throw new ArgumentException("The specified type is no System.IO.Stream type");
			}
						
			this.transformFilterType = transformFilterType;
		}

		/// <summary>
		/// Gets the type of the transform filter.
		/// </summary>
		/// <value>The type of the transform filter.</value>
		public Type TransformFilterType
		{
			get { return transformFilterType; }
		}

		/// <summary>
		/// Gets or sets the execution order.
		/// </summary>
		/// <value>The execution order.</value>
		public int ExecutionOrder
		{
			get { return executionOrder; }
			set { executionOrder = value; }
		}

		/// <summary>
		/// Implementation of <see cref="ITransformFilterDescriptorBuilder"/>.
		/// Returns the descriptor for this filter association.
		/// </summary>
		/// <returns></returns>
		public TransformFilterDescriptor[] BuildTransformFilterDescriptors()
		{
			return new TransformFilterDescriptor[] { new TransformFilterDescriptor(TransformFilterType, ExecutionOrder, this) };
		}
	}
}
