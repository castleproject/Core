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

namespace Castle.MicroKernel.Tests.ClassComponents
{
	using System;

	using NUnit.Framework;

	
	public class HiperFacility : IFacility
	{
		public bool Initialized = false;
		public bool Terminated = false;

		public HiperFacility()
		{
		}

		public void Init(IKernel kernel, Castle.Model.Configuration.IConfiguration facilityConfig)
		{
			Assert.IsNotNull(kernel);
			Assert.IsNotNull(facilityConfig);

			Initialized = true;
		}

		public void Terminate()
		{
			Terminated = true;
		}
	}
}
