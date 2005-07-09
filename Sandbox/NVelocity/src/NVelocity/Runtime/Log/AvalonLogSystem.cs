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
    using StringUtils = org.apache.velocity.util.StringUtils;

    /// <summary> Implementation of a Avalon logger.
    /// *
    /// </summary>
    /// <author> <a href="mailto:jon@latchkey.com">Jon S. Stevens</a>
    /// </author>
    /// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <version> $Id: AvalonLogSystem.cs,v 1.2 2003/10/27 13:54:10 corts Exp $
    ///
    /// </version>
    public class AvalonLogSystem : LogSystem {
	private Logger logger = null;

	private RuntimeServices rsvc = null;

	/// <summary>  default CTOR.  Initializes itself using the property RUNTIME_LOG
	/// from the Velocity properties
	/// </summary>

	public AvalonLogSystem() {}

	public virtual void  init(RuntimeServices rs) {
	    this.rsvc = rs;

	    /*
	    *  if a logger is specified, we will use this instead of
	    *  the default
	    */
	    System.String loggerName = (System.String) rsvc.getProperty("runtime.log.logsystem.avalon.logger");

	    if (loggerName != null) {
		this.logger = Hierarchy.DefaultHierarchy.getLoggerFor(loggerName);
	    } else {
		/*
		*  since this is a Velocity-provided logger, we will
		*  use the Runtime configuration
		*/
		System.String logfile = (System.String) rsvc.getProperty(org.apache.velocity.runtime.RuntimeConstants_Fields.RUNTIME_LOG);

		/*
		*  now init.  If we can't, panic!
		*/
		try {
		    init(logfile);

		    logVelocityMessage(0, "AvalonLogSystem initialized using logfile '" + logfile + "'");
		} catch (System.Exception e) {
		    System.Console.Out.WriteLine("PANIC : Error configuring AvalonLogSystem : " + e);
		    System.Console.Error.WriteLine("PANIC : Error configuring AvalonLogSystem : " + e);

		    throw new System.Exception("Unable to configure AvalonLogSystem : " + e);
		}
	    }
	}

	/// <summary>  initializes the log system using the logfile argument
	/// *
	/// </summary>
	/// <param name="logFile">  file for log messages
	///
	/// </param>
	public virtual void  init(System.String logFile) {

	    /*
	    *  make our FileTarget.  Note we are going to keep the 
	    *  default behavior of not appending...
	    */
	    FileTarget target = new FileTarget(new System.IO.FileInfo(logFile), false, new VelocityFormatter("%{time} %{message}\\n%{throwable}"));

	    /*
	    *  use the toString() of RuntimeServices to make a unique logger
	    */

	    //UPGRADE_TODO: The equivalent in .NET for method 'java.Object.toString' may return a different value. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1043"'
	    logger = Hierarchy.DefaultHierarchy.getLoggerFor(rsvc.ToString());
	    logger.Priority = Priority.DEBUG;
	    logger.LogTargets = new LogTarget[]{target};
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
	    /*
	    *  based on level, call teh right logger method
	    *  and prefix with the appropos prefix
	    */

	    switch (level) {
		case org.apache.velocity.runtime.log.LogSystem_Fields.WARN_ID:
		    logger.warn(org.apache.velocity.runtime.RuntimeConstants_Fields.WARN_PREFIX + message);
		    break;

		case org.apache.velocity.runtime.log.LogSystem_Fields.INFO_ID:
		    logger.info(org.apache.velocity.runtime.RuntimeConstants_Fields.INFO_PREFIX + message);
		    break;

		case org.apache.velocity.runtime.log.LogSystem_Fields.DEBUG_ID:
		    logger.debug(org.apache.velocity.runtime.RuntimeConstants_Fields.DEBUG_PREFIX + message);
		    break;

		case org.apache.velocity.runtime.log.LogSystem_Fields.ERROR_ID:
		    logger.error(org.apache.velocity.runtime.RuntimeConstants_Fields.ERROR_PREFIX + message);
		    break;

		default:
		    logger.info(message);
		    break;

	    }
	}
    }
}
