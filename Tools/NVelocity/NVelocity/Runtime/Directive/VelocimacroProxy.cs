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
	using System.Collections;
	using System.IO;
	using Context;
	using NVelocity.Exception;
	using NVelocity.Runtime.Parser.Node;
	using Parser;
	using Visitor;

	/// <summary>
	/// VelocimacroProxy
	/// a proxy Directive-derived object to fit with the current directive system
	/// </summary>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a></author>
	/// <version> $Id: VelocimacroProxy.cs,v 1.4 2003/10/27 13:54:10 corts Exp $ </version>
	public class VelocimacroProxy : Directive
	{
		private String macroName = string.Empty;
		private String macroBody = string.Empty;
		private String[] argArray = null;
		private SimpleNode nodeTree = null;
		private int numMacroArgs = 0;
		private String ns = string.Empty;

		private bool init = false;
		private String[] callingArgs;
		private int[] callingArgTypes;
		private Hashtable proxyArgHash;

		public VelocimacroProxy()
		{
			proxyArgHash = new Hashtable();
		}

		/// <summary>
		/// The major meat of VelocimacroProxy, init() checks the # of arguments, 
		/// patches the macro body, renders the macro into an AST, and then initiates 
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
				runtimeServices.Error(
					string.Format("VM #{0}: error : too {1} arguments to macro. Wanted {2} got {3}", macroName,
					              ((NumArgs > i) ? "few" : "many"), NumArgs, i));

				return;
			}

			// get the argument list to the instance use of the VM
			callingArgs = getArgArray(node);

			// now proxy each arg in the context
			setupMacro(callingArgs, callingArgTypes);
			return;
		}

		/// <summary>
		/// Renders the macro using the context
		/// </summary>
		public override bool Render(IInternalContextAdapter context, TextWriter writer, INode node)
		{
			try
			{
				// it's possible the tree hasn't been parsed yet, so get 
				// the VMManager to parse and init it
				if (nodeTree == null)
				{
					runtimeServices.Error(string.Format("VM error : {0}. Null AST", macroName));
				}
				else
				{
					if (!init)
					{
						nodeTree.Init(context, runtimeServices);
						init = true;
					}

					// wrap the current context and add the VMProxyArg objects
					VMContext vmContext = new VMContext(context, runtimeServices);

					for(int i = 1; i < argArray.Length; i++)
					{
						// we can do this as VMProxyArgs don't change state. They change
						// the context.
						VMProxyArg arg = (VMProxyArg) proxyArgHash[argArray[i]];
						vmContext.AddVMProxyArg(arg);
					}

					// now render the VM
					nodeTree.Render(vmContext, writer);
				}
			}
			catch(Exception e)
			{
				// if it's a MIE, it came from the render.... throw it...
				if (e is MethodInvocationException)
				{
					throw;
				}

				runtimeServices.Error(string.Format("VelocimacroProxy.render() : exception VM = #{0}() : {1}", macroName, e));
			}

			return true;
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
				nodeTree = runtimeServices.Parse(br, ns, false);

				// now, to make null references render as proper schmoo
				// we need to tweak the tree and change the literal of
				// the appropriate references

				// we only do this at init time, so it's the overhead
				// is irrelevant
				Hashtable hm = new Hashtable();

				for(int i = 1; i < argArray.Length; i++)
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
			catch(Exception e)
			{
				runtimeServices.Error(string.Format("VelocimacroManager.parseTree() : exception {0} : {1}", macroName, e));
			}
		}

		private void setupProxyArgs(String[] callArgs, int[] callArgTypes)
		{
			// for each of the args, make a ProxyArg
			for(int i = 1; i < argArray.Length; i++)
			{
				VMProxyArg arg = new VMProxyArg(runtimeServices, argArray[i], callArgs[i - 1], callArgTypes[i - 1]);
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

			while(i < numArgs)
			{
				args[i] = string.Empty;

				// we want string liberalises to lose the quotes.  #foo( "blargh" ) should have 'blargh' patched 
				// into macro body.  So for each arg in the use-instance, treat the stringlierals specially...
				callingArgTypes[i] = node.GetChild(i).Type;

//				if (false && node.GetChild(i).Type == ParserTreeConstants.STRING_LITERAL)
//				{
//					args[i] += node.GetChild(i).FirstToken.Image.Substring(1, (node.GetChild(i).FirstToken.Image.Length - 1) - (1));
//				}
//				else
				{
					// just wander down the token list, concatenating everything together
					t = node.GetChild(i).FirstToken;
					tLast = node.GetChild(i).LastToken;

					while(t != tLast)
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
		/// Returns the number of arguments needed for this VM
		/// </summary>
		public int NumArgs
		{
			get { return numMacroArgs; }
		}

		/// <summary>
		/// Sets the original macro body.  This is simply the cat of the 
		/// macroArray, but the Macro object creates this once during parsing, 
		/// and everyone shares it.
		/// 
		/// Note : it must not be modified.
		/// </summary>
		public String MacroBody
		{
			set { macroBody = value; }
		}

		public String Namespace
		{
			set { ns = value; }
		}
	}
}