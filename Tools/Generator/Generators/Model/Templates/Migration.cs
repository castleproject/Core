using System;
using Migrator;

namespace <%= MigrationNamespace %>
{
	/// <summary>
	/// Add <%= HumanName %> table to the database
	/// </summary>
	[Migration(<%= Version %>)]
	public class Add<%= ClassName %>Table : Migration
	{
		public override void Up()
		{
			Database.AddTable("<%= ClassName %>",
				new Column("Id", typeof(int), ColumnProperties.PrimaryKeyWithIdentity)<% for prop in Properties: %>,
				new Column("<%= prop %>", typeof(string), 50)<% end %>

			);
		}
		
		public override void Down()
		{
			Database.RemoveTable("<%= ClassName %>");
		}
		
	}
}
