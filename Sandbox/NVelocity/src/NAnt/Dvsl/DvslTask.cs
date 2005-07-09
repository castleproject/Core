using System;
using System.Collections;
using System.IO;

using SourceForge.NAnt;
using SourceForge.NAnt.Attributes;

using Commons.Collections;
using NVelocity.Dvsl;

namespace NVelocity.NAnt.Dvsl {

    [TaskName("dvsl")]
    public class DvslTask : Task {

	/**
	 *  Supported app values
	 */
	public static String INFILENAME = "infilename";
	public static String OUTFILENAME = "outfilename";

	private NVelocity.Dvsl.Dvsl dvsl;

	private DirectoryInfo destDir = null;
	private FileInfo stylesheet = null;
	private FileInfo inFile = null;
	private FileInfo outFile = null;
	private FileInfo logFile = null;
	private String targetExtension = ".html";
	private String outputEncoding = "UTF-8";
	private bool force = false;
	private ArrayList toolAttr = new ArrayList();
	private FileInfo toolboxFile = null;
	private ExtendedProperties toolboxProps = null;
	private bool validatingParser = false;

	FileSet fileset = new FileSet();

	/// <summary>
	/// Sets the file to use for stylesheet.
	/// </summary>
	[TaskAttribute("style", Required=true)]
	public virtual String Style {
	    set {
		this.stylesheet = new FileInfo(value);
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
	/// Set whether to check dependencies, or always generate.
	/// </summary>
	[TaskAttribute("force", Required=false)]
	public bool Force {
	    set { this.force = force; }
	}

	/// <summary>
	/// Set the destination directory where the generated files should be directed.
	/// </summary>
	[TaskAttribute("destdir", Required=false)]
	public String Destdir {
	    set { destDir = new DirectoryInfo(value); }
	}

	/// <summary>
	/// Set the desired file extension to be used for the target files.
	/// If not specified, &quot;<code>.html</code>&quot; is used.
	/// </summary>
	[TaskAttribute("extension", Required=false)]
	public String Extension {
	    set { targetExtension = value; }
	}

	/// <summary>
	/// Sets the file to use for logging.  If not specified, all logging
	/// is directed through NAnt's logging system.
	/// </summary>
	[TaskAttribute("logfile", Required=false)]
	public String LogFile {
	    set { this.logFile = new FileInfo(value); }
	}

	/// <summary>
	/// Sets the Toolbox properties file to use.
	/// </summary>
	[TaskAttribute("toolboxfile", Required=false)]
	public String ToolboxFile {
	    set { this.toolboxFile = new FileInfo(value); }
	}

	/// <summary>
	/// Sets an output file
	/// </summary>
	[TaskAttribute("out", Required=false)]
	public String Out {
	    set { this.outFile = new FileInfo(value); }
	}

	/// <summary>
	/// Sets an input xml file to be styled
	/// </summary>
	[TaskAttribute("in", Required=false)]
	public String In {
	    set { this.inFile = new FileInfo(value); }
	}

	/// <summary>
	/// Sets the character encoding for output files.  If not specified,
	/// output is written with UTF-8 encodin6g.
	/// </summary>
	[TaskAttribute("outputencoding", Required=false)]
	public String OutputEncoding {
	    set { this.outputEncoding = value; }
	}

	/// <summary>
	/// Sets the flag to have DVSL use a validating parser for the
	/// input documents
	/// </summary>
	[TaskAttribute("validatingparser", Required=false)]
	public bool ValidatingParser {
	    set {
		if (value == true) {
		    Log.WriteLine(LogPrefix + "Parser is validating.");
		}
		validatingParser = value;
	    }
	}

	/// <summary>
	/// Sets an application value from outside of the DVSL task
	/// </summary>
	/// <param name="name"></param>
	/// <param name="o"></param>
	public void PutAppValue(String name, Object o) {
	    dvsl.PutAppValue(name,o);
	}


	protected override void ExecuteTask() {
	    if (stylesheet == null) {
		throw new BuildException("no stylesheet specified", Location);
	    }

	    /*
	     * make a DVSL and set validation
	     */
	    dvsl = new NVelocity.Dvsl.Dvsl();
	    dvsl.ValidatingParser = validatingParser;

	    /*
	     * If a logfile attribute was specified, use that for the log file name,
	     * TODO: otherwise use a NVelocity to NAnt logging adapter.
	     */
	    if (logFile != null) {
		dvsl.LogFile = logFile;
	    } else {
		//dvsl.setLogSystem(new AntLogSystem(this));
	    }

	    /*
	     * now the stylesheet
	     */
	    try {
		Log.WriteLine(LogPrefix + "Loading stylesheet " + stylesheet.FullName);
		dvsl.SetStylesheet(stylesheet);
	    } catch (System.Exception ex) {
		Log.WriteLine(LogPrefix + "Failed to read stylesheet " + stylesheet.FullName);
		throw new BuildException(ex.ToString());
	    }

	    /*
	     *  now, if we were given a toolbox, set that up too
	     */
	    toolboxProps = new ExtendedProperties();

	    try {
		if (toolboxFile != null) {
		    toolboxProps.Load(toolboxFile.OpenRead());
		}

		/*
		 *  Overlay any parameters
		 */
		//		for (Enumeration e = toolAttr.elements(); e.hasMoreElements();) {
		//		    Tool p = (Tool)e.nextElement();
		//		    toolboxProps.setProperty(p.getName(), p.getValue());
		//		}

		dvsl.Toolbox = toolboxProps;
	    } catch(System.Exception ee) {
		throw new BuildException("Error loading the toolbox : " + ee);
	    }

	    /*
	     * if we have an in file and out then process them
	     */

	    if (inFile != null && outFile != null) {
		Process(inFile, outFile, stylesheet);
		return;
	    }

	    /*
	     * if we get here, in and out have not been specified, we are
	     * in batch processing mode.
	     */

	    /*
	     *   make sure Source directory exists...
	     */
	    if (destDir == null) {
		throw new BuildException("destdir attributes must be set!");
	    }

	    Log.WriteLine(LogPrefix + "Transforming into "+destDir);

	    /*
	     *  Process all the files marked for styling
	     */
	    // get the base directory from the fileset - needed to colapse ../ syntax
	    DirectoryInfo di = new DirectoryInfo(fileset.BaseDirectory);

	    // get a list of files to work on
	    foreach (string filename in fileset.FileNames) {
		String relativeFilename = filename.Substring(di.FullName.Length+1);
		FileInfo file = new FileInfo(filename);
		if (file.Exists) {
		    Process(di, file, destDir, stylesheet);
		}
	    }
	}

	protected void xExecuteTask() {
	    int fileCount = fileset.FileNames.Count;
	    Log.WriteLine(LogPrefix + "processing {0} files", fileCount);

	    foreach (string filename in fileset.FileNames) {
		FileInfo file = new FileInfo(filename);
		if (file.Exists) {
		    String outfile = filename.Substring(0,filename.Length-file.Extension.Length) + ".html";
		    dvsl = new NVelocity.Dvsl.Dvsl();

		    if (stylesheet == null) {
			System.Console.Out.WriteLine("usage :need to specify a stylesheet. ");
			System.Console.Out.WriteLine("Dvsl.exe -STYLE stylesheeet [-IN infile] [-OUT outfile] [-TOOL toolboxname]");
			return;
		    } else {
			dvsl.SetStylesheet(stylesheet.FullName);
		    }

		    if (toolboxFile != null) {
			ExtendedProperties p = new ExtendedProperties();
			Stream fis = new FileStream(toolboxFile.FullName, FileMode.Open, FileAccess.Read);
			p.Load(fis);
			fis.Close();
			dvsl.Toolbox = p;
		    }

		    TextReader reader = new StreamReader(filename);
		    TextWriter writer = new StreamWriter(outfile);

		    long time = dvsl.Transform(reader, writer);
		    writer.Flush();
		    reader.Close();
		    writer.Close();
		}
	    }
	}


	/// <summary>
	/// Processes the given input XML file and stores the result in the given resultFile.
	/// </summary>
	/// <param name="baseDir"></param>
	/// <param name="xmlFile"></param>
	/// <param name="destDir"></param>
	/// <param name="stylesheet"></param>
	private void Process(DirectoryInfo baseDir, FileInfo xmlFile, DirectoryInfo destDir, FileInfo stylesheet) {
	    String relativeFilename = xmlFile.FullName.Substring(baseDir.FullName.Length+1);
	    String fileExt=targetExtension;
	    FileInfo outFile=null;
	    FileInfo inFile=null;

	    try {
		inFile = xmlFile;
		int dotPos = relativeFilename.LastIndexOf('.');
		dvsl.PutAppValue(INFILENAME, relativeFilename);

		String outfilename;
		if (dotPos > 0) {
		    outfilename = relativeFilename.Substring(0, dotPos) + fileExt;
		} else {
		    outfilename = relativeFilename + fileExt;
		}

		outFile = new FileInfo(destDir + Path.DirectorySeparatorChar.ToString() + outfilename);
		dvsl.PutAppValue(OUTFILENAME, outfilename);

		if (force ||
			inFile.LastWriteTime.Ticks > outFile.LastWriteTime.Ticks ||
			stylesheet.LastWriteTime.Ticks > outFile.LastWriteTime.Ticks) {

		    EnsureDirectoryFor(outFile);
		    Log.WriteLine(LogPrefix + "Processing "+inFile+" to "+outFile);
		    long time = Transform(inFile, outFile);
		    Log.WriteLine(LogPrefix + "Processed "+inFile+" in "+time+" ms.");
		}
	    } catch (System.Exception ex) {
		/*
		 * If failed to process document, must delete target document,
		 * or it will not attempt to process it the second time
		 */
		Log.WriteLine(LogPrefix + "Failed to process " + inFile);
		if (outFile != null) {
		    outFile.Delete();
		}
		throw new BuildException(ex.ToString());
	    }
	}

	private void Process(FileInfo inFile, FileInfo outFile, FileInfo stylesheet) {
	    try {
		if (force ||
			inFile.LastWriteTime.Ticks > outFile.LastWriteTime.Ticks ||
			stylesheet.LastWriteTime.Ticks > outFile.LastWriteTime.Ticks) {

		    EnsureDirectoryFor(outFile);
		    Log.WriteLine(LogPrefix + "Processing "+inFile+" to "+outFile);
		    long time = Transform(inFile, outFile);
		    Log.WriteLine(LogPrefix + "Processed "+inFile+" in "+time+" ms.");
		}
	    } catch (System.Exception ex) {
		/*
		 * If failed to process document, must delete target document,
		 * or it will not attempt to process it the second time
		 */
		Log.WriteLine(LogPrefix + "Failed to process " + inFile);
		if (outFile != null) {
		    outFile.Delete();
		}
		throw new BuildException(ex.ToString());
	    }
	}

	/// <summary>
	/// Does the actual transform
	/// </summary>
	/// <param name="inFile"></param>
	/// <param name="outFile"></param>
	/// <returns></returns>
	private long Transform(FileInfo inFile, FileInfo outFile) {
	    TextWriter writer = new StreamWriter(outFile.FullName);
	    long time = dvsl.Transform(inFile, writer);
	    writer.Close();
	    return time;
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
