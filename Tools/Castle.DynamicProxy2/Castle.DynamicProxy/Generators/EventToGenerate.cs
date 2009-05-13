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

namespace Castle.DynamicProxy.Generators
{
	using System;
	using System.Reflection;
	using Castle.DynamicProxy.Generators.Emitters;

	public class EventToGenerate
	{
		private MethodInfo addMethod, removeMethod;
		private EventEmitter emitter;
		private EventAttributes attributes;

		/// <summary>
		/// Initializes a new instance of the <see cref="EventToGenerate"/> class.
		/// </summary>
		/// <param name="emitter">The emitter.</param>
		/// <param name="addMethod">The add method.</param>
		/// <param name="removeMethod">The remove method.</param>
		/// <param name="attributes">The attributes.</param>
		public EventToGenerate(EventEmitter emitter, MethodInfo addMethod, MethodInfo removeMethod, EventAttributes attributes)
		{
			this.addMethod = addMethod;
			this.removeMethod = removeMethod;
			this.emitter = emitter;
			this.attributes = attributes;
		}

		public EventEmitter Emitter
		{
			get { return emitter; }
		}

		public MethodInfo AddMethod
		{
			get { return addMethod; }
			set { addMethod = value; }
		}

		public MethodInfo RemoveMethod
		{
			get { return removeMethod; }
			set { removeMethod = value; }
		}

		public EventAttributes Attributes
		{
			get { return attributes; }
			set { attributes = value; }
		}
	}
}