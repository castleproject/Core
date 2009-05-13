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

namespace NVelocity.Runtime.Parser.Node
{
	using System;
	using System.Reflection;
	using Context;
	using NVelocity.App.Events;
	using NVelocity.Exception;
	using NVelocity.Util.Introspection;

	/// <summary>
	/// ASTIdentifier.java
	/// 
	/// Method support for identifiers :  $foo
	/// 
	/// mainly used by ASTReference
	/// 
	/// Introspection is now moved to 'just in time' or at render / execution
	/// time. There are many reasons why this has to be done, but the
	/// primary two are   thread safety, to remove any context-derived
	/// information from class member  variables.
	/// </summary>
	/// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a> </author>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a> </author>
	/// <version> $Id: ASTIdentifier.cs,v 1.5 2004/12/27 05:55:30 corts Exp $ </version>
	public class ASTIdentifier : SimpleNode
	{
		private String identifier = string.Empty;

		// This is really immutable after the init, so keep one for this node
		protected Info uberInfo;

		public ASTIdentifier(int id) : base(id)
		{
		}

		public ASTIdentifier(Parser p, int id) : base(p, id)
		{
		}

		/// <summary>
		/// Accept the visitor.
		/// </summary>
		public override Object Accept(IParserVisitor visitor, Object data)
		{
			return visitor.Visit(this, data);
		}

		/// <summary>
		/// simple init - don't do anything that is context specific.
		/// just get what we need from the AST, which is static.
		/// </summary>
		public override Object Init(IInternalContextAdapter context, Object data)
		{
			base.Init(context, data);

			identifier = FirstToken.Image;

			uberInfo = new Info(context.CurrentTemplateName, Line, Column);
			return data;
		}

		/// <summary>
		/// invokes the method on the object passed in
		/// </summary>
		public override Object Execute(Object o, IInternalContextAdapter context)
		{
			bool isString = o.GetType() == typeof(string);
			bool isDecimal = o.GetType() == typeof(decimal);
			bool isPrimitive = o.GetType().IsPrimitive;
			if (identifier == "to_quote" && (isString || isPrimitive || isDecimal))
			{
				return string.Format("\"{0}\"", EscapeDoubleQuote(o.ToString()));
			}
			else if (identifier == "to_squote" && (isString || isPrimitive || isDecimal))
			{
				return string.Format("'{0}'", EscapeSingleQuote(o.ToString()));
			}

			IDuck duck = o as IDuck;

			if (duck != null)
			{
				return duck.GetInvoke(identifier);
			}

			IVelPropertyGet velPropertyGet = null;

			try
			{
				Type c = o.GetType();

				// first, see if we have this information cached.
				IntrospectionCacheData introspectionCacheData = context.ICacheGet(this);

				// if we have the cache data and the class of the object we are 
				// invoked with is the same as that in the cache, then we must
				// be all-right.  The last 'variable' is the method name, and 
				// that is fixed in the template :)

				if (introspectionCacheData != null && introspectionCacheData.ContextData == c)
				{
					velPropertyGet = (IVelPropertyGet) introspectionCacheData.Thingy;
				}
				else
				{
					// otherwise, do the introspection, and cache it
					velPropertyGet = runtimeServices.Uberspect.GetPropertyGet(o, identifier, uberInfo);

					if (velPropertyGet != null && velPropertyGet.Cacheable)
					{
						introspectionCacheData = new IntrospectionCacheData(c, velPropertyGet);
						context.ICachePut(this, introspectionCacheData);
					}
				}
			}
			catch(Exception e)
			{
				runtimeServices.Error(string.Format("ASTIdentifier.execute() : identifier = {0} : {1}", identifier, e));
			}

			// we have no executor... punt...
			if (velPropertyGet == null)
				return null;

			// now try and execute.  If we get a TargetInvocationException, 
			// throw that as the app wants to get these. If not, log and punt.
			try
			{
				return velPropertyGet.Invoke(o);
			}
			catch(TargetInvocationException targetInvocationException)
			{
				EventCartridge ec = context.EventCartridge;

				// if we have an event cartridge, see if it wants to veto
				// also, let non-Exception Throwables go...
				if (ec == null)
				{
					// no event cartridge to override. Just throw
					String message = String.Format(
						"Invocation of method '{0}' in {1}, template {2} Line {3} Column {4} threw an exception",
						velPropertyGet.MethodName, o != null ? o.GetType().FullName : string.Empty,
						uberInfo.TemplateName, uberInfo.Line, uberInfo.Column);

					throw new MethodInvocationException(message, targetInvocationException.InnerException, velPropertyGet.MethodName);
				}
				else
				{
					try
					{
						return ec.HandleMethodException(o.GetType(), velPropertyGet.MethodName, targetInvocationException.InnerException);
					}
					catch(Exception)
					{
						String message = String.Format(
							"Invocation of method '{0}' in {1}, template {2} Line {3} Column {4} threw an exception",
							velPropertyGet.MethodName, o != null ? o.GetType().FullName : string.Empty,
							uberInfo.TemplateName, uberInfo.Line, uberInfo.Column);

						throw new MethodInvocationException(message, targetInvocationException.InnerException, velPropertyGet.MethodName);
					}
				}
			}
			catch(ArgumentException)
			{
				return null;
			}
			catch(Exception e)
			{
				runtimeServices.Error(
					string.Format("ASTIdentifier() : exception invoking method for identifier '{0}' in {1} : {2}", identifier,
					              o.GetType(), e));
			}

			return null;
		}

		private static string EscapeSingleQuote(string content)
		{
			return content.Replace("'", "\'");
		}

		private static string EscapeDoubleQuote(string content)
		{
			return content.Replace("\"", "\\\"");
		}
	}
}