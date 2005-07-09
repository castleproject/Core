using System;
using System.IO;

using Commons.Collections;
using NVelocity.Runtime.Resource.Loader;

namespace NVelocity.NAnt.Texen {

    /// <summary>
    /// A property utility class for the texen text/code generator
    /// Usually this class is only used from a Velocity context.
    /// </summary>
    /// <author> <a href="mailto:leon@opticode.co.za">Leon Messerschmidt</a></author>
    /// <author> <a href="mailto:sbailliez@apache.org">Stephane Bailliez</a></author>
    public class PropertiesUtil {

	/// <summary>
	/// Load properties from either a file in the templatePath if there
	/// is one or the classPath.
	/// </summary>
	/// <param name="propertiesFile">the properties file to load through
	/// either the templatePath or the classpath.
	/// </param>
	/// <returns>a properties instance filled with the properties found
	/// in the file or an empty instance if no file was found.
	/// </returns>
	public virtual ExtendedProperties load(System.String propertiesFile) {
	    ExtendedProperties properties = new ExtendedProperties();
	    System.String templatePath = Generator.Instance.TemplatePath;
	    if (templatePath != null) {
		properties = loadFromTemplatePath(propertiesFile);
	    } else {
		properties = loadFromClassPath(propertiesFile);
	    }

	    return properties;
	}

	/// <summary>
	/// Load a properties file from the templatePath defined in the
	/// generator. As the templatePath can contains multiple paths,
	/// it will cycle through them to find the file. The first file
	/// that can be successfully loaded is considered. (kind of
	/// like the java classpath), it is done to clone the Velocity
	/// process of loading templates.
	/// </summary>
	/// <param name="propertiesFile">the properties file to load. It must be
	/// a relative pathname.
	/// </param>
	/// <returns>a properties instance loaded with the properties from
	/// the file. If no file can be found it returns an empty instance.
	/// </returns>
	protected internal virtual ExtendedProperties loadFromTemplatePath(System.String propertiesFile) {
	    ExtendedProperties properties = new ExtendedProperties();
	    System.String templatePath = Generator.Instance.TemplatePath;

	    // We might have something like the following:
	    //
	    // #set ($dbprops = $properties.load("$generator.templatePath/path/props")
	    //
	    // as we have in Torque but we want people to start using
	    //
	    // #set ($dbprops = $properties.load("path/props")
	    //
	    // so that everything works from the filesystem or from
	    // a JAR. So the actual Generator.getTemplatePath()
	    // is not deprecated but it's use in templates
	    // should be.
	    SupportClass.Tokenizer st = new SupportClass.Tokenizer(templatePath, ",");
	    while (st.HasMoreTokens()) {
		System.String templateDir = st.NextToken();
		try {
		    // If the properties file is being pulled from the
		    // file system and someone is using the method whereby
		    // the properties file is assumed to be in the template
		    // path and they are simply using:
		    //
		    // #set ($dbprops = $properties.load("props") (1)
		    //
		    // than we have to tack on the templatePath in order
		    // for the properties file to be found. We want (1)
		    // to work whether the generation is being run from
		    // the file system or from a JAR file.
		    System.String fullPath = propertiesFile;

		    // FIXME probably not that clever since there could be
		    // a mix of file separators and the test will fail :-(
		    if (!fullPath.StartsWith(templateDir)) {
			fullPath = templateDir + "\\" + propertiesFile;
		    }

		    properties.Load(new System.IO.FileStream(fullPath, System.IO.FileMode.Open, System.IO.FileAccess.Read));
		    // first pick wins, we don't need to go further since
		    // we found a valid file.
		    break;
		} catch (System.Exception) {
		    // do nothing
		}
	    }
	    return properties;
	}

	/// <summary>
	/// Load a properties file from the classpath
	/// </summary>
	/// <param name="propertiesFile">the properties file to load.</param>
	/// <returns>a properties instance loaded with the properties from
	/// the file. If no file can be found it returns an empty instance.
	/// </returns>
	protected internal virtual ExtendedProperties loadFromClassPath(String propertiesFile) {
	    ExtendedProperties properties = new ExtendedProperties();

	    try {
		// This is a hack for now to make sure that properties
		// files referenced in the filesystem work in
		// a JAR file. We have to deprecate the use
		// of $generator.templatePath in templates first
		// and this hack will allow those same templates
		// that use $generator.templatePath to work in
		// JAR files.
		if (propertiesFile.StartsWith("$generator")) {
		    propertiesFile = propertiesFile.Substring("$generator.templatePath/".Length);
		}

		ResourceLocator rl = new ResourceLocator(propertiesFile);
		Stream inputStream = rl.OpenRead();
		properties.Load(inputStream);
	    } catch (System.IO.IOException) {
		// do nothing
	    }
	    return properties;
	}
    }
}
