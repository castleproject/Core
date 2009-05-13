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

[assembly: Castle.ActiveRecord.Tests.Model.RegisterNHibernateClassMapping]

namespace Castle.ActiveRecord.Tests.Model
{
	public class RegisterNHibernateClassMapping : RawXmlMappingAttribute
	{
		public override string[] GetMappings()
		{
			return new string[]
			{
				@"<hibernate-mapping  xmlns='urn:nhibernate-mapping-2.2'>
	<class name='Castle.ActiveRecord.Tests.Model.NHibernateClass, Castle.ActiveRecord.Tests.Model'>
		<id name='Id'>
			<generator class='native'/>
		</id>
	</class>
</hibernate-mapping>"
			};
		}
	}
}