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

#if FEATURE_APPDOMAIN

namespace Castle.DynamicProxy.Tests
{
	using System;

#if FEATURE_SERIALIZATION
	[Serializable]
#endif
	public class CrossAppDomainCaller
	{
		public static void RunInOtherAppDomain(Action<object[]> callback, params object[] args)
		{
			CrossAppDomainCaller callbackObject = new CrossAppDomainCaller(callback, args);
			AppDomain newDomain = AppDomain.CreateDomain("otherDomain", AppDomain.CurrentDomain.Evidence,
			                                             AppDomain.CurrentDomain.SetupInformation);
			try
			{
				newDomain.DoCallBack(callbackObject.Run);
			}
			finally
			{
				AppDomain.Unload(newDomain);
			}
		}

		private readonly Action<object[]> callback;
		private readonly object[] args;

		public CrossAppDomainCaller(Action<object[]> callback, object[] args)
		{
			this.callback = callback;
			this.args = args;
		}

		private void Run()
		{
			callback(args);
		}
	}
}

#endif