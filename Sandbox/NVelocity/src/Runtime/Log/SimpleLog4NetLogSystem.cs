using System;
using System.Collections;

using log4net;
using log4net.Appender;
using log4net.Layout;
using log4net.Repository;
using log4net.Repository.Hierarchy;

namespace NVelocity.Runtime.Log {

    /// <summary>
    /// Implementation of a simple log4net system that will either
    /// latch onto an existing category, or just do a simple
    /// rolling file log.  Derived from Jon's 'complicated'
    /// version :)
    /// </summary>
    /// <author> <a href="mailto:geirm@apache.org">Geir Magnusson Jr.</a></author>
    public class SimpleLog4NetLogSystem : LogSystem {
	private RuntimeServices rsvc = null;

	/// <summary>
	/// log4net logging interface
	/// </summary>
	protected log4net.spi.ILogger logger = null;
	private Boolean isInitialized = false;
	private ArrayList cache = new ArrayList();

	public SimpleLog4NetLogSystem() {}

	public virtual void Init(RuntimeServices rs) {
	    rsvc = rs;

	    // first see if there is a category specified and just use that - it allows
	    // the application to make us use an existing logger
	    String categoryname = (String)rsvc.getProperty("runtime.log.logsystem.log4net.category");
	    if (categoryname != null) {
		logger = log4net.LogManager.GetLogger(categoryname).Logger;
		isInitialized = true;
		LogVelocityMessage(0, "SimpleLog4NetLogSystem using category '" + categoryname + "'");
		return ;
	    }

	    // if not, use the file...
	    String logfile = rsvc.getString(NVelocity.Runtime.RuntimeConstants_Fields.RUNTIME_LOG);

	    // now init.  If we can't, panic!
	    try {
		internalInit(logfile);

		LogVelocityMessage(0, "SimpleLog4NetLogSystem initialized using logfile '" + logfile + "'");
		isInitialized = true;

		foreach(LogMessage lm in cache) {
		    LogVelocityMessage(lm.Level, lm.Message);
		}
		cache = new ArrayList();
	    } catch (System.Exception e) {
		System.Console.Out.WriteLine("PANIC : error configuring SimpleLog4NetLogSystem : " + e);
		isInitialized = false;
	    }
	}

	/// <summary>
	/// initializes the log system using the logfile argument
	/// </summary>
	private void  internalInit(String logfile) {
	    // create a new logging domain
	    ILoggerRepository domain = null;
	    try {
		domain = log4net.LogManager.GetLoggerRepository(System.Reflection.Assembly.GetExecutingAssembly().FullName);
	    } catch (log4net.spi.LogException) {
		// Do nothing, this is expected if the domain has not been created
	    }

	    // create and initialize the domain if it does not exist
	    if (domain == null) {
		domain = log4net.LogManager.CreateDomain(System.Reflection.Assembly.GetExecutingAssembly().FullName);

		// create an appender for the domain
		RollingFileAppender appender = new RollingFileAppender();
		appender.Layout = new PatternLayout("%d - %m%n");
		appender.File = logfile;
		appender.AppendToFile = true;
		appender.MaxSizeRollBackups = 1;
		appender.MaximumFileSize = "100000";
		appender.ActivateOptions();

		// initialize/configure the domain
		log4net.Config.BasicConfigurator.Configure(domain, appender);
	    }

	    // create the category
	    Object o = domain.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

	    if (o is log4net.spi.ILogger) {
		this.logger = (log4net.spi.ILogger)domain.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
	    } else {
		throw new System.Exception("got unexpected logger type: " + o.GetType().ToString());
	    }
	}

	/// <summary>
	/// logs messages
	/// </summary>
	/// <param name="level">severity level</param>
	/// <param name="message">complete error message</param>
	public virtual void LogVelocityMessage(int level, String message) {
	    if (isInitialized) {
		switch (level) {
		    case NVelocity.Runtime.Log.LogSystem_Fields.WARN_ID:
			logger.Log("NVelocity", log4net.spi.Level.WARN, message, null);
			break;

		    case NVelocity.Runtime.Log.LogSystem_Fields.INFO_ID:
			logger.Log("NVelocity", log4net.spi.Level.INFO, message, null);
			break;

		    case NVelocity.Runtime.Log.LogSystem_Fields.DEBUG_ID:
			logger.Log("NVelocity", log4net.spi.Level.DEBUG, message, null);
			break;

		    case NVelocity.Runtime.Log.LogSystem_Fields.ERROR_ID:
			logger.Log("NVelocity", log4net.spi.Level.ERROR, message, null);
			break;
		    default:
			logger.Log("NVelocity", log4net.spi.Level.DEBUG, message, null);
			break;
		}
	    } else {
		cache.Add(new LogMessage(level, message));
	    }
	}

	/// <summary>
	/// Also do a shutdown if the object is destroy()'d.
	/// </summary>
	~SimpleLog4NetLogSystem() {
	    Shutdown();
	}

	/// <summary>
	/// Close all destinations
	/// </summary>
	public virtual void Shutdown() {
	    if (logger != null) {
		logger.Repository.Shutdown();
	    }
	}

	private class LogMessage {
	    private Int32 level;
	    private String message = String.Empty;

	    public LogMessage(int level, String message) {
		this.level = level;
		this.message = message;
	    }

	    public Int32 Level {
		get { return level; }
	    }

	    public String Message {
		get { return message; }
	    }
	}

    }
}
