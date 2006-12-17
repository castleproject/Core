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

using System;
using System.Globalization;

namespace Castle.Igloo.Util
{
    /// <summary>
    /// Assertion utility methods that simplify things such as argument checks.
    /// </summary>
    public static class AssertUtils
    {
        /// <summary>
        /// Checks the value of the supplied <paramref name="argument"/> and throws an
        /// <see cref="System.ArgumentNullException"/> if it is <see langword="null"/>.
        /// </summary>
        /// <param name="argument">The object to check.</param>
        /// <param name="name">The argument name.</param>
        /// <exception cref="System.ArgumentNullException">
        /// If the supplied <paramref name="argument"/> is <see langword="null"/>.
        /// </exception>
        public static void ArgumentNotNull<T>(T argument, string name) where T : class
        {
            if (argument == null)
            {
                throw new ArgumentNullException(
                    name,
                    string.Format(
                        CultureInfo.InvariantCulture,
                    "Argument '{0}' cannot be null.", name));
            }
        }

        /// <summary>
        /// Checks the value of the supplied <paramref name="argument"/> and throws an
        /// <see cref="System.ArgumentNullException"/> if it is <see langword="null"/>.
        /// </summary>
        /// <param name="argument">The object to check.</param>
        /// <param name="name">The argument name.</param>
        /// <param name="message">
        /// An arbitrary message that will be passed to any thrown
        /// <see cref="System.ArgumentNullException"/>.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// If the supplied <paramref name="argument"/> is <see langword="null"/>.
        /// </exception>
        public static void ArgumentNotNull<T>(T argument, string name, string message) where T : class
        {
            if (argument == null)
            {
                throw new ArgumentNullException(name, message);
            }
        }

        /// <summary>
        /// Checks the value of the supplied string <paramref name="argument"/> and throws an
        /// <see cref="System.ArgumentException"/> if it is <see langword="null"/> or
        /// contains only whitespace character(s).
        /// </summary>
        /// <param name="argument">The string to check.</param>
        /// <param name="name">The argument name.</param>
        /// <exception cref="System.ArgumentNullException">
        /// If the supplied <paramref name="argument"/> is <see langword="null"/> or
        /// contains only whitespace character(s).
        /// </exception>
        public static void AssertNotNullOrEmpty(string argument, string name)
        {
            if (string.IsNullOrEmpty(argument) || (argument.Trim().Length == 0))
            {
                throw new ArgumentNullException(
                    name,
                    string.Format(
                    CultureInfo.InvariantCulture,
                    "Argument '{0}' cannot be null or resolve to an empty string : '{1}'.", name, argument));
            }
        }

        /// <summary>
        /// Checks the value of the supplied string <paramref name="argument"/> and throws an
        /// <see cref="System.ArgumentNullException"/> if it is <see langword="null"/> or
        /// contains only whitespace character(s).
        /// </summary>
        /// <param name="argument">The string to check.</param>
        /// <param name="name">The argument name.</param>
        /// <param name="message">
        /// An arbitrary message that will be passed to any thrown
        /// <see cref="System.ArgumentException"/>.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// If the supplied <paramref name="argument"/> is <see langword="null"/> or
        /// contains only whitespace character(s).
        /// </exception>
        public static void AssertNotNullOrEmpty(string argument, string name, string message)
        {
            if (string.IsNullOrEmpty(argument) || (argument.Trim().Length == 0))
            {
                throw new ArgumentNullException(
                    name,
                    string.Format(
                    CultureInfo.InvariantCulture,
                    "Argument '{0}' cannot be null or resolve to an empty string : '{1}'.", name, argument));
            }
        }


        /// <summary>
        /// Checks whether the specified <paramref name="argument"/> can be cast 
        /// into T.
        /// </summary>
        /// <param name="argument">The argument to check.</param>
        /// <param name="argumentName">The name of the argument to check.</param>
        /// <param name="message">An arbitrary message that will be passed to any thrown</param>
        public static void AssertArgumentType<T>(object argument, string argumentName, string message)
        {
            if (argument != null && !typeof(T).IsAssignableFrom(argument.GetType()))
            {
                throw new ArgumentException(message, argumentName);
            }
        }
    }
}
