using System;
using System.Text;

namespace Commons.Collections {

    /// <summary>
    /// This class divides into tokens a property value.  Token
    /// separator is "," but commas into the property value are escaped
    /// using the backslash in front.
    /// </summary>
    internal class PropertiesTokenizer : StringTokenizer {
	/// <summary>
	/// The property delimiter used while parsing (a comma).
	/// </summary>
	internal const String DELIMITER = ",";

	/// <summary>
	/// Constructor.
	/// </summary>
	/// <param name="string">A String</param>
	public PropertiesTokenizer(String string_Renamed):base(string_Renamed, DELIMITER) {}

	/// <summary> Check whether the object has more tokens.
	/// </summary>
	/// <returns>True if the object has more tokens.
	/// </returns>
	public bool HasMoreTokens() {
	    return base.HasMoreTokens();
	}

	/// <summary>
	/// Get next token.
	/// </summary>
	/// <returns>A String</returns>
	public String NextToken() {
	    StringBuilder buffer = new StringBuilder();

	    while (HasMoreTokens()) {
		String token = base.NextToken();
		if (token.EndsWith("\\")) {
		    buffer.Append(token.Substring(0, (token.Length - 1) - (0)));
		    buffer.Append(DELIMITER);
		} else {
		    buffer.Append(token);
		    break;
		}
	    }

	    return buffer.ToString().Trim();
	}

    }
}
