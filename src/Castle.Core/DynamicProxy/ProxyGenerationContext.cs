// Copyright 2004-2021 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy
{
	using System;
	using System.Collections.Generic;

	using Castle.Core.Logging;

	internal sealed class ProxyGenerationContext
	{
		public ProxyGenerationContext(ProxyGenerationOptions options,
		                              ILogger logger = null)
		{
			Logger = logger ?? NullLogger.Instance;
		
			Options = options ?? ProxyGenerationOptions.Default;
			Options.Initialize();

			AttributesToAvoidReplicating = Generators.AttributesToAvoidReplicating.AsList();

			Hook = Options.Hook ?? new AllMethodsHook();
		}

		public IList<Type> AttributesToAvoidReplicating { get; }

		public IProxyGenerationHook Hook { get; }

		public ProxyGenerationOptions Options { get; }

		public ILogger Logger { get; }
	}
}
