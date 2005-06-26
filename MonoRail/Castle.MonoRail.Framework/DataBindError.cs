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
		private string _Parent;
		private string _Property;
		private Exception _Exception;

		public DataBindError( string parent, string property ) : this( parent, property, null )
		{
		}

		public DataBindError( string parent, string property, Exception exception )
		{
			_Parent = parent;
			_Property = property;
			_Exception = exception;
		}

        public string Key
		{
			get { return _Parent + "." + Property; }
		}

		public string Parent
		{
			get { return _Parent; }
		}

		public string Property
		{
			get { return _Property; }
		}

		public Exception Exception
		{
			get { return _Exception; }
		}

		public override string ToString()
		{
			return "BindError." + Key;
		}
	}
}
