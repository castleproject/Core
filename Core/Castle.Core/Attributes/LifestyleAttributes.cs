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

		protected LifestyleAttribute(LifestyleType type)
		{
			this.type = type;
		}

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

		public PooledAttribute() : this(Initial_PoolSize, Max_PoolSize)
		{
			
		}

		public PooledAttribute(int initialPoolSize, int maxPoolSize) : base(LifestyleType.Pooled)
		{
			this.initialPoolSize = initialPoolSize;
			this.maxPoolSize = maxPoolSize;
		}

		public int InitialPoolSize
		{
			get { return initialPoolSize; }
		}

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
		private Type lifestyleHandler;

		public CustomLifestyleAttribute( Type lifestyleHandler ) : base(LifestyleType.Custom)
		{
			this.lifestyleHandler = lifestyleHandler;
		}

		public Type LifestyleHandlerType
		{
			get { return lifestyleHandler; }
		}
	}
}
