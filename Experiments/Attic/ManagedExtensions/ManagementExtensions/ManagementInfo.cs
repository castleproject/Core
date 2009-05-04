// Copyright 2003-2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.ManagementExtensions
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;

	/// <summary>
	/// Summary description for ManagementInfo.
	/// </summary>
	[Serializable]
	public class ManagementInfo
	{
		protected String description;
		// TODO: Replate by Collection
		protected ManagementObjectCollection operations = new ManagementObjectCollection();
		// TODO: Replate by Collection
		protected ManagementObjectCollection attributes = new ManagementObjectCollection();

		public ManagementInfo()
		{
		}

		public String Description
		{
			get
			{
				return description;
			}
			set
			{
				description = value;
			}
		}

		public ManagementObjectCollection Operations
		{
			get
			{
				return operations;
			}
		}

		public ManagementObjectCollection Attributes
		{
			get
			{
				return attributes;
			}
		}
	}

	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class ManagementObject 
	{
		protected String name;
		protected String description;

		public String Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		public String Description
		{
			get
			{
				return description;
			}
			set
			{
				description = value;
			}
		}
	}

	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class ManagementOperation : ManagementObject
	{
		private Type[] arguments = new Type[0];

		public ManagementOperation()
		{
		}

		public ManagementOperation(String name)
		{
			Name = name;
		}

		public ManagementOperation(String name, String description)
		{
			Name = name;
			Description = description;
		}

		public ManagementOperation(String name, String description, Type[] args)
		{
			Name = name;
			Description = description;
			arguments = args;
		}

		public Type[] Arguments
		{
			get
			{
				return arguments;
			}
		}
	}

	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class ManagementAttribute : ManagementObject
	{
		protected Type type;

		public ManagementAttribute(String name, Type attType)
		{
			Name = name;
			type = attType;
		}

		public ManagementAttribute(String name, String description, Type attType)
		{
			Name = name;
			Description = description;
			type = attType;
		}

		public Type AttributeType
		{
			get
			{
				return type;
			}
		}
	}

	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class ManagementObjectCollection : NameObjectCollectionBase, IEnumerable
	{
		public ManagementObjectCollection() : 
			base(CaseInsensitiveHashCodeProvider.Default, CaseInsensitiveComparer.Default)
		{
		}

		public ManagementObjectCollection(
			System.Runtime.Serialization.SerializationInfo info, 
			System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{
		}

		public void Add(ManagementObject obj)
		{
			base.BaseAdd(obj.Name, obj);
		}

		public ManagementObject this[String name]
		{
			get
			{
				return (ManagementObject) base.BaseGet(name);
			}
		}

		public bool Contains(String name)
		{
			return BaseGet(name) != null;
		}

		#region IEnumerable Members

		public new IEnumerator GetEnumerator()
		{
			return base.BaseGetAllValues().GetEnumerator();
		}

		#endregion
	}
}
