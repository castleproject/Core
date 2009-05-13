// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.




namespace Castle.ActiveRecord.Tests.Model.CompositeUserType
{
	[ActiveRecord]
	public class Product : ActiveRecordBase<Product>
	{
		private int id;
		private string[] manufacturerName;
		private string[] name;

		private Product()
		{
		}
        
		public Product(string[] name, string[] manufacturerName)
		{
			this.name = name;
			this.manufacturerName = manufacturerName;
		}

		[PrimaryKey(Access = PropertyAccess.NosetterCamelcase)]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[CompositeUserType(typeof (DoubleStringType), new string[] {"FirstName", "LastName"}, Access = PropertyAccess.NosetterCamelcase)]
		public string[] Name
		{
			get { return name; }
		}

		[CompositeUserType(
			typeof (DoubleStringType),
			new string[] {"Manufacturer_FirstName", "Manufacturer_LastName"},
			Length = new int[] {4, 5}, Access = PropertyAccess.NosetterCamelcase)]
		public string[] ManufacturerName
		{
			get { return manufacturerName; }
		}
	}
}
