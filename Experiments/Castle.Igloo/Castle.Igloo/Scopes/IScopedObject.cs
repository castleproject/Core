#region Apache Notice
/*****************************************************************************
 * 
 * Castle.Igloo
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 ********************************************************************************/
#endregion

using Castle.Igloo.ComponentActivator;

namespace Castle.Igloo.Scopes
{
    /// <summary>
    /// Objects created from the <see cref="ScopeComponentActivator"/> can be cast to this interface, 
    /// enabling access to the raw target object
    /// and programmatic removal of the target object.
    /// </summary>
    public interface IScopedObject
    {
        /// <summary>
        /// Return the current target object behind this scoped object proxy, in its raw form (as stored in the target scope).
        /// </summary>
        object TargetObject { get; }

        /// <summary>
        /// Remove this object from its target scope0
        /// </summary>
        void RemoveFromScope();

    }
}
