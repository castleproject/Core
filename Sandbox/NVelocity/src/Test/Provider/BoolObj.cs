using System;

namespace NVelocity.Test.Provider {

    /// <summary>
    /// simple class to test boolean property
    /// introspection - can't use TestProvider
    /// as there is a get( String )
    /// and that comes before isProperty
    /// in the search pattern
    /// </summary>
    /// <author> <a href="mailto:geirm@apache.org">Geir Magnusson Jr.</a></author>
    public class BoolObj {

	public Boolean isBoolean {
	    get { return true; }
	}

	/*
	*  not isProperty as it's not
	*  boolean return valued...
	*/
	public String isNotboolean {
	    get { return "hello"; }
	}


    }
}
