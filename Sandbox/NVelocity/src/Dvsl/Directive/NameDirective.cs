using System;
using System.IO;

using NVelocity.Context;
using NVelocity.Runtime.Directive;
using NVelocity.Runtime.Parser.Node;
using NVelocity.Runtime.Parser;

namespace NVelocity.Dvsl.Directive {

    /// <summary>
    /// To implement the functionality of &lt;xsl:template name=&gt;
    /// </summary>
    /// <author> <a href="mailto:geirm@apache.org">Geir Magnusson Jr.</a></author>
    public class NameDirective : NVelocity.Runtime.Directive.Directive {

	public override String Name {
	    get {
		return "name";
	    }
	    set {
		throw new NotSupportedException();
	    }
	}


	public override Int32 Type {
	    get {
		return DirectiveConstants_Fields.BLOCK;
	    }
	}


	public override Boolean render(InternalContextAdapter context, TextWriter writer, INode node) {
	    return true;
	}


    }
}
