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

namespace NVelocity.App.Events
{
	using System;
	using System.Collections;

	public class ReferenceInsertionEventArgs : EventArgs
	{
		private Stack referenceStack;
		private Object originalValue, newValue;
		private String rootString;

		public ReferenceInsertionEventArgs(Stack referenceStack, String rootString, Object value)
		{
			this.rootString = rootString;
			this.referenceStack = referenceStack;
			originalValue = newValue = value;
		}

		public Stack GetCopyOfReferenceStack()
		{
			return (Stack) referenceStack.Clone();
		}

		public String RootString
		{
			get { return rootString; }
		}

		public Object OriginalValue
		{
			get { return originalValue; }
		}

		public Object NewValue
		{
			get { return newValue; }
			set { newValue = value; }
		}
	}

	///// <summary>
	///// Reference 'Stream insertion' event handler.  Called with object
	///// that will be inserted into stream via value.toString().
	///// Make sure you return an Object that will toString() without throwing an exception.
	///// </summary>
	//public delegate void ReferenceInsertionEventHandler(Object sender, ReferenceInsertionEventArgs e);
}