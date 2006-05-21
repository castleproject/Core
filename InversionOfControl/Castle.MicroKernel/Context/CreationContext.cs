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

namespace Castle.MicroKernel
{
	using System;

	[Serializable]
	public sealed class CreationContext
	{
		public readonly static CreationContext Empty = new CreationContext();

		#if DOTNET2
		private readonly Type[] arguments;
		#endif

		public CreationContext()
		{
		}

		#if DOTNET2
		
		public CreationContext(Type target)
		{
			arguments = ExtractGenericArguments(target);
		}

		public Type[] GenericArguments
		{
			get { return arguments; }
		}

		private static Type[] ExtractGenericArguments(Type target)
		{
			return target.GetGenericArguments();
		}
		
		#endif
	}
}
