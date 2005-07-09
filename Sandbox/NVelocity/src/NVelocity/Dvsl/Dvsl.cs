using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;

using System.Xml;
using System.Xml.Schema;

using Commons.Collections;

using NVelocity.App;
using NVelocity.Context;
using NVelocity.Runtime;
using NVelocity.Runtime.Log;

namespace NVelocity.Dvsl {

    /// <summary>
    /// Main DVSL class - use this as the helper class for apps
    /// </summary>
    /// <author> <a href="mailto:geirm@apache.org">Geir Magnusson Jr.</a></author>
    /// <author> <a href="mailto:billb@progress.com">Bill Burton.</a></author>
    public class Dvsl {

	private static String TOOL_PROP_PREFIX = "toolbox.tool.";
	private static String STRING_PROP_PREFIX = "toolbox.string.";
	private static String INTEGER_PROP_PREFIX = "toolbox.integer.";
	private static String TOOLBOX_NAME = "toolbox.contextname.";

	private VelocityEngine ve = null;
	private XmlDocument currentDocument = null;
	private StreamWriter currentWriter = null;
	private IContext toolContext;
	private IContext userContext;
	private IContext styleContext;
	private DvslContext baseContext = new DvslContext();
	private Transformer transformer;

	private bool ready = false;
	private Hashtable velConfig = null;
	private FileInfo logFile;
	private LogSystem logger;
	private Hashtable appVals = new Hashtable();
	private TemplateHandler templateHandler = new TemplateHandler();
	internal bool validate = false;


	/// <summary>
	/// lets the user specify a filename for logging.
	/// </summary>
	public virtual FileInfo LogFile {
	    set {
		this.logFile = value;

		if (velConfig == null) {
		    velConfig = new Hashtable();
		}

		velConfig[RuntimeConstants_Fields.RUNTIME_LOG] = value.FullName;
	    }
	}

	/// <summary>
	/// lets the user specify a class instance for logging.
	/// </summary>
	public virtual LogSystem LogSystem {
	    set {
		this.logger = value;

		if (velConfig == null) {
		    velConfig = new Hashtable();
		}

		velConfig[RuntimeConstants_Fields.RUNTIME_LOG_LOGSYSTEM] = value;
	    }
	}

	/// <summary>
	/// lets the user pass a java.util.Properties containing
	/// properties for the configuration of the VelocityEngine
	/// used by DVSL
	/// </summary>
	public virtual Hashtable VelocityConfig {
	    set {
		if (velConfig != null) {
		    foreach(Object key in velConfig.Keys) {
			value.Add(key, velConfig[key]);
		    }
		}

		velConfig = value;
	    }
	}

	/// <summary>
	/// Sets the user context.  The user context is
	/// a Velocity Context containing user-supplied
	/// objects and data that are to be made available
	/// in the template
	/// </summary>
	/// <param name="ctx">User context of data</param>
	public virtual IContext UserContext {
	    set {
		ready = false;
		userContext = value;
	    }
	}

	/// <summary>
	/// Uses a validating parser on all input documents
	/// </summary>
	/// <param name="">validate</param>
	public virtual bool ValidatingParser {
	    set {
		validate = value;
	    }
	}

	/// <summary>
	/// <p>Loads the toolbox from the input Properties.</p>
	/// <p>Currently supports specification of the Toolbox
	/// name in the context, creating classes, and string
	/// and integer values.  Ex :
	/// </p>
	///
	/// <pre>
	/// toolbox.contextname = floyd
	/// toolbox.tool.footool = Footool
	/// toolbox.string.mystring = Hello there!
	/// toolbox.integer.myint = 7
	/// toolbox.string.sourcebase = ./xdocs/
	/// </pre>
	///
	/// <p>
	/// So in template, this toolbox and it's values would
	/// be accessed as :
	/// </p>
	/// <pre>
	/// $context.floyd.footool.getFoo()
	/// $context.floyd.mystring
	/// $context.floyd.myint
	/// </pre>
	/// </summary>
	public virtual ExtendedProperties Toolbox {
	    set {
		ready = false;

		/*
		*  for each key that looks like
		*     toolbox.tool.<token> = class
		*/

		Hashtable toolbox = new Hashtable();
		String toolboxname = "toolbox";

		IEnumerator it = value.Keys;
		while (it.MoveNext()) {
		    String key = it.Current.ToString();
		    String val = value.GetString(key);

		    if (key.StartsWith(TOOL_PROP_PREFIX)) {
			String toolname = key.Substring(TOOL_PROP_PREFIX.Length);

			Type type = Type.GetType(val);
			Object o = Activator.CreateInstance(type);
			toolbox[toolname] = o;
		    } else if (key.StartsWith(INTEGER_PROP_PREFIX)) {
			String toolname = key.Substring(INTEGER_PROP_PREFIX.Length);

			int i = 0;

			try {
			    i = Int32.Parse(val);
			} catch (System.Exception ee) {}

			toolbox[toolname] = i;
		    } else if (key.StartsWith(STRING_PROP_PREFIX)) {
			String toolname = key.Substring(STRING_PROP_PREFIX.Length);
			toolbox[toolname] = val;
		    } else if (key.StartsWith(TOOLBOX_NAME)) {
			toolboxname = val;
		    }
		}

		toolContext = new VelocityContext();
		toolContext.Put(toolboxname, toolbox);
	    }

	}

	/// <summary>
	/// Convenience function.  See...
	/// </summary>
	public virtual void SetStylesheet(String value) {
	    SetStylesheet(new FileInfo(value));
	}

	/// <summary>
	/// Convenience function.  See...
	/// </summary>
	public virtual void SetStylesheet(FileInfo value) {
	    StreamReader fr = null;

	    try {
		fr = new StreamReader(value.FullName);
		SetStylesheet(fr);
	    } catch (System.Exception e) {
		throw;
	    } finally {
		if (fr != null) {
		    fr.Close();
		}
	    }
	}


	/// <summary>
	/// <p>
	/// Sets the stylesheet for this transformation set
	/// </p>
	///
	/// <p>
	/// Note that don't need this for each document you want
	/// to transform.  Just do it once, and transform away...
	/// </p>
	/// </summary>
	/// <param name="styleReader">Reader with stylesheet char stream</param>
	public virtual void SetStylesheet(TextReader value) {
	    ready = false;
	    /*
	    *  now initialize Velocity - we need to do that
	    *  on change of stylesheet
	    */
	    ve = new VelocityEngine();

	    /*
	    * if there are user properties, set those first - carefully
	    */

	    if (velConfig != null) {
		ConfigureVelocityEngine(ve, velConfig);
	    }

	    /*
	    *  register our template() directive
	    */

	    ve.SetProperty("userdirective", @"NVelocity.Dvsl.Directive.MatchDirective\,NVelocity");
	    ve.Init();

	    /*
	    *  add our template accumulator
	    */

	    ve.SetApplicationAttribute("NVelocity.Dvsl.TemplateHandler", templateHandler);

	    /*
	    *  load and render the stylesheet
	    *
	    *  this sets stylesheet specific context
	    *  values
	    */

	    StringWriter junkWriter = new StringWriter();

	    styleContext = new VelocityContext();
	    ve.Evaluate(styleContext, junkWriter, "DVSL:stylesheet", value);

	    /*
	    *  now run the base template through for the rules
	    */

	    // TODO - use ResourceLocator or something else - I don't like the path to the resource
	    Stream s = this.GetType().Assembly.GetManifestResourceStream("NVelocity.Dvsl.Resource.defaultroot.dvsl");

	    if (s == null) {
		System.Console.Out.WriteLine("DEFAULT TRANSFORM RULES NOT FOUND ");
	    } else {
		ve.Evaluate(new VelocityContext(), junkWriter, "defaultroot.dvsl", new StreamReader(s));
		s.Close();
	    }

	    /*
	    *  need a new transformer, as it depends on the
	    *  velocity engine
	    */

	    transformer = new Transformer(ve, templateHandler, baseContext, appVals, validate);
	}

	/// <summary>
	/// <p>
	/// Add mapped properties from hashtable on VelocityEngine.
	/// </p>
	/// <p>
	/// If you are going to use this, ensure you do it *before* setting
	/// the stylesheet, as that creates the VelocityEngine
	/// </p>
	/// </summary>
	private void ConfigureVelocityEngine(VelocityEngine ve, Hashtable map) {
	    if (ve == null || map == null) {
		return ;
	    }

	    foreach (DictionaryEntry entry in map) {
		ve.SetProperty((String)entry.Key, entry.Value);
	    }
	}

	/// <summary>
	/// sets up all the context goodies
	/// </summary>
	protected internal virtual void MakeReady() {
	    /*
	    *  put all the contexts together
	    */
	    baseContext.ClearContexts();

	    baseContext.AddContext(userContext);
	    baseContext.AddContext(toolContext);
	    baseContext.StyleContext = styleContext;

	    ready = true;
	}

	/// <summary>
	/// does the transformation of the inputstream into
	/// the output writer
	/// </summary>
	protected internal virtual long XForm(TextReader reader, TextWriter writer) {
	    if (!ready) {
		MakeReady();
	    }

	    return transformer.Transform(reader, writer);
	}

	protected internal virtual long XForm(XmlDocument dom4jdoc, TextWriter writer) {
	    if (!ready) {
		MakeReady();
	    }

	    return transformer.Transform(dom4jdoc, writer);
	}

	public virtual long Transform(FileInfo f, TextWriter writer) {
	    StreamReader reader = null;

	    try {
		reader = new StreamReader(f.FullName);
		return XForm(reader, writer);
	    } catch (System.Exception e) {
		throw e;
	    } finally {
		if (reader != null) {
		    reader.Close();
		}
	    }
	}

	public virtual long Transform(TextReader reader, TextWriter writer) {
	    return XForm(reader, writer);
	}

	public virtual long Transform(Stream stream, TextWriter writer) {
	    return XForm(new StreamReader(stream), writer);
	}

	/// <summary>
	/// Transforms the given dom4j Document into the writer.
	/// </summary>
	/// <param name="dom4jdoc">dom4j Document object</param>
	/// <param name="writer">Writer for output</param>
	public virtual long Transform(XmlDocument dom4jdoc, TextWriter writer) {
	    return XForm(dom4jdoc, writer);
	}

	public virtual long Transform(String infile, TextWriter writer) {
	    StreamReader reader = null;

	    try {
		reader = new StreamReader(infile);
		return XForm(reader, writer);
	    } catch (System.Exception e) {
		throw e;
	    } finally {
		if (reader != null) {
		    reader.Close();
		}
	    }
	}


	/// <summary>
	/// Gets the application value for the specified key
	/// </summary>
	/// <param name="key">key to use to retrieve value</param>
	/// <returns>value if found, null otherwise</returns>
	public virtual Object GetAppValue(Object key) {
	    return appVals[key];
	}

	/// <summary>
	/// Sets the application value for the specified key
	/// </summary>
	/// <param name="key">key to use to store value</param>
	/// <param name="value">value to be stored</param>
	/// <returns>old value if any, null otherwise</returns>
	public virtual Object PutAppValue(Object key, Object value) {
	    return appVals[key] = value;
	}

	/// <summary>
	/// <p>
	/// Allows command-line access.
	/// </p>
	/// <p>
	/// Usage :  Dvsl.exe -STYLE stylesheeet [-IN infile] [-OUT outfile] [-TOOL toolboxname]
	/// </p>
	/// </summary>
	[STAThread]
	public static void  Main(String[] args) {
	    Dvsl dvsl = new Dvsl();

	    TextReader reader = System.Console.In;
	    String infile = null;
	    String style = null;
	    String outfile = null;
	    TextWriter writer = System.Console.Out;
	    String toolfile = null;

	    for (int i = 0; i < args.Length; i++) {
		if (args[i].Equals("-IN"))
		    infile = args[++i];
		else if (args[i].Equals("-OUT"))
		    outfile = args[++i];
		else if (args[i].Equals("-STYLE"))
		    style = args[++i];
		else if (args[i].Equals("-TOOL"))
		    toolfile = args[++i];
	    }

	    if (style == null) {
		System.Console.Out.WriteLine("usage :need to specify a stylesheet. ");
		System.Console.Out.WriteLine("Dvsl.exe -STYLE stylesheeet [-IN infile] [-OUT outfile] [-TOOL toolboxname]");
		return;
	    }

	    if (style != null)
		dvsl.SetStylesheet(style);

	    if (toolfile != null) {
		ExtendedProperties p = new ExtendedProperties();
		Stream fis = new FileStream(toolfile, FileMode.Open, FileAccess.Read);
		p.Load(fis);

		dvsl.Toolbox = p;
	    }

	    if (infile != null)
		reader = new StreamReader(infile);

	    if (outfile != null)
		writer = new StreamWriter(outfile);

	    long time = dvsl.Transform(reader, writer);
	    writer.Flush();
	}


    }
}
