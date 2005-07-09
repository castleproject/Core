using System;

namespace NVelocity.Test.Provider {

    /// <summary>
    /// Rudimentary class used in the testbed to test
    /// introspection with subclasses of a particular
    /// class.
    ///
    /// This class need to be greatly extended to
    /// be useful :-)
    /// </summary>
    /// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a></author>
    /// <version> $Id: Person.cs,v 1.3 2003/10/27 13:54:11 corts Exp $</version>
    public class Person {

	public virtual System.String Name {
	    get { return "Person"; }
	}


    }
}
