// Copyright 2004-2026 Castle Project - http://www.castleproject.org/
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

#if FEATURE_BYREFLIKE

#nullable enable

namespace Castle.DynamicProxy.Tokens
{
	using System.Linq;
	using System.Reflection;

	internal static class ByRefLikeReferenceMethods
	{
		public static ConstructorInfo Constructor = typeof(ByRefLikeReference).GetConstructors().Single();

		public static MethodInfo GetPtr = typeof(ByRefLikeReference).GetMethod(nameof(ByRefLikeReference.GetPtr))!;

		public static MethodInfo Invalidate = typeof(ByRefLikeReference).GetMethod(nameof(ByRefLikeReference.Invalidate))!;
	}
}

#endif
