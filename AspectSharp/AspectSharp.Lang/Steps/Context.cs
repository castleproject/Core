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

namespace AspectSharp.Lang.Steps
{
	using System;
	using System.ComponentModel;

	using AspectSharp.Lang.AST;

	public delegate void ErrorDelegate( LexicalInfo info, String message );

	/// <summary>
	/// Summary description for Context.
	/// </summary>
	public class Context
	{
		private object ErrorEvent = new object();
		private bool _hasErrors = false;
		private EventHandlerList _events;

		public Context()
		{
			_events = new EventHandlerList();
		}

		private void SetHasError()
		{
			_hasErrors = true;
		}

		public bool HasErrors
		{
			get { return _hasErrors; }
		}

		public event ErrorDelegate Error
		{
			add { _events.AddHandler(ErrorEvent, value); }
			remove { _events.RemoveHandler(ErrorEvent, value); }
		}

		public void RaiseErrorEvent( LexicalInfo info, String message )
		{
			SetHasError();

			ErrorDelegate errorDelegate = (ErrorDelegate) _events[ErrorEvent];

			if (errorDelegate != null)
			{
				errorDelegate(info, message);
			}
		}
	}
}
