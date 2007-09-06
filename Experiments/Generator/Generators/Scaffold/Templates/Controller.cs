using System;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Framework.Helpers;
using Castle.MonoRail.ActiveRecordSupport;
using <%= ModelsNamespace %>;
using <%= HelpersNamespace %>;
<% if Area != null: %>
using <%= ApplicationControllerNamespace %>;
<% end %>

namespace <%= Namespace %>
{
	/// <summary>
	/// <%= PluralHumanName %> controller
	/// </summary>
	[Layout("<%= ControllerFileName %>"), Helper(typeof(ScaffoldHelper))]
<% if Area != null: %>
	[ControllerDetails(Area="<%= Area %>")]
<% end %>
	public class <%= ControllerName %>Controller : ApplicationController
	{
		public void Index()
		{
			RedirectToAction("list");
		}
		
		public void List()
		{
			PropertyBag["<%= PluralVarName %>"] = PaginationHelper.CreatePagination(<%= ClassName %>.FindAll(), 10);
		}
		
		public void View(int id)
		{
			PropertyBag["<%= VarName %>"] = <%= ModelClassName %>.Find(id);
		}
		
		public void Edit(int id)
		{
			PropertyBag["<%= VarName %>"] = <%= ModelClassName %>.Find(id);
		}
		
		public void Update([ARDataBind("<%= VarName %>", AutoLoadBehavior.Always)] <%= ModelClassName %> <%= VarName %>)
		{			
			if (<%= VarName %>.IsValid())
			{
				<%= VarName %>.Update();
				Flash["edited"] = <%= VarName %>.Id;
				RedirectToAction("list");
			}
			else
			{
				PropertyBag["<%= VarName %>"] = <%= VarName %>;
				RenderView("edit");
			}
		}
		
		public void New()
		{
			PropertyBag["<%= VarName %>"] = new <%= ModelClassName %>();
		}
		
		public void Create([DataBind("<%= VarName %>")] <%= ModelClassName %> <%= VarName %>)
		{			
			if (<%= VarName %>.IsValid())
			{
				<%= VarName %>.Create();
				Flash["edited"] = <%= VarName %>.Id;
				RedirectToAction("list");
			}
			else
			{
				PropertyBag["<%= VarName %>"] = <%= VarName %>;
				RenderView("new");
			}
		}
		
		public void Delete(int id)
		{
			<%= ModelClassName %>.Find(id).Delete();
			CancelView();
		}
	}
}
