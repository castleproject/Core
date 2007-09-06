using System;
using System.Collections;
using Castle.ActiveRecord;

namespace <%= Namespace %>
{
	/// <summary>
	/// An instance of this class represents a <%= Name %>.
	/// </summary>
	[ActiveRecord]
	public class <%= ClassName %> : ActiveRecordValidationBase<% if UseGeneric: %><%= "<" + ClassName + ">" %><% end %>
	{
		#region Fields
		private int _id;
		<% for field in Fields: %>
		private string _<%= field.ToLower() %>;
		<% end %>
		#endregion
		
		#region Persistent properties
		[PrimaryKey]
		public int Id {
			get { return _id; }
			set { _id = value; }
		}
		<% i=0;	for prop in Properties: %>
		[Property]
		public string <%= prop %> {
			get { return _<%= Fields[i] %>; }
			set { _<%= Fields[i++] %> = value; }
		}
		<% end %>
		#endregion
		
		<% if not UseGeneric: %>
		#region Finders
		public static <%= ClassName %>[] FindAll()
		{
			return ((<%= ClassName %>[]) (ActiveRecordBase.FindAll(typeof(<%= ClassName %>))));
		}
		
		public static <%= ClassName %> Find(int id)
		{
			return ((<%= ClassName %>) (ActiveRecordBase.FindByPrimaryKey(typeof(<%= ClassName %>), id)));
		}
		#endregion
		<% end %>
	}

}
