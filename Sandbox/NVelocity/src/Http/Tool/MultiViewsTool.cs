using System;
using System.Globalization;
using System.Web;

using NVelocity.App;
using NVelocity.Context;
using NVelocity.Http.Context;
using NVelocity.Http.Tool;

namespace NVelocity.Http.Tool {

    /// <summary>
    /// <p>Allows for transparent content negotiation in a manner mimicking
    /// Apache httpd's <a
    /// href="http://httpd.apache.org/docs-2.0/content-negotiation.html">MultiViews</a>.</p>
    /// <p>Reads the default language out of the ViewContext as
    /// <code>org.apache.velocity.tools.view.i18n.defaultLanguage</code>.
    /// See {@link #findLocalizedResource(String, String)} and {@link
    /// #findLocalizedResource(String, Locale)} for usage.</p>
    /// </summary>
    /// <author> <a href="mailto:dlr@finemaltcoding.com">Daniel Rall</a></author>
    public class MultiViewsTool : IViewTool {

	/// <summary>
	/// The key used to search initialization, context, and JVM
	/// parameters for the default language to use.
	/// </summary>
	protected internal const String DEFAULT_LANGUAGE_KEY = "NVvelocity.Http.Tools.view.i18n.defaultLanguage";

	/// <summary>
	/// The two character abbreviation for the request's default
	/// language.
	/// </summary>
	protected internal String defaultLanguage;

	/// <summary>
	/// Creates a new uninitialized instance.  Call {@link #init}
	/// to initialize it.
	/// </summary>
	public MultiViewsTool() {}

	/// <summary>
	/// Extracts the default language from the specified
	/// <code>ViewContext</code>, looking first at the Velocity
	/// context, then the servlet context, then lastly at the JVM
	/// default.  This "narrow scope to wide scope" pattern makes it
	/// easy to setup language overrides at different levels within
	/// your application.
	/// </summary>
	/// <param name="obj">the current ViewContext
	/// @throws IllegalArgumentException if the param is not a ViewContext
	/// </param>
	public virtual void Init(Object obj) {
	    if (!(obj is IViewContext)) {
		throw new ArgumentException("Tool can only be initialized with a ViewContext");
	    }

	    IViewContext context = (IViewContext) obj;
	    IContext vc = context.VelocityContext;
	    defaultLanguage = (String) vc.Get(DEFAULT_LANGUAGE_KEY);
	    if (defaultLanguage == null || defaultLanguage.Trim().Equals("")) {
		HttpContext sc = context.HttpContext;
		defaultLanguage = (String) sc.Application[DEFAULT_LANGUAGE_KEY];
		if (defaultLanguage == null || defaultLanguage.Trim().Equals("")) {
		    // Use default.
		    defaultLanguage = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
		}
	    }
	}

	/// <summary>
	/// Calls {@link #findLocalizedResource(String, String)} using the
	/// language extracted from <code>locale</code>.
	/// </summary>
	/// <seealso cref="#findLocalizedResource(String, String)"/>
	public virtual String findLocalizedResource(String name, CultureInfo locale) {
	    return findLocalizedResource(name, locale.TwoLetterISOLanguageName);
	}

	/// <summary>
	/// Calls {@link #findLocalizedResource(String, String)} using the
	/// default language.
	/// </summary>
	/// <seealso cref="#findLocalizedResource(String, String)"/>
	public virtual String findLocalizedResource(String name) {
	    return findLocalizedResource(defaultLanguage);
	}

	/// <summary>
	/// <p>Finds the a localized version of the requested Velocity
	/// resource (such as a file or template) which is most appropriate
	/// for the locale of the current request.  Use in conjuction with
	/// Apache httpd's <code>MultiViews</code>, or by itself.</p>
	///
	/// <p>Usage from a template would be something like the following:
	/// <blockquote><code><pre>
	/// #parse ($multiviews.findLocalizedResource("header.vm", "en"))
	/// #include ($multiviews.findLocalizedResource("my_page.html", "en"))
	/// #parse ($multiviews.findLocalizedResource("footer.vm", "en"))
	/// </pre></code></blockquote>
	///
	/// You might also wrap this method using another pull/view tool
	/// which does internationalization/localization/content negation
	/// for a single point of access.</p>
	/// </summary>
	/// <param name="name">The unlocalized name of the file to find.
	/// </param>
	/// <param name="language">The language to find localized context for.
	/// </param>
	/// <returns>The localized file name, or <code>name</code> if it is
	/// not localizable.
	/// </returns>
	public virtual String findLocalizedResource(String name, String language) {
	    String localizedName = name + '.' + language;
	    // templateExists() checks for static content as well
	    if (!Velocity.TemplateExists(localizedName)) {
		// Fall back to the default lanaguage.
		String defaultLangSuffix = '.' + defaultLanguage;
		if (localizedName.EndsWith(defaultLangSuffix)) {
		    // Assume no localized version of the resource.
		    localizedName = name;
		} else {
		    localizedName = name + defaultLangSuffix;
		    if (!Velocity.TemplateExists(localizedName)) {
			localizedName = name;
		    }
		}
	    }
	    return localizedName;
	}


    }
}
