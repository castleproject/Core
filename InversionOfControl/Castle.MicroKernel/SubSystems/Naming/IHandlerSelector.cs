using System;

namespace Castle.MicroKernel.SubSystems.Naming
{
    /// <summary>
    /// Implementors of this interface allow to extend the way the container perform
    /// component resolution based on some application specific business logic.
    /// </summary>
    /// <remarks>
    /// This is the siblig interface to <seealso cref="ISubDependencyResolver"/>.
    /// This is dealing strictly with root components, while the <seealso cref="ISubDependencyResolver"/> is dealing with
    /// dependent components.
    /// </remarks>
    public interface IHandlerSelector
    {
        /// <summary>
        /// Whatever the selector has an opinion about resolving a component with the 
        /// specified service and key.
        /// </summary>
        /// <param name="key">The service key - can be null</param>
        /// <param name="service">The service interface that we want to resolve</param>
        bool HasOpinionAbout(string key, Type service);

        /// <summary>
        /// Select the appropriate handler from the list of defined handlers.
        /// The returned handler should be a member from the <paramref name="handlers"/> array.
        /// </summary>
        /// <param name="key">The service key - can be null</param>
        /// <param name="service">The service interface that we want to resolve</param>
        /// <param name="handlers">The defined handlers</param>
        /// <returns>The selected handler, or null</returns>
        IHandler Select(string key, Type service, IHandler[] handlers);
    }
}