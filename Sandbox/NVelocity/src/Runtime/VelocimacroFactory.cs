namespace NVelocity.Runtime {
    /*
    * The Apache Software License, Version 1.1
    *
    * Copyright (c) 2000-2001 The Apache Software Foundation.  All rights
    * reserved.
    *
    * Redistribution and use in source and binary forms, with or without
    * modification, are permitted provided that the following conditions
    * are met:
    *
    * 1. Redistributions of source code must retain the above copyright
    *    notice, this list of conditions and the following disclaimer.
    *
    * 2. Redistributions in binary form must reproduce the above copyright
    *    notice, this list of conditions and the following disclaimer in
    *    the documentation and/or other materials provided with the
    *    distribution.
    *
    * 3. The end-user documentation included with the redistribution, if
    *    any, must include the following acknowlegement:
    *       "This product includes software developed by the
    *        Apache Software Foundation (http://www.apache.org/)."
    *    Alternately, this acknowlegement may appear in the software itself,
    *    if and wherever such third-party acknowlegements normally appear.
    *
    * 4. The names "The Jakarta Project", "Velocity", and "Apache Software
    *    Foundation" must not be used to endorse or promote products derived
    *    from this software without prior written permission. For written
    *    permission, please contact apache@apache.org.
    *
    * 5. Products derived from this software may not be called "Apache"
    *    nor may "Apache" appear in their names without prior written
    *    permission of the Apache Group.
    *
    * THIS SOFTWARE IS PROVIDED ``AS IS'' AND ANY EXPRESSED OR IMPLIED
    * WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
    * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
    * DISCLAIMED.  IN NO EVENT SHALL THE APACHE SOFTWARE FOUNDATION OR
    * ITS CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
    * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
    * LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF
    * USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
    * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
    * OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
    * OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
    * SUCH DAMAGE.
    * ====================================================================
    *
    * This software consists of voluntary contributions made by many
    * individuals on behalf of the Apache Software Foundation.  For more
    * information on the Apache Software Foundation, please see
    * <http://www.apache.org/>.
    */
    using System;
    using System.Collections;
    using Template = NVelocity.Template;
    using NVelocity.Runtime.Directive;
    using VelocimacroProxy = NVelocity.Runtime.Directive.VelocimacroProxy;
    using SimpleNode = NVelocity.Runtime.Parser.Node.SimpleNode;

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
    public class VelocimacroFactory {
	private bool TemplateLocalInline {
	    get {
		return templateLocal;
	    }

	    set {
		templateLocal = value;
	    }

	}
	private bool AddMacroPermission {
	    set {
		bool b = addNewAllowed;

		addNewAllowed = value;
		// TODO: looks like original code must have returned the value that was replaced
		//return b;
	    }

	}
	private bool ReplacementPermission {
	    set {
		bool b = replaceAllowed;
		replaceAllowed = value;
		// TODO: looks like original code must have returned the value that was replaced
		//return b;
	    }

	}
	private bool Blather {
	    get {
		return blather;
	    }

	    set {
		blather = value;
	    }

	}
	private bool Autoload {
	    get {
		return autoReloadLibrary;
	    }

	    set {
		autoReloadLibrary = value;
	    }

	}
	/// <summary>  runtime services for this instance
	/// </summary>
	private RuntimeServices rsvc = null;

	/// <summary>  VMManager : deal with namespace management
	/// and actually keeps all the VM definitions
	/// </summary>
	private VelocimacroManager vmManager = null;

	/// <summary>  determines if replacement of global VMs are allowed
	/// controlled by  VM_PERM_ALLOW_INLINE_REPLACE_GLOBAL
	/// </summary>
	private bool replaceAllowed = false;

	/// <summary>  controls if new VMs can be added.  Set by
	/// VM_PERM_ALLOW_INLINE  Note the assumption that only
	/// through inline defs can this happen.
	/// additions through autoloaded VMs is allowed
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
	private System.Collections.ArrayList macroLibVec = null;

	/// <summary>  map of the library Template objects
	/// used for reload determination
	/// </summary>
	private Hashtable libModMap;

	/// <summary>  CTOR : requires a runtime services from now
	/// on
	/// </summary>
	public VelocimacroFactory(RuntimeServices rs) {
	    this.rsvc = rs;

	    /*
	    *  we always access in a synchronized(), so we 
	    *  can use an unsynchronized hashmap
	    */
	    libModMap = new Hashtable();
	    vmManager = new VelocimacroManager(rsvc);
	}

	/// <summary>  initialize the factory - setup all permissions
	/// load all global libraries.
	/// </summary>
	public virtual void  initVelocimacro() {
	    /*
	    *  maybe I'm just paranoid...
	    */
	    lock(this) {
		/*
		*   allow replacements while we add the libraries, if exist
		*/
		ReplacementPermission = true;
		Blather = true;

		logVMMessageInfo("Velocimacro : initialization starting.");

		/*
		*  add all library macros to the global namespace
		*/

		vmManager.NamespaceUsage = false;

		/*
		*  now, if there is a global or local libraries specified, use them.
		*  All we have to do is get the template. The template will be parsed;
		*  VM's  are added during the parse phase
		*/
		System.Object libfiles = rsvc.getProperty(NVelocity.Runtime.RuntimeConstants_Fields.VM_LIBRARY);

		if (libfiles != null) {
		    if (libfiles is System.Collections.ArrayList) {
			macroLibVec = (System.Collections.ArrayList) libfiles;
		    } else if (libfiles is System.String) {
			macroLibVec = new System.Collections.ArrayList();
			macroLibVec.Add(libfiles);
		    }

		    for (int i = 0; i < macroLibVec.Count; i++) {
			System.String lib = (System.String) macroLibVec[i];

			/*
			* only if it's a non-empty string do we bother
			*/
			if (lib != null && !lib.Equals("")) {
			    /*
			    *  let the VMManager know that the following is coming
			    *  from libraries - need to know for auto-load
			    */
			    vmManager.RegisterFromLib = true;

			    logVMMessageInfo("Velocimacro : adding VMs from " + "VM library template : " + lib);

			    try {
				Template template = rsvc.getTemplate(lib);

				/*
				*  save the template.  This depends on the assumption
				*  that the Template object won't change - currently
				*  this is how the Resource manager works
				*/
				Twonk twonk = new Twonk(this);
				twonk.template = template;
				twonk.modificationTime = template.LastModified;
				libModMap[lib] = twonk;
			    } catch (System.Exception e) {
				logVMMessageInfo("Velocimacro : error using  VM " + "library template " + lib + " : " + e);
			    }

			    logVMMessageInfo("Velocimacro :  VM library template " + "macro registration complete.");

			    vmManager.RegisterFromLib = false;
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

		if (!rsvc.getBoolean(NVelocity.Runtime.RuntimeConstants_Fields.VM_PERM_ALLOW_INLINE, true)) {
		    AddMacroPermission = false;

		    logVMMessageInfo("Velocimacro : allowInline = false : VMs can not " + "be defined inline in templates");
		} else {
		    logVMMessageInfo("Velocimacro : allowInline = true : VMs can be " + "defined inline in templates");
		}

		/*
		*  allowInlineToReplaceGlobal : allows an inline VM , if allowed at all,
		*  to replace an existing global VM
		*
		*  default = false
		*/
		ReplacementPermission = false;

		if (rsvc.getBoolean(NVelocity.Runtime.RuntimeConstants_Fields.VM_PERM_ALLOW_INLINE_REPLACE_GLOBAL, false)) {
		    ReplacementPermission = true;

		    logVMMessageInfo("Velocimacro : allowInlineToOverride = true : VMs " + "defined inline may replace previous VM definitions");
		} else {
		    logVMMessageInfo("Velocimacro : allowInlineToOverride = false : VMs " + "defined inline may NOT replace previous VM definitions");
		}

		/*
		* now turn on namespace handling as far as permissions allow in the 
		* manager, and also set it here for gating purposes
		*/
		vmManager.NamespaceUsage = true;

		/*
		*  template-local inline VM mode : default is off
		*/
		TemplateLocalInline = rsvc.getBoolean(NVelocity.Runtime.RuntimeConstants_Fields.VM_PERM_INLINE_LOCAL, false);

		if (TemplateLocalInline) {
		    logVMMessageInfo("Velocimacro : allowInlineLocal = true : VMs " + "defined inline will be local to their defining template only.");
		} else {
		    logVMMessageInfo("Velocimacro : allowInlineLocal = false : VMs " + "defined inline will be  global in scope if allowed.");
		}

		vmManager.TemplateLocalInlineVM = TemplateLocalInline;

		/*
		*  general message switch.  default is on
		*/
		Blather = rsvc.getBoolean(NVelocity.Runtime.RuntimeConstants_Fields.VM_MESSAGES_ON, true);

		if (Blather) {
		    logVMMessageInfo("Velocimacro : messages on  : VM system " + "will output logging messages");
		} else {
		    logVMMessageInfo("Velocimacro : messages off : VM system will be quiet");
		}

		/*
		*  autoload VM libraries
		*/
		Autoload = rsvc.getBoolean(NVelocity.Runtime.RuntimeConstants_Fields.VM_LIBRARY_AUTORELOAD, false);

		if (Autoload) {
		    logVMMessageInfo("Velocimacro : autoload on  : VM system " + "will automatically reload global library macros");
		} else {
		    logVMMessageInfo("Velocimacro : autoload off  : VM system " + "will not automatically reload global library macros");
		}

		rsvc.info("Velocimacro : initialization complete.");
	    }

	    return ;
	}

	/// <summary>  adds a macro to the factory.
	/// </summary>
	public virtual bool addVelocimacro(System.String name, System.String macroBody, System.String[] argArray, System.String sourceTemplate) {
	    /*
	    * maybe we should throw an exception, maybe just tell 
	    * the caller like this...
	    * 
	    * I hate this : maybe exceptions are in order here...
	    */
	    if (name == null || macroBody == null || argArray == null || sourceTemplate == null) {
		logVMMessageWarn("Velocimacro : VM addition rejected : " + "programmer error : arg null");

		return false;
	    }

	    /*
	    *  see if the current ruleset allows this addition
	    */

	    if (!canAddVelocimacro(name, sourceTemplate)) {
		return false;
	    }

	    /*
	    *  seems like all is good.  Lets do it.
	    */
	    lock(this) {
		vmManager.addVM(name, macroBody, argArray, sourceTemplate);
	    }

	    /*
	    *  if we are to blather, blather...
	    */
	    if (blather) {
		System.String s = "#" + argArray[0];
		s += "(";

		for (int i = 1; i < argArray.Length; i++) {
		    s += " ";
		    s += argArray[i];
		}

		s += " ) : source = ";
		s += sourceTemplate;

		logVMMessageInfo("Velocimacro : added new VM : " + s);
	    }

	    return true;
	}

	/// <summary>  determines if a given macro/namespace (name, source) combo is allowed
	/// to be added
	/// *
	/// </summary>
	/// <param name="name">Name of VM to add
	/// </param>
	/// <param name="sourceTemplate">Source template that contains the defintion of the VM
	/// </param>
	/// <returns>true if it is allowed to be added, false otherwise
	///
	/// </returns>
	private bool canAddVelocimacro(System.String name, System.String sourceTemplate) {
	    /*
	    *  short circuit and do it if autoloader is on, and the
	    *  template is one of the library templates
	    */

	    if (Autoload) {
		/*
		*  see if this is a library template
		*/

		for (int i = 0; i < macroLibVec.Count; i++) {
		    System.String lib = (System.String) macroLibVec[i];

		    if (lib.Equals(sourceTemplate)) {
			return true;
		    }
		}
	    }


	    /*
	    * maybe the rules should be in manager?  I dunno. It's to manage 
	    * the namespace issues first, are we allowed to add VMs at all? 
	    * This trumps all.
	    */
	    if (!addNewAllowed) {
		logVMMessageWarn("Velocimacro : VM addition rejected : " + name + " : inline VMs not allowed.");

		return false;
	    }

	    /*
	    *  are they local in scope?  Then it is ok to add.
	    */
	    if (!templateLocal) {
		/*
				* otherwise, if we have it already in global namespace, and they can't replace
				* since local templates are not allowed, the global namespace is implied.
				*  remember, we don't know anything about namespace managment here, so lets
				*  note do anything fancy like trying to give it the global namespace here
				*
				*  so if we have it, and we aren't allowed to replace, bail
				*/
		if (isVelocimacro(name, sourceTemplate) && !replaceAllowed) {
		    logVMMessageWarn("Velocimacro : VM addition rejected : " + name + " : inline not allowed to replace existing VM");
		    return false;
		}
	    }

	    return true;
	}

	/// <summary>  localization of the logging logic
	/// </summary>
	private void  logVMMessageInfo(System.String s) {
	    if (blather)
		rsvc.info(s);
	}

	/// <summary>  localization of the logging logic
	/// </summary>
	private void  logVMMessageWarn(System.String s) {
	    if (blather)
		rsvc.warn(s);
	}

	/// <summary>  Tells the world if a given directive string is a Velocimacro
	/// </summary>
	public virtual bool isVelocimacro(System.String vm, System.String sourceTemplate) {
	    lock(this) {
		/*
		* first we check the locals to see if we have 
		* a local definition for this template
		*/
		if (vmManager.get(vm, sourceTemplate) != null)
		    return true;
	    }
	    return false;
	}

	/// <summary>  actual factory : creates a Directive that will
	/// behave correctly wrt getting the framework to
	/// dig out the correct # of args
	/// </summary>
	public virtual Directive.Directive getVelocimacro(System.String vmName, System.String sourceTemplate) {
	    VelocimacroProxy vp = null;

	    lock(this) {
		/*
		*  don't ask - do
		*/

		vp = vmManager.get(vmName, sourceTemplate);

		/*
		*  if this exists, and autoload is on, we need to check
		*  where this VM came from
		*/

		if (vp != null && Autoload) {
		    /*
		    *  see if this VM came from a library.  Need to pass sourceTemplate
		    *  in the event namespaces are set, as it could be masked by local
		    */

		    System.String lib = vmManager.getLibraryName(vmName, sourceTemplate);

		    if (lib != null) {
			try {
			    /*
			    *  get the template from our map
			    */

			    Twonk tw = (Twonk) libModMap[lib];

			    if (tw != null) {
				Template template = tw.template;

				/*
				*  now, compare the last modified time of the resource
				*  with the last modified time of the template
				*  if the file has changed, then reload. Otherwise, we should
				*  be ok.
				*/

				long tt = tw.modificationTime;
				long ft = template.ResourceLoader.getLastModified(template)
				;

				if (ft > tt) {
				    logVMMessageInfo("Velocimacro : autoload reload for VMs from " + "VM library template : " + lib);

				    /*
				    *  when there are VMs in a library that invoke each other,
				    *  there are calls into getVelocimacro() from the init() 
				    *  process of the VM directive.  To stop the infinite loop
				    *  we save the current time reported by the resource loader
				    *  and then be honest when the reload is complete
				    */

				    tw.modificationTime = ft;

				    template = rsvc.getTemplate(lib)
				    ;

				    /*
				    * and now we be honest
				    */

				    tw.template = template;
				    tw.modificationTime = template.LastModified;

				    /*
				    *  note that we don't need to put this twonk back 
				    *  into the map, as we can just use the same reference
				    *  and this block is synchronized
				    */
				}
			    }
			}
			catch (System.Exception e) {
			    logVMMessageInfo("Velocimacro : error using  VM " + "library template " + lib + " : " + e);
			}

			/*
			*  and get again
			*/
			vp = vmManager.get(vmName, sourceTemplate);
		    }
		}
	    }

	    return vp;
	}

	/// <summary>  tells the vmManager to dump the specified namespace
	/// </summary>
	public virtual bool dumpVMNamespace(System.String namespace_Renamed) {
	    return vmManager.dumpNamespace(namespace_Renamed);
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

	/// <summary> small continer class to hold the duple
	/// of a template and modification time.
	/// We keep the modification time so we can
	/// 'override' it on a reload to prevent
	/// recursive reload due to inter-calling
	/// VMs in a library
	/// </summary>
	//UPGRADE_NOTE: Field 'EnclosingInstance' was added to class 'Twonk' to access its enclosing instance. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1019"'
	private class Twonk {
	    public Twonk(VelocimacroFactory enclosingInstance) {
		InitBlock(enclosingInstance);
	    }
	    private void  InitBlock(VelocimacroFactory enclosingInstance) {
		this.enclosingInstance = enclosingInstance;
	    }
	    private VelocimacroFactory enclosingInstance;
	    public VelocimacroFactory Enclosing_Instance {
		get {
		    return enclosingInstance;
		}

	    }
	    public Template template;
	    public long modificationTime;
	}
    }
}
