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

namespace Castle.ActiveRecord.Framework.Internal.Tests.Model
{
	using System;

	[Serializable]
	public class CompositeKeyForClassWithCompositeKey
	{
		private string keyA;
		private string keyB;

		[KeyProperty]
		public virtual string KeyA
		{
			get { return keyA; }
			set { keyA = value; }
		}

		[KeyProperty]
		public virtual string KeyB
		{
			get { return keyB; }
			set { keyB = value; }
		}

		public override string ToString()
		{
			return string.Join( ":", new string[] { keyA, keyB } );
		}

		public override bool Equals( object obj )
		{
			if( obj == this ) return true;
			if( obj == null || obj.GetType() != this.GetType() ) return false;
			CompositeKeyForClassWithCompositeKey test = ( CompositeKeyForClassWithCompositeKey ) obj;
			return ( keyA == test.KeyA || (keyA != null && keyA.Equals( test.KeyA ) ) ) &&
				( keyB == test.KeyB || ( keyB != null && keyB.Equals( test.KeyB ) ) );
		}

		public override int GetHashCode()
		{
			return keyA.GetHashCode() ^ keyB.GetHashCode();
		}
	}
}
