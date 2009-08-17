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
	using Directive;

	/// <summary>  VelocimacroFactory.java
	/// *
	/// manages the set of VMs in a running Velocity engine.
	/// *
	/// </summary>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
	/// </author>
	/// <version> $Id: VelocimacroFactory.cs,v 1.5 2003/10/27 15:37:24 corts Exp $
	///
	/// </version>
	public class VelocimacroFactory
	{
		/// <summary>  runtime services for this instance
		/// </summary>
		private IRuntimeServices runtimeServices = null;

		/// <summary>  VMManager : deal with namespace management
		/// and actually keeps all the VM definitions
		/// </summary>
		private VelocimacroManager velocimacroManager = null;

		/// <summary>  determines if replacement of global VMs are allowed
		/// controlled by  VM_PERM_ALLOW_INLINE_REPLACE_GLOBAL
		/// </summary>
		private bool replaceAllowed = false;

		/// <summary>  controls if new VMs can be added.  Set by
		/// VM_PERM_ALLOW_INLINE  Note the assumption that only
		/// through inline defs can this happen.
		/// additions through auto-loaded VMs is allowed
		/// </summary>
		private bool addNewAllowed = true;

		/// <summary>  sets if template-local namespace in used
		/// </summary>
		private bool templateLocal = false;

		/// <summary>  controls log output
		/// </summary>
		private bool blather = false;

		/// <summary>  determines if the libraries are auto-loaded
		/// when they change
		/// </summary>
		private bool autoReloadLibrary = false;

		/// <summary>  vector of the library names
		/// </summary>
		private ArrayList macroLibVec = null;

		/// <summary>  map of the library Template objects
		/// used for reload determination
		/// </summary>
		private Hashtable libModMap;

		/// <summary>  CTOR : requires a runtime services from now
		/// on
		/// </summary>
		public VelocimacroFactory(IRuntimeServices rs)
		{
			runtimeServices = rs;

			/*
		*  we always access in a synchronized(), so we 
		*  can use an unsynchronized hashmap
		*/
			libModMap = new Hashtable();
			velocimacroManager = new VelocimacroManager(runtimeServices);
		}

		private bool TemplateLocalInline
		{
			get { return templateLocal; }
			set { templateLocal = value; }
		}

		private bool AddMacroPermission
		{
			set
			{
				bool b = addNewAllowed;

				addNewAllowed = value;
				// TODO: looks like original code must have returned the value that was replaced
				//return b;
			}
		}

		private bool ReplacementPermission
		{
			set
			{
				bool b = replaceAllowed;
				replaceAllowed = value;
				// TODO: looks like original code must have returned the value that was replaced
				//return b;
			}
		}

		private bool Blather
		{
			get { return blather; }

			set { blather = value; }
		}

		private bool Autoload
		{
			get { return autoReloadLibrary; }

			set { autoReloadLibrary = value; }
		}

		/// <summary>  initialize the factory - setup all permissions
		/// load all global libraries.
		/// </summary>
		public void InitVelocimacro()
		{
			/*
	    *  maybe I'm just paranoid...
	    */
			lock(this)
			{
				/*
		*   allow replacements while we add the libraries, if exist
		*/
				ReplacementPermission = true;
				Blather = true;

				LogVMMessageInfo("Velocimacro : initialization starting.");

				/*
		*  add all library macros to the global namespace
		*/

				velocimacroManager.NamespaceUsage = false;

				/*
		*  now, if there is a global or local libraries specified, use them.
		*  All we have to do is get the template. The template will be parsed;
		*  VM's  are added during the parse phase
		*/
				Object libraryFiles = runtimeServices.GetProperty(RuntimeConstants.VM_LIBRARY);

				if (libraryFiles != null)
				{
					if (libraryFiles is ArrayList)
					{
						macroLibVec = (ArrayList) libraryFiles;
					}
					else if (libraryFiles is String)
					{
						macroLibVec = new ArrayList();
						macroLibVec.Add(libraryFiles);
					}

					for(int i = 0; i < macroLibVec.Count; i++)
					{
						String lib = (String) macroLibVec[i];

						/*
			* only if it's a non-empty string do we bother
			*/
						if (lib != null && !lib.Equals(string.Empty))
						{
							/*
			    *  let the VMManager know that the following is coming
			    *  from libraries - need to know for auto-load
			    */
							velocimacroManager.RegisterFromLib = true;

							LogVMMessageInfo(string.Format("Velocimacro : adding VMs from VM library template : {0}", lib));

							try
							{
								Template template = runtimeServices.GetTemplate(lib);

								/*
				*  save the template.  This depends on the assumption
				*  that the Template object won't change - currently
				*  this is how the Resource manager works
				*/
								Twonk twonk = new Twonk(this);
								twonk.template = template;
								twonk.modificationTime = template.LastModified;
								libModMap[lib] = twonk;
							}
							catch(System.Exception e)
							{
								LogVMMessageInfo(string.Format("Velocimacro : error using  VM library template {0} : {1}", lib, e));
							}

							LogVMMessageInfo("Velocimacro :  VM library template macro registration complete.");

							velocimacroManager.RegisterFromLib = false;
						}
					}
				}

				/*
		*   now, the permissions
		*/

				/*
		*  allowinline : anything after this will be an inline macro, I think
		*  there is the question if a #include is an inline, and I think so
		*
		*  default = true
		*/
				AddMacroPermission = true;

				if (!runtimeServices.GetBoolean(RuntimeConstants.VM_PERM_ALLOW_INLINE, true))
				{
					AddMacroPermission = false;

					LogVMMessageInfo("Velocimacro : allowInline = false : VMs can not be defined inline in templates");
				}
				else
				{
					LogVMMessageInfo("Velocimacro : allowInline = true : VMs can be defined inline in templates");
				}

				/*
		*  allowInlineToReplaceGlobal : allows an inline VM , if allowed at all,
		*  to replace an existing global VM
		*
		*  default = false
		*/
				ReplacementPermission = false;

				if (runtimeServices.GetBoolean(RuntimeConstants.VM_PERM_ALLOW_INLINE_REPLACE_GLOBAL, false))
				{
					ReplacementPermission = true;

					LogVMMessageInfo(
						"Velocimacro : allowInlineToOverride = true : VMs defined inline may replace previous VM definitions");
				}
				else
				{
					LogVMMessageInfo(
						"Velocimacro : allowInlineToOverride = false : VMs defined inline may NOT replace previous VM definitions");
				}

				/*
		* now turn on namespace handling as far as permissions allow in the 
		* manager, and also set it here for gating purposes
		*/
				velocimacroManager.NamespaceUsage = true;

				/*
		*  template-local inline VM mode : default is off
		*/
				TemplateLocalInline = runtimeServices.GetBoolean(RuntimeConstants.VM_PERM_INLINE_LOCAL, false);

				if (TemplateLocalInline)
				{
					LogVMMessageInfo(
						"Velocimacro : allowInlineLocal = true : VMs defined inline will be local to their defining template only.");
				}
				else
				{
					LogVMMessageInfo("Velocimacro : allowInlineLocal = false : VMs defined inline will be  global in scope if allowed.");
				}

				velocimacroManager.TemplateLocalInlineVM = TemplateLocalInline;

				/*
		*  general message switch.  default is on
		*/
				Blather = runtimeServices.GetBoolean(RuntimeConstants.VM_MESSAGES_ON, true);

				if (Blather)
				{
					LogVMMessageInfo("Velocimacro : messages on  : VM system will output logging messages");
				}
				else
				{
					LogVMMessageInfo("Velocimacro : messages off : VM system will be quiet");
				}

				/*
		*  autoload VM libraries
		*/
				Autoload = runtimeServices.GetBoolean(RuntimeConstants.VM_LIBRARY_AUTORELOAD, false);

				if (Autoload)
				{
					LogVMMessageInfo("Velocimacro : autoload on  : VM system will automatically reload global library macros");
				}
				else
				{
					LogVMMessageInfo("Velocimacro : autoload off  : VM system will not automatically reload global library macros");
				}

				runtimeServices.Info("Velocimacro : initialization complete.");
			}

			return;
		}

		/// <summary>  adds a macro to the factory.
		/// </summary>
		public bool AddVelocimacro(String name, String macroBody, String[] argArray, String sourceTemplate)
		{
			/*
	    * maybe we should throw an exception, maybe just tell 
	    * the caller like this...
	    * 
	    * I hate this : maybe exceptions are in order here...
	    */
			if (name == null || macroBody == null || argArray == null || sourceTemplate == null)
			{
				LogVMMessageWarn("Velocimacro : VM addition rejected : programmer error : arg null");

				return false;
			}

			/*
	    *  see if the current ruleset allows this addition
	    */

			if (!CanAddVelocimacro(name, sourceTemplate))
			{
				return false;
			}

			/*
	    *  seems like all is good.  Lets do it.
	    */
			lock(this)
			{
				velocimacroManager.AddVM(name, macroBody, argArray, sourceTemplate);
			}

			/*
	    *  if we are to blather, blather...
	    */
			if (blather)
			{
				String s = string.Format("#{0}", argArray[0]);
				s += "(";

				for(int i = 1; i < argArray.Length; i++)
				{
					s += " ";
					s += argArray[i];
				}

				s += " ) : source = ";
				s += sourceTemplate;

				LogVMMessageInfo(string.Format("Velocimacro : added new VM : {0}", s));
			}

			return true;
		}

		/// <summary>  determines if a given macro/namespace (name, source) combo is allowed
		/// to be added
		/// *
		/// </summary>
		/// <param name="name">Name of VM to add
		/// </param>
		/// <param name="sourceTemplate">Source template that contains the definition of the VM
		/// </param>
		/// <returns>true if it is allowed to be added, false otherwise
		///
		/// </returns>
		private bool CanAddVelocimacro(String name, String sourceTemplate)
		{
			/*
	    *  short circuit and do it if autoloader is on, and the
	    *  template is one of the library templates
	    */

			if (Autoload)
			{
				/*
		*  see if this is a library template
		*/

				for(int i = 0; i < macroLibVec.Count; i++)
				{
					String lib = (String) macroLibVec[i];

					if (lib.Equals(sourceTemplate))
					{
						return true;
					}
				}
			}


			/*
	    * maybe the rules should be in manager?  I don't. It's to manage 
	    * the namespace issues first, are we allowed to add VMs at all? 
	    * This trumps all.
	    */
			if (!addNewAllowed)
			{
				LogVMMessageWarn(string.Format("Velocimacro : VM addition rejected : {0} : inline VMs not allowed.", name));

				return false;
			}

			/*
	    *  are they local in scope?  Then it is ok to add.
	    */
			if (!templateLocal)
			{
				/*
				* otherwise, if we have it already in global namespace, and they can't replace
				* since local templates are not allowed, the global namespace is implied.
				*  remember, we don't know anything about namespace management here, so lets
				*  note do anything fancy like trying to give it the global namespace here
				*
				*  so if we have it, and we aren't allowed to replace, bail
				*/
				if (IsVelocimacro(name, sourceTemplate) && !replaceAllowed)
				{
					LogVMMessageWarn(
						string.Format("Velocimacro : VM addition rejected : {0} : inline not allowed to replace existing VM", name));
					return false;
				}
			}

			return true;
		}

		/// <summary>  localization of the logging logic
		/// </summary>
		private void LogVMMessageInfo(String s)
		{
			if (blather)
				runtimeServices.Info(s);
		}

		/// <summary>  localization of the logging logic
		/// </summary>
		private void LogVMMessageWarn(String s)
		{
			if (blather)
				runtimeServices.Warn(s);
		}

		/// <summary>  Tells the world if a given directive string is a Velocimacro
		/// </summary>
		public bool IsVelocimacro(String vm, String sourceTemplate)
		{
			lock(this)
			{
				/*
		* first we check the locals to see if we have 
		* a local definition for this template
		*/
				if (velocimacroManager.get(vm, sourceTemplate) != null)
					return true;
			}
			return false;
		}

		/// <summary>  actual factory : creates a Directive that will
		/// behave correctly wrt getting the framework to
		/// dig out the correct # of args
		/// </summary>
		public Directive.Directive GetVelocimacro(String vmName, String sourceTemplate)
		{
			VelocimacroProxy velocimacroProxy = null;

			lock(this)
			{
				/*
		*  don't ask - do
		*/

				velocimacroProxy = velocimacroManager.get(vmName, sourceTemplate);

				/*
		*  if this exists, and autoload is on, we need to check
		*  where this VM came from
		*/

				if (velocimacroProxy != null && Autoload)
				{
					/*
		    *  see if this VM came from a library.  Need to pass sourceTemplate
		    *  in the event namespaces are set, as it could be masked by local
		    */

					String lib = velocimacroManager.GetLibraryName(vmName, sourceTemplate);

					if (lib != null)
					{
						try
						{
							/*
			    *  get the template from our map
			    */

							Twonk twonk = (Twonk) libModMap[lib];

							if (twonk != null)
							{
								Template template = twonk.template;

								/*
				*  now, compare the last modified time of the resource
				*  with the last modified time of the template
				*  if the file has changed, then reload. Otherwise, we should
				*  be ok.
				*/

								long tt = twonk.modificationTime;
								long ft = template.ResourceLoader.GetLastModified(template)
									;

								if (ft > tt)
								{
									LogVMMessageInfo(string.Format("Velocimacro : autoload reload for VMs from VM library template : {0}", lib));

									/*
				    *  when there are VMs in a library that invoke each other,
				    *  there are calls into getVelocimacro() from the init() 
				    *  process of the VM directive.  To stop the infinite loop
				    *  we save the current time reported by the resource loader
				    *  and then be honest when the reload is complete
				    */

									twonk.modificationTime = ft;

									template = runtimeServices.GetTemplate(lib)
										;

									/*
				    * and now we be honest
				    */

									twonk.template = template;
									twonk.modificationTime = template.LastModified;

									/*
				    *  note that we don't need to put this twonk back 
				    *  into the map, as we can just use the same reference
				    *  and this block is synchronized
				    */
								}
							}
						}
						catch(System.Exception e)
						{
							LogVMMessageInfo(string.Format("Velocimacro : error using  VM library template {0} : {1}", lib, e));
						}

						/*
			*  and get again
			*/
						velocimacroProxy = velocimacroManager.get(vmName, sourceTemplate);
					}
				}
			}

			return velocimacroProxy;
		}

		/// <summary>  tells the velocimacroManager to dump the specified namespace
		/// </summary>
		public bool DumpVMNamespace(String ns)
		{
			return velocimacroManager.DumpNamespace(ns);
		}

		/// <summary>  sets permission to have VMs local in scope to their declaring template
		/// note that this is really taken care of in the VMManager class, but
		/// we need it here for gating purposes in addVM
		/// eventually, I will slide this all into the manager, maybe.
		/// </summary>
		/// <summary>   sets the permission to add new macros
		/// </summary>
		/// <summary>    sets the permission for allowing addMacro() calls to
		/// replace existing VM's
		/// </summary>
		/// <summary>  set output message mode
		/// </summary>
		/// <summary> get output message mode
		/// </summary>
		/// <summary>  set the switch for automatic reloading of
		/// global library-based VMs
		/// </summary>
		/// <summary>  get the switch for automatic reloading of
		/// global library-based VMs
		/// </summary>
		/// <summary> small container class to hold the duple
		/// of a template and modification time.
		/// We keep the modification time so we can
		/// 'override' it on a reload to prevent
		/// recursive reload due to inter-calling
		/// VMs in a library
		/// </summary>
		//UPGRADE_NOTE: Field 'EnclosingInstance' was added to class 'Twonk' to access its enclosing instance. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1019"'
		private class Twonk
		{
			public Twonk(VelocimacroFactory enclosingInstance)
			{
				InitBlock(enclosingInstance);
			}

			private void InitBlock(VelocimacroFactory enclosingInstance)
			{
				this.enclosingInstance = enclosingInstance;
			}

			private VelocimacroFactory enclosingInstance;

			public VelocimacroFactory Enclosing_Instance
			{
				get { return enclosingInstance; }
			}

			public Template template;
			public long modificationTime;
		}
	}
}