// Copyright 2004-2016 Castle Project - http://www.castleproject.org/
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

#if FEATURE_SECURITY_PERMISSIONS

namespace Castle.DynamicProxy.Tests
{
	using System.Windows.Forms;

	using NUnit.Framework;

	[TestFixture]
	public class WinFormsTestCase : BasePEVerifyTestCase
	{
		[Test]
		[ExcludeOnFramework(Framework.Mono, "Disabled on Mono to remove the need to have X installed.")]
		public void Can_proxy_windows_forms_control()
		{
			var proxy = generator.CreateClassProxy<Control>();
			Assert.IsNotNull(proxy);
		}
	}
}

#endif