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

namespace AspectSharp.Lang.AST
{
	using System;
	using System.Collections;

	/// <summary>
	/// Summary description for EngineConfiguration.
	/// </summary>
	public class EngineConfiguration : NodeBase
	{
		private ImportDirectiveCollection _imports;
		private InterceptorGlobalDeclarationCollection _interceptors;
		private MixinGlobalDeclarationCollection _mixins;
		private AspectDefinitionCollection _aspects;

		public EngineConfiguration() : base()
		{
			_imports = new ImportDirectiveCollection();
			_interceptors = new InterceptorGlobalDeclarationCollection();
			_mixins = new MixinGlobalDeclarationCollection();
			_aspects = new AspectDefinitionCollection();
		}

		public ImportDirectiveCollection Imports
		{
			get { return _imports; }
		}

		public InterceptorGlobalDeclarationCollection Interceptors
		{
			get { return _interceptors; }
		}

		public MixinGlobalDeclarationCollection Mixins
		{
			get { return _mixins; }
		}

		public AspectDefinitionCollection Aspects
		{
			get { return _aspects; }
		}

		public override void Accept(IVisitor visitor)
		{
			visitor.OnEngineConfiguration(this);
		}
	}
}
