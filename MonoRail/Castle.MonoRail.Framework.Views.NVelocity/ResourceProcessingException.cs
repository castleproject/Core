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

using System;
using NVelocity.Exception;

namespace Castle.MonoRail.Framework.Views.NVelocity
{
	///<summary>
	/// This exception is thrown when an error occurs during resource processing (expansion)
	///</summary>
	public class ResourceProcessingException : VelocityException
	{
		internal ResourceProcessingException(string resourceName, Exception innerException)
			: base("Unable to process resource '" + resourceName + "': " + innerException.Message, innerException)
		{
		}
	}
}
