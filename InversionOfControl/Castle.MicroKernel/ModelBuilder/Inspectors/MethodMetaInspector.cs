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

namespace Castle.MicroKernel.ModelBuilder.Inspectors
{
	using System;
	using System.Collections;
	using System.Configuration;
	using System.Reflection;

	using Castle.Core;
	using Castle.Core.Configuration;

	using Castle.MicroKernel.SubSystems.Conversion;


	/// <summary>
	/// Base for inspectors that want configuration associated with methods.
	/// For each child a <see cref="MethodMetaModel"/> is created
	/// and added to ComponentModel's methods collection
	/// </summary>
	/// <remarks>
	/// Implementors should override the <see cref="ObtainNodeName"/> return
	/// the name of the node to be inspected. For example:
	/// <code>
	/// <pre>
	///   <transactions>
	///     <method name="Save" transaction="requires" />
	///   </transactions>
	/// </pre>
	/// </code>
	/// </remarks>
	public abstract class MethodMetaInspector : IContributeComponentModelConstruction
	{
		private static readonly BindingFlags AllMethods = 
			BindingFlags.Public|BindingFlags.NonPublic|
			BindingFlags.Instance|BindingFlags.Static|
			BindingFlags.IgnoreCase|BindingFlags.IgnoreReturn;

		private ITypeConverter converter;

		public virtual void ProcessModel(IKernel kernel, ComponentModel model)
		{
			if (model == null) throw new ArgumentNullException("model");

			if (model.Configuration == null || model.Implementation == null) return;

			IConfiguration methodsNode = model.Configuration.Children[ObtainNodeName()];

			if (methodsNode == null) return;

			EnsureHasReferenceToConverter(kernel);

			foreach(IConfiguration methodNode in methodsNode.Children)
			{
				String name = methodNode.Name;

				if ("method".Equals(name))
				{
					name = methodNode.Attributes["name"];
				}

				AssertNameIsNotNull(name, model);

				MethodMetaModel metaModel = new MethodMetaModel(methodNode);

				if (IsValidMeta(model, metaModel))
				{
					if (ShouldUseMetaModel)
					{
						// model.MethodMetaModels.Add( metaModel );
					}

					String signature = methodNode.Attributes["signature"];

					MethodInfo[] methods = GetMethods(model.Implementation, name, signature);

					if (methods.Length == 0)
					{
						String message = String.Format( "The class {0} has tried to expose configuration for " + 
							"a method named {1} which could not be found.", model.Implementation.FullName, name );

#if DOTNET2
						throw new ConfigurationErrorsException(message);
#else
						throw new ConfigurationException(message);
#endif
					}

					ProcessMeta(model, methods, metaModel);

					if (ShouldUseMetaModel)
					{
						// RegisterMethodsForFastAccess(methods, signature, metaModel, model);
					}
				}
			}
		}

		protected virtual void ProcessMeta(ComponentModel model, MethodInfo[] methods, MethodMetaModel metaModel)
		{
		}

		protected virtual bool IsValidMeta(ComponentModel model, MethodMetaModel metaModel)
		{
			return true;
		}

		protected virtual bool ShouldUseMetaModel
		{
			get { return false; }
		}

		protected abstract String ObtainNodeName();

//		private void RegisterMethodsForFastAccess(MethodInfo[] methods, 
//			String signature, MethodMetaModel metaModel, ComponentModel model)
//		{
//			foreach(MethodInfo method in methods)
//			{
//				if (signature != null && signature.Length != 0)
//				{
//					model.MethodMetaModels.MethodInfo2Model[method] = metaModel;
//				}
//				else
//				{
//					if (!model.MethodMetaModels.MethodInfo2Model.Contains(method))
//					{
//						model.MethodMetaModels.MethodInfo2Model[method] = metaModel;
//					}
//				}
//			}
//		}

		private void AssertNameIsNotNull(string name, ComponentModel model)
		{
			if (name == null)
			{
				String message = String.Format("The configuration nodes within 'methods' " + 
					"for the component '{0}' does not have a name. You can either name " + 
					"the node as the method name or provide an attribute 'name'", model.Name);
#if DOTNET2
				throw new ConfigurationErrorsException(message);
#else
				throw new ConfigurationException(message);
#endif
			}
		}

		private void EnsureHasReferenceToConverter(IKernel kernel)
		{
			if (converter != null) return;

			converter = (ITypeConverter) 
				kernel.GetSubSystem( SubSystemConstants.ConversionManagerKey );
		}

		private MethodInfo[] GetMethods(Type implementation, String name, String signature)
		{
			if (signature == null || signature.Length == 0)
			{
				MethodInfo[] allmethods = implementation.GetMethods(AllMethods);

				ArrayList methods = new ArrayList();

				foreach(MethodInfo method in allmethods)
				{
					if (String.Compare(method.Name, name, true) == 0)
					{
						methods.Add(method);
					}
				}

				return (MethodInfo[]) methods.ToArray( typeof(MethodInfo) );
			}
			else
			{
				MethodInfo methodInfo = implementation.GetMethod(name, AllMethods, null, ConvertSignature(signature), null );

				if (methodInfo == null) return new MethodInfo[0];

				return new MethodInfo[] { methodInfo };
			}
		}

		private Type[] ConvertSignature(string signature)
		{
			String[] parameters = signature.Split(';');

			ArrayList types = new ArrayList();

			foreach(String param in parameters)
			{
				try
				{
					types.Add( converter.PerformConversion( param, typeof(Type) ) );
				}
				catch(Exception)
				{
					String message = String.Format("The signature {0} contains an entry type {1} " + 
						"that could not be converted to System.Type. Check the inner exception for " + 
						"details", signature, param);

#if DOTNET2
					throw new ConfigurationErrorsException(message);
#else
					throw new ConfigurationException(message);
#endif
				}
			}

			return (Type[]) types.ToArray( typeof(Type) );
		}
	}
}
