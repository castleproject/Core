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

namespace Castle.DynamicProxy.Generators
{
	using System.Reflection;

	using Castle.DynamicProxy.Generators.Emitters;

	public class PropertyToGenerate
	{
		private readonly bool canRead, canWrite;
		private readonly PropertyEmitter emitter;
		private readonly MethodInfo getMethod, setMethod;

		public PropertyToGenerate(bool canRead, bool canWrite, PropertyEmitter emitter, MethodInfo getMethod, MethodInfo setMethod)
		{
			this.canRead = canRead;
			this.canWrite = canWrite;
			this.emitter = emitter;
			this.getMethod = getMethod;
			this.setMethod = setMethod;
		}

		public bool CanRead
		{
			get { return canRead; }
		}

		public bool CanWrite
		{
			get { return canWrite; }
		}

		public PropertyEmitter Emitter
		{
			get { return emitter; }
		}

		public MethodInfo GetMethod
		{
			get { return getMethod; }
		}

		public MethodInfo SetMethod
		{
			get { return setMethod; }
		}
	}
}
