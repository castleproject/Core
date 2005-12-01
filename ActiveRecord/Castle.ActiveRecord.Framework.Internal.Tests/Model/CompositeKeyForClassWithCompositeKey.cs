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

using System;
using Castle.ActiveRecord;

namespace Castle.ActiveRecord.Framework.Internal.Tests.Model
{
	[CompositeKey, Serializable]
	public class CompositeKeyForClassWithCompositeKey
	{
		private string _keyA;
		private string _keyB;

		[KeyProperty]
		public virtual string KeyA
		{
			get { return _keyA; }
			set { _keyA = value; }
		}

		[KeyProperty]
		public virtual string KeyB
		{
			get { return _keyB; }
			set { _keyB = value; }
		}

		public override string ToString()
		{
			return string.Join( ":", new string[] { _keyA, _keyB } );
		}

		public override bool Equals( object obj )
		{
			if( obj == this ) return true;
			if( obj == null || obj.GetType() != this.GetType() ) return false;
			CompositeKeyForClassWithCompositeKey test = ( CompositeKeyForClassWithCompositeKey ) obj;
			return ( _keyA == test.KeyA || (_keyA != null && _keyA.Equals( test.KeyA ) ) ) &&
				( _keyB == test.KeyB || ( _keyB != null && _keyB.Equals( test.KeyB ) ) );
		}

		public override int GetHashCode()
		{
			return _keyA.GetHashCode() ^ _keyB.GetHashCode();
		}
	}
}
