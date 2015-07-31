// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

namespace Castle.Core.Tests.Internal
{
#if !SILVERLIGHT && !NETCORE
	using System.Security.Permissions;

	using Castle.Core.Internal;

	using Xunit;

	public class PermissionUtilTestCase
	{
		[Fact]
		public void Correctly_determines_permissions()
		{
			// Execution has to be always granted. Otherwise this code wouldn't run in the first place.
			var securityPermission = new SecurityPermission(SecurityPermissionFlag.Execution);

			Assert.True(securityPermission.IsGranted());
		}
	}
#endif
}