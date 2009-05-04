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

	/// <summary>
	/// Represents an instance of and Managed class.
	/// </summary>
	[Serializable]
	public class ManagedInstance
	{
		protected String typeName;
		protected ManagedObjectName name;

		/// <summary>
		/// Creates a ManagedInstance instance.
		/// </summary>
		/// <param name="typeName">Full qualified name of the type</param>
		/// <param name="name"><see cref="ManagedObjectName"/> of instance.</param>
		public ManagedInstance(String typeName, ManagedObjectName name)
		{
			if (typeName == null)
			{
				throw new ArgumentNullException("typeName");
			}
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}

			this.name = name;
			this.typeName = typeName;
		}

		public String TypeName
		{
			get
			{
				return typeName;
			}
		}

		public ManagedObjectName Name
		{
			get
			{
				return name;
			}
		}

		public override bool Equals(object obj)
		{
			ManagedInstance other = (ManagedInstance) obj;

			if (other != null)
			{
				return other.name.Equals(name) && 
					other.typeName.Equals(typeName);
			}
			
			return false;
		}
	
		public override int GetHashCode()
		{
			return name.GetHashCode() ^ typeName.GetHashCode();
		}
	}
}
