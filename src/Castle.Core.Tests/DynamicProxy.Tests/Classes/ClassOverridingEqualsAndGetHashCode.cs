// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.Tests.Classes
{
	using System;

#if FEATURE_SERIALIZATION
	[Serializable]
#endif
	public class ClassOverridingEqualsAndGetHashCode
	{
		private Guid _id;
		private string _name;

		public virtual Guid Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public virtual string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public virtual bool Equals(ClassOverridingEqualsAndGetHashCode other)
		{
			if (other == null)
				return false;

			// use this pattern to compare value members
			if (!Id.Equals(other.Id))
				return false;

			// use this pattern to compare reference members
			// if (!Object.Equals(Id, other.Id)) return false;

			return true;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			if (!(obj is ClassOverridingEqualsAndGetHashCode))
				return false;

			// safe because of the GetType check
			return Equals((ClassOverridingEqualsAndGetHashCode) obj);
		}

		public override int GetHashCode()
		{
			int hash = 7;

			hash = 31*hash + Id.GetHashCode();

			return hash;
		}
	}
}