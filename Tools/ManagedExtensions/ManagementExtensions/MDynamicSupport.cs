// Copyright 2003-2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.ManagementExtensions
{
	using System;

	/// <summary>
	/// Summary description for MDynamicSupport.
	/// </summary>
	public interface MDynamicSupport
	{
		/// <summary>
		/// TODO: Summary
		/// </summary>
		/// <param name="action"></param>
		/// <param name="args"></param>
		/// <param name="signature"></param>
		/// <returns></returns>
		Object Invoke(String action, Object[] args, Type[] signature); 

		/// <summary>
		/// TODO: Summary
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		Object GetAttributeValue(String name);

		/// <summary>
		/// TODO: Summary
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		void SetAttributeValue(String name, Object value);

		/// <summary>
		/// TODO: Summary
		/// </summary>
		ManagementInfo Info
		{
			get;
		}
	}
}
