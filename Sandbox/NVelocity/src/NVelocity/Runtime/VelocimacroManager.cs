namespace NVelocity.Runtime
{
    /*
    * The Apache Software License, Version 1.1
    *
    * Copyright (c) 2000-2002 The Apache Software Foundation.  All rights
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
    using Template = NVelocity.Template;
    using NVelocity.Runtime.Directive;
    using VelocimacroProxy = NVelocity.Runtime.Directive.VelocimacroProxy;
    using SimpleNode = NVelocity.Runtime.Parser.Node.SimpleNode;
    using StringUtils = NVelocity.Util.StringUtils;
    using NVelocity.Context;
    using NVelocity;

    /// <summary> Manages VMs in namespaces.  Currently, two namespace modes are
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
    /// <version> $Id: VelocimacroManager.cs,v 1.3 2003/10/27 13:54:10 corts Exp $
    ///
    /// </version>
    public class VelocimacroManager {
	private void  InitBlock() {
	    namespaceHash = new System.Collections.Hashtable();
	    libraryMap = new System.Collections.Hashtable();
	}
	public virtual bool NamespaceUsage
	{
	    set {
		namespacesOn = value;
	    }

	}
	public virtual bool RegisterFromLib
	{
	    set {
		registerFromLib = value;
	    }

	}
	public virtual bool TemplateLocalInlineVM
	{
	    set {
		inlineLocalMode = value;
	    }

	}
	private RuntimeServices rsvc = null;
	private static System.String GLOBAL_NAMESPACE = "";

	private bool registerFromLib = false;

	/// <summary>Hash of namespace hashes.
	/// </summary>
	//UPGRADE_NOTE: The initialization of  'namespaceHash' was moved to method 'InitBlock'. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1005"'
	private System.Collections.Hashtable namespaceHash;

	/// <summary>map of names of library tempates/namespaces
	/// </summary>
	//UPGRADE_NOTE: The initialization of  'libraryMap' was moved to method 'InitBlock'. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1005"'
	private System.Collections.Hashtable libraryMap;

	/*
	* big switch for namespaces.  If true, then properties control 
	* usage. If false, no. 
	*/
	private bool namespacesOn = true;
	private bool inlineLocalMode = false;

	/// <summary> Adds the global namespace to the hash.
	/// </summary>
	internal VelocimacroManager(RuntimeServices rs) {
	    InitBlock();
	    this.rsvc = rs;

	    /*
	    *  add the global namespace to the namespace hash. We always have that.
	    */

	    addNamespace(GLOBAL_NAMESPACE);
	}

	/// <summary> Adds a VM definition to the cache.
	/// </summary>
	/// <returns>Whether everything went okay.
	///
	/// </returns>
	public virtual bool addVM(System.String vmName, System.String macroBody, System.String[] argArray, System.String namespace_Renamed) {
	    MacroEntry me = new MacroEntry(this, this, vmName, macroBody, argArray, namespace_Renamed);

	    me.FromLibrary = registerFromLib;

	    /*
	    *  the client (VMFactory) will signal to us via
	    *  registerFromLib that we are in startup mode registering
	    *  new VMs from libraries.  Therefore, we want to
	    *  addto the library map for subsequent auto reloads
	    */

	    bool isLib = true;

	    if (registerFromLib) {
		SupportClass.PutElement(libraryMap, namespace_Renamed, namespace_Renamed);
	    } else {
		/*
		*  now, we first want to check to see if this namespace (template)
		*  is actually a library - if so, we need to use the global namespace
		*  we don't have to do this when registering, as namespaces should
		*  be shut off. If not, the default value is true, so we still go
		*  global
		*/

		isLib = libraryMap.ContainsKey(namespace_Renamed);
	    }

	    if (!isLib && usingNamespaces(namespace_Renamed)) {
		/*
		*  first, do we have a namespace hash already for this namespace?
		*  if not, add it to the namespaces, and add the VM
		*/

		System.Collections.Hashtable local = getNamespace(namespace_Renamed, true);
		SupportClass.PutElement(local, (System.String) vmName, me);

		return true;
	    } else {
		/*
		*  otherwise, add to global template.  First, check if we
		*  already have it to preserve some of the autoload information
		*/

		MacroEntry exist = (MacroEntry) getNamespace(GLOBAL_NAMESPACE)[vmName];

		if (exist != null) {
		    me.FromLibrary = exist.FromLibrary;
		}

		/*
		*  now add it
		*/

		SupportClass.PutElement(getNamespace(GLOBAL_NAMESPACE), vmName, me);

		return true;
	    }
	}

	/// <summary> gets a new living VelocimacroProxy object by the
	/// name / source template duple
	/// </summary>
	public virtual VelocimacroProxy get
	    (System.String vmName, System.String namespace_Renamed) {

	    if (usingNamespaces(namespace_Renamed)) {
		System.Collections.Hashtable local = getNamespace(namespace_Renamed, false);

		/*
		*  if we have macros defined for this template
		*/

		if (local != null) {
		    MacroEntry me = (MacroEntry) local[vmName];

		    if (me != null) {
			return me.createVelocimacro(namespace_Renamed);
		    }
		}
	    }

	    /*
	    * if we didn't return from there, we need to simply see 
	    * if it's in the global namespace
	    */

	    //UPGRADE_NOTE: Variable me was renamed because block definition does not hide it. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1008"'
	    MacroEntry me2 = (MacroEntry) getNamespace(GLOBAL_NAMESPACE)[vmName];

	    if (me2 != null) {
		return me2.createVelocimacro(namespace_Renamed);
	    }

	    return null;
	}

	/// <summary> Removes the VMs and the namespace from the manager.
	/// Used when a template is reloaded to avoid
	/// accumulating drek
	/// *
	/// </summary>
	/// <param name="namespace">namespace to dump
	/// </param>
	/// <returns>boolean representing success
	///
	/// </returns>
	public virtual bool dumpNamespace(System.String namespace_Renamed) {
	    lock(this) {
		if (usingNamespaces(namespace_Renamed)) {
		    System.Object temp_key;
		    System.Collections.Hashtable temp_hashtable;
		    temp_key = namespace_Renamed;
		    temp_hashtable = namespaceHash;
		    System.Collections.Hashtable h = (System.Collections.Hashtable) temp_hashtable[temp_key];
		    temp_hashtable.Remove(temp_key);

		    if (h == null)
			return false;

		    h.Clear();

		    return true;
		}

		return false;
	    }
	}

	/// <summary>  public switch to let external user of manager to control namespace
	/// usage indep of properties.  That way, for example, at startup the
	/// library files are loaded into global namespace
	/// </summary>



	/// <summary>  returns the hash for the specified namespace.  Will not create a new one
	/// if it doesn't exist
	/// *
	/// </summary>
	/// <param name="namespace"> name of the namespace :)
	/// </param>
	/// <returns>namespace Hashtable of VMs or null if doesn't exist
	///
	/// </returns>
	private System.Collections.Hashtable getNamespace(System.String namespace_Renamed) {
	    return getNamespace(namespace_Renamed, false);
	}

	/// <summary>  returns the hash for the specified namespace, and if it doesn't exist
	/// will create a new one and add it to the namespaces
	/// *
	/// </summary>
	/// <param name="namespace"> name of the namespace :)
	/// </param>
	/// <param name="addIfNew"> flag to add a new namespace if it doesn't exist
	/// </param>
	/// <returns>namespace Hashtable of VMs or null if doesn't exist
	///
	/// </returns>
	private System.Collections.Hashtable getNamespace(System.String namespace_Renamed, bool addIfNew) {
	    System.Collections.Hashtable h = (System.Collections.Hashtable) namespaceHash[namespace_Renamed];

	    if (h == null && addIfNew) {
		h = addNamespace(namespace_Renamed);
	    }

	    return h;
	}

	/// <summary>   adds a namespace to the namespaces
	/// *
	/// </summary>
	/// <param name="namespace">name of namespace to add
	/// </param>
	/// <returns>Hash added to namespaces, ready for use
	///
	/// </returns>
	private System.Collections.Hashtable addNamespace(System.String namespace_Renamed) {
	    System.Collections.Hashtable h = new System.Collections.Hashtable();
	    System.Object oh;

	    if ((oh = SupportClass.PutElement(namespaceHash, namespace_Renamed, h)) != null) {
		/*
		* There was already an entry on the table, restore it!
		* This condition should never occur, given the code
		* and the fact that this method is private.
		* But just in case, this way of testing for it is much
		* more efficient than testing before hand using get().
		*/
		SupportClass.PutElement(namespaceHash, namespace_Renamed, oh);
		/*
		* Should't we be returning the old entry (oh)?
		* The previous code was just returning null in this case.
		*/
		return null;
	    }

	    return h;
	}

	/// <summary>  determines if currently using namespaces.
	/// *
	/// </summary>
	/// <param name="namespace">currently ignored
	/// </param>
	/// <returns>true if using namespaces, false if not
	///
	/// </returns>
	private bool usingNamespaces(System.String namespace_Renamed) {
	    /*
	    *  if the big switch turns of namespaces, then ignore the rules
	    */

	    if (!namespacesOn) {
		return false;
	    }

	    /*
	    *  currently, we only support the local template namespace idea
	    */

	    if (inlineLocalMode) {
		return true;
	    }

	    return false;
	}

	public virtual System.String getLibraryName(System.String vmName, System.String namespace_Renamed) {
	    if (usingNamespaces(namespace_Renamed)) {
		System.Collections.Hashtable local = getNamespace(namespace_Renamed, false);

		/*
		*  if we have this macro defined in this namespace, then
		*  it is masking the global, library-based one, so 
		*  just return null
		*/

		if (local != null) {
		    MacroEntry me = (MacroEntry) local[vmName];

		    if (me != null) {
			return null;
		    }
		}
	    }

	    /*
	    * if we didn't return from there, we need to simply see 
	    * if it's in the global namespace
	    */

	    //UPGRADE_NOTE: Variable me was renamed because block definition does not hide it. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1008"'
	    MacroEntry me2 = (MacroEntry) getNamespace(GLOBAL_NAMESPACE)[vmName];

	    if (me2 != null) {
		return me2.SourceTemplate;
	    }

	    return null;
	}


	/// <summary>  wrapper class for holding VM information
	/// </summary>
	//UPGRADE_NOTE: Field 'EnclosingInstance' was added to class 'MacroEntry' to access its enclosing instance. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1019"'
	protected internal class MacroEntry {
	    private void  InitBlock(VelocimacroManager enclosingInstance) {
		this.enclosingInstance = enclosingInstance;
	    }
	    private VelocimacroManager enclosingInstance;
	    public virtual bool FromLibrary
	    {
		get {
		    return fromLibrary;
		}

		set {
		    fromLibrary = value;
		}

	    }
	    public virtual SimpleNode NodeTree
	    {
		get {
		    return nodeTree;
		}

	    }
	    public virtual System.String SourceTemplate
	    {
		get {
		    return sourcetemplate;
		}

	    }
	    public VelocimacroManager Enclosing_Instance
	    {
		get {
		    return enclosingInstance;
		}

	    }
	    internal System.String macroname;
	    internal System.String[] argarray;
	    internal System.String macrobody;
	    internal System.String sourcetemplate;
	    internal SimpleNode nodeTree = null;
	    internal VelocimacroManager manager = null;
	    internal bool fromLibrary = false;

	    internal MacroEntry(VelocimacroManager enclosingInstance, VelocimacroManager vmm, System.String vmName, System.String macroBody, System.String[] argArray, System.String sourceTemplate) {
		InitBlock(enclosingInstance);
		this.macroname = vmName;
		this.argarray = argArray;
		this.macrobody = macroBody;
		this.sourcetemplate = sourceTemplate;
		this.manager = vmm;
	    }





	    internal virtual VelocimacroProxy createVelocimacro(System.String namespace_Renamed) {
		VelocimacroProxy vp = new VelocimacroProxy();
		vp.Name = this.macroname;
		vp.ArgArray = this.argarray;
		vp.Macrobody = this.macrobody;
		vp.NodeTree = this.nodeTree;
		vp.Namespace = namespace_Renamed;
		return vp;
	    }

	    internal virtual void  setup(InternalContextAdapter ica) {
		/*
		*  if not parsed yet, parse!
		*/

		if (nodeTree == null)
		    parseTree(ica);
	    }

	    internal virtual void  parseTree(InternalContextAdapter ica) {
		try {
		    //UPGRADE_ISSUE: The equivalent of constructor 'java.io.BufferedReader.BufferedReader' is incompatible with the expected type in C#. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1109"'
		    System.IO.TextReader br = new System.IO.StringReader(macrobody);

		    nodeTree = Enclosing_Instance.rsvc.parse(br, "VM:" + macroname, true);
		    nodeTree.init(ica, null);
		} catch (System.Exception e) {
		    Enclosing_Instance.rsvc.error("VelocimacroManager.parseTree() : exception " + macroname + " : " + StringUtils.stackTrace(e));
		}
	    }
	}
    }
}
