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

namespace NVelocity.Runtime
{
	using System;
	using System.Collections;
	using System.IO;
	using Context;
	using Directive;
	using NVelocity.Runtime.Parser.Node;

	/// <summary> 
	/// Manages VMs in namespaces.  Currently, two namespace modes are
	/// supported:
	/// *
	/// <ul>
	/// <li>flat - all allowable VMs are in the global namespace</li>
	/// <li>local - inline VMs are added to it's own template namespace</li>
	/// </ul>
	/// *
	/// Thanks to <a href="mailto:JFernandez@viquity.com">Jose Alberto Fernandez</a>
	/// for some ideas incorporated here.
	/// *
	/// </summary>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
	/// </author>
	/// <author> <a href="mailto:JFernandez@viquity.com">Jose Alberto Fernandez</a>
	/// </author>
	public class VelocimacroManager
	{
		private IRuntimeServices runtimeServices = null;
		private static String GLOBAL_NAMESPACE = string.Empty;

		private bool registerFromLib = false;

		/// <summary>Hash of namespace hashes.
		/// </summary>
		//UPGRADE_NOTE: The initialization of  'namespaceHash' was moved to method 'InitBlock'. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1005"'
		private Hashtable namespaceHash;

		/// <summary>map of names of library templates/namespaces</summary>
		//UPGRADE_NOTE: The initialization of  'libraryMap' was moved to method 'InitBlock'. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1005"'
		private Hashtable libraryMap;

		/*
		* big switch for namespaces.  If true, then properties control 
		* usage. If false, no. 
		*/
		private bool namespacesOn = true;
		private bool inlineLocalMode = false;

		/// <summary> Adds the global namespace to the hash.</summary>
		internal VelocimacroManager(IRuntimeServices rs)
		{
			InitBlock();
			runtimeServices = rs;

			/*
		*  add the global namespace to the namespace hash. We always have that.
		*/

			AddNamespace(GLOBAL_NAMESPACE);
		}


		private void InitBlock()
		{
			namespaceHash = new Hashtable();
			libraryMap = new Hashtable();
		}

		public bool NamespaceUsage
		{
			set { namespacesOn = value; }
		}

		public bool RegisterFromLib
		{
			set { registerFromLib = value; }
		}

		public bool TemplateLocalInlineVM
		{
			set { inlineLocalMode = value; }
		}

		/// <summary> Adds a VM definition to the cache.
		/// </summary>
		/// <returns>Whether everything went okay.
		///
		/// </returns>
		public bool AddVM(String vmName, String macroBody, String[] argArray, String ns)
		{
			MacroEntry me = new MacroEntry(this, this, vmName, macroBody, argArray, ns);

			me.FromLibrary = registerFromLib;

			/*
	    *  the client (VMFactory) will signal to us via
	    *  registerFromLib that we are in startup mode registering
	    *  new VMs from libraries.  Therefore, we want to
	    *  addto the library map for subsequent auto reloads
	    */

			bool isLib = true;

			if (registerFromLib)
			{
				SupportClass.PutElement(libraryMap, ns, ns);
			}
			else
			{
				/*
				 *  now, we first want to check to see if this namespace (template)
				 *  is actually a library - if so, we need to use the global namespace
				 *  we don't have to do this when registering, as namespaces should
				 *  be shut off. If not, the default value is true, so we still go
				 *  global
				 */

				isLib = libraryMap.ContainsKey(ns);
			}

			if (!isLib && UsingNamespaces(ns))
			{
				/*
				 *  first, do we have a namespace hash already for this namespace?
				 *  if not, add it to the namespaces, and add the VM
				 */

				Hashtable local = GetNamespace(ns, true);
				SupportClass.PutElement(local, vmName, me);

				return true;
			}
			else
			{
				/*
				 *  otherwise, add to global template.  First, check if we
				 *  already have it to preserve some of the autoload information
				 */

				MacroEntry exist = (MacroEntry) GetNamespace(GLOBAL_NAMESPACE)[vmName];

				if (exist != null)
				{
					me.FromLibrary = exist.FromLibrary;
				}

				/*
				 *  now add it
				 */

				SupportClass.PutElement(GetNamespace(GLOBAL_NAMESPACE), vmName, me);

				return true;
			}
		}

		/// <summary> gets a new living VelocimacroProxy object by the
		/// name / source template duple
		/// </summary>
		public VelocimacroProxy get(String vmName, String ns)
		{
			if (UsingNamespaces(ns))
			{
				Hashtable local = GetNamespace(ns, false);

				/*
				 *  if we have macros defined for this template
				 */

				if (local != null)
				{
					MacroEntry me = (MacroEntry) local[vmName];

					if (me != null)
					{
						return me.CreateVelocimacro(ns);
					}
				}
			}

			/*
	    * if we didn't return from there, we need to simply see 
	    * if it's in the global namespace
	    */

			//UPGRADE_NOTE: Variable me was renamed because block definition does not hide it. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1008"'
			MacroEntry me2 = (MacroEntry) GetNamespace(GLOBAL_NAMESPACE)[vmName];

			if (me2 != null)
			{
				return me2.CreateVelocimacro(ns);
			}

			return null;
		}

		/// <summary> Removes the VMs and the namespace from the manager.
		/// Used when a template is reloaded to avoid
		/// accumulating drek
		/// </summary>
		/// <param name="ns">namespace to dump
		/// </param>
		/// <returns>boolean representing success
		///
		/// </returns>
		public bool DumpNamespace(String ns)
		{
			lock(this)
			{
				if (UsingNamespaces(ns))
				{
					Object temp_key;
					Hashtable temp_hashtable;
					temp_key = ns;
					temp_hashtable = namespaceHash;
					Hashtable h = (Hashtable) temp_hashtable[temp_key];
					temp_hashtable.Remove(temp_key);

					if (h == null)
						return false;

					h.Clear();

					return true;
				}

				return false;
			}
		}

		/// <summary>
		/// public switch to let external user of manager to control namespace
		/// usage indep of properties.  That way, for example, at startup the
		/// library files are loaded into global namespace
		/// 
		/// returns the hash for the specified namespace.  Will not create a new one
		/// if it doesn't exist
		/// </summary>
		/// <param name="ns"> name of the namespace :) </param>
		/// <returns>namespace Hashtable of VMs or null if doesn't exist </returns>
		private Hashtable GetNamespace(String ns)
		{
			return GetNamespace(ns, false);
		}

		/// <summary>
		/// returns the hash for the specified namespace, and if it doesn't exist
		/// will create a new one and add it to the namespaces
		/// </summary>
		/// <param name="ns"> name of the namespace :)</param>
		/// <param name="addIfNew"> flag to add a new namespace if it doesn't exist</param>
		/// <returns>namespace Hashtable of VMs or null if doesn't exist</returns>
		private Hashtable GetNamespace(String ns, bool addIfNew)
		{
			Hashtable h = (Hashtable) namespaceHash[ns];

			if (h == null && addIfNew)
			{
				h = AddNamespace(ns);
			}

			return h;
		}

		/// <summary>adds a namespace to the namespaces</summary>
		/// <param name="ns">name of namespace to add</param>
		/// <returns>Hash added to namespaces, ready for use</returns>
		private Hashtable AddNamespace(String ns)
		{
			Hashtable h = new Hashtable();
			Object oh;

			if ((oh = SupportClass.PutElement(namespaceHash, ns, h)) != null)
			{
				/*
				 * There was already an entry on the table, restore it!
				 * This condition should never occur, given the code
				 * and the fact that this method is private.
				 * But just in case, this way of testing for it is much
				 * more efficient than testing before hand using get().
				 */
				SupportClass.PutElement(namespaceHash, ns, oh);
				/*
				 * Should't we be returning the old entry (oh)?
				 * The previous code was just returning null in this case.
				 */
				return null;
			}

			return h;
		}

		/// <summary>determines if currently using namespaces.</summary>
		/// <param name="ns">currently ignored</param>
		/// <returns>true if using namespaces, false if not</returns>
		private bool UsingNamespaces(String ns)
		{
			/*
	    *  if the big switch turns of namespaces, then ignore the rules
	    */

			if (!namespacesOn)
			{
				return false;
			}

			/*
	    *  currently, we only support the local template namespace idea
	    */

			if (inlineLocalMode)
			{
				return true;
			}

			return false;
		}

		public String GetLibraryName(String vmName, String ns)
		{
			if (UsingNamespaces(ns))
			{
				Hashtable local = GetNamespace(ns, false);

				/*
				 *  if we have this macro defined in this namespace, then
				 *  it is masking the global, library-based one, so 
				 *  just return null
				 */

				if (local != null)
				{
					MacroEntry me = (MacroEntry) local[vmName];

					if (me != null)
					{
						return null;
					}
				}
			}

			/*
	    * if we didn't return from there, we need to simply see 
	    * if it's in the global namespace
	    */

			//UPGRADE_NOTE: Variable me was renamed because block definition does not hide it. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1008"'
			MacroEntry me2 = (MacroEntry) GetNamespace(GLOBAL_NAMESPACE)[vmName];

			if (me2 != null)
			{
				return me2.SourceTemplate;
			}

			return null;
		}


		/// <summary>  wrapper class for holding VM information
		/// </summary>
		//UPGRADE_NOTE: Field 'EnclosingInstance' was added to class 'MacroEntry' to access its enclosing instance. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1019"'
		protected internal class MacroEntry
		{
			private void InitBlock(VelocimacroManager enclosingInstance)
			{
				this.enclosingInstance = enclosingInstance;
			}

			private VelocimacroManager enclosingInstance;

			public bool FromLibrary
			{
				get { return fromLibrary; }

				set { fromLibrary = value; }
			}

			public SimpleNode NodeTree
			{
				get { return nodeTree; }
			}

			public String SourceTemplate
			{
				get { return sourceTemplate; }
			}

			public VelocimacroManager Enclosing_Instance
			{
				get { return enclosingInstance; }
			}

			internal String macroName;
			internal String[] argumentArray;
			internal String macroBody;
			internal String sourceTemplate;
			internal SimpleNode nodeTree = null;
			internal VelocimacroManager manager = null;
			internal bool fromLibrary = false;

			internal MacroEntry(VelocimacroManager enclosingInstance, VelocimacroManager velocimacroManager, String vmName,
			                    String macroBody,
			                    String[] argArray, String sourceTemplate)
			{
				InitBlock(enclosingInstance);
				macroName = vmName;
				argumentArray = argArray;
				this.macroBody = macroBody;
				this.sourceTemplate = sourceTemplate;
				manager = velocimacroManager;
			}


			internal VelocimacroProxy CreateVelocimacro(String ns)
			{
				VelocimacroProxy velocimacroProxy = new VelocimacroProxy();
				velocimacroProxy.Name = macroName;
				velocimacroProxy.ArgArray = argumentArray;
				velocimacroProxy.MacroBody = macroBody;
				velocimacroProxy.NodeTree = nodeTree;
				velocimacroProxy.Namespace = ns;
				return velocimacroProxy;
			}

			internal void setup(IInternalContextAdapter internalContextAdapter)
			{
				/*
				 *  if not parsed yet, parse!
				 */

				if (nodeTree == null)
					parseTree(internalContextAdapter);
			}

			internal void parseTree(IInternalContextAdapter internalContextAdapter)
			{
				try
				{
					//UPGRADE_ISSUE: The equivalent of constructor 'java.io.BufferedReader.BufferedReader' is incompatible with the expected type in C#. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1109"'
					TextReader br = new StringReader(macroBody);

					nodeTree = Enclosing_Instance.runtimeServices.Parse(br, string.Format("VM:{0}", macroName), true);
					nodeTree.Init(internalContextAdapter, null);
				}
				catch(System.Exception e)
				{
					Enclosing_Instance.runtimeServices.Error(
						string.Format("VelocimacroManager.parseTree() : exception {0} : {1}", macroName, e));
				}
			}
		}
	}
}