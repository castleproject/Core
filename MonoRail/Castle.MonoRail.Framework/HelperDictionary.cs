using System.Collections.Specialized;
using Castle.MonoRail.Framework.Helpers;

namespace Castle.MonoRail.Framework
{
	/// <summary>
	/// Dictionary containing helpers.
	/// </summary>
	public class HelperDictionary : HybridDictionary
	{
		/// <summary>
		/// Instatiates a new HelperDictionary.
		/// </summary>
		public HelperDictionary() : base(true)
		{
		}

		/// <summary>
		/// Adds the supplied helper using the standard key naming rules.
		/// </summary>
		/// <param name="helper">The helper to be added</param>
		public void Add(AbstractHelper helper)
		{
			string helperName = helper.GetType().Name;

			if (!Contains(helperName))
			{
				Add(helperName, helper);
			}

			// Also makes the helper available with a less verbose name
			// i.e. FormHelper and Form, AjaxHelper and Ajax
			if (helperName.EndsWith("Helper"))
			{
				string alias = helperName.Substring(0, helperName.Length - 6);

				if (!Contains(alias))
				{
					Add(alias, helper);
				}
			}
		}
	}
}
