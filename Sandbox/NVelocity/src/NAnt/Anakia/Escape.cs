using System;
using System.Text;

namespace NVelocity.NAnt.Anakia {

    /// <summary>
    /// This class is for escaping CDATA sections. The code was
    /// "borrowed" from the JDOM code. I also added in escaping
    /// of the " -> &amp;quot; character.
    /// </summary>
    /// <author><a href="mailto:jon@latchkey.com">Jon S. Stevens</a></author>
    public class Escape {

	/// <summary>
	/// Empty constructor
	/// </summary>
	public Escape() {
	    // left blank on purpose
	}

	/// <summary>
	/// Do the escaping.
	/// </summary>
	public static String GetText(String st) {
	    StringBuilder buff = new StringBuilder();
	    char[] block = st.ToCharArray();
	    String stEntity = null;
	    int i, last;

	    for (i = 0, last = 0; i < block.Length; i++) {
		switch (block[i]) {
		    case '<':
			stEntity = "&lt;";
			break;

		    case '>':
			stEntity = "&gt;";
			break;

		    case '&':
			stEntity = "&amp;";
			break;

		    case '"':
			stEntity = "&quot;";
			break;

		    default:
			/* no-op */
			;
			break;

		}
		if (stEntity != null) {
		    buff.Append(block, last, i - last);
		    buff.Append(stEntity);
		    stEntity = null;
		    last = i + 1;
		}
	    }
	    if (last < block.Length) {
		buff.Append(block, last, i - last);
	    }
	    return buff.ToString();
	}


    }
}
