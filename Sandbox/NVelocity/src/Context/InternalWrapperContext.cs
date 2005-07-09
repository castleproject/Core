using System;


namespace NVelocity.Context {

    /// <summary>
    /// interface for internal context wrapping functionality
    /// </summary>
    /// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a></author>
    /// <version> $Id: InternalWrapperContext.cs,v 1.4 2003/10/27 13:54:08 corts Exp $ </version>
    public interface InternalWrapperContext {
	/// <summary>
	/// returns the wrapped user context
	/// </summary>
	IContext InternalUserContext {
	    get
		;
	    }

	    /// <summary>
	    /// returns the base full context impl
	    /// </summary>
	    InternalContextAdapter BaseContext {
		get
		    ;
		}

	    }
}
