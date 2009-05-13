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

	/// <summary>
	/// Lets an app approve / veto writing a log message when RHS of #set() is null.
	/// </summary>
	public delegate void NullSetEventHandler(Object sender, NullSetEventArgs e);

	public class NullSetEventArgs : EventArgs
	{
		private Boolean shouldLog = true;
		private String lhs, rhs;

		public NullSetEventArgs(String lhs, String rhs)
		{
			this.lhs = lhs;
			this.rhs = rhs;
		}

		/// <summary>
		/// Reference literal of left-hand-side of set statement
		/// </summary>
		public String LHS
		{
			get { return lhs; }
		}

		/// <summary>
		/// reference literal of right-hand-side of set statement
		/// </summary>
		public String RHS
		{
			get { return rhs; }
		}

		public Boolean ShouldLog
		{
			get { return shouldLog; }
			set { shouldLog = value; }
		}
	}
}