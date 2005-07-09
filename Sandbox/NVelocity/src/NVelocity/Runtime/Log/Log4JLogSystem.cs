namespace org.apache.velocity.runtime.log
{
    /*
    * The Apache Software License, Version 1.1
    *
    * Copyright (c) 2001 The Apache Software Foundation.  All rights
    * reserved.
    *
    * Redistribution and use in source and binary forms, with or without
    * modification, are permitted provided that the following conditions
    * are met:
    *
    * 1. Redistributions of source code must retain the above copyright
    *    notice, this list of conditions and the following disclaimer.
    *
    * 2. Redistributions in binary form must reproduce the above copyright
    *    notice, this list of conditions and the following disclaimer in
    *    the documentation and/or other materials provided with the
    *    distribution.
    *
    * 3. The end-user documentation included with the redistribution, if
    *    any, must include the following acknowlegement:
    *       "This product includes software developed by the
    *        Apache Software Foundation (http://www.apache.org/)."
    *    Alternately, this acknowlegement may appear in the software itself,
    *    if and wherever such third-party acknowlegements normally appear.
    *
    * 4. The names "The Jakarta Project", "Velocity", and "Apache Software
    *    Foundation" must not be used to endorse or promote products derived
    *    from this software without prior written permission. For written
    *    permission, please contact apache@apache.org.
    *
    * 5. Products derived from this software may not be called "Apache"
    *    nor may "Apache" appear in their names without prior written
    *    permission of the Apache Group.
    *
    * THIS SOFTWARE IS PROVIDED ``AS IS'' AND ANY EXPRESSED OR IMPLIED
    * WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
    * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
    * DISCLAIMED.  IN NO EVENT SHALL THE APACHE SOFTWARE FOUNDATION OR
    * ITS CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
    * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
    * LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF
    * USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
    * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
    * OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
    * OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
    * SUCH DAMAGE.
    * ====================================================================
    *
    * This software consists of voluntary contributions made by many
    * individuals on behalf of the Apache Software Foundation.  For more
    * information on the Apache Software Foundation, please see
    * <http://www.apache.org/>.
    */
    using System;

    /// <summary> Implementation of a Log4J logger.
    /// *
    /// </summary>
    /// <author> <a href="mailto:jon@latchkey.com">Jon S. Stevens</a>
    /// </author>
    /// <version> $Id: Log4JLogSystem.cs,v 1.2 2003/10/27 13:54:10 corts Exp $
    /// *
    /// </version>
    /// <deprecated>As of v1.3.  Use
    /// {@link SimpleLog4jLogSystem}
    ///
    /// </deprecated>
    public class Log4JLogSystem : LogSystem {
	private RuntimeServices rsvc = null;

	/// <summary>log4java logging interface
	/// </summary>
	protected internal Category logger = null;

	/// <summary>logging layout
	/// </summary>
	protected internal Layout layout = null;

	/// <summary>the runtime.log property value
	/// </summary>
	private System.String logfile = "";

	/// <summary>  default CTOR.  Initializes itself using the property RUNTIME_LOG
	/// from the Velocity properties
	/// </summary>
	public Log4JLogSystem() {}

	public virtual void  init(RuntimeServices rs) {
	    rsvc = rs;

	    /*
	    *  since this is a Velocity-provided logger, we will
	    *  use the Runtime configuration
	    */
	    logfile = rsvc.getString(RuntimeConstants.RUNTIME_LOG);

	    /*
	    *  now init.  If we can't, panic!
	    */
	    try {
		internalInit();

		logVelocityMessage(0, "Log4JLogSystem initialized using logfile " + logfile);
	    } catch (System.Exception e) {
		System.Console.Out.WriteLine("PANIC : error configuring Log4JLogSystem : " + e);
	    }
	}

	/// <summary>  initializes the log system using the logfile argument
	/// *
	/// </summary>
	/// <param name="logFile">  file for log messages
	///
	/// </param>
	private void  internalInit() {
	    logger = Category.getInstance("");
	    logger.Additivity = false;

	    /*
	    * Priority is set for DEBUG becouse this implementation checks 
	    * log level.
	    */
	    logger.Priority = Priority.DEBUG;

	    System.String pattern = rsvc.getString(RuntimeConstants.LOGSYSTEM_LOG4J_PATTERN);

	    if (pattern == null || pattern.Length == 0) {
		pattern = "%d - %m%n";
	    }

	    layout = new PatternLayout(pattern);

	    configureFile();
	    configureRemote();
	    configureSyslog();
	    configureEmail();
	}

	/// <summary> Configures the logging to a file.
	/// </summary>
	private void  configureFile() {
	    int backupFiles = rsvc.getInt(RuntimeConstants.LOGSYSTEM_LOG4J_FILE_BACKUPS, 1);
	    int fileSize = rsvc.getInt(RuntimeConstants.LOGSYSTEM_LOG4J_FILE_SIZE, 100000);

	    Appender appender = new RollingFileAppender(layout, logfile, true);

	    ((RollingFileAppender) appender).MaxBackupIndex = backupFiles;

	    /* finding file size */
	    if (fileSize > - 1) {
		((RollingFileAppender) appender).MaximumFileSize = fileSize;
	    }
	    logger.addAppender(appender);
	}

	/// <summary> Configures the logging to a remote server
	/// </summary>
	private void  configureRemote() {
	    System.String remoteHost = rsvc.getString(RuntimeConstants.LOGSYSTEM_LOG4J_REMOTE_HOST);
	    int remotePort = rsvc.getInt(RuntimeConstants.LOGSYSTEM_LOG4J_REMOTE_PORT, 1099);

	    if (remoteHost == null || remoteHost.Trim().Equals("") || remotePort <= 0) {
		return ;
	    }

	    Appender appender = new SocketAppender(remoteHost, remotePort);

	    logger.addAppender(appender);
	}

	/// <summary> Configures the logging to syslogd
	/// </summary>
	private void  configureSyslog() {
	    System.String syslogHost = rsvc.getString(RuntimeConstants.LOGSYSTEM_LOG4J_SYSLOGD_HOST);
	    System.String syslogFacility = rsvc.getString(RuntimeConstants.LOGSYSTEM_LOG4J_SYSLOGD_FACILITY);

	    if (syslogHost == null || syslogHost.Trim().Equals("") || syslogFacility == null) {
		return ;
	    }

	    Appender appender = new SyslogAppender();

	    ((SyslogAppender) appender).Layout = layout;
	    ((SyslogAppender) appender).SyslogHost = syslogHost;
	    ((SyslogAppender) appender).Facility = syslogFacility;

	    logger.addAppender(appender);
	}

	/// <summary> Configures the logging to email
	/// </summary>
	private void  configureEmail() {
	    System.String smtpHost = rsvc.getString(RuntimeConstants.LOGSYSTEM_LOG4J_EMAIL_SERVER);
	    System.String emailFrom = rsvc.getString(RuntimeConstants.LOGSYSTEM_LOG4J_EMAIL_FROM);
	    System.String emailTo = rsvc.getString(RuntimeConstants.LOGSYSTEM_LOG4J_EMAIL_TO);
	    System.String emailSubject = rsvc.getString(RuntimeConstants.LOGSYSTEM_LOG4J_EMAIL_SUBJECT);
	    System.String bufferSize = rsvc.getString(RuntimeConstants.LOGSYSTEM_LOG4J_EMAIL_BUFFER_SIZE);

	    if (smtpHost == null || smtpHost.Trim().Equals("") || emailFrom == null || smtpHost.Trim().Equals("") || emailTo == null || emailTo.Trim().Equals("") || emailSubject == null || emailSubject.Trim().Equals("") || bufferSize == null || bufferSize.Trim().Equals("")) {
		return ;
	    }

	    SMTPAppender appender = new SMTPAppender();

	    appender.SMTPHost = smtpHost;
	    appender.From = emailFrom;
	    appender.To = emailTo;
	    appender.Subject = emailSubject;

	    appender.BufferSize = System.Int32.Parse(bufferSize);

	    appender.Layout = layout;
	    appender.activateOptions();
	    logger.addAppender(appender);
	}

	/// <summary>  logs messages
	/// *
	/// </summary>
	/// <param name="level">severity level
	/// </param>
	/// <param name="message">complete error message
	///
	/// </param>
	public virtual void  logVelocityMessage(int level, System.String message) {
	    switch (level) {
		case org.apache.velocity.runtime.log.LogSystem_Fields.WARN_ID:
		    logger.warn(message);
		    break;

		case org.apache.velocity.runtime.log.LogSystem_Fields.INFO_ID:
		    logger.info(message);
		    break;

		case org.apache.velocity.runtime.log.LogSystem_Fields.DEBUG_ID:
		    logger.debug(message);
		    break;

		case org.apache.velocity.runtime.log.LogSystem_Fields.ERROR_ID:
		    logger.error(message);
		    break;

		default:
		    logger.debug(message);
		    break;

	    }
	}

	/// <summary> Also do a shutdown if the object is destroy()'d.
	/// </summary>
	~Log4JLogSystem() {
	    shutdown();
	}

	/// <summary>Close all destinations
	/// </summary>
	public virtual void  shutdown() {
	    System.Collections.IEnumerator appenders = logger.AllAppenders;
	    //UPGRADE_TODO: method 'java.util.Enumeration.hasMoreElements' was converted to ' ' which has a different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1073_javautilEnumerationhasMoreElements"'
	    while (appenders.MoveNext()) {
		//UPGRADE_TODO: method 'java.util.Enumeration.nextElement' was converted to ' ' which has a different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1073_javautilEnumerationnextElement"'
		Appender appender = (Appender) appenders.Current;
		appender.close();
	    }
	}
    }
}
