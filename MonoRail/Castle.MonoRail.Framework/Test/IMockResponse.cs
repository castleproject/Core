// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Test
{
	using System.Collections.Specialized;
	using Framework;

	/// <summary>
	/// Exposes methods on top of <see cref="IResponse"/>
	/// that are used by unit tests
	/// </summary>
	public interface IMockResponse : IResponse
	{
		/// <summary>
		/// Gets the urls the request was redirected to.
		/// </summary>
		/// <value>The redirected to.</value>
		string RedirectedTo { get; }

		/// <summary>
		/// Gets the http headers.
		/// </summary>
		/// <value>The headers.</value>
		NameValueCollection Headers { get; }
	}
}
