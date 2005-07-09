using System;

namespace NVelocity.Tool {


    /// <summary> Interface to simplify and abstract tool handling.
    /// *
    /// Implementations of this class should hold both the context
    /// key for the tool and sufficient information to return
    /// an instance of the tool.
    /// *
    /// </summary>
    /// <author> <a href="mailto:nathan@esha.com">Nathan Bubna</a>
    /// *
    /// </author>
    /// <version> $Id: IToolInfo.cs,v 1.2 2003/10/27 13:54:12 corts Exp $
    ///
    /// </version>
    public interface IToolInfo {
	System.String Key
	{
	    get
		;

	    }
	    System.String Classname
	    {
		get
		    ;

		}
		/// <returns>the context key for the tool
		///
		/// </returns>
		/// <returns>the fully qualified classname for the tool
		///
		/// </returns>
		/// <summary> Returns an instance of the tool.
		/// *
		/// Instances returned may be new on each call, pooled, or
		/// the be same instance every time depending on the
		/// implementation.  The object passed to this method may
		/// be used to initialize or create the tool that is returned,
		/// or it may be null if no such data is required.
		/// *
		/// </summary>
		/// <param name="initData">an object that may be used to initialize the instance
		/// </param>
		/// <returns>an instance of the tool
		///
		/// </returns>
		System.Object getInstance(System.Object initData);
    }
}
