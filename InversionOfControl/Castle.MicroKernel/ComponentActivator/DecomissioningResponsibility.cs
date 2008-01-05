// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MicroKernel.ComponentActivator
{
	using System;
	using System.Collections;

	public class DecomissioningResponsibility
	{
		private IList references = new ArrayList();

		public void AddResponsiblity(object instance, IHandler handler)
		{
			if (instance == null) throw new ArgumentNullException("instance");
			if (handler == null) throw new ArgumentNullException("handler");
			
			references.Add(new DecomissioningResponsibilityReference(instance, handler));
		}

		public bool HasResponsibilities
		{
			get { return references.Count != 0; }
		}

		public void ReleaseResponsibilities()
		{
			foreach(DecomissioningResponsibilityReference data in references)
			{
				data.Handler.Release(data.Instance);
			}

			references.Clear();
		}
	}

	internal struct DecomissioningResponsibilityReference
	{
		private object instance;
		private IHandler handler;

		public DecomissioningResponsibilityReference(object instance, IHandler handler)
		{
			this.instance = instance;
			this.handler = handler;
		}

		public object Instance
		{
			get { return instance; }
		}

		public IHandler Handler
		{
			get { return handler; }
		}
	}
}