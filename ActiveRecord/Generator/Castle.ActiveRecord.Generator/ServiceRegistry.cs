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

namespace Castle.ActiveRecord.Generator
{
	using System;

	using Castle.Windsor;

	using Castle.ActiveRecord.Generator.Components;
	using Castle.ActiveRecord.Generator.Components.Database;
	using Castle.ActiveRecord.Generator.Components.CodeGenerator;


	public class ServiceRegistry : WindsorContainer
	{
		private static ServiceRegistry instance = new ServiceRegistry();

		public ServiceRegistry() : base()
		{
			// TODO: Put these on the configuration file

			AddComponent("conn.factory", typeof(IConnectionFactory), typeof(ConnectionFactory));
			AddComponent("db.def.builder", typeof(IDatabaseDefinitionBuilder), typeof(DatabaseDefinitionBuilder));
			AddComponent("naming.service", typeof(INamingService), typeof(NamingService));
			AddComponent("typeinfer.service", typeof(ITypeInferenceService), typeof(TypeInferenceService) );
			AddComponent("arbuilder", typeof(IActiveRecordDescriptorBuilder), typeof(ActiveRecordDescriptorBuilder) );
			AddComponent("plainfields.service", typeof(IPlainFieldInferenceService), typeof(PlainFieldInferenceService) );
			AddComponent("relation.service", typeof(IRelationshipInferenceService), typeof(RelationshipInferenceService) );
			AddComponent("codeproviderfactory", typeof(ICodeProviderFactory), typeof(CodeProviderFactory) );
		}

		public static ServiceRegistry Instance
		{
			get { return instance; }
		}
	}
}
