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

namespace Castle.MicroKernel
{
	using System;
	using System.Collections;
	using System.Reflection;
	using System.Text;

	using Castle.MicroKernel.Exceptions;
	using Castle.Core;

	/// <summary>
	/// Used during a component request, passed along to the whole process.
	/// This allow some data to be passed along the process, which is used 
	/// to detected cycled dependency graphs and now it's also being used
	/// to provide arguments to components.
	/// </summary>
	[Serializable]
	public class CreationContext : MarshalByRefObject, ISubDependencyResolver
	{
		/// <summary>Creates a new, empty <see cref="CreationContext" /> instance.</summary>
		/// <remarks>A new CreationContext should be created every time, as the contexts keeps some state related to dependency resolution.</remarks>
		public static CreationContext Empty
		{
			get { return new CreationContext(new DependencyModel[0]); }
		}

		/// <summary>
		/// 
		/// </summary>
		private readonly IHandler handler;

		/// <summary>
		/// The list of handlers that are used to resolve
		/// the component.
		/// We track that in order to try to avoid attempts to resolve a service
		/// with itself.
		/// </summary>
        private readonly ArrayList handlersChain = new ArrayList();

		private readonly IDictionary additionalArguments;

		/// <summary>
		/// Holds the scoped dependencies being resolved. 
		/// If a dependency appears twice on the same scope, we'd have a cycle.
		/// </summary>
		private readonly DependencyModelCollection dependencies;

		private readonly Type[] genericArguments;

		public CreationContext(IHandler handler, IDictionary additionalArguments)
		{
			this.handler = handler;
			this.additionalArguments = additionalArguments;
			dependencies = new DependencyModelCollection();
		}

		public CreationContext(DependencyModel[] dependencies)
		{
			handler = null;
			this.dependencies = new DependencyModelCollection(dependencies);
		}

		public CreationContext(IHandler handler, DependencyModelCollection dependencies, IList handlersChain)
			: this(handler, (IDictionary)null)
		{
			this.dependencies = new DependencyModelCollection(dependencies);
            this.handlersChain.AddRange(handlersChain);
		}

		public CreationContext(IHandler handler, Type typeToExtractGenericArguments, IDictionary additionalArguments)
			: this(handler, additionalArguments)
		{
			genericArguments = ExtractGenericArguments(typeToExtractGenericArguments);
		}

		public CreationContext(IHandler handler, Type typeToExtractGenericArguments,
							   CreationContext parentContext)
			: this(handler, parentContext.Dependencies, parentContext.handlersChain)
		{
			additionalArguments = parentContext.additionalArguments;
			genericArguments = ExtractGenericArguments(typeToExtractGenericArguments);
		}

		#region ISubDependencyResolver

		public virtual object Resolve(CreationContext context, ISubDependencyResolver parentResolver,
							  ComponentModel model, DependencyModel dependency)
		{
			if (additionalArguments != null)
			{
				return additionalArguments[dependency.DependencyKey];
			}

			return null;
		}

		public virtual bool CanResolve(CreationContext context, ISubDependencyResolver parentResolver,
							   ComponentModel model, DependencyModel dependency)
		{
			if (dependency.DependencyKey == null) return false;

			if (additionalArguments != null)
			{
				return additionalArguments.Contains(dependency.DependencyKey);
			}

			return false;
		}

		#endregion

		public IDictionary AdditionalParameters
		{
			get { return additionalArguments; }
		}

		public bool HasAdditionalParameters
		{
			get { return additionalArguments != null && additionalArguments.Count != 0; }
		}

		/// <summary>
		/// Pendent
		/// </summary>
		public IHandler Handler
		{
			get { return handler; }
		}

		#region Cycle detection related members

		public DependencyModelCollection Dependencies
		{
			get { return dependencies; }
		}

		#endregion

		public Type[] GenericArguments
		{
			get { return genericArguments; }
		}

		private static Type[] ExtractGenericArguments(Type typeToExtractGenericArguments)
		{
			return typeToExtractGenericArguments.GetGenericArguments();
		}

		/// <summary>
		/// Check if we are now in the middle of resolving this handler, 
		/// and as such, we shouldn't try to resolve that.
		/// </summary>
		public bool HandlerIsCurrentlyBeingResolved(IHandler handlerToTest)
		{
			return handlersChain.Contains(handlerToTest);
		}

		public IDisposable ResolvingHandler(IHandler handlerBeingResolved)
		{
			handlersChain.Add(handlerBeingResolved);
			return new RemoveHandlerFromCurrentlyResolving(this, handlerBeingResolved);
		}

		private class RemoveHandlerFromCurrentlyResolving : IDisposable
		{
			private readonly CreationContext parent;
			private readonly IHandler handler;

			public RemoveHandlerFromCurrentlyResolving(CreationContext parent, IHandler handler)
			{
				this.parent = parent;
				this.handler = handler;
			}

			public void Dispose()
			{
				parent.handlersChain.Remove(handler);
			}
		}
	}
}
