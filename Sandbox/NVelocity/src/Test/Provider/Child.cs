using System;

namespace NVelocity.Test.Provider {

    /// <summary>
    /// Rudimentary class used in the testbed to test
    /// introspection with subclasses of a particular
    /// class.
    /// </summary>
    /// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a> </author>
    /// <version> $Id: Child.cs,v 1.3 2003/10/27 13:54:11 corts Exp $ </version>
    public class Child:Person {

	public override System.String Name {
	    get { return "Child"; }
	}

    }
}
