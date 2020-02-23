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

#if FEATURE_TEST_COM

namespace Castle.DynamicProxy.Tests.ComInteropTypes.ADODB
{
	using System.Runtime.InteropServices;

	[ComImport]
	[Guid("0000050d-0000-0010-8000-00aa006d2ea4")]
	[TypeLibType(TypeLibTypeFlags.FDispatchable | TypeLibTypeFlags.FDual | TypeLibTypeFlags.FNonExtensible)]
	public interface Parameters // : (inherited interfaces omitted)
	{
		// (member definitions omitted)
	}
}

#endif
