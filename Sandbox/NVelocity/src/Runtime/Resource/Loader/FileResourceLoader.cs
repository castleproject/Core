using System;
using System.Collections;
using System.IO;
using StringUtils = NVelocity.Util.StringUtils;
using Resource = NVelocity.Runtime.Resource.Resource;
using ResourceNotFoundException = NVelocity.Exception.ResourceNotFoundException;
using Commons.Collections;
using NVelocity;

namespace NVelocity.Runtime.Resource.Loader {

    /// <summary>
    /// A loader for templates stored on the file system.
    /// </summary>
    /// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a> </author>
    public class FileResourceLoader : ResourceLoader {

	/// <summary>
	/// The paths to search for templates.
	/// </summary>
	protected ArrayList paths = null;

	/// <summary>
	/// Used to map the path that a template was found on
	/// so that we can properly check the modification
	/// times of the files.
	/// </summary>
	protected Hashtable templatePaths = new Hashtable();

	public FileResourceLoader() {}

	public override void init(ExtendedProperties configuration) {
	    rsvc.info("FileResourceLoader : initialization starting.");

	    paths = configuration.GetVector("path");

	    // lets tell people what paths we will be using
	    foreach(String path in paths) {
		rsvc.info("FileResourceLoader : adding path '" + path + "'");
	    }

	    rsvc.info("FileResourceLoader : initialization complete.");
	}


	/// <summary>
	/// Get an InputStream so that the Runtime can build a
	/// template with it.
	/// </summary>
	/// <param name="name">name of template to get</param>
	/// <returns>InputStream containing the template
	/// @throws ResourceNotFoundException if template not found
	/// in the file template path.
	/// </returns>
	public override Stream getResourceStream(String templateName) {
	    lock(this) {
		String template = null;
		int size = paths.Count;

		for (int i = 0; i < size; i++) {
		    String path = (String)
		    paths[i];

		    // Make sure we have a valid templateName.
		    if (templateName == null || templateName.Length == 0) {
			// If we don't get a properly formed templateName
			// then there's not much we can do. So
			// we'll forget about trying to search
			// any more paths for the template.
			throw new ResourceNotFoundException("Need to specify a file name or file path!");
		    }

		    template = StringUtils.normalizePath(templateName)
		    ;
		    if (template == null || template.Length == 0) {
			String msg = "File resource error : argument " + template + " contains .. and may be trying to access " + "content outside of template root.  Rejected.";

			rsvc.error("FileResourceLoader : " + msg)
			;

			throw new ResourceNotFoundException(msg);
		    }

		    // if a / leads off, then just nip that :)
		    if (template.StartsWith("/")) {
			template = template.Substring(1)
			;
		    }

		    Stream inputStream = findTemplate(path, template)
		    ;

		    if (inputStream != null) {
			// Store the path that this template came
			// from so that we can check its modification
			// time.
			SupportClass.PutElement(templatePaths, templateName, path);
			return inputStream;
		    }
		}

		// We have now searched all the paths for
		// templates and we didn't find anything so
		// throw an exception.
		String msg2 = "FileResourceLoader Error: cannot find resource " + template;
		throw new ResourceNotFoundException(msg2);
	    }
	}

	/// <summary>
	/// Try to find a template given a normalized path.
	/// </summary>
	/// <param name="String">a normalized path</param>
	/// <returns>InputStream input stream that will be parsed</returns>
	private Stream findTemplate(String path, String template) {
	    try {
		ResourceLocator rl = new ResourceLocator(path, template)
		;
		if (rl.Exists) {
		    return new BufferedStream(rl.OpenRead());
		} else {
		    return null;
		}
	    } catch (FileNotFoundException fnfe) {
		// log and convert to a general Velocity ResourceNotFoundException
		return null;
	    }
	}

	/// <summary>
	/// How to keep track of all the modified times
	/// across the paths.
	/// </summary>
	public override bool isSourceModified(Resource resource) {
	    String path = (String) templatePaths[resource.Name];
	    FileInfo file = new FileInfo(path + "\\" + resource.Name);

	    if (file.Exists) {
		if (file.LastWriteTime.Ticks != resource.LastModified) {
		    return true;
		} else {
		    return false;
		}
	    }

	    // If the file is now unreadable, or it has
	    // just plain disappeared then we'll just say
	    // that it's modified :-) When the loader attempts
	    // to load the stream it will fail and the error
	    // will be reported then.
	    return true;
	}

	public override long getLastModified(Resource resource) {
	    String path = (String) templatePaths[resource.Name];
	    FileInfo file = new FileInfo(path + "\\" + resource.Name);

	    if (file.Exists) {
		return file.LastWriteTime.Ticks;
	    } else {
		return 0;
	    }
	}

    }
}
