using System;
using System.Collections;
using System.IO;

using Commons.Collections;

using SourceForge.NAnt;
using SourceForge.NAnt.Attributes;

using NVelocity.App;
using NVelocity.Context;
using NVelocity.Exception;
using NVelocity.Runtime;
using NVelocity.Runtime.Resource.Loader;
using NVelocity.Util;

namespace NVelocity.NAnt.Texen {

    /// <summary>
    /// An ant task for generating output by using Velocity
    /// </summary>
    /// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a></author>
    /// <author><a href="robertdonkin@mac.com">Robert Burrell Donkin</a></author>
    [TaskName("texen")]
    public class TexenTask : Task {

	/// <summary> This message fragment (telling users to consult the log or
	/// invoke ant with the -debug flag) is appended to rethrown
	/// exception messages.
	/// </summary>
	private static System.String ERR_MSG_FRAGMENT = ". For more information consult the velocity log, or invoke ant " + "with the -debug flag.";

	/// <summary> This is the control template that governs the output.
	/// It may or may not invoke the services of worker
	/// templates.
	/// </summary>
	protected internal System.String controlTemplate;

	/// <summary> This is where Velocity will look for templates
	/// using the file template loader.
	/// </summary>
	protected internal System.String templatePath;

	/// <summary>
	/// This is where texen will place all the output
	/// that is a product of the generation process.
	/// </summary>
	protected internal System.String outputDirectory;

	/// <summary>
	/// This is the file where the generated text
	/// will be placed.
	/// </summary>
	protected internal System.String outputFile;

	/// <summary> This is the encoding for the output file(s).
	/// </summary>
	protected internal System.String outputEncoding;

	/// <summary> This is the encoding for the input file(s)
	/// (templates).
	/// </summary>
	protected internal System.String inputEncoding;

	/// <summary> <p>
	/// These are properties that are fed into the
	/// initial context from a properties file. This
	/// is simply a convenient way to set some values
	/// that you wish to make available in the context.
	/// </p>
	/// <p>
	/// These values are not critical, like the template path
	/// or output path, but allow a convenient way to
	/// set a value that may be specific to a particular
	/// generation task.
	/// </p>
	/// <p>
	/// For example, if you are generating scripts to allow
	/// user to automatically create a database, then
	/// you might want the <code>$databaseName</code>
	/// to be placed
	/// in the initial context so that it is available
	/// in a script that might look something like the
	/// following:
	/// <code><pre>
	/// #!bin/sh
	///
	/// echo y | mysqladmin create $databaseName
	/// </pre></code>
	/// The value of <code>$databaseName</code> isn't critical to
	/// output, and you obviously don't want to change
	/// the ant task to simply take a database name.
	/// So initial context values can be set with
	/// properties file.
	/// </summary>
	protected internal ExtendedProperties contextProperties;

	/// <summary>
	/// Property which controls whether the classpath
	/// will be used when trying to locate templates.
	/// </summary>
	protected internal bool useClasspath;

	/// <summary>
	/// Path separator.
	/// </summary>
	private System.String fileSeparator = System.IO.Path.DirectorySeparatorChar.ToString();



	public TexenTask() {}

	/// <summary> [REQUIRED] Set the control template for the
	/// generating process.
	/// </summary>
	[TaskAttribute("controlTemplate", Required=true)]
	public virtual System.String ControlTemplate {
	    get {
		return controlTemplate;
	    }

	    set {
		this.controlTemplate = value;
	    }

	}



	/// <summary> Get the control template for the
	/// generating process.
	/// </summary>

	/// <summary> [REQUIRED] Set the path where Velocity will look
	/// for templates using the file template
	/// loader.
	/// </summary>


	/// <summary> Get the path where Velocity will look
	/// for templates using the file template
	/// loader.
	/// </summary>

	/// <summary> [REQUIRED] Set the output directory. It will be
	/// created if it doesn't exist.
	/// </summary>

	/// <summary> Get the output directory.
	/// </summary>

	/// <summary> [REQUIRED] Set the output file for the
	/// generation process.
	/// </summary>

	/// <summary> Set the output encoding.
	/// </summary>

	/// <summary> Set the input (template) encoding.
	/// </summary>

	/// <summary> Get the output file for the
	/// generation process.
	/// </summary>

	/// <summary> Set the context properties that will be
	/// fed into the initial context be the
	/// generating process starts.
	/// </summary>

	/// <summary> Get the context properties that will be
	/// fed into the initial context be the
	/// generating process starts.
	/// </summary>

	/// <summary> Set the use of the classpath in locating templates
	/// *
	/// </summary>
	/// <param name="boolean">true means the classpath will be used.
	///
	/// </param>

	/// <summary> Creates a VelocityContext.
	/// *
	/// </summary>
	/// <returns>new Context
	/// @throws Exception the execute method will catch
	/// and rethrow as a <code>BuildException</code>
	///
	/// </returns>
	public virtual IContext initControlContext() {
	    return new VelocityContext();
	}


	[TaskAttribute("templatePath", Required=true)]
	public virtual System.String TemplatePath {
	    get {
		return templatePath;
	    }

	    set {
		System.Text.StringBuilder resolvedPath = new System.Text.StringBuilder();
		SupportClass.Tokenizer st = new SupportClass.Tokenizer(value, ",");
		while (st.HasMoreTokens()) {
		    // resolve relative path from basedir and leave
		    // absolute path untouched.
		    //System.IO.FileInfo fullPath = project.resolveFile(st.NextToken());
		    System.IO.FileInfo fullPath = new FileInfo(st.NextToken());
		    resolvedPath.Append(fullPath.FullName);
		    if (st.HasMoreTokens()) {
			resolvedPath.Append(",");
		    }
		}
		this.templatePath = resolvedPath.ToString();

		//System.Console.Out.WriteLine(value);
	    }

	}


	[TaskAttribute("outputDirectory", Required=true)]
	public virtual System.String OutputDirectory {
	    get {
		return outputDirectory;
	    }
	    set {
		this.outputDirectory = value;
	    }

	}

	[TaskAttribute("outputFile", Required=false)]
	public virtual System.String OutputFile {
	    get {
		return outputFile;
	    }

	    set {
		this.outputFile = value;
	    }
	}


	[TaskAttribute("outputEncoding", Required=false)]
	public virtual System.String OutputEncoding {
	    set {
		this.outputEncoding = value;
	    }
	}


	[TaskAttribute("inputEncoding", Required=false)]
	public virtual System.String InputEncoding {
	    set {
		this.inputEncoding = value;
	    }
	}


	[TaskAttribute("contextProperties", Required=false)]
	public virtual System.String ContextProperties {
	    set {
		System.String[] sources = StringUtils.split(value, ",");
		contextProperties = new ExtendedProperties();

		// Always try to get the context properties resource
		// from a file first. Templates may be taken from a JAR
		// file but the context properties resource may be a
		// resource in the filesystem. If this fails than attempt
		// to get the context properties resource from the
		// classpath.
		for (int i = 0; i < sources.Length; i++) {
		    ExtendedProperties source = new ExtendedProperties();

		    try {
			// resolve relative path from basedir and leave
			// absolute path untouched.
			//System.IO.FileInfo fullPath = project.resolveFile(sources[i]);
			System.IO.FileInfo fullPath = new FileInfo(sources[i]);
			Log.WriteLine(LogPrefix + "Using contextProperties file: " + fullPath);
			source.Load(new System.IO.FileStream(fullPath.FullName, System.IO.FileMode.Open, System.IO.FileAccess.Read));
		    } catch (System.Exception) {
			try {
			    ResourceLocator rl = new ResourceLocator(sources[i]);
			    Stream inputStream = rl.OpenRead();

			    if (inputStream == null) {
				throw new BuildException("Context properties file " + sources[i] + " could not be found in the file system or on the classpath!");
			    } else {
				source.Load(inputStream);
			    }
			} catch (System.IO.IOException) {
			    source = null;
			}
		    }

		    IEnumerator j = source.Keys;

		    while (j.MoveNext()) {
			System.String name = (System.String) j.Current;
			System.String value_Renamed = source.GetString(name);
			contextProperties.SetProperty(name, value_Renamed);
		    }
		}
	    }

	}

	//	public virtual ExtendedProperties GetContextProperties() {
	//		return contextProperties;
	//	}
	//
	//	public virtual bool UseClasspath {
	//	    set {
	//		this.useClasspath = value;
	//	    }
	//	}





	/// <summary>
	/// Execute the input script with Velocity
	/// @throws BuildException
	/// BuildExceptions are thrown when required attributes are missing.
	/// Exceptions thrown by Velocity are rethrown as BuildExceptions.
	/// </summary>
	protected override void ExecuteTask() {
	    // Make sure the template path is set.
	    if (templatePath == null && useClasspath == false) {
		throw new BuildException("The template path needs to be defined if you are not using " + "the classpath for locating templates!");
	    }

	    // Make sure the control template is set.
	    if (controlTemplate == null) {
		throw new BuildException("The control template needs to be defined!");
	    }

	    // Make sure the output directory is set.
	    if (outputDirectory == null) {
		throw new BuildException("The output directory needs to be defined!");
	    }

	    // Make sure there is an output file.
	    if (outputFile == null) {
		throw new BuildException("The output file needs to be defined!");
	    }

	    VelocityEngine ve = new VelocityEngine();

	    try {
		// Setup the Velocity Runtime.
		if (templatePath != null) {
		    //log("Using templatePath: " + templatePath, project.MSG_VERBOSE);
		    Log.WriteLine(LogPrefix + "Using templatePath: " + templatePath);
		    ve.SetProperty(RuntimeConstants_Fields.FILE_RESOURCE_LOADER_PATH, templatePath);
		}

		if (useClasspath) {
		    Log.WriteLine(LogPrefix + "Using classpath");
		    ve.AddProperty(RuntimeConstants_Fields.RESOURCE_LOADER, "classpath");
		    ve.SetProperty("classpath." + RuntimeConstants_Fields.RESOURCE_LOADER + ".class", "org.apache.velocity.runtime.resource.loader.ClasspathResourceLoader");
		    ve.SetProperty("classpath." + RuntimeConstants_Fields.RESOURCE_LOADER + ".cache", "false");
		    ve.SetProperty("classpath." + RuntimeConstants_Fields.RESOURCE_LOADER + ".modificationCheckInterval", "2");
		}

		ve.Init();

		// Create the text generator.
		Generator generator = Generator.Instance;
		generator.LogPrefix = LogPrefix;
		generator.VelocityEngine = ve;
		generator.OutputPath = outputDirectory;
		generator.InputEncoding = inputEncoding;
		generator.OutputEncoding = outputEncoding;

		if (templatePath != null) {
		    generator.TemplatePath = templatePath;
		}

		// Make sure the output directory exists, if it doesn't
		// then create it.
		System.IO.FileInfo file = new System.IO.FileInfo(outputDirectory);
		bool tmpBool;
		if (System.IO.File.Exists(file.FullName))
		    tmpBool = true;
		else
		    tmpBool = System.IO.Directory.Exists(file.FullName);
		if (!tmpBool) {
		    System.IO.Directory.CreateDirectory(file.FullName);
		}

		System.String path = outputDirectory + System.IO.Path.DirectorySeparatorChar.ToString() + outputFile;
		Log.WriteLine(LogPrefix + "Generating to file " + path);
		System.IO.StreamWriter writer = generator.getWriter(path, outputEncoding);

		// The generator and the output path should
		// be placed in the init context here and
		// not in the generator class itself.
		IContext c = initControlContext();

		// Everything in the generator class should be
		// pulled out and placed in here. What the generator
		// class does can probably be added to the Velocity
		// class and the generator class can probably
		// be removed all together.
		populateInitialContext(c);

		// Feed all the options into the initial
		// control context so they are available
		// in the control/worker templates.
		if (contextProperties != null) {
		    IEnumerator i = contextProperties.Keys;

		    while (i.MoveNext()) {
			System.String property = (System.String) i.Current;
			System.String value_Renamed = contextProperties.GetString(property);

			// Now lets quickly check to see if what
			// we have is numeric and try to put it
			// into the context as an Integer.
			try {
			    c.Put(property, System.Int32.Parse(value_Renamed));
			} catch (System.FormatException nfe) {
			    // Now we will try to place the value into
			    // the context as a boolean value if it
			    // maps to a valid boolean value.
			    System.String booleanString = contextProperties.TestBoolean(value_Renamed);

			    if (booleanString != null) {
				c.Put(property, booleanString.ToUpper().Equals("TRUE"));
			    } else {
				// We are going to do something special
				// for properties that have a "file.contents"
				// suffix: for these properties will pull
				// in the contents of the file and make
				// them available in the context. So for
				// a line like the following in a properties file:
				//
				// license.file.contents = license.txt
				//
				// We will pull in the contents of license.txt
				// and make it available in the context as
				// $license. This should make texen a little
				// more flexible.
				if (property.EndsWith("file.contents")) {
				    // We need to turn the license file from relative to
				    // absolute, and let Ant help :)
				    //value_Renamed = StringUtils.fileContentsToString(project.resolveFile(value_Renamed).CanonicalPath);
				    value_Renamed = StringUtils.fileContentsToString(new FileInfo(value_Renamed).FullName);

				    property = property.Substring(0, (property.IndexOf("file.contents") - 1) - (0));
				}

				c.Put(property, value_Renamed);
			    }
			}
		    }
		}

		writer.Write(generator.parse(controlTemplate, c));
		writer.Flush();
		writer.Close();
		generator.shutdown();
		cleanup();
	    } catch (BuildException e) {
		throw e;
	    } catch (MethodInvocationException e) {
		throw new BuildException("Exception thrown by '" + e.ReferenceName + "." + e.MethodName + "'" + ERR_MSG_FRAGMENT, e.WrappedThrowable);
	    } catch (ParseErrorException e) {
		throw new BuildException("Velocity syntax error" + ERR_MSG_FRAGMENT, e);
	    } catch (ResourceNotFoundException e) {
		throw new BuildException("Resource not found" + ERR_MSG_FRAGMENT, e);
	    } catch (System.Exception e) {
		throw new BuildException("Generation failed" + ERR_MSG_FRAGMENT, e);
	    }
	}

	/// <summary>
	/// <p>Place useful objects into the initial context.</p>
	/// <p>TexenTask places <code>Date().toString()</code> into the
	/// context as <code>$now</code>.  Subclasses who want to vary the
	/// objects in the context should override this method.</p>
	/// <p><code>$generator</code> is not put into the context in this
	/// method.</p>
	/// </summary>
	/// <param name="context">The context to populate, as retrieved from
	/// {@link #initControlContext()}.
	/// @throws Exception Error while populating context.  The {@link
	/// #execute()} method will catch and rethrow as a
	/// <code>BuildException</code>.
	/// </param>
	protected internal virtual void  populateInitialContext(IContext context) {
	    context.Put("now", System.DateTime.Now.ToString("r"));
	}

	/// <summary>
	/// A hook method called at the end of {@link #execute()} which can
	/// be overridden to perform any necessary cleanup activities (such
	/// as the release of database connections, etc.).  By default,
	/// does nothing.
	/// </summary>
	/// <exception cref="">Exception Problem cleaning up.</exception>
	protected internal virtual void  cleanup() {}}
}
