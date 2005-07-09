using System;
using ParseException = NVelocity.Runtime.Parser.ParseException;
using SimpleNode = NVelocity.Runtime.Parser.Node.SimpleNode;
using ResourceLoader = NVelocity.Runtime.Resource.Loader.ResourceLoader;
using ResourceNotFoundException = NVelocity.Exception.ResourceNotFoundException;
using ParseErrorException = NVelocity.Exception.ParseErrorException;


namespace NVelocity.Runtime.Resource {

    /// <summary>
    /// This class represent a general text resource that
    /// may have been retrieved from any number of possible
    /// sources.
    /// </summary>
    /// <author><a href="mailto:jvanzyl@apache.org">Jason van Zyl</a></author>
    /// <author><a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a></author>
    /// <version> $Id: Resource.cs,v 1.5 2004/01/02 00:13:51 corts Exp $</version>
    public abstract class Resource {

	/// <summary>
	/// The number of milliseconds in a minute, used to calculate the
	/// check interval.
	/// </summary>
	protected internal const long MILLIS_PER_SECOND = 1000;

	/// <summary>
	/// Resource might require ancillary storage of some kind
	/// </summary>
	protected internal System.Object data = null;

	/// <summary>
	/// Character encoding of this resource
	/// </summary>
	protected internal System.String encoding = NVelocity.Runtime.RuntimeConstants_Fields.ENCODING_DEFAULT;

	/// <summary>
	/// The file modification time (in milliseconds) for the cached template.
	/// </summary>
	protected internal long lastModified = 0;

	/// <summary>
	/// How often the file modification time is checked (in milliseconds).
	/// </summary>
	protected internal long modificationCheckInterval = 0;

	/// <summary>
	/// Name of the resource
	/// </summary>
	protected internal System.String name;

	/// <summary>
	/// The next time the file modification time will be checked (in milliseconds).
	/// </summary>
	protected internal long nextCheck = 0;

	/// <summary>
	/// The template loader that initially loaded the input
	/// stream for this template, and knows how to check the
	/// source of the input stream for modification.
	/// </summary>
	protected internal ResourceLoader resourceLoader;

	protected internal RuntimeServices rsvc = null;

	
	public virtual bool IsSourceModified() {
	    return resourceLoader.isSourceModified(this);
	}

	/// <summary> Perform any subsequent processing that might need
	/// to be done by a resource. In the case of a template
	/// the actual parsing of the input stream needs to be
	/// performed.
	/// </summary>
	/// <returns>Whether the resource could be processed successfully.
	/// For a {@link org.apache.velocity.Template} or {@link
	/// org.apache.velocity.runtime.resource.ContentResource}, this
	/// indicates whether the resource could be read.
	/// @exception ResourceNotFoundException Similar in semantics as
	/// returning <code>false</code>.
	/// </returns>
	public abstract bool Process();

	/// <summary> Set the modification check interval.
	/// </summary>
	/// <param name="interval">The interval (in seconds).
	///
	/// </param>

	/// <summary> Is it time to check to see if the resource
	/// source has been updated?
	/// </summary>
	public virtual bool RequiresChecking() {
	    /*
	    *  short circuit this if modificationCheckInterval == 0
	    *  as this means "don't check"
	    */

	    if (modificationCheckInterval <= 0) {
		return false;
	    }

	    /*
	    *  see if we need to check now
	    */

	    return ((System.DateTime.Now.Ticks - 621355968000000000) / 10000 >= nextCheck);
	}

	/// <summary>
	/// 'Touch' this template and thereby resetting the nextCheck field.
	/// </summary>
	public virtual void  Touch() {
	    nextCheck = (System.DateTime.Now.Ticks - 621355968000000000) / 10000 + (MILLIS_PER_SECOND * modificationCheckInterval);
	}

	/// <summary>
	/// Set arbitrary data object that might be used
	/// by the resource.
	///
	/// Get arbitrary data object that might be used
	/// by the resource.
	/// </summary>
	public virtual System.Object Data {
	    get {
		return data;
	    }
	    set {
		this.data = value;
	    }
	}

	/// <summary>
	/// set the encoding of this resource
	/// for example, "ISO-8859-1"
	/// 
	/// get the encoding of this resource
	/// for example, "ISO-8859-1"
	/// </summary>
	public virtual System.String Encoding {
	    get {
		return encoding;
	    }
	    set {
		this.encoding = value;
	    }
	}

	/// <summary>
	/// Return the lastModifed time of this
	/// template.
	/// 
	/// Set the last modified time for this
	/// template.
	/// </summary>
	public virtual long LastModified {
	    get {
		return lastModified;
	    }
	    set {
		this.lastModified = value;
	    }
	}

	public virtual long ModificationCheckInterval {
	    set {
		this.modificationCheckInterval = value;
	    }
	}

	/// <summary>
	/// Set the name of this resource, for example test.vm.
	/// 
	/// Get the name of this template.
	/// </summary>
	public virtual System.String Name {
	    get {
		return name;
	    }
	    set {
		this.name = value;
	    }
	}

	/// <summary>
	/// Return the template loader that pulled
	/// in the template stream
	/// 
	/// Set the template loader for this template. Set
	/// when the Runtime determines where this template
	/// came from the list of possible sources.
	/// </summary>
	public virtual ResourceLoader ResourceLoader {
	    get {
		return resourceLoader;
	    }
	    set {
		this.resourceLoader = value;
	    }
	}

	public virtual RuntimeServices RuntimeServices {
	    set {
		rsvc = value;
	    }
	}

    }
}
