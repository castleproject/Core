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

namespace Castle.Windsor.Configuration.Interpreters
{
	using System;

	using Castle.Model.Configuration;

	using Castle.MicroKernel;
	
	using Castle.Windsor.Configuration.Interpreters.CastleLanguage.Internal;

	/// <summary>
	/// 
	/// </summary>
	public class ConfigLanguageInterpreter : AbstractInterpreter
	{
		public ConfigLanguageInterpreter()
		{
		}

		public ConfigLanguageInterpreter(String filename) : base(filename)
		{
		}

		public ConfigLanguageInterpreter(IConfigurationSource source) : base(source)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="store"></param>
		public override void Process(IConfigurationStore store)
		{
			WindsorConfLanguageLexer lexer = new WindsorConfLanguageLexer(Source.Contents);

			WindsorLanguageParser parser = new WindsorLanguageParser(
				new IndentTokenStream(lexer));

			ConfigurationDefinition confDef = parser.start();

			Imports = confDef.Imports;

			foreach(IConfiguration facility in confDef.Root.Children["facilities"].Children)
			{
				AddFacilityConfig(facility, store);
			}
			foreach(IConfiguration component in confDef.Root.Children["components"].Children)
			{
				AddComponentConfig(component, store);
			}
		}
	}
}
