// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework
{
	using System;

	/// <summary>
	/// Enum to identify a http verb 
	/// </summary>
	[Flags]
	public enum Verb
	{
		/// <summary>
		/// Not defined
		/// </summary>
		Undefined,
		/// <summary>
		/// The GET method means retrieve whatever information is identified by the Request-URI.
		/// <remarks>
		/// The convention has been established that the GET method SHOULD 
		/// NOT have the significance of taking an action other than retrieval. 
		/// </remarks>
		/// </summary>
		Get = 1,
		/// <summary>
		/// The POST method is used to request that the origin server accept the entity 
		/// enclosed in the request as a new subordinate of the resource identified by the 
		/// Request-URI in the Request-Line. 
		/// <remarks>
		/// The convention has been established that the POST method will
		/// take an action other than just retrieval. 
		/// </remarks>
		/// </summary>
		Post = 2,
		/// <summary>
		/// Just like a <see cref="Get"/> but without returning the body.
		/// </summary>
		Head = 4,
		/// <summary>
		/// Query for server options and capabilities.
		/// </summary>
		Options = 8,
		/// <summary>
		/// Put an entity into the server
		/// </summary>
		Put = 16,
		/// <summary>
		/// Delete an entity from the server
		/// </summary>
		Delete = 32,
		/// <summary>
		/// Diagnostic trace on server
		/// </summary>
		Trace = 64,
		/// <summary>
		/// Proxy Connect
		/// </summary>
		Connect = 128
	}

	/// <summary>
	/// Decorates an action with a restriction to the HTTP method 
	/// that is allowed to request it.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method), Serializable]
	public class AccessibleThroughAttribute : Attribute
	{
		private readonly Verb verb;

		/// <summary>
		/// Constructs a AccessibleThroughAttribute with 
		/// the specified <paramref name="verb"/>.
		/// </summary>
		/// <param name="verb">The <see cref="Verb"/> to allow for this action.</param>
		public AccessibleThroughAttribute(Verb verb)
		{
			this.verb = verb;
		}

		/// <summary>
		/// The Verb to allow.
		/// </summary>
		public Verb Verb
		{
			get { return verb; }
		}

		/// <summary>
		/// Checks if the <paramref name="httpMethod"/> is accessible.
		/// </summary>
		/// <param name="httpMethod">The <see cref="IRequest.HttpMethod"/> to check.</param>
		/// <returns><c>true</c> if <paramref name="httpMethod"/> is allowed, otherwise <c>false.</c></returns>
		public bool ForHttpMethod(string httpMethod)
		{
			return ForHttpMethod(Verb, HttpMethodToVerb(httpMethod));
		}

		/// <summary>
		/// Checks if the <paramref name="httpMethod"/> is accessible.
		/// </summary>
		/// <param name="httpMethod">The Http Method to check.</param>
		/// <returns><c>true</c> if <paramref name="httpMethod"/> is allowed, otherwise <c>false.</c></returns>
		public bool ForHttpMethod(Verb httpMethod)
		{
			return ForHttpMethod(Verb, httpMethod);
		}

		/// <summary>
		/// Checks it the <paramref name="testMethod"/> is accessible from the <paramref name="allowedMethods"/>.
		/// </summary>
		/// <param name="allowedMethods">THe verbs, http methods, that are allowed.</param>
		/// <param name="testMethod">The verb, http method, that is being testing for access.</param>
		/// <returns><c>true</c> if the <paramref name="testMethod"/> is an allowed method in the <paramref name="allowedMethods"/> otherwise <c>false</c></returns>
		public static bool ForHttpMethod(Verb allowedMethods, Verb testMethod)
		{
			return (allowedMethods & testMethod) == testMethod;
		}

		/// <summary>
		/// Converts a <see cref="IRequest.HttpMethod"/> to a <see cref="Verb"/>
		/// </summary>
		/// <param name="httpMethod">The http method to convert.</param>
		/// <returns>The http method as a <see cref="Verb"/></returns>
		public static Verb HttpMethodToVerb(string httpMethod)
		{
			return (Verb)Enum.Parse(typeof(Verb), httpMethod, true);
		}
	}
}
