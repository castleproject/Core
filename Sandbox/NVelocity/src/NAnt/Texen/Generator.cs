using System;
using System.Collections;
using System.IO;

using Commons.Collections;
using NVelocity.App;
using NVelocity.Context;
using NVelocity.Runtime.Resource.Loader;

using SourceForge.NAnt;

namespace NVelocity.NAnt.Texen {

    /// <summary>
    /// A text/code generator class
    /// </summary>
    /// <author><a href="mailto:leon@opticode.co.za">Leon Messerschmidt</a></author>
    /// <author><a href="mailto:jvanzyl@apache.org">Jason van Zyl</a></author>
    public class Generator {

	/// <summary> Where the texen output will placed.
	/// </summary>
	public const System.String OUTPUT_PATH = "output.path";

	/// <summary> Where the velocity templates live.
	/// </summary>
	public const System.String TEMPLATE_PATH = "template.path";

	/// <summary> Default properties file used for controlling the
	/// tools placed in the context.
	/// </summary>
	private const System.String DEFAULT_TEXEN_PROPERTIES = "NVelocity\\NAnt\\NAnt\\Texen\\texen.properties";

	private String logPrefix = String.Empty;

	/// <summary>
	/// Default properties used by texen.
	/// </summary>
	private ExtendedProperties props = new ExtendedProperties();

	/// <summary> Context used for generating the texen output.
	/// </summary>
	private IContext controlContext;

	/// <summary> Keep track of the file writers used for outputting
	/// to files. If we come across a file writer more
	/// then once then the additional output will be
	/// appended to the file instead of overwritting
	/// the contents.
	/// </summary>
	//UPGRADE_NOTE: The initialization of  'writers' was moved to method 'InitBlock'. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1005"'
	private System.Collections.Hashtable writers = new System.Collections.Hashtable();

	/// <summary> The generator tools used for creating additional
	/// output withing the control template. This could
	/// use some cleaning up.
	/// </summary>
	private static Generator instance = new Generator();

	/// <summary> This is the encoding for the output file(s).
	/// </summary>
	protected internal System.String outputEncoding;

	/// <summary> This is the encoding for the input file(s)
	/// (templates).
	/// </summary>
	protected internal System.String inputEncoding;

	/// <summary> Velocity engine.
	/// </summary>
	protected internal VelocityEngine ve;

	/// <summary> Default constructor.
	/// </summary>
	public Generator() {
	    setDefaultProps();
	}

	/// <summary> Create a new generator object with default properties.
	/// *
	/// </summary>
	/// <returns>Generator generator used in the control context.
	///
	/// </returns>

	/// <summary> Set the velocity engine.
	/// </summary>

	/// <summary> Create a new generator object with properties loaded from
	/// a file.  If the file does not exist or any other exception
	/// occurs during the reading operation the default properties
	/// are used.
	/// *
	/// </summary>
	/// <param name="String">properties used to help populate the control context.
	/// </param>
	/// <returns>Generator generator used in the control context.
	///
	/// </returns>
	public Generator(System.String propFile) {
	    ResourceLocator rl = new ResourceLocator(propFile);
	    if (rl.Exists) {
		Stream s = rl.OpenRead();
		props.Load(rl.OpenRead());
		s.Close();
	    } else {
		setDefaultProps();
	    }
	}

	/// <summary> Create a new Generator object with a given property
	/// set. The property set will be duplicated.
	/// *
	/// </summary>
	/// <param name="Properties">properties object to help populate the control context.
	///
	/// </param>
	public Generator(ExtendedProperties props) {
	    this.props = (ExtendedProperties) props.Clone();
	}

	public String LogPrefix {
	    get { return this.logPrefix; }
	    set { this.logPrefix = value; }
	}

	/// <summary> Set default properties.
	/// </summary>
	protected internal virtual void  setDefaultProps() {
	    try {
		ResourceLocator rl = new ResourceLocator(DEFAULT_TEXEN_PROPERTIES);
		if (rl.Exists) {
		    Stream s = rl.OpenRead();
		    props.Load(rl.OpenRead());
		    s.Close();
		} else {
		    Log.WriteLine(LogPrefix + "PANIC: Could not locate default properties!");
		}
	    } catch (System.Exception) {
		Log.WriteLine(LogPrefix + "Cannot get default properties!");
	    }
	}

	/// <summary> Set the template path, where Texen will look
	/// for Velocity templates.
	/// *
	/// </summary>
	/// <param name="String">template path for velocity templates.
	///
	/// </param>

	/// <summary> Get the template path.
	/// *
	/// </summary>
	/// <returns>String template path for velocity templates.
	///
	/// </returns>

	/// <summary> Set the output path for the generated
	/// output.
	/// *
	/// </summary>
	/// <returns>String output path for texen output.
	///
	/// </returns>

	/// <summary> Get the output path for the generated
	/// output.
	/// *
	/// </summary>
	/// <returns>String output path for texen output.
	///
	/// </returns>

	/// <summary> Set the output encoding.
	/// </summary>

	/// <summary> Set the input (template) encoding.
	/// </summary>

	/// <summary> Returns a writer, based on encoding and path.
	/// *
	/// </summary>
	/// <param name="path">     path to the output file
	/// </param>
	/// <param name="encoding"> output encoding
	///
	/// </param>
	public virtual System.IO.StreamWriter getWriter(System.String path, System.String encoding) {
	    System.IO.StreamWriter writer;
	    if (encoding == null || encoding.Length == 0 || encoding.Equals("8859-1") || encoding.Equals("8859_1")) {
		writer = new System.IO.StreamWriter(path);
	    } else {
		//UPGRADE_ISSUE: Constructor 'java.io.BufferedWriter.BufferedWriter' was not converted. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1000_javaioBufferedWriterBufferedWriter_javaioWriter"'
		writer = new System.IO.StreamWriter(new System.IO.FileStream(path, System.IO.FileMode.Create));
	    }
	    return writer;
	}

	/// <summary> Returns a template, based on encoding and path.
	/// *
	/// </summary>
	/// <param name="templateName"> name of the template
	/// </param>
	/// <param name="encoding">     template encoding
	///
	/// </param>
	public virtual Template getTemplate(System.String templateName, System.String encoding) {
	    Template template;
	    if (encoding == null || encoding.Length == 0 || encoding.Equals("8859-1") || encoding.Equals("8859_1")) {
		template = ve.GetTemplate(templateName)
		;
	    }
	    else {
		template = ve.GetTemplate(templateName, encoding)
		;
	    }
	    return template;
	}

	/// <summary> Parse an input and write the output to an output file.  If the
	/// output file parameter is null or an empty string the result is
	/// returned as a string object.  Otherwise an empty string is returned.
	/// *
	/// </summary>
	/// <param name="String">input template
	/// </param>
	/// <param name="String">output file
	///
	/// </param>
	public virtual System.String parse(System.String inputTemplate, System.String outputFile) {
	    return parse(inputTemplate, outputFile, null, null);
	}

	/// <summary> Parse an input and write the output to an output file.  If the
	/// output file parameter is null or an empty string the result is
	/// returned as a string object.  Otherwise an empty string is returned.
	/// You can add objects to the context with the objs Hashtable.
	/// *
	/// </summary>
	/// <param name="String">input template
	/// </param>
	/// <param name="String">output file
	/// </param>
	/// <param name="String">id for object to be placed in the control context
	/// </param>
	/// <param name="String">object to be placed in the context
	/// </param>
	/// <returns>String generated output from velocity
	///
	/// </returns>
	public virtual System.String parse(System.String inputTemplate, System.String outputFile, System.String objectID, System.Object object_Renamed) {
	    return parse(inputTemplate, null, outputFile, null, objectID, object_Renamed);
	}
	/// <summary> Parse an input and write the output to an output file.  If the
	/// output file parameter is null or an empty string the result is
	/// returned as a string object.  Otherwise an empty string is returned.
	/// You can add objects to the context with the objs Hashtable.
	/// *
	/// </summary>
	/// <param name="String">input template
	/// </param>
	/// <param name="String">inputEncoding template encoding
	/// </param>
	/// <param name="String">output file
	/// </param>
	/// <param name="String">outputEncoding encoding of output file
	/// </param>
	/// <param name="String">id for object to be placed in the control context
	/// </param>
	/// <param name="String">object to be placed in the context
	/// </param>
	/// <returns>String generated output from velocity
	///
	/// </returns>
	public virtual System.String parse(System.String inputTemplate, System.String intputEncoding, System.String outputFile, System.String outputEncoding, System.String objectID, System.Object object_Renamed) {
	    if (objectID != null && object_Renamed != null) {
		controlContext.Put(objectID, object_Renamed);
	    }

	    Template template = getTemplate(inputTemplate, inputEncoding != null?inputEncoding:this.inputEncoding)
	    ;

	    if (outputFile == null || outputFile.Equals("")) {
		System.IO.StringWriter sw = new System.IO.StringWriter();
		template.Merge(controlContext, sw);
		return sw.ToString();
	    } else {
		System.IO.StreamWriter writer = null;

		if (writers[outputFile] == null) {
		    /*
		    * We have never seen this file before so create
		    * a new file writer for it.
		    */
		    writer = getWriter(OutputPath + System.IO.Path.DirectorySeparatorChar.ToString() + outputFile, outputEncoding != null?outputEncoding:this.outputEncoding);

		    /*
		    * Place the file writer in our collection
		    * of file writers.
		    */
		    SupportClass.PutElement(writers, outputFile, writer);
		} else {
		    writer = (System.IO.StreamWriter) writers[outputFile];
		}

		VelocityContext vc = new VelocityContext(controlContext);
		template.Merge(vc, writer);

		// commented because it is closed in shutdown();
		//fw.close();

		return "";
	    }
	}

	/// <summary> Parse the control template and merge it with the control
	/// context. This is the starting point in texen.
	/// *
	/// </summary>
	/// <param name="String">control template
	/// </param>
	/// <param name="Context">control context
	/// </param>
	/// <returns>String generated output
	///
	/// </returns>
	public virtual System.String parse(System.String controlTemplate, IContext controlContext) {
	    this.controlContext = controlContext;
	    fillContextDefaults(this.controlContext);
	    fillContextProperties(this.controlContext);

	    Template template = getTemplate(controlTemplate, inputEncoding)
	    ;
	    System.IO.StringWriter sw = new System.IO.StringWriter();
	    template.Merge(controlContext, sw);

	    return sw.ToString();
	}


	/// <summary> Create a new context and fill it with the elements of the
	/// objs Hashtable.  Default objects and objects that comes from
	/// the properties of this Generator object is also added.
	/// *
	/// </summary>
	/// <param name="Hashtable">objects to place in the control context
	/// </param>
	/// <returns>Context context filled with objects
	///
	/// </returns>
	protected internal virtual IContext getContext(System.Collections.Hashtable objs) {
	    fillContextHash(controlContext, objs);
	    return controlContext;
	}

	///
	/// <summary> Add all the contents of a Hashtable to the context.
	/// *
	/// </summary>
	/// <param name="Context">context to fill with objects
	/// </param>
	/// <param name="Hashtable">source of objects
	///
	/// </param>
	protected internal virtual void  fillContextHash(IContext context, System.Collections.Hashtable objs) {
	    System.Collections.IEnumerator enum_Renamed = (System.Collections.IEnumerator) objs.Keys;
	    //UPGRADE_TODO: method 'java.util.Enumeration.hasMoreElements' was converted to ' ' which has a different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1073_javautilEnumerationhasMoreElements"'
	    while (enum_Renamed.MoveNext()) {
		//UPGRADE_TODO: The equivalent in .NET for method 'java.Object.toString' may return a different value. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1043"'
		//UPGRADE_TODO: method 'java.util.Enumeration.nextElement' was converted to ' ' which has a different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1073_javautilEnumerationnextElement"'
		System.String key = enum_Renamed.Current.ToString();
		context.Put(key, objs[key]);
	    }
	}

	/// <summary> Add properties that will aways be in the context by default
	/// *
	/// </summary>
	/// <param name="Context">control context to fill with default values.
	///
	/// </param>
	protected internal virtual void  fillContextDefaults(IContext context) {
	    context.Put("generator", instance);
	    context.Put("outputDirectory", OutputPath);
	}

	/// <summary> Add objects to the context from the current properties.
	/// *
	/// </summary>
	/// <param name="Context">control context to fill with objects
	/// that are specified in the default.properties
	/// file
	///
	/// </param>
	protected internal virtual void  fillContextProperties(IContext context) {
	    IDictionaryEnumerator enum_Renamed = props.GetEnumerator();

	    while (enum_Renamed.MoveNext()) {
		DictionaryEntry de = (DictionaryEntry)enum_Renamed.Current;
		String nm = (String)de.Key;
		if (nm.StartsWith("context.objects.")) {

		    System.String contextObj = (System.String) props.GetString(nm);
		    int colon = nm.LastIndexOf((System.Char) '.');
		    System.String contextName = nm.Substring(colon + 1);

		    //try {
		    Log.WriteLine(LogPrefix + "loading " + contextObj + " as " + nm);
		    System.Type cls = System.Type.GetType(contextObj);
		    System.Object o = System.Activator.CreateInstance(cls);
		    context.Put(contextName, o);
		    //} catch (System.Exception e) {
		    //	SupportClass.WriteStackTrace(e, Console.Error);
		    //	//TO DO: Log Something Here
		    //}
		}
	    }
	}

	/// <summary> Properly shut down the generator, right now
	/// this is simply flushing and closing the file
	/// writers that we have been holding on to.
	/// </summary>
	public virtual void  shutdown() {
	    IEnumerator iterator = writers.Values.GetEnumerator();

	    while (iterator.MoveNext()) {
		StreamWriter writer = (StreamWriter) iterator.Current;
		try {
		    writer.Flush();
		    writer.Close();
		} catch (System.Exception) {
		    /* do nothing */
		}
	    }
	    // clear the file writers cache
	    writers.Clear();
	}


	public static Generator Instance {
	    get {
		return instance;
	    }

	}
	public virtual VelocityEngine VelocityEngine {
	    set {
		this.ve = value;
	    }

	}
	public virtual System.String TemplatePath {
	    get {
		return (System.String) props.GetString(TEMPLATE_PATH);
	    }

	    set {
		SupportClass.PutElement(props, TEMPLATE_PATH, value);
	    }

	}
	public virtual System.String OutputPath {
	    get {
		return (System.String) props.GetString(OUTPUT_PATH);
	    }

	    set {
		SupportClass.PutElement(props, OUTPUT_PATH, value);
	    }

	}
	public virtual System.String OutputEncoding {
	    set {
		this.outputEncoding = value;
	    }

	}
	public virtual System.String InputEncoding {
	    set {
		this.inputEncoding = value;
	    }

	}



    }
}
