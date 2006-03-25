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
	/// VelocimacroProxy
	/// a proxy Directive-derived object to fit with the current directive system
	/// </summary>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a></author>
	/// <version> $Id: VelocimacroProxy.cs,v 1.4 2003/10/27 13:54:10 corts Exp $ </version>
	public class VelocimacroProxy : Directive
	{
		/// <summary>
		/// The name of this Velocimacro.
		/// </summary>
		public override String Name
		{
			get { return macroName; }
			set { macroName = value; }
		}

		/// <summary>
		/// Velocimacros are always LINE
		/// type directives.
		/// </summary>
		public override DirectiveType Type
		{
			get { return DirectiveType.LINE; }
		}

		/// <summary>
		/// Sets the array of arguments specified in the macro definition
		/// </summary>
		public String[] ArgArray
		{
			set
			{
				argArray = value;

				// get the arg count from the arg array.  remember that the arg array 
				// has the macro name as it's 0th element
				numMacroArgs = argArray.Length - 1;
			}
		}

		public SimpleNode NodeTree
		{
			set { nodeTree = value; }
		}

		/// <summary>
		/// Returns the number of ars needed for this VM
		/// </summary>
		public int NumArgs
		{
			get { return numMacroArgs; }
		}

		/// <summary>
		/// Sets the orignal macro body.  This is simply the cat of the 
		/// macroArray, but the Macro object creates this once during parsing, 
		/// and everyone shares it.
		/// 
		/// Note : it must not be modified.
		/// </summary>
		public String Macrobody
		{
			set { macroBody = value; }
		}

		public String Namespace
		{
			set { this.ns = value; }
		}

		private String macroName = "";
		private String macroBody = "";
		private String[] argArray = null;
		private SimpleNode nodeTree = null;
		private int numMacroArgs = 0;
		private String ns = "";

		private bool init = false;
		private String[] callingArgs;
		private int[] callingArgTypes;
		private Hashtable proxyArgHash = new Hashtable();

		/// <summary>
		/// Renders the macro using the context
		/// </summary>
		public override bool Render(IInternalContextAdapter context, TextWriter writer, INode node)
		{
			try
			{
				// it's possible the tree hasn't been parsed yet, so get 
				// the VMManager to parse and init it
				if (nodeTree != null)
				{
					if (!init)
					{
						nodeTree.Init(context, rsvc);
						init = true;
					}

					// wrap the current context and add the VMProxyArg objects
					VMContext vmc = new VMContext(context, rsvc);

					for (int i = 1; i < argArray.Length; i++)
					{
						// we can do this as VMProxyArgs don't change state. They change
						// the context.
						VMProxyArg arg = (VMProxyArg) proxyArgHash[argArray[i]];
						vmc.AddVMProxyArg(arg);
					}

					// now render the VM
					nodeTree.Render(vmc, writer);
				}
				else
				{
					rsvc.Error("VM error : " + macroName + ". Null AST");
				}
			}
			catch (Exception e)
			{
				// if it's a MIE, it came from the render.... throw it...
				if (e is MethodInvocationException)
					throw;

				rsvc.Error("VelocimacroProxy.render() : exception VM = #" + macroName + "() : " + StringUtils.StackTrace(e));
			}

			return true;
		}

		/// <summary>
		/// The major meat of VelocimacroProxy, init() checks the # of arguments, 
		/// patches the macro body, renders the macro into an AST, and then inits 
		/// the AST, so it is ready for quick rendering.  Note that this is only 
		/// AST dependant stuff. Not context.
		/// </summary>
		public override void Init(IRuntimeServices rs, IInternalContextAdapter context, INode node)
		{
			base.Init(rs, context, node);

			// how many args did we get?
			int i = node.ChildrenCount;

			// right number of args?
			if (NumArgs != i)
			{
				rsvc.Error("VM #" + macroName + ": error : too " + ((NumArgs > i) ? "few" : "many") + " arguments to macro. Wanted " + NumArgs + " got " + i);

				return;
			}

			// get the argument list to the instance use of the VM
			callingArgs = getArgArray(node);

			// now proxy each arg in the context
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
				TextReader br = new StringReader(macroBody);

				// now parse the macro - and don't dump the namespace
				nodeTree = rsvc.Parse(br, ns, false);

				// now, to make null references render as proper schmoo
				// we need to tweak the tree and change the literal of
				// the appropriate references

				// we only do this at init time, so it's the overhead
				// is irrelevant
				Hashtable hm = new Hashtable();

				for (int i = 1; i < argArray.Length; i++)
				{
					String arg = callArgs[i - 1];

					// if the calling arg is indeed a reference
					// then we add to the map.  We ignore other
					// stuff
					if (arg[0] == '$')
					{
						hm[argArray[i]] = arg;
					}
				}

				// now make one of our reference-munging visitor, and 
				// let 'er rip
				VMReferenceMungeVisitor v = new VMReferenceMungeVisitor(hm);
				nodeTree.Accept(v, null);
			}
			catch (Exception e)
			{
				rsvc.Error("VelocimacroManager.parseTree() : exception " + macroName + " : " + StringUtils.StackTrace(e));
			}
		}

		private void setupProxyArgs(String[] callArgs, int[] callArgTypes)
		{
			// for each of the args, make a ProxyArg
			for (int i = 1; i < argArray.Length; i++)
			{
				VMProxyArg arg = new VMProxyArg(rsvc, argArray[i], callArgs[i - 1], callArgTypes[i - 1]);
				proxyArgHash[argArray[i]] = arg;
			}
		}

		/// <summary>
		/// Gets the args to the VM from the instance-use AST
		/// </summary>
		private String[] getArgArray(INode node)
		{
			int numArgs = node.ChildrenCount;

			String[] args = new String[numArgs];
			callingArgTypes = new int[numArgs];

			// eat the args
			int i = 0;
			Token t;
			Token tLast;

			while (i < numArgs)
			{
				args[i] = "";
				
				// we want string literalss to lose the quotes.  #foo( "blargh" ) should have 'blargh' patched 
				// into macro body.  So for each arg in the use-instance, treat the stringlierals specially...
				callingArgTypes[i] = node.GetChild(i).Type;


				if (false && node.GetChild(i).Type == ParserTreeConstants.STRING_LITERAL)
				{
					args[i] += node.GetChild(i).FirstToken.Image.Substring(1, (node.GetChild(i).FirstToken.Image.Length - 1) - (1));
				}
				else
				{
					// just wander down the token list, concatenating everything together
					t = node.GetChild(i).FirstToken;
					tLast = node.GetChild(i).LastToken;

					while (t != tLast)
					{
						args[i] += t.Image;
						t = t.Next;
					}

					// don't forget the last one... :)
					args[i] += t.Image;
				}
				i++;
			}
			return args;
		}
	}
}
