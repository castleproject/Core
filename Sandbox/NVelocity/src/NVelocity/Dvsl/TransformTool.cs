using System;

namespace NVelocity.Dvsl {

    /// <summary>
    /// This is the tool interface exposed to the stylesheet.
    /// </summary>
    /// <author> <a href="mailto:geirm@apache.org">Geir Magnusson Jr.</a></author>
    public interface TransformTool {

	/// <summary>
	/// Applies templates in the current stylesheet
	/// to the nodeset returned by the XPath expression
	/// </summary>
	/// <param name="xpath">XPath expression to select nodes</param>
	/// <returns>The rendered result</returns>
	String ApplyTemplates(String xpath);

	String ApplyTemplates(DvslNode node);

	String ApplyTemplates(DvslNode node, String xpath);

	String ApplyTemplates();

	String Copy();

	Object Get(String key);

	Object GetAppValue(Object key);

	Object PutAppValue(Object key, Object value);

    }
}
