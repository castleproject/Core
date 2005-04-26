// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.ActiveRecord.Tests.Model
{
  using System;


  [ActiveRecord("Line_Item")]
  public class LineItem : ActiveRecordBase
  {
	private int id;
	private int quantity;

	[PrimaryKey(PrimaryKeyType.Native, "line_number")]
	public int ID
	{
	  get { return this.id; }
	  set { this.id = value; }
	}

	[Property()]
	public int Quantity
	{
	  get { return this.quantity; }
	  set { this.quantity = value; }
	}

	public static LineItem Find(int id)
	{
	  return ((LineItem) (ActiveRecordBase.FindByPrimaryKey(typeof (LineItem), id)));
	}

	public static void DeleteAll()
	{
	  ActiveRecordBase.DeleteAll( typeof(LineItem) );
	}
  }
}
