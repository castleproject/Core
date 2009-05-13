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

using System;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework.Internal.Tests;

[assembly: Import(typeof(ImportClassRow), "ImportClassRow")]
[assembly: HqlNamedQuery("allAdultUsers", "from User user where user.Age > 21")]
[assembly: SqlNamedQuery("allAdultUsersSql", "select * from Users where Age > 21")]
[assembly: ConstantMappingTestAttribute]

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false),Serializable]
public class ConstantMappingTestAttribute : RawXmlMappingAttribute
{
	/// <summary>
	/// Get the mapping xml to add to NHibernate's configuration.
	/// Note that we allow to return more than a single mapping, each string is 
	/// treated as a seperated document.
	/// </summary>
	public override string[] GetMappings()
	{
		return new string[]{
@"<hibernate-mapping xmlns='urn:nhibernate-mapping-2.2'>
	<import class='Castle.ActiveRecord.Framework.Internal.Tests.ImportClassRow2, Castle.ActiveRecord.Framework.Internal.Tests' rename='ImportClassRow2'/>
</hibernate-mapping>"};
	}
}