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

namespace Castle.Windsor.Tests.ModelBuilders
{
	using System;
	using System.Reflection;

	using NUnit.Framework;

	using Castle.Model;
	using Castle.MicroKernel;


	[TestFixture]
	public class MethodMetaInspectorTestCase
	{
		[Test]
		public void CorrectSignatureMatches()
		{
			WindsorContainer container = 
				new WindsorContainer("../Castle.Windsor.Tests/ModelBuilders/methodusage1.xml");

			Assert.IsTrue( container.Kernel.HasComponent("comp1") );
			
			IHandler handler = container.Kernel.GetHandler("comp1");

			ComponentModel model = handler.ComponentModel;

			Assert.IsNotNull( model.MethodMetaModels );
			Assert.AreEqual( 3, model.MethodMetaModels.Count );

			Type target = typeof(Component1);

			MethodInfo method = target.GetMethod("Save", new Type[0]);
			Assert.IsTrue( model.MethodMetaModels.MethodInfo2Model.Contains(method) );
			MethodMetaModel metaModel = (MethodMetaModel) model.MethodMetaModels.MethodInfo2Model[method];
			Assert.AreEqual( "1", metaModel.ConfigNode.Attributes["myattribute"] );

			method = target.GetMethod("Save", new Type[] { typeof(String) });
			Assert.IsTrue( model.MethodMetaModels.MethodInfo2Model.Contains(method) );
			metaModel = (MethodMetaModel) model.MethodMetaModels.MethodInfo2Model[method];
			Assert.AreEqual( "2", metaModel.ConfigNode.Attributes["myattribute"] );

			method = target.GetMethod("Save", new Type[] { typeof(String), typeof(String) });
			Assert.IsTrue( model.MethodMetaModels.MethodInfo2Model.Contains(method) );
			metaModel = (MethodMetaModel) model.MethodMetaModels.MethodInfo2Model[method];
			Assert.AreEqual( "3", metaModel.ConfigNode.Attributes["myattribute"] );
		}
	}
}
