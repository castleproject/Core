using System;

namespace NVelocity.Tool {


    /// <summary> ToolInfo implementation to handle "primitive" data types.
    /// It currently supports String, Number, and Boolean data.
    /// *
    /// </summary>
    /// <author> <a href="mailto:nathan@esha.com">Nathan Bubna</a>
    /// *
    /// </author>
    /// <version> $Id: DataInfo.cs,v 1.2 2003/10/27 13:54:12 corts Exp $
    ///
    /// </version>
    public class DataInfo : IToolInfo {
	public virtual System.String Key
	{
	    get {
		return key;
	    }

	}
	public virtual System.String Classname
	{
	    get {
		return data.GetType().FullName;
	    }

	}

	public static System.String TYPE_STRING = "string";
	public static System.String TYPE_NUMBER = "number";
	public static System.String TYPE_BOOLEAN = "boolean";

	private System.String key;
	private System.Object data;


	/// <summary> Parses the value string into a recognized type. If
	/// the type specified is not supported, the data will
	/// be held and returned as a string.
	/// *
	/// </summary>
	/// <param name="key">the context key for the data
	/// </param>
	/// <param name="type">the data type
	/// </param>
	/// <param name="value">the data
	///
	/// </param>
	public DataInfo(System.String key, System.String type, System.String value_Renamed) {
	    this.key = key;

	    if (type.ToUpper().Equals(TYPE_BOOLEAN.ToUpper())) {
		this.data = System.Boolean.Parse(value_Renamed);
	    } else if (type.ToUpper().Equals(TYPE_NUMBER.ToUpper())) {
		if (value_Renamed.IndexOf((System.Char) '.') >= 0) {
		    //UPGRADE_TODO: Format of parameters of constructor 'java.lang.Double.Double' are different in the equivalent in .NET. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1092"'
		    this.data = System.Double.Parse(value_Renamed);
		} else {
		    this.data = System.Int32.Parse(value_Renamed);
		}
	    } else {
		this.data = value_Renamed;
	    }
	}






	/// <summary> Returns the data. Always returns the same
	/// object since the data is a constant. Initialization
	/// data is ignored.
	/// </summary>
	public virtual System.Object getInstance(System.Object initData) {
	    return data;
	}
    }
}
