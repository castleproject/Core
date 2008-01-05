// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.Core
{
	/// <summary>
	/// This interface should be implemented by classes
	/// that are available in a bigger context, exposing
	/// the container to different areas in the same application.
	/// <para>
	/// For example, in Web application, the (global) HttpApplication
	/// subclasses should implement this interface to expose 
	/// the configured container
	/// </para>
	/// </summary>
	public interface IServiceProviderExAccessor
	{
		IServiceProviderEx ServiceProvider { get; }
	}
}