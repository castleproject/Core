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

namespace Castle.ActiveRecord.Tests.Model.CompositeModel
{
	using System;

	/// <summary>
	/// Summary description for BlogKey.
	/// </summary>
	[CompositeKey, Serializable]
	public class AgentKey
	{
		private string _orgId;
		private string _name;

		public AgentKey()
		{
		}

		public AgentKey(string orgId, string name)
		{
			this._orgId = orgId;
			this._name = name;
		}

		[KeyProperty]
		public virtual string OrgId
		{
			get { return _orgId; }
			set { _orgId = value; }
		}

		[KeyProperty]
		public virtual string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public override string ToString()
		{
			return String.Join(":", new string[] {_orgId, _name});
		}

		public override bool Equals(object obj)
		{
			if (null == obj)
			{
				return false;
			}
			if (ReferenceEquals(this, obj))
			{
				return true;
			}
			if (!(obj is AgentKey))
			{
				return false;
			}
			AgentKey rhs = (AgentKey) obj;
			return (this._orgId == rhs._orgId || (this._orgId != null && this._orgId.Equals(rhs._orgId))) &&
				(this._name == rhs._name || (this._name != null && this._name.Equals(rhs._name)));
		}

		public override int GetHashCode()
		{
			return (this._orgId.GetHashCode() ^ this._name.GetHashCode());
		}
	}
}