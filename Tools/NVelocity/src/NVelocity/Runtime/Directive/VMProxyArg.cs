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

namespace NVelocity.Runtime.Directive
{
	using System;
	using System.IO;
	using Context;
	using NVelocity.Exception;
	using NVelocity.Runtime.Parser.Node;
	using Parser;

	/// <summary>  
	/// The function of this class is to proxy for the calling parameter to the VM.
	/// *
	/// This class is designed to be used in conjunction with the VMContext class
	/// which knows how to get and set values via it, rather than a simple get()
	/// or put() from a hashtable-like object.
	/// *
	/// There is probably a lot of undocumented subtlety here, so step lightly.
	/// *
	/// We rely on the observation that an instance of this object has a constant
	/// state throughout its lifetime as it's bound to the use-instance of a VM.
	/// In other words, it's created by the VelocimacroProxy class, to represent
	/// one of the arguments to a VM in a specific template.  Since the template
	/// is fixed (it's a file...), we don't have to worry that the args to the VM
	/// will change.  Yes, the VM will be called in other templates, or in other
	/// places on the same template, bit those are different use-instances.
	/// *
	/// These arguments can be, in the lingo of
	/// the parser, one of :
	/// <ul>
	/// <li> Reference() : anything that starts with '$'</li>
	/// <li> StringLiteral() : something like "$foo" or "hello geir"</li>
	/// <li> NumberLiteral() : 1, 2 etc</li>
	/// <li> IntegerRange() : [ 1..2] or [$foo .. $bar]</li>
	/// <li> ObjectArray() : [ "a", "b", "c"]</li>
	/// <li> True() : true</li>
	/// <li> False() : false</li>
	/// <li>Word() : not likely - this is simply allowed by the parser so we can have
	/// syntactical sugar like #foreach($a in $b)  where 'in' is the Word</li>
	/// </ul>
	/// Now, Reference(), StringLit, NumberLit, IntRange, ObjArr are all dynamic things, so
	/// their value is gotten with the use of a context.  The others are constants.  The trick
	/// we rely on is that the context rather than this class really represents the
	/// state of the argument. We are simply proxying for the thing, returning the proper value
	/// when asked, and storing the proper value in the appropriate context when asked.
	/// *
	/// So, the hope here, so an instance of this can be shared across threads, is to
	/// keep any dynamic stuff out of it, relying on trick of having the appropriate
	/// context handed to us, and when a constant argument, letting VMContext punch that
	/// into a local context.
	///
	/// </summary>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
	/// </author>
	/// <version> $Id: VMProxyArg.cs,v 1.4 2003/10/27 13:54:10 corts Exp $
	///
	/// </version>
	public class VMProxyArg
	{
		/// <summary>type of arg I will have
		/// </summary>
		private int type = 0;

		/// <summary>the AST if the type is such that it's dynamic (ex. JJTREFERENCE )
		/// </summary>
		private SimpleNode nodeTree = null;

		/// <summary>reference for the object if we proxy for a static arg like an NumberLiteral
		/// </summary>
		private Object staticObject = null;

		/// <summary>not used in this impl : carries the appropriate user context
		/// </summary>
		private IInternalContextAdapter userContext = null;

		/// <summary>number of children in our tree if a reference
		/// </summary>
		private readonly int numTreeChildren = 0;

		/// <summary>our identity in the current context
		/// </summary>
		private readonly String contextReference = null;

		/// <summary>the reference we are proxying for
		/// </summary>
		private readonly String callerReference = null;

		/// <summary>the 'de-dollared' reference if we are a ref but don't have a method attached
		/// </summary>
		private readonly String singleLevelRef = null;

		/// <summary>by default, we are dynamic.  safest
		/// </summary>
		private bool constant = false;

		/// <summary>in the event our type is switched - we don't care really what it is
		/// </summary>
		private const int GENERALSTATIC = - 1;

		private readonly IRuntimeServices runtimeServices = null;

		/// <summary>  ctor for current impl
		/// *
		/// takes the reference literal we are proxying for, the literal
		/// the VM we are for is called with...
		/// *
		/// </summary>
		/// <param name="rs">
		/// </param>
		/// <param name="contextRef">reference arg in the definition of the VM, used in the VM
		/// </param>
		/// <param name="callerRef"> reference used by the caller as an arg to the VM
		/// </param>
		/// <param name="t"> type of arg : JJTREFERENCE, JJTTRUE, etc
		///
		/// </param>
		public VMProxyArg(IRuntimeServices rs, String contextRef, String callerRef, int t)
		{
			runtimeServices = rs;

			contextReference = contextRef;
			callerReference = callerRef;
			type = t;

			// make our AST if necessary
			setup();

			// if we are multi-node tree, then save the size to 
			// avoid fn call overhead 
			if (nodeTree != null)
			{
				numTreeChildren = nodeTree.ChildrenCount;
			}

			// if we are a reference, and 'scalar' (i.e. $foo )
			// then get the de-dollared ref so we can
			// hit our context directly, avoiding the AST
			if (type == ParserTreeConstants.REFERENCE)
			{
				if (numTreeChildren == 0)
				{
					// do this properly and use the Reference node
					singleLevelRef = ((ASTReference) nodeTree).RootString;
				}
			}
		}

		public String CallerReference
		{
			get { return callerReference; }
		}

		public String ContextReference
		{
			get { return contextReference; }
		}

		public SimpleNode NodeTree
		{
			get { return nodeTree; }
		}

		public Object StaticObject
		{
			get { return staticObject; }
		}

		public int Type
		{
			get { return type; }
		}

		/// <summary>  tells if arg we are proxying for is
		/// dynamic or constant.
		/// *
		/// </summary>
		/// <returns>true of constant, false otherwise
		///
		/// </returns>
		public bool isConstant()
		{
			return constant;
		}

		/// <summary>  Invoked by VMContext when Context.put() is called for a proxied reference.
		/// *
		/// </summary>
		/// <param name="context">context to modify via direct placement, or AST.setValue()
		/// </param>
		/// <param name="o"> new value of reference
		/// </param>
		/// <returns>Object currently null
		///
		/// </returns>
		public Object setObject(IInternalContextAdapter context, Object o)
		{
			/*
	    *  if we are a reference, we could be updating a property
	    */

			if (type == ParserTreeConstants.REFERENCE)
			{
				if (numTreeChildren > 0)
				{
					/*
		    *  we are a property, and being updated such as
		    *  #foo( $bar.BangStart) 
		    */

					try
					{
						((ASTReference) nodeTree).SetValue(context, o);
					}
					catch(MethodInvocationException methodInvocationException)
					{
						runtimeServices.Error(
							string.Format("VMProxyArg.getObject() : method invocation error setting value : {0}", methodInvocationException));
					}
				}
				else
				{
					/*
		    *  we are a 'single level' reference like $foo, so we can set
		    *  out context directly
		    */

					context.Put(singleLevelRef, o);

					// alternate impl : userContext.put( singleLevelRef, o);
				}
			}
			else
			{
				/*
		*  if we aren't a reference, then we simply switch type, 
		*  get a new value, and it doesn't go into the context
		*
		*  in current impl, this shouldn't happen.
		*/

				type = GENERALSTATIC;
				staticObject = o;

				runtimeServices.Error(
					string.Format("VMProxyArg.setObject() : Programmer error : I am a constant!  No setting! : {0} / {1}",
					              contextReference, callerReference));
			}

			return null;
		}

		/// <summary>  returns the value of the reference.  Generally, this is only
		/// called for dynamic proxies, as the static ones should have
		/// been stored in the VMContext's localContext store
		/// *
		/// </summary>
		/// <param name="context">Context to use for getting current value
		/// </param>
		/// <returns>Object value
		/// *
		///
		/// </returns>
		public Object getObject(IInternalContextAdapter context)
		{
			try
			{
				/*
		*  we need to output based on our type
		*/

				Object retObject = null;

				if (type == ParserTreeConstants.REFERENCE)
				{
					/*
		    *  two cases :  scalar reference ($foo) or multi-level ($foo.bar....)
		    */

					if (numTreeChildren == 0)
					{
						/*
			*  if I am a single-level reference, can I not get get it out of my context?
			*/

						retObject = context.Get(singleLevelRef);
					}
					else
					{
						/*
			*  I need to let the AST produce it for me.
			*/

						retObject = nodeTree.Execute(null, context);
					}
				}
				else if (type == ParserTreeConstants.OBJECT_ARRAY)
				{
					retObject = nodeTree.Value(context);
				}
				else if (type == ParserTreeConstants.INTEGER_RANGE)
				{
					retObject = nodeTree.Value(context);
				}
				else if (type == ParserTreeConstants.TRUE)
				{
					retObject = staticObject;
				}
				else if (type == ParserTreeConstants.FALSE)
				{
					retObject = staticObject;
				}
				else if (type == ParserTreeConstants.STRING_LITERAL)
				{
					retObject = nodeTree.Value(context);
				}
				else if (type == ParserTreeConstants.NUMBER_LITERAL)
				{
					retObject = staticObject;
				}
				else if (type == ParserTreeConstants.TEXT)
				{
					/*
		    *  this really shouldn't happen.  text is just a throwaway arg for #foreach()
		    */

					try
					{
						StringWriter writer = new StringWriter();
						nodeTree.Render(context, writer);

						retObject = writer;
					}
					catch(Exception e)
					{
						runtimeServices.Error(string.Format("VMProxyArg.getObject() : error rendering reference : {0}", e));
					}
				}
				else if (type == GENERALSTATIC)
				{
					retObject = staticObject;
				}
				else
				{
					runtimeServices.Error(
						string.Format("Unsupported VM arg type : VM arg = {0} type = {1}( VMProxyArg.getObject() )", callerReference, type));
				}

				return retObject;
			}
			catch(MethodInvocationException mie)
			{
				/*
		*  not ideal, but otherwise we propagate out to the 
		*  VMContext, and the Context interface's put/get 
		*  don't throw. So this is a the best compromise
		*  I can think of
		*/

				runtimeServices.Error(string.Format("VMProxyArg.getObject() : method invocation error getting value : {0}", mie));

				return null;
			}
		}

		/// <summary>  does the housekeeping upon creating.  If a dynamic type
		/// it needs to make an AST for further get()/set() operations
		/// Anything else is constant.
		/// </summary>
		private void setup()
		{
			switch(type)
			{
				case ParserTreeConstants.INTEGER_RANGE:
				case ParserTreeConstants.REFERENCE:
				case ParserTreeConstants.OBJECT_ARRAY:
				case ParserTreeConstants.STRING_LITERAL:
				case ParserTreeConstants.TEXT:
					{
						/*
			*  dynamic types, just render
			*/

						constant = false;

						try
						{
							/*
			    *  fakie : wrap in  directive to get the parser to treat our args as args
			    *   it doesn't matter that #include() can't take all these types, because we 
			    *   just want the parser to consider our arg as a Directive/VM arg rather than
			    *   as if inline in schmoo
			    */

							String buff = string.Format("#include({0} ) ", callerReference);

							//ByteArrayInputStream inStream = new ByteArrayInputStream( buff.getBytes() );

							//UPGRADE_ISSUE: The equivalent of constructor 'java.io.BufferedReader.BufferedReader' is incompatible with the expected type in C#. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1109"'
							TextReader br = new StringReader(buff);

							nodeTree = runtimeServices.Parse(br, string.Format("VMProxyArg:{0}", callerReference), true);

							/*
			    *  now, our tree really is the first DirectiveArg(), and only one
			    */

							nodeTree = (SimpleNode) nodeTree.GetChild(0).GetChild(0);

							/*
			    * sanity check
			    */

							if (nodeTree != null && nodeTree.Type != type)
							{
								runtimeServices.Error("VMProxyArg.setup() : programmer error : type doesn't match node type.");
							}

							/*
			    *  init.  We can do this as they are only references
			    */

							nodeTree.Init(null, runtimeServices);
						}
						catch(Exception e)
						{
							runtimeServices.Error(string.Format("VMProxyArg.setup() : exception {0} : {1}", callerReference, e));
						}

						break;
					}

				case ParserTreeConstants.TRUE:
					{
						constant = true;
						staticObject = true;
						break;
					}

				case ParserTreeConstants.FALSE:
					{
						constant = true;
						staticObject = false;
						break;
					}

				case ParserTreeConstants.NUMBER_LITERAL:
					{
						constant = true;
						staticObject = Int32.Parse(callerReference);
						break;
					}

				case ParserTreeConstants.WORD:
					{
						/*
						*  this is technically an error...
						*/

						runtimeServices.Error(
							string.Format(
								"Unsupported arg type : {0}  You most likely intended to call a VM with a string literal, so enclose with ' or \" characters. (VMProxyArg.setup())",
								callerReference));
						constant = true;
						staticObject = new String(callerReference.ToCharArray());

						break;
					}

				default:
					{
						runtimeServices.Error(string.Format(" VMProxyArg.setup() : unsupported type : {0}", callerReference));
					}
					break;
			}
		}

		/*
	* CODE FOR ALTERNATE IMPL : please ignore.  I will remove when comfortable with current.
	*/

		/// <summary>  not used in current impl
		/// *
		/// Constructor for alternate impl where VelProxy class would make new
		/// VMProxyArg objects, and use this constructor to avoid re-parsing the
		/// reference args
		/// *
		/// that impl also had the VMProxyArg carry it's context
		/// </summary>
		public VMProxyArg(VMProxyArg model, IInternalContextAdapter c)
		{
			userContext = c;
			contextReference = model.ContextReference;
			callerReference = model.CallerReference;
			nodeTree = model.NodeTree;
			staticObject = model.StaticObject;
			type = model.Type;

			if (nodeTree != null)
			{
				numTreeChildren = nodeTree.ChildrenCount;
			}

			if (type == ParserTreeConstants.REFERENCE)
			{
				if (numTreeChildren == 0)
				{
					/*
		    *  use the reference node to do this...
		    */
					singleLevelRef = ((ASTReference) nodeTree).RootString;
				}
			}
		}
	}
}
