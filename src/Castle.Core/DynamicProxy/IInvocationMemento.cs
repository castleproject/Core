// Copyright 2004-2019 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy
{
	using System.ComponentModel;

	/// <summary>
	///   Represents a full or partial state of an <see cref="IInvocation"/> captured at an earlier time.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public interface IInvocationMemento
	{
		/// <summary>
		///   Restores the state captured by this instance to the associated <see cref="IInvocation"/>.
		/// </summary>
		void Restore();
	}
}
