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

namespace NVelocity.Runtime.Parser.Node
{
	using System;
	using System.Reflection;
	using Context;
	using NVelocity.App.Events;
	using NVelocity.Exception;
	using NVelocity.Util.Introspection;

	/// <summary>
	/// Method support for references :  $foo.method()
	///
	/// NOTE :
	///
	/// introspection is now done at render time.
	///
	/// Please look at the Parser.jjt file which is
	/// what controls the generation of this class.
	/// </summary>
	public class ASTMethod : SimpleNode
	{
		private string methodName;
		private int paramCount;
		private int paramArrayIndex = -1;

		public ASTMethod(int id) : base(id)
		{
		}

		public ASTMethod(Parser p, int id) : base(p, id)
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
		/// simple init - init our subtree and get what we can from
		/// the AST
		/// </summary>
		public override Object Init(IInternalContextAdapter context, Object data)
		{
			base.Init(context, data);

			methodName = FirstToken.Image;
			paramCount = ChildrenCount - 1;

			return data;
		}

		/// <summary>
		/// invokes the method.  Returns null if a problem, the
		/// actual return if the method returns something, or
		/// an empty string "" if the method returns void
		/// </summary>
		public override Object Execute(Object o, IInternalContextAdapter context)
		{
			IDuck duck = o as IDuck;

			object[] parameters = new object[paramCount];

			if (duck != null)
			{
				EvalParameters(parameters, context);

				return duck.Invoke(methodName, parameters);
			}

			/*
			*  new strategy (strategery!) for introspection. Since we want 
			*  to be thread- as well as context-safe, we *must* do it now,
			*  at execution time.  There can be no in-node caching,
			*  but if we are careful, we can do it in the context.
			*/

			MethodInfo method = null;
			PropertyInfo property = null;
			bool preparedAlready = false;
			object[] methodArguments = new object[paramCount];

			try
			{
				/*
				*   check the cache 
				*/

				IntrospectionCacheData icd = context.ICacheGet(this);
				Type c = o.GetType();

				/*
				*  like ASTIdentifier, if we have cache information, and the
				*  Class of Object o is the same as that in the cache, we are
				*  safe.
				*/

				EvalParameters(parameters, context);

				if (icd != null && icd.ContextData == c)
				{
					preparedAlready = true;

					/*
					* and get the method from the cache
					*/
					if (icd.Thingy is MethodInfo)
					{
						method = (MethodInfo) icd.Thingy;

						methodArguments = BuildMethodArgs(method, parameters, paramArrayIndex);
					}
					if (icd.Thingy is PropertyInfo)
					{
						property = (PropertyInfo) icd.Thingy;
					}
				}
				else
				{
					/*
					*  otherwise, do the introspection, and then
					*  cache it
					*/

					Object obj = PerformIntrospection(context, c, parameters);

					if (obj is MethodInfo)
					{
						method = (MethodInfo) obj;
					}
					if (obj is PropertyInfo)
					{
						property = (PropertyInfo) obj;
					}

					if (obj != null)
					{
						icd = new IntrospectionCacheData();
						icd.ContextData = c;
						icd.Thingy = obj;
						context.ICachePut(this, icd);
					}
				}

				/*
				*  if we still haven't gotten the method, either we are calling 
				*  a method that doesn't exist (which is fine...)  or I screwed
				*  it up.
				*/

				if (method == null && property == null)
				{
					return null;
				}
			}
			catch(Exception ex)
			{
				rsvc.Error("ASTMethod.execute() : exception from introspection : " + ex);

				throw new RuntimeException(String.Format("Error during object instrospection. " +
				                                         "Check inner exception for details. Node literal {0} Line {1} Column {2}",
				                                         base.Literal, Line, Column), ex);
			}

			try
			{
				/*
				*  get the returned object.  It may be null, and that is
				*  valid for something declared with a void return type.
				*  Since the caller is expecting something to be returned,
				*  as long as things are peachy, we can return an empty 
				*  String so ASTReference() correctly figures out that
				*  all is well.
				*/

				Object obj;

				if (method != null)
				{
					if (!preparedAlready)
					{
						methodArguments = BuildMethodArgs(method, parameters);
					}

					obj = method.Invoke(o, methodArguments);

					if (obj == null && method.ReturnType == typeof(void))
					{
						obj = String.Empty;
					}
				}
				else
				{
					obj = property.GetValue(o, null);
				}

				return obj;
			}
			catch(TargetInvocationException ite)
			{
				/*
				*  In the event that the invocation of the method
				*  itself throws an exception, we want to catch that
				*  wrap it, and throw.  We don't log here as we want to figure
				*  out which reference threw the exception, so do that 
				*  above
				*/

				EventCartridge ec = context.EventCartridge;

				/*
				*  if we have an event cartridge, see if it wants to veto
				*  also, let non-Exception Throwables go...
				*/

				if (ec != null)
				{
					try
					{
						return ec.HandleMethodException(o.GetType(), methodName, ite.GetBaseException());
					}
					catch(Exception e)
					{
						throw new MethodInvocationException(
							"Invocation of method '" + methodName + "' in  " + o.GetType() + " threw exception " + e.GetType() + " : " +
							e.Message, e, methodName);
					}
				}
				else
				{
					/*
					* no event cartridge to override. Just throw
					*/

					throw new MethodInvocationException(
						"Invocation of method '" + methodName + "' in  " + o.GetType() + " threw exception " +
						ite.GetBaseException().GetType() + " : " + ite.GetBaseException().Message, ite.GetBaseException(), methodName);
				}
			}
			catch(Exception e)
			{
				rsvc.Error("ASTMethod.execute() : exception invoking method '" + methodName + "' in " + o.GetType() + " : " + e);
				throw e;
			}
		}

		private void EvalParameters(object[] parameters, IInternalContextAdapter context)
		{
			for(int j = 0; j < paramCount; j++)
			{
				parameters[j] = GetChild(j + 1).Value(context);
			}
		}

		/// <summary>
		/// does the instrospection of the class for the method needed.
		///
		/// NOTE: this will try to flip the case of the first character for
		/// convience (compatability with Java version).  If there are no arguments,
		/// it will also try to find a property with the same name (also flipping first character).
		/// </summary>
		private Object PerformIntrospection(IInternalContextAdapter context, Type data, object[] parameters)
		{
			String methodNameUsed = methodName;

			MethodInfo m = rsvc.Introspector.GetMethod(data, methodNameUsed, parameters);

			PropertyInfo p = null;

			if (m == null)
			{
				// methodNameUsed = methodName.Substring(0, 1).ToUpper() + methodName.Substring(1);
				m = rsvc.Introspector.GetMethod(data, methodNameUsed, parameters);
				if (m == null)
				{
					// methodNameUsed = methodName.Substring(0, 1).ToLower() + methodName.Substring(1);
					m = rsvc.Introspector.GetMethod(data, methodNameUsed, parameters);

					// if there are no arguments, look for a property
					if (m == null && paramCount == 0)
					{
						methodNameUsed = methodName;
						p = rsvc.Introspector.GetProperty(data, methodNameUsed);
						if (p == null)
						{
							methodNameUsed = methodName.Substring(0, 1).ToUpper() + methodName.Substring(1);
							p = rsvc.Introspector.GetProperty(data, methodNameUsed);
							if (p == null)
							{
								methodNameUsed = methodName.Substring(0, 1).ToLower() + methodName.Substring(1);
								p = rsvc.Introspector.GetProperty(data, methodNameUsed);
							}
						}
					}
				}
			}

			// if a method was found, return it.  Otherwise, return whatever was found with a property, may be null
			if (m != null)
			{
				return m;
			}
			else
			{
				return p;
			}
		}

		private static object[] BuildMethodArgs(MethodInfo method, object[] parameters, int paramArrayIndex)
		{
			if (method == null) throw new ArgumentNullException("method");
			if (parameters == null) throw new ArgumentNullException("parameters");

			object[] methodArguments = parameters;

			if (paramArrayIndex != -1)
			{
				ParameterInfo[] methodArgs = method.GetParameters();

				Type arrayParamType = methodArgs[paramArrayIndex].ParameterType;

				object[] newParams = new object[methodArgs.Length];

				Array.Copy(parameters, newParams, methodArgs.Length - 1);

				if (parameters.Length < (paramArrayIndex + 1))
				{
					newParams[paramArrayIndex] = Array.CreateInstance(
						arrayParamType.GetElementType(), 0);
				}
				else
				{
					Array args = Array.CreateInstance(arrayParamType.GetElementType(), (parameters.Length + 1) - newParams.Length);

					Array.Copy(parameters, methodArgs.Length - 1, args, 0, args.Length);

					newParams[paramArrayIndex] = args;
				}

				methodArguments = newParams;
			}

			return methodArguments;
		}

		private object[] BuildMethodArgs(MethodInfo method, object[] parameters)
		{
			if (method == null) throw new ArgumentNullException("method");
			if (parameters == null) throw new ArgumentNullException("parameters");

			ParameterInfo[] methodArgs = method.GetParameters();

			int indexOfParamArray = -1;

			for(int i = 0; i < methodArgs.Length; ++i)
			{
				ParameterInfo paramInfo = methodArgs[i];

				if (paramInfo.IsDefined(typeof(ParamArrayAttribute), false))
				{
					indexOfParamArray = i;
					break;
				}
			}

			paramArrayIndex = indexOfParamArray;

			return BuildMethodArgs(method, parameters, indexOfParamArray);
		}
	}
}