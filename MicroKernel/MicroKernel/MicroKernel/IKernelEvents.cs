// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

	using Castle.MicroKernel.Model;
	using Castle.MicroKernel.Interceptor;

    /// <summary>
    /// Represents a delegate which holds basic information about a component.
    /// </summary>
    /// <param name="model">Component information</param>
    /// <param name="key">Key which identifies the component</param>
    /// <param name="handler">handler that holds this component and is capable of 
    /// creating an instance of it.
    /// </param>
    public delegate void ComponentDataDelegate( IComponentModel model, String key, IHandler handler );

    /// <summary>
    /// Represents a delegate which holds basic information about a component
    /// and allows listeners to intercept (and proxy) the component instance.
    /// </summary>
    /// <param name="model">Component information</param>
    /// <param name="key">Key which identifies the component</param>
    /// <param name="handler">handler that holds this component and is capable of 
    /// creating an instance of it.
    /// </param>
    /// <param name="interceptedComponent">
    /// Holds the component instance and its proxied version. If no interception is added
    /// to the component, the proxied version is discarded and the kernel will work with
    /// the component instance instead.
    /// </param>
    public delegate void WrapDelegate( IComponentModel model, String key, IHandler handler, IInterceptedComponent interceptedComponent );

    /// <summary>
    /// Represents a delegate which holds basic information about a component
    /// and its instance.
    /// </summary>
    /// <param name="model">Component information</param>
    /// <param name="key">Key which identifies the component</param>
    /// <param name="handler">handler that holds this component and is capable of 
    /// creating an instance of it.
    /// </param>
    /// <param name="instance">Component instance</param>
    public delegate void ComponentInstanceDelegate( IComponentModel model, String key, IHandler handler, object instance );

    /// <summary>
    /// Delegate which holds a group of component information.
    /// </summary>
    /// <param name="model">Component information</param>
    /// <param name="key">Key which identifies the component</param>
	public delegate void ComponentModelDelegate( IComponentModel model, String key );

    /// <summary>
    /// Kernel events contract.
    /// </summary>
    public interface IKernelEvents
    {
        /// <summary>
        /// Event fired when a new component is registered 
        /// on the kernel.
        /// </summary>
        event ComponentDataDelegate ComponentRegistered;

		/// <summary>
		/// Event fired when a component is removed from the kernel.
		/// </summary>
		event ComponentDataDelegate ComponentUnregistered;

		/// <summary>
		/// Event fired when a component was instantiated
		/// and its lifecycle phases was already performed.
		/// Extensions can be applied by listening to this event
		/// and proxying the component instance
		/// </summary>
		event WrapDelegate ComponentWrap;

		/// <summary>
		/// When the component is given back to the kernel
		/// this event is fired. At the end, if a proxied instance was create
		/// previously it will be discarded and the real component instance 
		/// will be available to the kernel, allowing it to performe 
		/// the final lifecycle phases.
		/// </summary>
        event WrapDelegate ComponentUnWrap;

        /// <summary>
		/// Event fired before the component is returned to the outside
		/// world.
		/// </summary>
		event ComponentInstanceDelegate ComponentReady;

		/// <summary>
		/// Event fired when a component instance is given back to the 
		/// kernel.
		/// </summary>
		event ComponentInstanceDelegate ComponentReleased;

		/// <summary>
		/// Event fired when all information about a component was
		/// successfully collected.
		/// </summary>
		event ComponentModelDelegate ComponentModelConstructed;

        /// <summary>
        /// Fires the ComponentWrap event. It should be called 
        /// only by the <see cref="IHandler"/> implementations.
        /// </summary>
        /// <param name="instance">The component instance</param>
        /// <param name="handler">The handler which owns the instance</param>
        /// <returns>Returns the component instance or a proxy</returns>
        object RaiseWrapEvent( IHandler handler, object instance );

        /// <summary>
        /// Fires the ComponentUnWrap event. It should be called 
        /// only by the <see cref="IHandler"/> implementations.
        /// </summary>
        /// <param name="instance">The component instance (or a proxy)</param>
        /// <param name="handler">The handler which owns the instance</param>
        /// <returns>Should return the component instance, not the proxy</returns>
        object RaiseUnWrapEvent( IHandler handler, object instance );

        /// <summary>
        /// Fires the ComponentReady event. It should be called 
        /// only by the <see cref="IHandler"/> implementation.
        /// </summary>
        /// <param name="instance">The component instance</param>
        /// <param name="handler">The handler which owns the instance</param>
        void RaiseComponentReadyEvent(IHandler handler, object instance);

        /// <summary>
        /// Fires the ComponentReleased event. It should be called 
        /// only by the <see cref="IHandler"/> implementation.
        /// </summary>
        /// <param name="instance">The component instance</param>
        /// <param name="handler">The handler which owns the instance</param>
        void RaiseComponentReleasedEvent( IHandler handler, object instance );
    }
}
