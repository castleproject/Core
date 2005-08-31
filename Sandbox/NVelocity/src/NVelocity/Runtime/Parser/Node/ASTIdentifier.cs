// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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
	using NVelocity.App.Events;
	using NVelocity.Context;
	using NVelocity.Exception;
	using NVelocity.Util.Introspection;

	/// <summary>  ASTIdentifier.java
	/// *
	/// Method support for identifiers :  $foo
	/// *
	/// mainly used by ASTRefrence
	/// *
	/// Introspection is now moved to 'just in time' or at render / execution
	/// time. There are many reasons why this has to be done, but the
	/// primary two are   thread safety, to remove any context-derived
	/// information from class member  variables.
	/// *
	/// </summary>
	/// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
	/// </author>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
	/// </author>
	/// <version> $Id: ASTIdentifier.cs,v 1.5 2004/12/27 05:55:30 corts Exp $
	///
	/// </version>
	public class ASTIdentifier : SimpleNode
	{
		private String identifier = "";

		/**
	 *  This is really immutable after the init, so keep one for this node
	 */
		protected Info uberInfo;

		public ASTIdentifier(int id) : base(id)
		{
		}

		public ASTIdentifier(Parser p, int id) : base(p, id)
		{
		}

		/// <summary>Accept the visitor. *
		/// </summary>
		public override Object jjtAccept(ParserVisitor visitor, Object data)
		{
			return visitor.visit(this, data);
		}

		/// <summary>  simple init - don't do anything that is context specific.
		/// just get what we need from the AST, which is static.
		/// </summary>
		public override Object init(InternalContextAdapter context, Object data)
		{
			base.init(context, data);

			identifier = FirstToken.image;

			uberInfo = new Info(context.CurrentTemplateName, Line, Column);
			return data;
		}


		/// <summary>
		/// invokes the method on the object passed in
		/// </summary>
		public override Object execute(Object o, InternalContextAdapter context)
		{
			VelPropertyGet vg = null;

			try
			{
				Type c = o.GetType();

				/*
		*  first, see if we have this information cached.
		*/

				IntrospectionCacheData icd = context.ICacheGet(this);

				/*
		* if we have the cache data and the class of the object we are 
		* invoked with is the same as that in the cache, then we must
		* be allright.  The last 'variable' is the method name, and 
		* that is fixed in the template :)
		*/

				if (icd != null && icd.contextData == c)
				{
					vg = (VelPropertyGet) icd.thingy;
				}
				else
				{
					/*
		    *  otherwise, do the introspection, and cache it
		    */
					vg = rsvc.Uberspect.getPropertyGet(o, identifier, uberInfo);

					if (vg != null && vg.Cacheable)
					{
						icd = new IntrospectionCacheData();
						icd.contextData = c;
						icd.thingy = vg;
						context.ICachePut(this, icd);
					}
				}
			}
			catch (Exception e)
			{
				rsvc.error("ASTIdentifier.execute() : identifier = " + identifier + " : " + e);
			}

			/*
	    *  we have no executor... punt...
	    */
			if (vg == null)
			{
				return null;
			}

			/*
	    *  now try and execute.  If we get a MIE, throw that
	    *  as the app wants to get these.  If not, log and punt.
	    */
			try
			{
				return vg.invoke(o);
			}
			catch (TargetInvocationException ite)
			{
				EventCartridge ec = context.EventCartridge;

				/*
				*  if we have an event cartridge, see if it wants to veto
				*  also, let non-Exception Throwables go...
				*/
				if (ec != null)
				{
					try
					{
						return ec.methodException(o.GetType(), vg.MethodName, ite.InnerException);
					}
					catch (Exception e)
					{
						String message = String.Format(
							"Invocation of method '{0}' in {1}, template {2} Line {3} Column {4} threw exception {5} {6}", 
							vg.MethodName, o != null ? o.GetType().FullName : "", 
							uberInfo.TemplateName, uberInfo.Line, uberInfo.Column,
							ite.InnerException.GetType(), ite.InnerException.Message);

						throw new MethodInvocationException(message, ite.InnerException, vg.MethodName);
					}
				}
				else
				{
					/*
		    * no event cartridge to override. Just throw
		    */
					String message = String.Format(
						"Invocation of method '{0}' in {1}, template {2} Line {3} Column {4} threw exception {5} {6}", 
						vg.MethodName, o != null ? o.GetType().FullName : "", 
						uberInfo.TemplateName, uberInfo.Line, uberInfo.Column,
						ite.InnerException.GetType(), ite.InnerException.Message);

					throw new MethodInvocationException(message, ite.InnerException, vg.MethodName);
				}
			}
			catch (ArgumentException iae)
			{
				return null;
			}
			catch (Exception e)
			{
				rsvc.error("ASTIdentifier() : exception invoking method "
					+ "for identifier '" + identifier + "' in "
					+ o.GetType() + " : " + e);
			}

			return null;
		}
	}
}