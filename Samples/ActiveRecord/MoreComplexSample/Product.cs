// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace MoreComplexSample
{
	using System;
	using Castle.ActiveRecord;
	using Iesi.Collections;

[ActiveRecord]
public class Product : ActiveRecordBase
{
	private int id;
	private String name;
	private float price;
	private ISet categories = new HashedSet();

	[PrimaryKey]
	public int Id
	{
		get { return id; }
		set { id = value; }
	}

	[Property]
	public string Name
	{
		get { return name; }
		set { name = value; }
	}

	[Property]
	public float Price
	{
		get { return price; }
		set { price = value; }
	}
	
	[HasAndBelongsToMany(typeof(Category), 
		Table="ProductCategory", ColumnKey="ProductId", ColumnRef="CategoryId", Lazy=true)]
	public ISet Categories
	{
		get { return categories; }
		set { categories = value; }
	}
}
}