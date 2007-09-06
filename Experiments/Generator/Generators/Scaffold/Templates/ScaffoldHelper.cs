using System;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;
using System.Reflection;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework.Internal;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Framework.Helpers;

namespace <%= HelpersNamespace %>
{
	/// <summary>
	/// Helper for displaying cool things on web pages!
	/// </summary>
	public class ScaffoldHelper : AbstractHelper
	{	
		private HtmlHelper _htmlHelper = new HtmlHelper();
		
		#region Formating helpers
		public string ToHumanName(string name)
		{
			return Regex.Replace(name, "([a-z])([A-Z])", new MatchEvaluator(HumanNameMatchEvaluator));
		}
		private string HumanNameMatchEvaluator(Match m)
		{
			return m.Groups[1] + " " + m.Groups[2].ToString().ToLower();
		}
		public string ToHumanName(PropertyModel prop)
		{
			return ToHumanName(prop.Property.Name);
		}
		
		public string ToColumnName(string name)
		{
			if (Controller.Params["order"] == name)
				return ToHumanName(name);
			return string.Format("<a href=\"?order={0}\">{1}</a>", name, ToHumanName(name));
		}
		public string ToColumnName(PropertyModel prop)
		{
			if (prop.PropertyAtt.ColumnType == "StringClob") // Can't order by!
				return ToHumanName(prop.Property.Name);
			return ToColumnName(prop.Property.Name);
		}
		#endregion
		
		#region Properties helpers
		public PropertyModel[] GetProperties(IEnumerable enumerable)
		{
			IEnumerator en = enumerable.GetEnumerator();
			en.MoveNext();
			return GetProperties(en.Current.GetType());
		}
		
		public PropertyModel[] GetProperties(Type arType)
		{
			ArrayList props = new ArrayList();
			ActiveRecordModel model = ActiveRecordModel.GetModel(arType);
			
			props.AddRange(model.Properties);
			
			foreach (BelongsToModel belong in model.BelongsTo)
			{
				props.Add(new PropertyModel(belong.Property, new PropertyAttribute()));
			}
			
			return (PropertyModel[]) props.ToArray(typeof(PropertyModel));
		}
		
		public object GetPropertyValue(object obj, PropertyModel prop)
		{
			return prop.Property.GetValue(obj, null);
		}
		
		public bool HasItems(IEnumerable enumerable)
		{
			return enumerable.GetEnumerator().MoveNext();
		}
		#endregion
		
		#region Input helpers
		public string InputFor(string prefix, object ar, PropertyModel prop)
		{
			string name = string.Format("{0}.{1}", prefix, prop.Property.Name);
			Type type = prop.Property.PropertyType;
			object value = GetPropertyValue(ar, prop);
			
			if (typeof(ActiveRecordBase).IsAssignableFrom(type))
			{
				return Select(name, value as ActiveRecordBase, type);
			}
			else if (type == typeof(DateTime))
			{
				return HtmlHelper.DateTime(name, (DateTime) value);
			}
			else if (type == typeof(string) && prop.PropertyAtt.ColumnType == "StringClob")
			{
				return HtmlHelper.TextArea(name, 40, 10, (string) value);
			}
			
			if (value == null)
				return HtmlHelper.InputText(name, "");
			else
				return HtmlHelper.InputText(name, value.ToString());
		}
		
		public string Select(string name, object ar)
		{
			return Select(name, ar, ar.GetType());
		}
		
		public string Select(string name, object ar, Type arType)
		{
			HtmlHelper helper = new HtmlHelper();
			ActiveRecordModel model = ActiveRecordModel.GetModel(arType);
			PropertyInfo prop = model.Key.Property;
			object selected = null;
			
			if (ar != null && prop != null)
			{
				selected = prop.GetValue(ar, null);
			}
			
			return helper.Select(name + ".Id")
				+ helper.CreateOptions(ActiveRecordMediator.FindAll(arType), "ToString", "Id", selected)
				+ helper.EndSelect();
		}
		#endregion
		
		#region Errors helpers
		public string ErrorsFor(ActiveRecordValidationBase ar)
		{
			if (this.Controller.Context.RequestType == "GET")
				return null;
			return String.Join("<br>", ar.ValidationErrorMessages);
		}
		
		public bool HasError(ActiveRecordValidationBase ar)
		{
			if (this.Controller.Context.RequestType == "GET")
				return false;
			return ar.ValidationErrorMessages.Length > 0;
		}
		#endregion
		
		#region Pagination helpers
		public string PageBrowser(Page page)
		{
			StringWriter output = new StringWriter();
			PaginationHelper helper = new PaginationHelper();
			helper.SetController(this.Controller);
			
			if (page.HasFirst)
				output.Write(helper.CreatePageLink(1, "First"));
			else
				output.Write("First");
			
			output.Write(" | ");
			
			if (page.HasPrevious)
				output.Write(helper.CreatePageLink(page.PreviousIndex, "Previous"));
			else
				output.Write("Previous");

			output.Write(" | ");
			
			if (page.HasNext) 
				output.Write(helper.CreatePageLink(page.NextIndex, "Next"));
			else
				output.Write("Next");
			
			output.Write(" | ");
			
			if (page.HasLast) 
				output.Write(helper.CreatePageLink(page.LastIndex, "Last"));
			else
				output.Write("Last");
			
			return output.ToString();
		}
		#endregion
		
		#region Debugging helpers
		public string Debug(object ar)
		{
			return "<code><pre>" + Debug(ar, 0) + "</code></pre>";
		}
		private string Debug(object ar, int indent)
		{
			if (ar == null) return null;
			
			StringWriter output = new StringWriter();
			string tab = new string('\t', indent);
			object value;
			
			output.WriteLine("{0}(#{1}):", tab, ar.GetType().Name);
			foreach (PropertyModel prop in GetProperties(ar.GetType()))
			{
				value = GetPropertyValue(ar, prop);
				
				output.WriteLine("{0}\t{1} ({2}) = {3}", tab, prop.Property.Name, prop.Property.PropertyType.Name, value);
				if (typeof(ActiveRecordBase).IsAssignableFrom(prop.Property.PropertyType))
				{
					output.WriteLine(Debug(value as ActiveRecordBase, indent+1));
				}
			}
			
			return output.ToString();
		}
		#endregion
		
		#region Sub helpers
		public HtmlHelper HtmlHelper {
			get {
				return _htmlHelper;
			}
		}
		#endregion
	}
}
