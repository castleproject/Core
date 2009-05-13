// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.MicroKernel.Registration
{
	using System;
	using Castle.Core.Configuration;

	#region Node
	
	/// <summary>
	/// Represents a configuration child.
	/// </summary>
	public abstract class Node
	{
		private readonly String name;

		protected Node(String name)
		{
			this.name = name;
		}

		protected string Name
		{
			get { return name; }
		}	
		
		/// <summary>
		/// Applies the configuration node.
		/// </summary>
		/// <param name="configuration">The configuration.</param>
		public abstract void ApplyTo(IConfiguration configuration);
	}
	
	#endregion

	#region Attribute

	/// <summary>
	/// Represents a configuration attribute.
	/// </summary>
	public class Attrib : Node
	{
		private readonly String value;

		internal Attrib(String name, String value)
			: base(name)
		{
			this.value = value;
		}

		/// <summary>
		/// Applies the configuration node.
		/// </summary>
		/// <param name="configuration">The configuration.</param>
		public override void ApplyTo(IConfiguration configuration)
		{
			configuration.Attributes.Add(Name, value);
		}
		
		/// <summary>
		/// Create a <see cref="NamedAttribute"/> with name.
		/// </summary>
		/// <param name="name">The attribute name.</param>
		/// <returns>The new <see cref="NamedAttribute"/></returns>
		public static NamedAttribute ForName(String name)
		{
			return new NamedAttribute(name);
		}
	}

	#endregion

	#region NamedChild
	
	/// <summary>
	/// Represents a named attribute.
	/// </summary>
	public class NamedAttribute
	{
		private readonly String name;
		
		internal NamedAttribute(String name)
		{
			this.name = name;
		}

		/// <summary>
		/// Builds the <see cref="Attribute"/> with name/value.
		/// </summary>
		/// <param name="value">The attribute value.</param>
		/// <returns>The new <see cref="SimpleChild"/></returns>
		public Attrib Eq(String value)
		{
			return new Attrib(name, value);
		}

		/// <summary>
		/// Builds the <see cref="Attribute"/> with name/value.
		/// </summary>
		/// <param name="value">The attribute value.</param>
		/// <returns>The new <see cref="SimpleChild"/></returns>
		public Attrib Eq(object value)
		{
			String valueStr = (value != null) ? value.ToString() : String.Empty;
			return new Attrib(name, valueStr);
		}		
	}
	
	#endregion
				
	#region Child 
	
	/// <summary>
	/// Represents a configuration child.
	/// </summary>
	public abstract class Child
	{
		/// <summary>
		/// Create a <see cref="NamedChild"/> with name.
		/// </summary>
		/// <param name="name">The child name.</param>
		/// <returns>The new <see cref="NamedChild"/></returns>
		public static NamedChild ForName(String name)
		{
			return new NamedChild(name);
		}
	}

	#endregion
	
	#region NamedChild
	
	/// <summary>
	/// Represents a named child.
	/// </summary>
	public class NamedChild : Node
	{
		internal NamedChild(String name)
			: base(name)
		{
		}

		/// <summary>
		/// Builds the <see cref="SimpleChild"/> with name/value.
		/// </summary>
		/// <param name="value">The child value.</param>
		/// <returns>The new <see cref="SimpleChild"/></returns>
		public SimpleChild Eq(String value)
		{
			return new SimpleChild(Name, value);
		}

		/// <summary>
		/// Builds the <see cref="SimpleChild"/> with name/value.
		/// </summary>
		/// <param name="value">The child value.</param>
		/// <returns>The new <see cref="SimpleChild"/></returns>
		public SimpleChild Eq(object value)
		{
			String valueStr = (value != null) ? value.ToString() : String.Empty;
			return new SimpleChild(Name, valueStr);
		}		
		
		/// <summary>
		/// Builds the <see cref="ComplexChild"/> with name/config.
		/// </summary>
		/// <param name="configNode">The child configuration.</param>
		/// <returns>The new <see cref="ComplexChild"/></returns>
		public ComplexChild Eq(IConfiguration configNode)
		{
			return new ComplexChild(Name, configNode);
		}

		/// <summary>
		/// Builds the <see cref="Child"/> with name/config.
		/// </summary>
		/// <param name="childNodes">The child nodes.</param>
		/// <returns>The new <see cref="CompoundChild"/></returns>
		public CompoundChild Eq(params Node[] childNodes)
		{
			return new CompoundChild(Name, childNodes);
		}

		/// <summary>
		/// Applies the configuration node.
		/// </summary>
		/// <param name="configuration">The configuration.</param>
		public override void ApplyTo(IConfiguration configuration)
		{
			MutableConfiguration node = new MutableConfiguration(Name);
			configuration.Children.Add(node);
		}
	}
	
	#endregion
	
	#region SimpleChild
	
	/// <summary>
	/// Represents a simple child node.
	/// </summary>
	public class SimpleChild : Node
	{
		private readonly String value;

		internal SimpleChild(String name, String value)
			: base(name)
		{
			this.value = value;
		}

		/// <summary>
		/// Applies the configuration node.
		/// </summary>
		/// <param name="configuration">The configuration.</param>
		public override void ApplyTo(IConfiguration configuration)
		{
			MutableConfiguration node = new MutableConfiguration(Name, value);
			configuration.Children.Add(node);
		}
	}

	#endregion
	
	#region ComplexChild
	
	/// <summary>
	/// Represents a complex child node.
	/// </summary>
	public class ComplexChild : Node
	{
		private readonly IConfiguration configNode;

		internal ComplexChild(String name, IConfiguration configNode)
			: base(name)
		{
			this.configNode = configNode;
		}

		/// <summary>
		/// Applies the configuration node.
		/// </summary>
		/// <param name="configuration">The configuration.</param>
		public override void ApplyTo(IConfiguration configuration)
		{
			MutableConfiguration node = new MutableConfiguration(Name);
			node.Children.Add(configNode);
			configuration.Children.Add(node);
		}		
	}

	#endregion
	
	#region CompoundChild
	
	/// <summary>
	/// Represents a compound child node.
	/// </summary>
	public class CompoundChild : Node
	{
		private readonly Node[] childNodes;

		internal CompoundChild(String name, Node[] childNodes)
			: base(name)
		{
			this.childNodes = childNodes;
		}
		
		/// <summary>
		/// Applies the configuration node.
		/// </summary>
		/// <param name="configuration">The configuration.</param>
		public override void ApplyTo(IConfiguration configuration)
		{
			MutableConfiguration node = new MutableConfiguration(Name);
			foreach (Node childNode in childNodes)
			{
				childNode.ApplyTo(node);
			}
			configuration.Children.Add(node);
		}
	}
	
	#endregion
}