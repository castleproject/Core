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

namespace Castle.MicroKernel
{
	using Castle.Model.Configuration;

	/// <summary>
	/// Unit of extension. A facility should use 
	/// the extension points offered by the kernel
	/// to augment its functionality.
	/// </summary>
	public interface IFacility
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="kernel"></param>
		/// <param name="facilityConfig"></param>
		void Init(IKernel kernel, IConfiguration facilityConfig);

		/// <summary>
		/// 
		/// </summary>
		void Terminate();
	}
}
