using System;
using System.IO;
using System.Xml;

using NVelocity.App;
using NVelocity.Runtime;
using NVelocity.Util;

using SourceForge.NAnt;
using SourceForge.NAnt.Attributes;


namespace NVelocity.NAnt.Anakia {

    /// <summary>
    /// The purpose of this Ant Task is to allow you to use
    /// Velocity as an XML transformation tool like XSLT is.
    /// So, instead of using XSLT, you will be able to use this
    /// class instead to do your transformations. It works very
    /// similar in concept to Ant's &lt;style&gt; task.
    /// <p>
    /// You can find more documentation about this class on the
    /// Velocity
    /// <a href="http://jakarta.apache.org/velocity/anakia.html">Website</a>.
    /// </summary>
    [TaskName("anakia")]
    public class AnakiaTask : Task {

	/// <summary>
	/// the destination directory
	/// </summary>
	private FileInfo destDir = null;

	/// <summary>
	/// the File to the style file
	/// </summary>
	private FileInfo styleFile = null;

	/// <summary>
	/// the File for the project.xml file
	/// </summary>
	private FileInfo projectFile = null;

	/// <summary>
	/// check the last modified date on files. defaults to true
	/// </summary>
	private bool lastModifiedCheck = true;

	/// <summary>
	/// the default output extension is .html
	/// </summary>
	private String extension = ".html";

	/// <summary>
	/// the template path
	/// </summary>
	private FileInfo templatePath = null;

	/// <summary>
	/// the file to get the velocity properties file
	/// </summary>
	private FileInfo velocityPropertiesFile = null;

	/// <summary>
	/// the VelocityEngine instance to use
	/// </summary>
	private VelocityEngine ve = new VelocityEngine();

	/// <summary>
	/// list of files to be processed
	/// </summary>
	private FileSet fileset = new FileSet();

	/// <summary>
	/// Constructor
	/// </summary>
	public AnakiaTask() {}

	/// <summary>
	/// Set the destination directory into which the VSL result
	/// files should be copied to
	/// </summary>
	/// <param name="dirName">the name of the destination directory</param>
	[TaskAttribute("destdir", Required=true)]
	public virtual String Destdir {
	    set {
		destDir = new FileInfo(value);
	    }
	}

	/// <summary>
	/// Allow people to set the default output file extension
	/// </summary>
	[TaskAttribute("extension", Required=false)]
	public virtual String Extension {
	    set {
		this.extension = value;
	    }
	}

	/// <summary>
	/// Allow people to set the path to the .vsl file
	/// </summary>
	[TaskAttribute("style", Required=true)]
	public virtual String Style {
	    set {
		this.styleFile = new FileInfo(value);
	    }
	}

	/// <summary>
	/// Allow people to set the path to the project.xml file
	/// </summary>
	[TaskAttribute("projectFile", Required=false)]
	public virtual String ProjectFile {
	    set {
		this.projectFile = new FileInfo(value);
	    }
	}

	/// <summary>
	/// Set the path to the templates.
	/// The way it works is this:
	/// If you have a Velocity.properties file defined, this method
	/// will <strong>override</strong> whatever is set in the
	/// Velocity.properties file. This allows one to not have to define
	/// a Velocity.properties file, therefore using Velocity's defaults
	/// only.
	/// </summary>
	[TaskAttribute("templatePath", Required=false)]
	public virtual String TemplatePath {
	    set {
		this.templatePath = new FileInfo(value);
	    }
	}

	/// <summary>
	/// Allow people to set the path to the velocity.properties file
	/// This file is found relative to the path where the JVM was run.
	/// For example, if build.sh was executed in the ./build directory,
	/// then the path would be relative to this directory.
	/// This is optional based on the setting of setTemplatePath().
	/// </summary>
	[TaskAttribute("velocityPropertiesFile", Required=false)]
	public virtual String VelocityPropertiesFile {
	    set {
		this.velocityPropertiesFile = new FileInfo(value);
	    }
	}

	/// <summary>
	/// Turn on/off last modified checking. by default, it is on.
	/// </summary>
	[TaskAttribute("lastModifiedCheck", Required=false)]
	public virtual String LastModifiedCheck {
	    set {
		if (value.ToLower().Equals("false") || value.ToLower().Equals("no") || value.ToLower().Equals("off")) {
		    this.lastModifiedCheck = false;
		}
	    }
	}

	/// <summary>
	/// The set of files to be included in the archive.
	/// </summary>
	[FileSet("fileset")]
	public FileSet FileSet {
	    get { return fileset; }
	}



	/// <summary>
	/// Main body of the application
	/// </summary>
	protected override void ExecuteTask() {
	    if (destDir == null) {
		String msg = "destdir attribute must be set!";
		throw new BuildException(msg);
	    }
	    if (styleFile == null) {
		throw new BuildException("style attribute must be set!");
	    }

	    // TODO: make sure style file exists

	    if (velocityPropertiesFile == null) {
		velocityPropertiesFile = new FileInfo("nvelocity.properties");
	    }

	    /*
	    * If the props file doesn't exist AND a templatePath hasn't 
	    * been defined, then throw the exception.
	    */
	    if (!velocityPropertiesFile.Exists && templatePath==null) {
		throw new BuildException("No template path and could not " + "locate nvelocity.properties file: " + velocityPropertiesFile);
	    }

	    Log.WriteLine(LogPrefix + "Transforming into: " + destDir);

	    // projectFile relative to baseDir
	    if (projectFile != null && !projectFile.Exists) {
		Log.WriteLine(LogPrefix + "Project file is defined, but could not be located: " + projectFile.FullName);
		projectFile = null;
	    }

	    AnakiaXmlDocument projectDocument = null;
	    try {
		if (velocityPropertiesFile.Exists) {
		    ve.Init(velocityPropertiesFile.FullName);
		} else {
		    if (templatePath != null && templatePath.FullName.Length > 0) {
			ve.SetProperty(RuntimeConstants_Fields.FILE_RESOURCE_LOADER_CACHE, true);
			ve.SetProperty(RuntimeConstants_Fields.FILE_RESOURCE_LOADER_PATH, templatePath.FullName);
			ve.Init();
		    }
		}

		// Build the Project file document
		if (projectFile != null) {
		    projectDocument = new AnakiaXmlDocument();
		    projectDocument.Load(projectFile.FullName);
		}
	    } catch (System.Exception e) {
		Log.WriteLine(LogPrefix + "Error: " + e.ToString());
		throw new BuildException(e.ToString());
	    }

	    // get the base directory from the fileset - needed to colapse ../ syntax
	    DirectoryInfo di = new DirectoryInfo(fileset.BaseDirectory);

	    // get a list of files to work on
	    foreach (string filename in fileset.FileNames) {
		String relativeFilename = filename.Substring(di.FullName.Length+1);
		FileInfo file = new FileInfo(filename);
		if (file.Exists) {
		    Process(di.FullName, relativeFilename, destDir, projectDocument);
		}
	    }
	}

	/// <summary>
	/// Process an XML file using Velocity
	/// </summary>
	private void Process(String basedir, String xmlFile, FileInfo destdir, AnakiaXmlDocument projectDocument) {
	    FileInfo outFile = null;
	    FileInfo inFile = null;
	    StreamWriter writer = null;
	    try {
		// the current input file relative to the baseDir
		inFile = new System.IO.FileInfo(basedir + Path.DirectorySeparatorChar.ToString() + xmlFile);
		// the output file relative to basedir
		outFile = new System.IO.FileInfo(destdir + Path.DirectorySeparatorChar.ToString() + xmlFile.Substring(0, (xmlFile.LastIndexOf((System.Char) '.')) - (0)) + extension);

		// only process files that have changed
		if (lastModifiedCheck == false || (inFile.LastWriteTime.Ticks > outFile.LastWriteTime.Ticks || styleFile.LastWriteTime.Ticks > outFile.LastWriteTime.Ticks || projectFile.LastWriteTime.Ticks > outFile.LastWriteTime.Ticks)) {
		    EnsureDirectoryFor(outFile);

		    //-- command line status
		    Log.WriteLine(LogPrefix + "Input:  " + inFile);

		    // Build the Anakia Document
		    AnakiaXmlDocument root = new AnakiaXmlDocument();
		    root.Load(inFile.FullName);

		    // Shove things into the Context
		    VelocityContext context = new VelocityContext();

		    /*
		    *  get the property TEMPLATE_ENCODING
		    *  we know it's a string...
		    */
		    String encoding = (String) ve.GetProperty(RuntimeConstants_Fields.OUTPUT_ENCODING);
		    if (encoding == null || encoding.Length == 0 || encoding.Equals("8859-1") || encoding.Equals("8859_1")) {
			encoding = "ISO-8859-1";
		    }

		    context.Put("root", root.DocumentElement);
		    context.Put("relativePath", getRelativePath(xmlFile));
		    context.Put("escape", new Escape());
		    context.Put("date", System.DateTime.Now);

		    // only put this into the context if it exists.
		    if (projectDocument != null) {
			context.Put("project", projectDocument.DocumentElement);
		    }

		    // Process the VSL template with the context and write out
		    // the result as the outFile.
		    writer = new System.IO.StreamWriter(new System.IO.FileStream(outFile.FullName, System.IO.FileMode.Create));
		    // get the template to process

		    Template template = ve.GetTemplate(styleFile.Name)
		    ;
		    template.Merge(context, writer);

		    Log.WriteLine(LogPrefix + "Output: " + outFile);
		}
	    } catch (System.Exception e) {
		Log.WriteLine(LogPrefix + "Failed to process " + inFile);
		if (outFile != null) {
		    bool tmpBool2;
		    if (System.IO.File.Exists(outFile.FullName)) {
			System.IO.File.Delete(outFile.FullName);
			tmpBool2 = true;
		    } else if (System.IO.Directory.Exists(outFile.FullName)) {
			System.IO.Directory.Delete(outFile.FullName);
			tmpBool2 = true;
		    } else
			tmpBool2 = false;
		    bool generatedAux3 = tmpBool2;
		}
		SupportClass.WriteStackTrace(e, Console.Error);
	    } finally {
		if (writer != null) {
		    try {
			writer.Flush();
			writer.Close();
		    } catch (System.Exception e) {
			// closing down, just ignore
		    }
		}
	    }
	}

	/// <summary>
	/// Hacky method to figure out the relative path
	/// that we are currently in. This is good for getting
	/// the relative path for images and anchor's.
	/// </summary>
	private String getRelativePath(String file) {
	    if (file == null || file.Length == 0)
		return "";
	    SupportClass.Tokenizer st = new SupportClass.Tokenizer(file, "/\\");
	    // needs to be -1 cause ST returns 1 even if there are no matches. huh?
	    int slashCount = st.Count - 1;
	    System.Text.StringBuilder sb = new System.Text.StringBuilder();
	    for (int i = 0; i < slashCount; i++) {
		sb.Append("../");
	    }

	    if (sb.ToString().Length > 0) {
		return StringUtils.chop(sb.ToString(), 1);
	    } else {
		return ".";
	    }
	}

	/// <summary>
	/// create directories as needed
	/// </summary>
	private void EnsureDirectoryFor(FileInfo targetFile) {
	    DirectoryInfo directory = targetFile.Directory;

	    if (!directory.Exists) {
		try {
		    Directory.CreateDirectory(directory.FullName);
		    Log.WriteLine(LogPrefix + "created output directory: " + directory.FullName);
		} catch (System.Exception ex) {
		    throw new BuildException("Unable to create directory: " + directory.FullName);
		}
	    }
	}


    }
}
