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

using System;

namespace Castle.MicroKernel
{
    /// <summary>
    /// Implementors of this interface allow to extend the way the container perform
    /// component resolution based on some application specific business logic.
    /// </summary>
    /// <remarks>
    /// This is the sibling interface to <seealso cref="ISubDependencyResolver"/>.
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
        IHandler SelectHandler(string key, Type service, IHandler[] handlers);
    }
}