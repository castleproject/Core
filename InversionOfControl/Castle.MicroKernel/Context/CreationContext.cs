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
	using System.Collections.Generic;
	using Castle.Core;
	using Releasers;

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
			get { return new CreationContext(new DependencyModel[0], new NoTrackingReleasePolicy()); }
		}

		private readonly IHandler handler;
		private readonly IReleasePolicy releasePolicy;
		private readonly IDictionary additionalArguments;
		private readonly Type[] genericArguments;

		/// <summary>
		/// Holds the scoped dependencies being resolved. 
		/// If a dependency appears twice on the same scope, we'd have a cycle.
		/// </summary>
		private readonly DependencyModelCollection dependencies;

		/// <summary>
		/// The list of handlers that are used to resolve
		/// the component.
		/// We track that in order to try to avoid attempts to resolve a service
		/// with itself.
		/// </summary>
		private readonly Stack<IHandler> handlerStack = new Stack<IHandler>();

		private readonly Stack<ResolutionContext> resolutionStack = new Stack<ResolutionContext>();

		/// <summary>
		/// Initializes a new instance of the <see cref="CreationContext"/> class.
		/// </summary>
		/// <param name="handler">The handler.</param>
		/// <param name="releasePolicy">The release policy.</param>
		/// <param name="additionalArguments">The additional arguments.</param>
		public CreationContext(IHandler handler, IReleasePolicy releasePolicy, IDictionary additionalArguments)
		{
			this.handler = handler;
			this.releasePolicy = releasePolicy;
			this.additionalArguments = additionalArguments;
			dependencies = new DependencyModelCollection();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CreationContext"/> class.
		/// </summary>
		/// <param name="dependencies">The dependencies.</param>
		/// <param name="releasePolicy">The release policy.</param>
		public CreationContext(DependencyModel[] dependencies, IReleasePolicy releasePolicy)
		{
			handler = null;
			this.releasePolicy = releasePolicy;
			this.dependencies = new DependencyModelCollection(dependencies);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CreationContext"/> class.
		/// </summary>
		/// <param name="handler">The handler.</param>
		/// <param name="releasePolicy">The release policy.</param>
		/// <param name="dependencies">The dependencies.</param>
		/// <param name="handlersChain">The handlers chain.</param>
		public CreationContext(IHandler handler, IReleasePolicy releasePolicy,
		                       DependencyModelCollection dependencies, IEnumerable<IHandler> handlersChain)
			: this(handler, releasePolicy, null)
		{
			this.dependencies = new DependencyModelCollection(dependencies);

			foreach(IHandler handlerItem in handlersChain)
			{
				this.handlerStack.Push(handlerItem);
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CreationContext"/> class.
		/// </summary>
		/// <param name="handler">The handler.</param>
		/// <param name="releasePolicy">The release policy.</param>
		/// <param name="typeToExtractGenericArguments">The type to extract generic arguments.</param>
		/// <param name="additionalArguments">The additional arguments.</param>
		public CreationContext(IHandler handler, IReleasePolicy releasePolicy,
		                       Type typeToExtractGenericArguments, IDictionary additionalArguments)
			: this(handler, releasePolicy, additionalArguments)
		{
			genericArguments = ExtractGenericArguments(typeToExtractGenericArguments);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CreationContext"/> class.
		/// </summary>
		/// <param name="handler">The handler.</param>
		/// <param name="releasePolicy">The release policy.</param>
		/// <param name="typeToExtractGenericArguments">The type to extract generic arguments.</param>
		/// <param name="parentContext">The parent context.</param>
		public CreationContext(IHandler handler, IReleasePolicy releasePolicy,
		                       Type typeToExtractGenericArguments, CreationContext parentContext)
			: this(handler, releasePolicy, parentContext.Dependencies, parentContext.handlerStack)
		{
			this.resolutionStack = parentContext.resolutionStack;
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

		public IReleasePolicy ReleasePolicy
		{
			get { return releasePolicy; }
		}

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
			return handlerStack.Contains(handlerToTest);
		}

		public ResolutionContext EnterResolutionContext(IHandler handlerBeingResolved)
		{
			return EnterResolutionContext(handlerBeingResolved, true);
		}

        public IDisposable ParentResolutionContext(CreationContext parent)
        {
            if (parent == null)
                return new RemoveDependencies(dependencies, null);
            dependencies.AddRange(parent.Dependencies);
            return new RemoveDependencies(dependencies, parent.Dependencies);
        }

	    internal class RemoveDependencies : IDisposable
	    {
	        private readonly DependencyModelCollection dependencies;
	        private readonly DependencyModelCollection parentDependencies;

	        public RemoveDependencies(DependencyModelCollection dependencies, 
                DependencyModelCollection parentDependencies)
	        {
	            this.dependencies = dependencies;
	            this.parentDependencies = parentDependencies;
	        }

	        public void Dispose()
	        {
                if(parentDependencies==null)
                    return;
	            foreach (DependencyModel model in parentDependencies)
	            {
	                dependencies.Remove(model);
	            }
	        }
	    }

	    public ResolutionContext EnterResolutionContext(IHandler handlerBeingResolved, bool createBurden)
		{
			ResolutionContext resCtx = new ResolutionContext(this, createBurden ? new Burden() : null);
			handlerStack.Push(handlerBeingResolved);
			if (createBurden)
			{
				resolutionStack.Push(resCtx);
			}
			return resCtx;
		}

		private void ExitResolutionContext(Burden burden)
		{
			handlerStack.Pop();

			if (burden == null)
			{
				return;
			}

			resolutionStack.Pop();

			if (resolutionStack.Count != 0)
			{
				resolutionStack.Peek().Burden.AddChild(burden);
			}
		}

		public class ResolutionContext : IDisposable
		{
			private readonly CreationContext context;
			private readonly Burden burden;

			public ResolutionContext(CreationContext context, Burden burden)
			{
				this.context = context;
				this.burden = burden;
			}

			public Burden Burden
			{
				get { return burden; }
			}

			public void Dispose()
			{
				context.ExitResolutionContext(burden);
			}
		}
	}
}