using System;
using System.IO;

namespace NVelocity.NAnt.Texen {

    /// <summary>
    /// A general file utility for use in the context
    /// </summary>
    /// <author><a href="mailto:leon@opticode.co.za">Leon Messerschmidt</a></author>
    /// <author><a href="mailto:jvanzyl@apache.org">Jason van Zyl</a></author>
    public class FileUtil {

	/// <summary>
	/// Creates the directory s (and any parent directories needed).
	/// </summary>
	/// <param name="String">path/directory to create.</param>
	/// <param name="String">report of path/directory creation.</param>
	public static String mkdir(String s) {
	    try {
		Directory.CreateDirectory((new FileInfo(s)).FullName);
		return "Created dir: " + s;
	    } catch (System.Exception e) {
		return "Failed to create dir: " + e.ToString();
	    }
	}

	/// <summary>
	/// A method to get a File object.
	/// </summary>
	/// <param name="String">path to file object to create.</param>
	/// <returns>File created file object.</returns>
	public static FileInfo file(String s) {
	    FileInfo f = new FileInfo(s);
	    return f;
	}

	/// <summary> A method to get a File object.
	/// *
	/// </summary>
	/// <param name="String">base path
	/// </param>
	/// <param name="String">file name
	/// </param>
	/// <returns>File created file object.
	///
	/// </returns>
	public static FileInfo file(String base_Renamed, String s) {
	    FileInfo f = new FileInfo(base_Renamed + "\\" + s);
	    return f;
	}
    }
}
