using System;
using System.Collections;
using System.IO;
using System.Reflection;

namespace NVelocity.Runtime.Resource.Loader {

    /// <summary>
    /// Locates a resource (file) in the file system or as a resource in an assembly.
    /// </summary>
    public class ResourceLocator {

	private String filename = String.Empty;
	private FileInfo file = null;
	private Boolean isFile = false;
	private Boolean isResource = false;
	private IList assemblies = new ArrayList();
	private Assembly assembly = null;

	public ResourceLocator(String filename) : this(null, filename) {}

	public ResourceLocator(String path, String filename) {
	    if (path != null) {
		this.filename = path + "\\" + filename;
	    } else {
		this.filename = filename;
	    }

	    file = new FileInfo(this.filename);
	    if (file.Exists) {
		this.filename = file.FullName;
		isFile = true;
	    } else {
		// the Calling, Executing and Entry assemblies may not have links to each other -
		// so attempt to get assemblies from all possible paths.  The current AppDomain may also
		// know of more assemblies.
		GetReferencedAssemblies(System.Reflection.Assembly.GetExecutingAssembly(), assemblies);
		if (System.Reflection.Assembly.GetEntryAssembly() != null) {
		    GetReferencedAssemblies(System.Reflection.Assembly.GetEntryAssembly(), assemblies);
		}
		if (System.Reflection.Assembly.GetCallingAssembly() != null) {
		    GetReferencedAssemblies(System.Reflection.Assembly.GetCallingAssembly(), assemblies);
		}
		foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
		    GetReferencedAssemblies(assembly, assemblies);
		}

		String fn = filename.ToLower().Replace(".\\", "");
		fn = fn.Replace("\\", ".");
		fn = fn.Replace("/", ".");
		foreach(Assembly a in assemblies) {
		    // don't attempt to find resources in dynamic assemblies as it is not supported
		    if (!(a is System.Reflection.Emit.AssemblyBuilder)) {
			String prefix = a.FullName.Substring(0,a.FullName.IndexOf(",")).ToLower();
			String[] names = a.GetManifestResourceNames();
			foreach(String s in names) {
			    if (s.ToLower().Equals(fn) || s.ToLower().Equals(prefix + "." + fn)) {
				this.filename = s;
				assembly = a;
				isResource = true;
			    }
			}
		    }
		}
	    }
	}

	public static void GetReferencedAssemblies(Assembly a, IList list) {
	    String s = a.GetName().FullName;
	    foreach(AssemblyName an in a.GetReferencedAssemblies()) {
		try {
		    Assembly c = Assembly.Load(an);
		    if (!list.Contains(c)) {
			list.Add(c);
			GetReferencedAssemblies(c, list);
		    }
		} catch {
		    // ignore problems loading assemblies, just move on
		}
	    }
	if (!list.Contains(a)) {
		list.Add(a);
	    }
	}

	public Stream OpenRead() {
	    if (isFile) {
		return file.OpenRead();
	    } else if (isResource) {
		return assembly.GetManifestResourceStream(filename);
	    } else {
		throw new FileNotFoundException("Resource could not be found", filename);
	    }
	}

	public String FullName {
	    get { return this.filename; }
	}

	public Boolean Exists {
	    get { return (isFile || isResource); }
	}

	private Stream FindResource(String path, String template) {
	    try {
		FileInfo file = new FileInfo(path + "\\" + template)
		;

		if (file.Exists) {
		    return new BufferedStream(new FileStream(file.FullName, FileMode.Open, FileAccess.Read));
		} else {
		    IList list = new ArrayList();
		    GetReferencedAssemblies(System.Reflection.Assembly.GetEntryAssembly(), list);

		    foreach(Assembly a in list) {
			System.IO.Stream s = a.GetManifestResourceStream(template)
			;
			if (s != null) {
			    return s;
			}
		    }


		    return null;
		}
	    } catch (System.IO.FileNotFoundException fnfe) {
		/*
		*  log and convert to a general Velocity ResourceNotFoundException
		*/
		return null;
	    }
	}

    }
}
