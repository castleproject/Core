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

namespace Castle.ActiveRecord.Generator.Common
{
	using System;

	using Castle.Windsor;

	using Castle.Facilities.Startable;
	using Castle.Facilities.TypedFactory;

	using Castle.ActiveRecord.Generator.CodeGenerator;
	using Castle.ActiveRecord.Generator.CodeGenerator.Default;
	using Castle.ActiveRecord.Generator.Common.Database;
	using Castle.ActiveRecord.Generator.Common.Database.Default;
	using Castle.ActiveRecord.Generator.Prevalence;
	using Castle.ActiveRecord.Generator.Prevalence.Default;
	using Castle.ActiveRecord.Generator.Database;
	using Castle.ActiveRecord.Generator.Model;


	public class ActiveRecordGeneratorContainer : WindsorContainer
	{
		public ActiveRecordGeneratorContainer()
		{
			AddFacilities();
			AddComponents();
		}

		protected virtual void AddFacilities()
		{
			AddFacility( "startable", new StartableFacility() );

			TypedFactoryFacility typedFactory = new TypedFactoryFacility();
	
			AddFacility( "typedFactory", typedFactory );

			typedFactory.AddTypedFactoryEntry( 
				new FactoryEntry("project.factory", typeof(IProjectFactory), "Create", "") );
		}

		protected virtual void AddComponents()
		{
			AddComponent( "project", typeof(Project) );

			AddComponent( "codeprovider", 
				typeof(ICodeProviderFactory), typeof(CodeProviderFactory) );

			AddComponent( "codedomgenerator", 
				typeof(ICodeDomGenerator), typeof(CodeDomGenerator) );

			AddComponent( "namingservice", 
				typeof(INamingService), typeof(NamingService) );

			AddComponent( "db.connection.factory", 
				typeof(IConnectionFactory), typeof(ConnectionFactory) );
			
			AddComponent( "db.def.builder", 
				typeof(IDatabaseDefinitionBuilder), typeof(DatabaseDefinitionBuilder) );

			AddComponent( "model.ar.builder", typeof(ActiveRecordDescriptorBuilder) );

			AddComponent( "project.prevalence", 
				typeof(IProjectPrevalence), typeof(ProjectPrevalence) );
		}
	}
}
