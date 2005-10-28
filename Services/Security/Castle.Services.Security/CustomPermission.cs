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

namespace Castle.Services.Security
{
	using System;
	using System.Security;
	using System.Security.Permissions;
	using System.Security.Principal;
	using System.Threading;

	[Serializable]
	public sealed class CustomPermission : IPermission, ISecurityEncodable
	{
		private string permissionName;

		public CustomPermission(String permissionName)
		{
			this.permissionName = permissionName;
		}

		public CustomPermission(PermissionState state)
		{
		}

		#region IPermission implementation

		public IPermission Copy()
		{
			return new CustomPermission(permissionName);
		}

		public IPermission Intersect(IPermission target)
		{
			if (target == null)
			{
				return null;
			}

			CustomPermission other = target as CustomPermission;

			if (other == null)
			{
				throw new ArgumentException("Wrong type specified. Expecting CustomPermission", "target");
			}

			if (other.permissionName.Equals(permissionName))
			{
				return new CustomPermission(permissionName);
			}

			return null;
		}

		public IPermission Union(IPermission target)
		{
			return Copy();
		}

		public bool IsSubsetOf(IPermission target)
		{
			if (target == null)
			{
				return false;
			}

			CustomPermission other = target as CustomPermission;

			if (other == null)
			{
				throw new ArgumentException("Wrong type specified. Expecting CustomPermission", "target");
			}

			return (other.permissionName.Equals(permissionName));
		}

		public void Demand()
		{
			IPrincipal principal = Thread.CurrentPrincipal;

			IExtendedPrincipal extendedPrincipal = principal as IExtendedPrincipal;

			if (extendedPrincipal == null)
			{
				throw new SecurityException("The current principal does not implement IExtendedPrincipal");
			}

			if (!extendedPrincipal.HasPermission(permissionName))
			{
				throw new SecurityException("Current principal does not have permission " + permissionName);
			}
		}

		#endregion

		#region ISecurityEncodable implementation

		public SecurityElement ToXml()
		{
			SecurityElement elem = new SecurityElement("IPermission");
			
			elem.AddAttribute("class", 
				String.Format("{0}, {1}", 
					typeof(CustomPermission).FullName, typeof(CustomPermission).Assembly.FullName ));
			elem.AddAttribute("version", "1");
			elem.AddAttribute("Unrestricted", "false");

			if (permissionName != null)
			{
				elem.AddAttribute("permName", permissionName);
			}
			
			return elem;
		}

		public void FromXml(SecurityElement elem)
		{
			object permName = elem.Attributes["permName"];
			
			if (permName != null)
			{
				permissionName = permName.ToString();
			}
		}

		#endregion
	}
}
