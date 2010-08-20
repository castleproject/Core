// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

namespace Castle.Components.Validator
{
	using System;
	using System.Collections;
	using System.Reflection;

	/// <summary>
	/// Allows for classes to define a custom attribute to validate themselves.  Classes can decorate
	/// methods like:
	/// 
	///     [ValidateSelf]
	///     public void Validate(ErrorSummary errors) { ... }
	/// 
	/// to provide custom validate logic for the class.  The method must take exactly one 
	/// ErrorSummary parameter.
	/// </summary>
	public class SelfValidationContributor : AbstractValidationContributor
	{
		private const BindingFlags PublicBinding = BindingFlags.Instance | BindingFlags.Public;
		private static readonly IDictionary methodsPerType = Hashtable.Synchronized(new Hashtable());

		/// <summary>
		/// Allows for custom initialization based on type.  This will only be called once
		/// for each type passed to the contributor.
		/// </summary>
		/// <param name="type">The type.</param>
		protected override void Initialize(Type type)
		{
			ArrayList problematicMethods = new ArrayList();
			ArrayList validationMeta = new ArrayList();
			foreach (MethodInfo methodInfo in type.GetMethods(PublicBinding))
			{
				if (!methodInfo.IsDefined(typeof(ValidateSelfAttribute), true)) continue;

				ValidateSelfAttribute[] attrs =
					(ValidateSelfAttribute[]) methodInfo.GetCustomAttributes(typeof (ValidateSelfAttribute), true);
				ValidateSelfAttribute validateSelf = attrs[0];

				ParameterInfo[] parameters = methodInfo.GetParameters();
				if (IsValidSelfValidationMethod(parameters))
					problematicMethods.Add(methodInfo.Name);
				else
					validationMeta.Add(new SelfValidationMeta(methodInfo, validateSelf.RunWhen, validateSelf.ExecutionOrder));
			}

			if (problematicMethods.Count == 0)
			{
				validationMeta.Sort(SelfValidationMetaComparer.Instance);
				methodsPerType.Add(type, validationMeta);
				return;
			}

			ThrowErrorForInvalidSignatures(type, problematicMethods);
		}

		private bool IsValidSelfValidationMethod(ParameterInfo[] parameters)
		{
			return parameters.Length != 1 ||
				   parameters[0].IsOut || 
				   parameters[0].ParameterType != typeof(ErrorSummary);
		}

		private void ThrowErrorForInvalidSignatures(Type type, ArrayList problematicMethods)
		{
			String[] methodNames = (String[])problematicMethods.ToArray(typeof(String));

			String message = String.Format("The class {0} wants to use self validation, " +
										"however the methods must be only taking one parameter of type ErrorSummary. Please correct " +
										"the following methods: {1}", type,
										String.Join(", ", methodNames));
			throw new ValidationException(message);
		}

		/// <summary>
		/// Determines whether the specified instance is valid.  Returns an
		/// <see cref="ErrorSummary"/> that will be appended to the existing
		/// error summary for an object.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <param name="runWhen">The run when.</param>
		/// <returns></returns>
		protected override ErrorSummary IsValidInternal(object instance, RunWhen runWhen)
		{
			ErrorSummary errorSummary = new ErrorSummary();

			ArrayList methods = (ArrayList)methodsPerType[instance.GetType()];
			
			if (methods == null) return errorSummary;

			foreach (SelfValidationMeta meta in methods)
			{
				if (!IsMetaOnPhase(meta, runWhen)) continue;
				
				MethodInfo methodInfo = meta.MethodInfo;
				methodInfo.Invoke(instance, new object[] {errorSummary});
			}                
			return errorSummary;
		}

		private static bool IsMetaOnPhase(SelfValidationMeta meta, RunWhen when)
		{
			return (when == RunWhen.Everytime || meta.RunWhen == RunWhen.Everytime
				|| ((meta.RunWhen & when) != 0));
		}

		private class SelfValidationMeta
		{

			private readonly MethodInfo methodInfo;
			private readonly RunWhen runWhen;
			private readonly int executionOrder;

			public SelfValidationMeta(MethodInfo methodInfo, RunWhen runWhen, int executionOrder)
			{
				this.methodInfo = methodInfo;
				this.runWhen = runWhen;
				this.executionOrder = executionOrder;
			}

			/// <summary>
			/// Gets the method info.
			/// </summary>
			/// <value>The method info.</value>
			public MethodInfo MethodInfo
			{
				get { return methodInfo; }
			}

			public RunWhen RunWhen
			{
				get { return runWhen; }
			}

			public int ExecutionOrder
			{
				get { return executionOrder; }
			}
		}

		private class SelfValidationMetaComparer : IComparer
		{
			public static readonly SelfValidationMetaComparer Instance = new SelfValidationMetaComparer();
			
			public int Compare(object x, object y)
			{
				SelfValidationMeta left = (SelfValidationMeta)x;
				SelfValidationMeta right = (SelfValidationMeta)y;

				return left.ExecutionOrder - right.ExecutionOrder;
			}
		}
	}
}
