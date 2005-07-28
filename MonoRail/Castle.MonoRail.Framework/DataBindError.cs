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

namespace Castle.MonoRail.Framework
{
	using System;

	/// <summary>
	/// Represents an error that occurred when trying to 
	/// databind a property of an instance.
	/// </summary>
	public class DataBindError : IPropertyError
	{
		private String _parent;
		private String _property;
		private Exception _exception;

		public DataBindError( String parent, String property ) : this( parent, property, null )
		{
		}

		public DataBindError( String parent, String property, Exception exception )
		{
			_parent = parent;
			_property = property;
			_exception = exception;
		}

        public String Key
		{
			get { return _parent + "." + Property; }
		}

		public String Parent
		{
			get { return _parent; }
		}

		public String Property
		{
			get { return _property; }
		}

		public Exception Exception
		{
			get { return _exception; }
		}

		public override String ToString()
		{
			if (Exception != null) 
			{
				return Exception.Message;
			}

			return "BindError." + Key;
		}
	}
}
