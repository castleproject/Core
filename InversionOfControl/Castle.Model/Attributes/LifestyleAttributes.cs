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

namespace Castle.Model
{
	using System;

	/// <summary>
	/// Base for Attributes that want to express lifestyle
	/// chosen by the component.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
	public abstract class LifestyleAttribute : Attribute
	{
		private LifestyleType _type;

		protected LifestyleAttribute(LifestyleType type)
		{
			_type = type;
		}

		public LifestyleType Lifestyle
		{
			get { return _type; }
			set { _type = value; }
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
	/// custom lifestyle.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
	public class CustomLifestyleAttribute : LifestyleAttribute
	{
		private Type _lifestyleHandler;

		public CustomLifestyleAttribute( Type lifestyleHandler ) : base(LifestyleType.Custom)
		{
			_lifestyleHandler = lifestyleHandler;
		}

		public Type LifestyleHandlerType
		{
			get { return _lifestyleHandler; }
		}
	}
}
