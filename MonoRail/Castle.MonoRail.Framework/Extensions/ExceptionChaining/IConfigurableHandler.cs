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

namespace Castle.MonoRail.Framework.Extensions.ExceptionChaining
{
	using System.Xml;

	/// <summary>
	/// Extends <see cref="IExceptionHandler"/> providing
	/// an <see cref="IConfigurableHandler.Configure"/> method
	/// that is invoked by the framework.
	/// </summary>
	public interface IConfigurableHandler : IExceptionHandler
	{
		/// <summary>
		/// Implementors should check for known attributes and child nodes
		/// within the <c>exceptionHandlerNode</c>
		/// </summary>
		/// <param name="exceptionHandlerNode">The Xml node 
		/// that represents this handler on the configuration file</param>
		void Configure(XmlNode exceptionHandlerNode);
	}
}
