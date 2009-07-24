// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.Facilities.WcfIntegration.Async.TypeSystem
{
	using System;
	using System.Runtime.CompilerServices;

	public static class HandleProvider
	{
		private static int _currentToken = typeof(object).GetConstructors()[0].MetadataToken;
		private static ModuleHandle _moduleHandle = typeof(object).Module.ModuleHandle;

		/// <summary>
		/// This is an ugly hack to circumvent lack of public constructor on <see cref="RuntimeMethodHandle"/> <c>struct</c>.
		/// </summary>
		/// <remarks>
		/// Since we need RuntimeMethodHandles and the <c>struct</c> does not have a constructor we can use, we need some other way of obtaining these.
		/// We're stealing them from mscorlib.dll. We can (hopefully) do this, because it's only for <see cref="AsyncType"/>'s Begin/End methods.
		/// Their handles are used (not that this is just internal implementation detail which can change and thus breaking my code) only
		/// as keys in the dictionary so we only need to have different ones, and we should be fine.
		/// </remarks>
		/// <returns>Method handle of some method from mscorlib.dll</returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public static RuntimeMethodHandle GetNextHandle()
		{
			return _moduleHandle.GetRuntimeMethodHandleFromMetadataToken(_currentToken++);
		}
	}
}