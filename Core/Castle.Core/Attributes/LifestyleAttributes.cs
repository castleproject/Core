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

namespace Castle.Core
{
	using System;

	/// <summary>
	/// Base for Attributes that want to express lifestyle
	/// chosen by the component.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
	public abstract class LifestyleAttribute : Attribute
	{
		private LifestyleType type;

		/// <summary>
		/// Initializes a new instance of the <see cref="LifestyleAttribute"/> class.
		/// </summary>
		/// <param name="type">The type.</param>
		protected LifestyleAttribute(LifestyleType type)
		{
			this.type = type;
		}

		/// <summary>
		/// Gets or sets the lifestyle.
		/// </summary>
		/// <value>The lifestyle.</value>
		public LifestyleType Lifestyle
		{
			get { return type; }
			set { type = value; }
		}
	}

	/// <summary>
	/// Indicates that the target components wants a
	/// singleton lifestyle.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
	public class SingletonAttribute : LifestyleAttribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SingletonAttribute"/> class.
		/// </summary>
		public SingletonAttribute() : base(LifestyleType.Singleton)
		{
		}
	}

	/// <summary>
	/// Indicates that the target components wants a
	/// transient lifestyle.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
	public class TransientAttribute : LifestyleAttribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TransientAttribute"/> class.
		/// </summary>
		public TransientAttribute() : base(LifestyleType.Transient)
		{
		}
	}

	/// <summary>
	/// Indicates that the target components wants a
	/// per thread lifestyle.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
	public class PerThreadAttribute : LifestyleAttribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PerThreadAttribute"/> class.
		/// </summary>
		public PerThreadAttribute() : base(LifestyleType.Thread)
		{
		}
	}

	/// <summary>
	/// Indicates that the target components wants a
	/// per web request lifestyle.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class PerWebRequestAttribute : LifestyleAttribute
	{
		public PerWebRequestAttribute() : base(LifestyleType.PerWebRequest)
		{
		}
	}

	/// <summary>
	/// Indicates that the target components wants a
	/// pooled lifestyle.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
	public class PooledAttribute : LifestyleAttribute
	{
		private static readonly int Initial_PoolSize = 5;
		private static readonly int Max_PoolSize = 15;

		private readonly int initialPoolSize;
		private readonly int maxPoolSize;

		/// <summary>
		/// Initializes a new instance of the <see cref="PooledAttribute"/> class
		/// using the default initial pool size (5) and the max pool size (15).
		/// </summary>
		public PooledAttribute() : this(Initial_PoolSize, Max_PoolSize)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PooledAttribute"/> class.
		/// </summary>
		/// <param name="initialPoolSize">Initial size of the pool.</param>
		/// <param name="maxPoolSize">Max pool size.</param>
		public PooledAttribute(int initialPoolSize, int maxPoolSize) : base(LifestyleType.Pooled)
		{
			this.initialPoolSize = initialPoolSize;
			this.maxPoolSize = maxPoolSize;
		}

		/// <summary>
		/// Gets the initial size of the pool.
		/// </summary>
		/// <value>The initial size of the pool.</value>
		public int InitialPoolSize
		{
			get { return initialPoolSize; }
		}

		/// <summary>
		/// Gets the maximum pool size.
		/// </summary>
		/// <value>The size of the max pool.</value>
		public int MaxPoolSize
		{
			get { return maxPoolSize; }
		}
	}

	/// <summary>
	/// Indicates that the target components wants a
	/// custom lifestyle.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
	public class CustomLifestyleAttribute : LifestyleAttribute
	{
		private readonly Type lifestyleHandlerType;

		/// <summary>
		/// Initializes a new instance of the <see cref="CustomLifestyleAttribute"/> class.
		/// </summary>
		/// <param name="lifestyleHandlerType">The lifestyle handler.</param>
		public CustomLifestyleAttribute(Type lifestyleHandlerType) : base(LifestyleType.Custom)
		{
			this.lifestyleHandlerType = lifestyleHandlerType;
		}

		/// <summary>
		/// Gets the type of the lifestyle handler.
		/// </summary>
		/// <value>The type of the lifestyle handler.</value>
		public Type LifestyleHandlerType
		{
			get { return lifestyleHandlerType; }
		}
	}
}