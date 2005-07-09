using System;
using System.Collections;

namespace NVelocity.Runtime.Log {

    /// <summary>
    /// <p>
    /// This class is responsible for instantiating the correct LoggingSystem
    /// </p>
    /// <p>
    /// The approach is :
    /// </p>
    /// <ul>
    /// <li>
    /// First try to see if the user is passing in a living object
    /// that is a LogSystem, allowing the app to give is living
    /// custom loggers.
    /// </li>
    /// <li>
    /// Next, run through the (possible) list of classes specified
    /// specified as loggers, taking the first one that appears to
    /// work.  This is how we support finding either log4j or
    /// logkit, whichever is in the classpath, as both are
    /// listed as defaults.
    /// </li>
    /// <li>
    /// Finally, we turn to 'faith-based' logging, and hope that
    /// logkit is in the classpath, and try for an AvalonLogSystem
    /// as a final gasp.  After that, there is nothing we can do.
    /// </li>
    /// </summary>
    /// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a></author>
    /// <author> <a href="mailto:jon@latchkey.com">Jon S. Stevens</a></author>
    /// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a></author>
    public class LogManager {

	/// <summary>  Creates a new logging system or returns an existing one
	/// specified by the application.
	/// </summary>
	public static LogSystem createLogSystem(RuntimeServices rsvc) {
	    // if a logSystem was set as a configuation value, use that.
	    // This is any class the user specifies.
	    System.Object o = rsvc.getProperty(NVelocity.Runtime.RuntimeConstants_Fields.RUNTIME_LOG_LOGSYSTEM);

	    if (o != null && o is LogSystem) {
		((LogSystem) o).Init(rsvc);

		return (LogSystem) o;
	    }

	    // otherwise, see if a class was specified.  You
	    // can put multiple classes, and we use the first one we find.
	    //
	    // Note that the default value of this property contains both the
	    // AvalonLogSystem and the SimpleLog4JLogSystem for convenience -
	    // so we use whichever we find.
	    IList classes = new ArrayList();
	    System.Object obj = rsvc.getProperty(NVelocity.Runtime.RuntimeConstants_Fields.RUNTIME_LOG_LOGSYSTEM_CLASS);

	    // we might have a list, or not - so check
	    if (obj is IList) {
		classes = (IList) obj;
	    } else if (obj is System.String) {
		classes.Add(obj);
	    }

	    // now run through the list, trying each.  It's ok to
	    // fail with a class not found, as we do this to also
	    // search out a default simple file logger
	    foreach(String clazz in classes) {
		if (clazz != null && clazz.Length > 0) {
		    rsvc.info("Trying to use logger class " + clazz);

		    try {
			System.Type type = System.Type.GetType(clazz);
			o = System.Activator.CreateInstance(type);

			if (o is LogSystem) {
			    ((LogSystem) o).Init(rsvc);

			    rsvc.info("Using logger class " + clazz);

			    return (LogSystem) o;
			} else {
			    rsvc.error("The specifid logger class " + clazz + " isn't a valid LogSystem");
			}
		    } catch (System.ApplicationException ncdfe) {
			rsvc.debug("Couldn't find class " + clazz + " or necessary supporting classes in classpath. Exception : " + ncdfe);
		    }
		}
	    }

	    // if the above failed, then we are in deep doo-doo, as the
	    // above means that either the user specified a logging class
	    // that we can't find, there weren't the necessary
	    // dependencies in the classpath for it, or there were no
	    // dependencies for the default logger.
	    // Since we really don't know,
	    // then take a wack at the log4net as a last resort.
	    LogSystem als = null;
	    try {
		als = new SimpleLog4NetLogSystem();
		als.Init(rsvc);
	    } catch (System.ApplicationException ncdfe) {
		System.String errstr = "PANIC : NVelocity cannot find any of the" + " specified or default logging systems in the classpath," + " or the classpath doesn't contain the necessary classes" + " to support them." + " Please consult the documentation regarding logging." + " Exception : " + ncdfe;

		System.Console.Out.WriteLine(errstr);
		System.Console.Error.WriteLine(errstr);

		throw;
	    }

	    rsvc.info("Using log4net as logger of final resort.");

	    return als;
	}

    }
}
