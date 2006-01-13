namespace NVelocity.Runtime.Directive
{
	using System;
	using System.Collections;
	using System.IO;
	using NVelocity.Context;
	using NVelocity.Exception;
	using NVelocity.Runtime.Parser;
	using NVelocity.Runtime.Parser.Node;
	using NVelocity.Runtime.Visitor;
	using NVelocity.Util;

	/// <summary>
	/// VelocimacroProxy.java
	/// a proxy Directive-derived object to fit with the current directive system
	/// </summary>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a></author>
	/// <version> $Id: VelocimacroProxy.cs,v 1.4 2003/10/27 13:54:10 corts Exp $ </version>
	public class VelocimacroProxy : Directive
	{
		public VelocimacroProxy()
		{
			InitBlock();
		}

		private void InitBlock()
		{
			proxyArgHash = new Hashtable();
		}

		public override String Name
		{
			get { return macroName; }
			set { macroName = value; }
		}

		public override DirectiveType Type
		{
			get { return DirectiveType.LINE; }

		}

		public String[] ArgArray
		{
			set
			{
				argArray = value;

				/*
				*  get the arg count from the arg array.  remember that the arg array 
				*  has the macro name as it's 0th element
				*/

				numMacroArgs = argArray.Length - 1;
			}

		}

		public SimpleNode NodeTree
		{
			set { nodeTree = value; }

		}

		public int NumArgs
		{
			get { return numMacroArgs; }

		}

		public String Macrobody
		{
			set { macroBody = value; }

		}

		public String Namespace
		{
			set { this.namespace_Renamed = value; }

		}

		private String macroName = "";
		private String macroBody = "";
		private String[] argArray = null;
		private SimpleNode nodeTree = null;
		private int numMacroArgs = 0;
		private String namespace_Renamed = "";

		//UPGRADE_NOTE: Field init was renamed. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1029"'
		private bool init_Renamed_Field = false;
		private String[] callingArgs;
		private int[] callingArgTypes;
		//UPGRADE_NOTE: The initialization of  'proxyArgHash' was moved to method 'InitBlock'. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1005"'
		private Hashtable proxyArgHash;


		/// <summary> Return name of this Velocimacro.
		/// </summary>
		/// <summary> Velocimacros are always LINE
		/// type directives.
		/// </summary>
		/// <summary>   sets the directive name of this VM
		/// </summary>
		/// <summary>  sets the array of arguments specified in the macro definition
		/// </summary>
		/// <summary>  returns the number of ars needed for this VM
		/// </summary>
		/// <summary>   Sets the orignal macro body.  This is simply the cat of the macroArray, but the
		/// Macro object creates this once during parsing, and everyone shares it.
		/// Note : it must not be modified.
		/// </summary>
		/// <summary>   Renders the macro using the context
		/// </summary>
		public override bool Render(InternalContextAdapter context, TextWriter writer, INode node)
		{
			try
			{
				/*
		*  it's possible the tree hasn't been parsed yet, so get 
		*  the VMManager to parse and init it
		*/

				if (nodeTree != null)
				{
					if (!init_Renamed_Field)
					{
						nodeTree.Init(context, rsvc);
						init_Renamed_Field = true;
					}

					/*
		    *  wrap the current context and add the VMProxyArg objects
		    */

					VMContext vmc = new VMContext(context, rsvc);

					for (int i = 1; i < argArray.Length; i++)
					{
						/*
			*  we can do this as VMProxyArgs don't change state. They change
			*  the context.
			*/

						VMProxyArg arg = (VMProxyArg) proxyArgHash[argArray[i]];
						vmc.AddVMProxyArg(arg);
					}

					/*
		    *  now render the VM
		    */

					nodeTree.Render(vmc, writer);
				}
				else
				{
					rsvc.Error("VM error : " + macroName + ". Null AST");
				}
			}
			catch (Exception e)
			{
				/*
		*  if it's a MIE, it came from the render.... throw it...
		*/

				if (e is MethodInvocationException)
				{
					throw (MethodInvocationException) e;
				}

				rsvc.Error("VelocimacroProxy.render() : exception VM = #" + macroName + "() : " + StringUtils.stackTrace(e));
			}

			return true;
		}

		/// <summary>   The major meat of VelocimacroProxy, init() checks the # of arguments, patches the
		/// macro body, renders the macro into an AST, and then inits the AST, so it is ready
		/// for quick rendering.  Note that this is only AST dependant stuff. Not context.
		/// </summary>
		public override void Init(RuntimeServices rs, InternalContextAdapter context, INode node)
		{
			base.Init(rs, context, node);

			/*
	    *  how many args did we get?
	    */

			int i = node.jjtGetNumChildren();

			/*
	    *  right number of args?
	    */

			if (NumArgs != i)
			{
				rsvc.Error("VM #" + macroName + ": error : too " + ((NumArgs > i) ? "few" : "many") + " arguments to macro. Wanted " + NumArgs + " got " + i);

				return;
			}

			/*
	    *  get the argument list to the instance use of the VM
	    */
			callingArgs = getArgArray(node);

			/*
	    *  now proxy each arg in the context
	    */
			setupMacro(callingArgs, callingArgTypes);
			return;
		}

		/// <summary>
		/// basic VM setup.  Sets up the proxy args for this
		/// use, and parses the tree
		/// </summary>
		public bool setupMacro(String[] callArgs, int[] callArgTypes)
		{
			setupProxyArgs(callArgs, callArgTypes);
			parseTree(callArgs);

			return true;
		}

		/// <summary>
		/// parses the macro.  We need to do this here, at init time, or else
		/// the local-scope template feature is hard to get to work :)
		/// </summary>
		private void parseTree(String[] callArgs)
		{
			try
			{
				//UPGRADE_ISSUE: The equivalent of constructor 'java.io.BufferedReader.BufferedReader' is incompatible with the expected type in C#. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1109"'
				TextReader br = new StringReader(macroBody);

				/*
		*  now parse the macro - and don't dump the namespace
		*/

				nodeTree = rsvc.Parse(br, namespace_Renamed, false);

				/*
		*  now, to make null references render as proper schmoo
		*  we need to tweak the tree and change the literal of
		*  the appropriate references
		*
		*  we only do this at init time, so it's the overhead
		*  is irrelevant
		*/
				Hashtable hm = new Hashtable();

				for (int i = 1; i < argArray.Length; i++)
				{
					String arg = callArgs[i - 1];

					/*
		    *  if the calling arg is indeed a reference
		    *  then we add to the map.  We ignore other
		    *  stuff
		    */
					if (arg[0] == '$')
					{
						hm[argArray[i]] = arg;
					}
				}

				/*
		*  now make one of our reference-munging visitor, and 
		*  let 'er rip
		*/
				VMReferenceMungeVisitor v = new VMReferenceMungeVisitor(hm);
				nodeTree.jjtAccept(v, null);
			}
			catch (Exception e)
			{
				rsvc.Error("VelocimacroManager.parseTree() : exception " + macroName + " : " + StringUtils.stackTrace(e));
			}
		}

		private void setupProxyArgs(String[] callArgs, int[] callArgTypes)
		{
			/*
	    * for each of the args, make a ProxyArg
	    */

			for (int i = 1; i < argArray.Length; i++)
			{
				VMProxyArg arg = new VMProxyArg(rsvc, argArray[i], callArgs[i - 1], callArgTypes[i - 1]);
				proxyArgHash[argArray[i]] = arg;
			}
		}

		/// <summary>   gets the args to the VM from the instance-use AST
		/// </summary>
		private String[] getArgArray(INode node)
		{
			int numArgs = node.jjtGetNumChildren();

			String[] args = new String[numArgs];
			callingArgTypes = new int[numArgs];

			/*
	    *  eat the args
	    */
			int i = 0;
			Token t = null;
			Token tLast = null;

			while (i < numArgs)
			{
				args[i] = "";
				/*
		*  we want string literalss to lose the quotes.  #foo( "blargh" ) should have 'blargh' patched 
		*  into macro body.  So for each arg in the use-instance, treat the stringlierals specially...
		*/

				callingArgTypes[i] = node.jjtGetChild(i).Type;


				if (false && node.jjtGetChild(i).Type == ParserTreeConstants.JJTSTRINGLITERAL)
				{
					args[i] += node.jjtGetChild(i).FirstToken.Image.Substring(1, (node.jjtGetChild(i).FirstToken.Image.Length - 1) - (1));
				}
				else
				{
					/*
		    *  just wander down the token list, concatenating everything together
		    */
					t = node.jjtGetChild(i).FirstToken;
					tLast = node.jjtGetChild(i).LastToken;

					while (t != tLast)
					{
						args[i] += t.Image;
						t = t.Next;
					}

					/*
		    *  don't forget the last one... :)
		    */
					args[i] += t.Image;
				}
				i++;
			}
			return args;
		}


	}
}