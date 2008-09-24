// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Helpers
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using System.Data;
	using System.Reflection;
	using System.Text;
	using Castle.Components.Binder;
	using Castle.Components.Validator;
	using Castle.Core.Logging;
	using Core;
	using Internal;
	using ValidationStrategy;

	/// <summary>
	/// Represents all scopes that the <see cref="FormHelper"/>
	/// uses to search for root values
	/// </summary>
	public enum RequestContext
	{
		/// <summary>
		/// All scopes should be searched
		/// </summary>
		All,
		/// <summary>
		/// Only PropertyBag should be searched
		/// </summary>
		PropertyBag,
		/// <summary>
		/// Only Flash should be searched
		/// </summary>
		Flash,
		/// <summary>
		/// Only Session should be searched
		/// </summary>
		Session,
		/// <summary>
		/// Only Request should be searched
		/// </summary>
		Request,
		/// <summary>
		/// Only Params should be searched
		/// </summary>
		Params
	}

	/// <summary>
	/// Base class that exposes common operations for form handling, field bindings and so on.
	/// </summary>
	public abstract class AbstractFormRelatedHelper : AbstractHelper
	{
		/// <summary>
		/// Common property flags for reflection
		/// </summary>
		protected static readonly BindingFlags PropertyFlags = BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase;
		/// <summary>
		/// Common property flags for reflection (with declared only)
		/// </summary>
		protected static readonly BindingFlags propertyFlagsDeclaredOnly = BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly;
		/// <summary>
		/// Common field flags for reflection
		/// </summary>
		protected static readonly BindingFlags FieldFlags = BindingFlags.GetField | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase;

		/// <summary>
		/// Logger instance
		/// </summary>
		protected static ILogger logger = NullLogger.Instance;

		/// <summary>
		/// 
		/// </summary>
		protected BrowserValidationConfiguration validationConfig;
		/// <summary>
		/// 
		/// </summary>
		private IBrowserValidatorProvider validatorProvider;
		/// <summary>
		/// 
		/// </summary>
		protected readonly Stack objectStack = new Stack();

		private IValidatorRegistry validatorRegistry;
		private ValidatorRunner validatorRunner;
		private bool isValidationDisabled;

		/// <summary>
		/// Initializes a new instance of the <see cref="AbstractFormRelatedHelper"/> class.
		/// </summary>
		protected AbstractFormRelatedHelper()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AbstractFormRelatedHelper"/> class.
		/// </summary>
		/// <param name="validatorRegistry">The validator registry.</param>
		/// <param name="validatorRunner">The validator runner.</param>
		protected AbstractFormRelatedHelper(IValidatorRegistry validatorRegistry, ValidatorRunner validatorRunner)
		{
			this.validatorRegistry = validatorRegistry;
			this.validatorRunner = validatorRunner;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AbstractFormRelatedHelper"/> class.
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		protected AbstractFormRelatedHelper(IEngineContext engineContext) : base(engineContext)
		{
		}

		/// <summary>
		/// Gets or sets the validator provider.
		/// </summary>
		/// <value>The validator provider.</value>
		public IBrowserValidatorProvider ValidatorProvider
		{
			get { return validatorProvider; }
			set { validatorProvider = value; }
		}

		/// <summary>
		/// Gets or sets the validator runner.
		/// </summary>
		/// <value>The validator runner.</value>
		public ValidatorRunner ValidatorRunner
		{
			get { return validatorRunner; }
			set { validatorRunner = value; }
		}

		/// <summary>
		/// Gets or sets the validator registry.
		/// </summary>
		/// <value>The validator registry.</value>
		public IValidatorRegistry ValidatorRegistry
		{
			get { return validatorRegistry; }
			set { validatorRegistry = value; }
		}

		#region protected members

		/// <summary>
		/// Rewrites the target if within object scope.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <returns></returns>
		protected string RewriteTargetIfWithinObjectScope(string target)
		{
			if (objectStack.Count == 0)
			{
				return target;
			}
			else
			{
				return ((FormScopeInfo) objectStack.Peek()).RootTarget + "." + target;
			}
		}

		/// <summary>
		/// Creates the specified input element 
		/// using the specified parameters to supply the name, value, id and others 
		/// html attributes.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="target">The object to get the value from and to be based on to create the element name.</param>
		/// <param name="value"></param>
		/// <param name="attributes">Attributes for the FormHelper method and for the html element it generates</param>
		/// <returns>The generated form element</returns>
		protected virtual string CreateInputElement(string type, string target, Object value, IDictionary attributes)
		{
			string id = CreateHtmlId(attributes, target);

			return CreateInputElement(type, id, target, FormatIfNecessary(value, attributes), attributes);
		}

		/// <summary>
		/// Creates the specified input element 
		/// using the specified parameters to supply the name, value, id and others 
		/// html attributes.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="id"></param>
		/// <param name="target">The object to get the value from and to be based on to create the element name.</param>
		/// <param name="value"></param>
		/// <param name="attributes">Attributes for the FormHelper method and for the html element it generates</param>
		/// <returns>The generated form element</returns>
		protected virtual string CreateInputElement(string type, string id, string target, string value, IDictionary attributes)
		{
			value = FormatIfNecessary(value, attributes);

			value = SafeHtmlEncode(value);

			if (attributes != null && attributes.Contains("mask"))
			{
				string mask = CommonUtils.ObtainEntryAndRemove(attributes, "mask");
				string maskSep = CommonUtils.ObtainEntryAndRemove(attributes, "mask_separator", "-");

				string onBlur = CommonUtils.ObtainEntryAndRemove(attributes, "onBlur", "void(0)");
				string onKeyUp = CommonUtils.ObtainEntryAndRemove(attributes, "onKeyUp", "void(0)");

				string js = "return monorail_formhelper_mask(event,this,'" + mask + "','" + maskSep + "');";

				attributes["onBlur"] = "javascript:" + onBlur + ";" + js;
				attributes["onKeyUp"] = "javascript:" + onKeyUp + ";" + js;
			}

			return String.Format("<input type=\"{0}\" id=\"{1}\" name=\"{2}\" value=\"{3}\" {4}/>",
								 type, id, target, value, GetAttributes(attributes));
		}

		/// <summary>
		/// Creates the input element.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="value">The value.</param>
		/// <param name="attributes">The attributes.</param>
		/// <returns></returns>
		protected virtual string CreateInputElement(string type, string value, IDictionary attributes)
		{
			return String.Format("<input type=\"{0}\" value=\"{1}\" {2}/>",
								 type, FormatIfNecessary(value, attributes), GetAttributes(attributes));
		}

		/// <summary>
		/// Formats if necessary.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="attributes">The attributes.</param>
		/// <returns></returns>
		protected static string FormatIfNecessary(object value, IDictionary attributes)
		{
			string formatString = CommonUtils.ObtainEntryAndRemove(attributes, "textformat");

			if (value != null && formatString != null)
			{
				IFormattable formattable = value as IFormattable;

				if (formattable != null)
				{
					value = formattable.ToString(formatString, null);
				}
			}
			else if (value == null)
			{
				value = String.Empty;
			}

			return value.ToString();
		}

		/// <summary>
		/// Obtains the target property.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="target">The target.</param>
		/// <param name="action">The action.</param>
		/// <returns></returns>
		protected PropertyInfo ObtainTargetProperty(RequestContext context, string target, Action<PropertyInfo> action)
		{
			string[] pieces;

			Type root = ObtainRootType(context, target, out pieces);

			if (root != null && pieces.Length > 1)
			{
				return QueryPropertyInfoRecursive(root, pieces, 1, action);
			}

			return null;
		}

		/// <summary>
		/// Queries the context for the target value
		/// </summary>
		/// <param name="target">The object to get the value from and to be based on to create the element name.</param>
		/// <param name="attributes">The attributes. Accepts a "defaultValue" attribute value.</param>
		/// <returns>The generated form element</returns>
		protected object ObtainValue(string target, IDictionary attributes)
		{
			object value = ObtainValue(target);
			if (attributes != null && attributes.Contains("defaultValue"))
			{
				object defaultValue = CommonUtils.ObtainEntryAndRemove(attributes, "defaultValue");
				if (value == null)
				{
					value = defaultValue;
				}
			}

			return value;
		}

		/// <summary>
		/// Queries the context for the target value
		/// </summary>
		/// <param name="target">The object to get the value from and to be based on to create the element name.</param>
		/// <returns>The generated form element</returns>
		protected object ObtainValue(string target)
		{
			return ObtainValue(RequestContext.All, target);
		}

		/// <summary>
		/// Queries the context for the target value
		/// </summary>
		/// <param name="context"></param>
		/// <param name="target">The object to get the value from and to be based on to create the element name.</param>
		/// <returns>The generated form element</returns>
		protected object ObtainValue(RequestContext context, string target)
		{
			string[] pieces;

			object rootInstance = ObtainRootInstance(context, target, out pieces);

			if (rootInstance != null && pieces.Length > 1)
			{
				return QueryPropertyRecursive(rootInstance, pieces, 1);
			}

			return rootInstance;
		}

		/// <summary>
		/// Obtains the root instance.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="target">The object to get the value from and to be based on to create the element name.</param>
		/// <returns>The generated form element</returns>
		protected object ObtainRootInstance(RequestContext context, string target)
		{
			object rootInstance = null;

			if (context == RequestContext.All || context == RequestContext.PropertyBag)
			{
				rootInstance = ControllerContext.PropertyBag[target];
			}
			if (rootInstance == null && (context == RequestContext.All || context == RequestContext.Flash) && Context != null && Context.Flash != null)
			{
				rootInstance = Context.Flash[target];
			}
			if (rootInstance == null && (context == RequestContext.All || context == RequestContext.Session) && Context != null && Context.Session != null)
			{
				rootInstance = Context.Session[target];
			}
			if (rootInstance == null &&  Context != null && (context == RequestContext.All || context == RequestContext.Params))
			{
				rootInstance = Context.Request.Params[target];
			}
			if (rootInstance == null && Context != null && (context == RequestContext.All || context == RequestContext.Request))
			{
				rootInstance = Context.Items[target];
			}

			return rootInstance;
		}

		/// <summary>
		/// Obtains the root instance.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="target">The target.</param>
		/// <param name="pieces">The pieces.</param>
		/// <returns></returns>
		protected object ObtainRootInstance(RequestContext context, string target, out string[] pieces)
		{
			pieces = target.Split(new char[] { '.' });

			string root = pieces[0];

			int index;

			bool isIndexed = CheckForExistenceAndExtractIndex(ref root, out index);

			object rootInstance = ObtainRootInstance(context, root);

			if (rootInstance == null)
			{
				return null;
			}

			if (isIndexed)
			{
				AssertIsValidArray(rootInstance, root, index);
			}

			if (!isIndexed && pieces.Length == 1)
			{
				return rootInstance;
			}
			else if (isIndexed)
			{
				rootInstance = GetArrayElement(rootInstance, index);
			}

			return rootInstance;
		}

		/// <summary>
		/// Obtains the type of the root.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="target">The target.</param>
		/// <param name="pieces">The pieces.</param>
		/// <returns></returns>
		private Type ObtainRootType(RequestContext context, string target, out string[] pieces)
		{
			pieces = target.Split(new char[] { '.' });

			Type foundType = (Type) ControllerContext.PropertyBag[pieces[0] + "type"];

			if (foundType == null)
			{
				string trimmed = pieces[0].Split('[')[0];

				foundType = (Type) ControllerContext.PropertyBag[trimmed + "type"];

				if (foundType == null)
				{
					object root = ObtainRootInstance(context, target, out pieces);

					if (root != null)
					{
						foundType = root.GetType();
					}
				}
			}

			return foundType;
		}

		/// <summary>
		/// Queries the property info recursive.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="propertyPath">The property path.</param>
		/// <returns></returns>
		protected static PropertyInfo QueryPropertyInfoRecursive(Type type, string[] propertyPath)
		{
			return QueryPropertyInfoRecursive(type, propertyPath, 0, null);
		}

		/// <summary>
		/// Queries the property info recursive.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="propertyPath">The property path.</param>
		/// <param name="piece">The piece.</param>
		/// <param name="action">The action.</param>
		/// <returns></returns>
		protected static PropertyInfo QueryPropertyInfoRecursive(Type type, string[] propertyPath, int piece, Action<PropertyInfo> action)
		{
			string property = propertyPath[piece];
			int index;

			bool isIndexed = CheckForExistenceAndExtractIndex(ref property, out index);

			//PropertyInfo propertyInfo = type.GetProperty(property, ResolveFlagsToUse(type));
			PropertyInfo propertyInfo = GetPropertyInfo(type, property);

			if (propertyInfo == null)
			{
				if (logger.IsErrorEnabled)
				{
					logger.Error("No public property '{0}' found on type '{1}'", property, type.FullName);
				}

				return null;
			}

			if (!propertyInfo.CanRead)
			{
				throw new BindingException("Property '{0}' for type '{1}' can not be read",
					propertyInfo.Name, type.FullName);
			}

			if (propertyInfo.GetIndexParameters().Length != 0)
			{
				throw new BindingException("Property '{0}' for type '{1}' has indexes, which are not supported",
					propertyInfo.Name, type.FullName);
			}

			if (action != null)
			{
				action(propertyInfo);
			}

			type = propertyInfo.PropertyType;

			if (typeof(ICollection).IsAssignableFrom(type))
			{
				return null;
			}

			if (isIndexed)
			{
				if (type.IsGenericType)
				{
					Type[] args = type.GetGenericArguments();
					if (args.Length != 1)
						throw new BindingException("Expected the generic indexed property '{0}' to be of 1 element", type.Name);
					type = args[0];
				}

				if (type.IsArray)
				{
					type = type.GetElementType();
				}
			}

			if (piece + 1 == propertyPath.Length)
			{
				return propertyInfo;
			}

			return QueryPropertyInfoRecursive(type, propertyPath, piece + 1, action);
		}

		/// <summary>
		/// Query property paths agains the rootInstance type
		/// </summary>
		/// <param name="rootInstance">the object to query</param>
		/// <param name="propertyPath">property path</param>
		/// <returns>The generated form element</returns>
		protected static object QueryPropertyRecursive(object rootInstance, string[] propertyPath)
		{
			return QueryPropertyRecursive(rootInstance, propertyPath, 0);
		}

		/// <summary>
		/// Query property paths agains the rootInstance type
		/// </summary>
		/// <param name="rootInstance">the object to query</param>
		/// <param name="propertyPath">property path</param>
		/// <param name="piece">start index</param>
		/// <returns>The generated form element</returns>
		protected static object QueryPropertyRecursive(object rootInstance, string[] propertyPath, int piece)
		{
			string property = propertyPath[piece];
			int index;

			Type instanceType = rootInstance.GetType();

			bool isIndexed = CheckForExistenceAndExtractIndex(ref property, out index);

			//PropertyInfo propertyInfo = instanceType.GetProperty(property, ResolveFlagsToUse(instanceType));
			PropertyInfo propertyInfo = GetPropertyInfo(instanceType, property);

			object instance = null;

			if (propertyInfo == null)
			{
				FieldInfo fieldInfo = instanceType.GetField(property, FieldFlags);

				if (fieldInfo != null)
				{
					instance = fieldInfo.GetValue(rootInstance);
				}
			}
			else
			{
				if (!propertyInfo.CanRead)
				{
					throw new BindingException("Property '{0}' for type '{1}' can not be read",
						propertyInfo.Name, instanceType.FullName);
				}

				if (propertyInfo.GetIndexParameters().Length != 0)
				{
					throw new BindingException("Property '{0}' for type '{1}' has indexes, which are not supported",
						propertyInfo.Name, instanceType.FullName);
				}

				instance = propertyInfo.GetValue(rootInstance, null);
			}

			if (isIndexed && instance != null)
			{
				AssertIsValidArray(instance, property, index);

				instance = GetArrayElement(instance, index);
			}

			if (instance == null || piece + 1 == propertyPath.Length)
			{
				return instance;
			}

			return QueryPropertyRecursive(instance, propertyPath, piece + 1);
		}

		/// <summary>
		/// Creates the HTML id.
		/// </summary>
		/// <param name="attributes">The attributes.</param>
		/// <param name="target">The target.</param>
		/// <returns>The generated form element</returns>
		protected static string CreateHtmlId(IDictionary attributes, string target)
		{
			return CreateHtmlId(attributes, target, true);
		}

		/// <summary>
		/// Creates the HTML id.
		/// </summary>
		/// <param name="attributes">The attributes.</param>
		/// <param name="target">The target.</param>
		/// <param name="removeEntry">if set to <c>true</c> [remove entry].</param>
		/// <returns>The generated form element</returns>
		protected static string CreateHtmlId(IDictionary attributes, string target, bool removeEntry)
		{
			string id;

			if (removeEntry)
			{
				id = CommonUtils.ObtainEntryAndRemove(attributes, "id");
			}
			else
			{
				id = CommonUtils.ObtainEntry(attributes, "id");
			}

			if (id == null)
			{
				id = CreateHtmlId(target);
			}

			return id;
		}

		#endregion


		#region Validation

		/// <summary>
		/// Disables the validation.
		/// </summary>
		public virtual void DisableValidation()
		{
			isValidationDisabled = true;
		}

		/// <summary>
		/// Applies the validation.
		/// </summary>
		/// <param name="inputType">Type of the input.</param>
		/// <param name="target">The target.</param>
		/// <param name="attributes">The attributes.</param>
		protected virtual void ApplyValidation(InputElementType inputType, string target, ref IDictionary attributes)
		{
			bool disableValidation = CommonUtils.ObtainEntryAndRemove(attributes, "disablevalidation", "false") == "true";

			if (!IsValidationEnabled || disableValidation)
			{
				return;
			}

			if (validatorRegistry == null || validationConfig == null)
			{
				return;
			}

			if (attributes == null)
			{
				attributes = new HybridDictionary(true);
			}

			IValidator[] validators = CollectValidators(RequestContext.All, target);

			IBrowserValidationGenerator generator = validatorProvider.CreateGenerator(validationConfig, inputType, attributes);

			string id = CreateHtmlId(attributes, target, false);

			foreach (IValidator validator in validators)
			{
				if (validator.SupportsBrowserValidation)
				{
					validator.ApplyBrowserValidation(validationConfig, inputType, generator, attributes, id);
				}
			}
		}

		private IValidator[] CollectValidators(RequestContext requestContext, string target)
		{
			List<IValidator> validators = new List<IValidator>();

			ObtainTargetProperty(requestContext, target, delegate(PropertyInfo property)
			{
				validators.AddRange(validatorRegistry.GetValidators(validatorRunner, property.DeclaringType, property, RunWhen.Everytime));
			});

			return validators.ToArray();
		}

		/// <summary>
		/// Gets a value indicating whether validation is enabled.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if validation is enabled; otherwise, <c>false</c>.
		/// </value>
		protected bool IsValidationEnabled
		{
			get
			{
				if (isValidationDisabled)
					return false;

				if (objectStack.Count == 0)
					return true;

				return ((FormScopeInfo) objectStack.Peek()).IsValidationEnabled;
			}
		}

		#endregion

		#region private helpers

		private static void AssertIsValidArray(object instance, string property, int index)
		{
			Type instanceType = instance.GetType();

			IList list = instance as IList;

			bool validList = false;

			if (list == null )
			{
				Type genericType = instanceType;

				do
				{
					if ( genericType.IsGenericType )
					{
						Type[] genArgs = instanceType.GetGenericArguments();

						Type genList = typeof( System.Collections.Generic.IList<> ).MakeGenericType( genArgs );
						Type genTypeDef = instanceType.GetGenericTypeDefinition().MakeGenericType( genArgs );

						validList = genList.IsAssignableFrom( genTypeDef );
					}
					else
						genericType = genericType.BaseType;
				}while ( genericType.BaseType != typeof( object ) );

			}

			if (!validList && list == null)
			{
				throw new MonoRailException("The property {0} is being accessed as " +
					"an indexed property but does not seem to implement IList. " +
					"In fact the type is {1}", property, instanceType.FullName);
			}

			if (index < 0)
			{
				throw new MonoRailException("The specified index '{0}' is outside the bounds " +
					"of the array. Property {1}", index, property);
			}
		}

		private static object GetArrayElement(object instance, int index)
		{
			IList list = instance as IList;

			if (list == null && instance != null && instance.GetType().IsGenericType)
			{
				Type instanceType = instance.GetType();

				Type[] genArguments = instanceType.GetGenericArguments();

				Type genType = instanceType.GetGenericTypeDefinition().MakeGenericType(genArguments);

				// I'm not going to retest for IList implementation as 
				// if we got here, the AssertIsValidArray has run successfully

				PropertyInfo countPropInfo = genType.GetProperty("Count");

				int count = (int) countPropInfo.GetValue(instance, null);

				if (count == 0 || index + 1 > count)
				{
					return null;
				}

				PropertyInfo indexerPropInfo = genType.GetProperty("Item");

				return indexerPropInfo.GetValue(instance, new object[] { index });
			}

			if (list == null || list.Count == 0 || index + 1 > list.Count)
			{
				return null;
			}

			return list[index];
		}

		private static bool CheckForExistenceAndExtractIndex(ref string property, out int index)
		{
			bool isIndexed = property.IndexOf('[') != -1;

			index = -1;

			if (isIndexed)
			{
				int start = property.IndexOf('[') + 1;
				int len = property.IndexOf(']', start) - start;

				string indexStr = property.Substring(start, len);

				try
				{
					index = Convert.ToInt32(indexStr);
				}
				catch (Exception)
				{
					throw new MonoRailException("Could not convert (param {0}) index to Int32. Value is {1}",
						property, indexStr);
				}

				property = property.Substring(0, start - 1);
			}

			return isIndexed;
		}

		/// <summary>
		/// Compares the left and right value for equality.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <returns></returns>
		protected static bool AreEqual(object left, object right)
		{
			if (left == null || right == null)
				return false;

			if (left is string && right is String)
			{
				return String.Compare(left.ToString(), right.ToString()) == 0;
			}

			if (left.GetType() == right.GetType())
			{
				return right.Equals(left);
			}

			IConvertible convertible = left as IConvertible;

			if (convertible != null)
			{
				try
				{
					object newleft = convertible.ToType(right.GetType(), null);
					return (newleft.Equals(right));
				}
				catch (Exception)
				{
					// Do nothing
				}
			}

			return left.ToString().Equals(right.ToString());
		}

		/// <summary>
		/// Encodes the content if there is a valid context set for this helper.
		/// </summary>
		/// <param name="content">The content to be encoded.</param>
		/// <returns></returns>
		protected string SafeHtmlEncode(string content)
		{
			if (Context != null)
			{
				return HtmlEncode(content);
			}

			return content;
		}

		/// <summary>
		/// Determines whether the present value matches the value on 
		/// the initialSetValue (which can be a single value or a set)
		/// </summary>
		/// <param name="value">Value from the datasource</param>
		/// <param name="initialSetValue">Value from the initial selection set</param>
		/// <param name="propertyOnInitialSet">Optional. Property to obtain the value from</param>
		/// <param name="isMultiple"><c>true</c> if the initial selection is a set</param>
		/// <returns><c>true</c> if it's selected</returns>
		protected internal static bool IsPresent(object value, object initialSetValue,
												 ValueGetter propertyOnInitialSet, bool isMultiple)
		{
			if (!isMultiple)
			{
				object valueToCompare = initialSetValue;

				if (propertyOnInitialSet != null)
				{
					// propertyOnInitialSet.GetValue(initialSetValue, null);
					valueToCompare = propertyOnInitialSet.GetValue(initialSetValue);
				}

				return AreEqual(value, valueToCompare);
			}
			else
			{
				foreach (object item in (IEnumerable) initialSetValue)
				{
					object valueToCompare = item;

					if (propertyOnInitialSet != null)
					{
						// valueToCompare = propertyOnInitialSet.GetValue(item, null);
						valueToCompare = propertyOnInitialSet.GetValue(item);
					}

					if (AreEqual(value, valueToCompare))
					{
						return true;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Adds the checked attribute to the attributes.
		/// </summary>
		/// <param name="attributes">The attributes.</param>
		protected static void AddChecked(IDictionary attributes)
		{
			attributes["checked"] = "checked";
		}

		/// <summary>
		/// Removes the checked attribute from the dictionary.
		/// </summary>
		/// <param name="attributes">The attributes.</param>
		protected static void RemoveChecked(IDictionary attributes)
		{
			attributes.Remove("checked");
		}

		/// <summary>
		/// Uses the specified name to create a valid id for the element
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		protected static string CreateHtmlId(string name)
		{
			StringBuilder sb = new StringBuilder(name.Length);

			bool canUseUnderline = false;

			foreach (char c in name.ToCharArray())
			{
				switch (c)
				{
					case '.':
					case '[':
					case ']':
						if (canUseUnderline)
						{
							sb.Append('_');
							canUseUnderline = false;
						}
						break;
					default:
						canUseUnderline = true;
						sb.Append(c);
						break;
				}

			}

			return sb.ToString();
		}

		/// <summary>
		/// Abstracts the approach to access values on objects. 
		/// </summary>
		public abstract class ValueGetter
		{
			/// <summary>
			/// Gets the name.
			/// </summary>
			/// <value>The name.</value>
			public abstract string Name { get; }

			/// <summary>
			/// Gets the value.
			/// </summary>
			/// <param name="instance">The instance.</param>
			/// <returns></returns>
			public abstract object GetValue(object instance);
		}

		/// <summary>
		/// Implementation of <see cref="ValueGetter"/>
		/// that uses reflection to access values
		/// </summary>
		public class ReflectionValueGetter : ValueGetter
		{
			private PropertyInfo propInfo;

			/// <summary>
			/// Initializes a new instance of the <see cref="ReflectionValueGetter"/> class.
			/// </summary>
			/// <param name="propInfo">The prop info.</param>
			public ReflectionValueGetter(PropertyInfo propInfo)
			{
				this.propInfo = propInfo;
			}

			/// <summary>
			/// Gets the name.
			/// </summary>
			/// <value>The name.</value>
			public override string Name
			{
				get { return propInfo.Name; }
			}

			/// <summary>
			/// Gets the value.
			/// </summary>
			/// <param name="instance">The instance.</param>
			/// <returns></returns>
			public override object GetValue(object instance)
			{
				try
				{
					return propInfo.GetValue(instance, null);
				}
				catch (TargetException)
				{
					PropertyInfo tempProp = instance.GetType().GetProperty(Name);

					if (tempProp == null)
					{
						throw;
					}

					return tempProp.GetValue(instance, null);
				}
			}
		}

		/// <summary>
		/// Implementation of <see cref="ValueGetter"/>
		/// that uses reflection and recusion to access values
		/// </summary>
		public class RecursiveReflectionValueGetter : ValueGetter
		{
			private readonly string[] keyName;
			private readonly string name = string.Empty;

			/// <summary>
			/// Initializes a new instance of the <see cref="RecursiveReflectionValueGetter"/> class.
			/// </summary>
			/// <param name="targetType">The target type to query</param>
			/// <param name="keyName">the property path</param>
			public RecursiveReflectionValueGetter(Type targetType, string keyName)
			{
				this.keyName = keyName.Split('.');
				name = QueryPropertyInfoRecursive(targetType, this.keyName).Name;
			}

			/// <summary>
			/// Gets the name.
			/// </summary>
			/// <value>The name.</value>
			public override string Name
			{
				get { return name; }
			}

			/// <summary>
			/// Gets the value.
			/// </summary>
			/// <param name="instance">The instance.</param>
			/// <returns></returns>
			public override object GetValue(object instance)
			{
				try
				{
					return QueryPropertyRecursive(instance, keyName);
				}
				catch (TargetException)
				{
					PropertyInfo tempProp = instance.GetType().GetProperty(Name);

					if (tempProp == null)
					{
						throw;
					}

					return tempProp.GetValue(instance, null);
				}
			}
		}

		/// <summary>
		/// Implementation of <see cref="ValueGetter"/>
		/// to access DataRow's value
		/// </summary>
		public class DataRowValueGetter : ValueGetter
		{
			private readonly string columnName;

			/// <summary>
			/// Initializes a new instance of the <see cref="DataRowValueGetter"/> class.
			/// </summary>
			/// <param name="columnName">Name of the column.</param>
			public DataRowValueGetter(string columnName)
			{
				this.columnName = columnName;
			}

			/// <summary>
			/// Gets the name.
			/// </summary>
			/// <value>The name.</value>
			public override string Name
			{
				get { return columnName; }
			}

			/// <summary>
			/// Gets the value.
			/// </summary>
			/// <param name="instance">The instance.</param>
			/// <returns></returns>
			public override object GetValue(object instance)
			{
				DataRow row = (DataRow) instance;

				return row[columnName];
			}
		}

		/// <summary>
		/// Implementation of <see cref="ValueGetter"/>
		/// to access DataRowView's value
		/// </summary>
		public class DataRowViewValueGetter : ValueGetter
		{
			private readonly string columnName;

			/// <summary>
			/// Initializes a new instance of the <see cref="DataRowViewValueGetter"/> class.
			/// </summary>
			/// <param name="columnName">Name of the column.</param>
			public DataRowViewValueGetter(string columnName)
			{
				this.columnName = columnName;
			}

			/// <summary>
			/// Gets the name.
			/// </summary>
			/// <value>The name.</value>
			public override string Name
			{
				get { return columnName; }
			}

			/// <summary>
			/// Gets the value.
			/// </summary>
			/// <param name="instance">The instance.</param>
			/// <returns></returns>
			public override object GetValue(object instance)
			{
				DataRowView row = (DataRowView) instance;

				return row[columnName];
			}
		}

		/// <summary>
		/// Empty implementation of a <see cref="ValueGetter"/>
		/// </summary>
		public class NoActionGetter : ValueGetter
		{
			/// <summary>
			/// Gets the name.
			/// </summary>
			/// <value>The name.</value>
			public override string Name
			{
				get { return string.Empty; }
			}

			/// <summary>
			/// Gets the value.
			/// </summary>
			/// <param name="instance">The instance.</param>
			/// <returns></returns>
			public override object GetValue(object instance)
			{
				return null;
			}
		}

		/// <summary>
		/// Implementation of <see cref="ValueGetter"/>
		/// to access enum fields
		/// </summary>
		public class EnumValueGetter : ValueGetter
		{
			private readonly Type enumType;

			/// <summary>
			/// Initializes a new instance of the <see cref="EnumValueGetter"/> class.
			/// </summary>
			/// <param name="enumType">Type of the enum.</param>
			public EnumValueGetter(Type enumType)
			{
				this.enumType = enumType;
			}

			/// <summary>
			/// Gets the name.
			/// </summary>
			/// <value>The name.</value>
			public override string Name
			{
				get { return string.Empty; }
			}

			/// <summary>
			/// Gets the value.
			/// </summary>
			/// <param name="instance">The instance.</param>
			/// <returns></returns>
			public override object GetValue(object instance)
			{
				return Convert.ToDecimal(Enum.Format(enumType, Enum.Parse(enumType, Convert.ToString(instance)), "d"));
			}
		}

		/// <summary>
		/// Abstract factory for <see cref="ValueGetter"/> implementations
		/// </summary>
		public class ValueGetterAbstractFactory
		{
			/// <summary>
			/// Creates the specified target type.
			/// </summary>
			/// <param name="targetType">Type of the target.</param>
			/// <param name="keyName">Name of the key.</param>
			/// <returns></returns>
			public static ValueGetter Create(Type targetType, string keyName)
			{
				if (targetType == null)
				{
					return new NoActionGetter();
				}
				else if (targetType == typeof(DataRow))
				{
					return new DataRowValueGetter(keyName);
				}
				else if (targetType == typeof(DataRowView))
				{
					return new DataRowViewValueGetter(keyName);
				}
				else if (targetType.IsEnum)
				{
					return new EnumValueGetter(targetType);
				}
				else
				{
					PropertyInfo info = null;

					// check for recursion
					if (keyName.Contains("."))
					{
						info = QueryPropertyInfoRecursive(targetType, keyName.Split('.'));

						if (info != null)
						{
							return new RecursiveReflectionValueGetter(targetType, keyName);
						}
					}
					else
					{
						info = GetPropertyInfo(targetType, keyName);

						if (info != null)
						{
							return new ReflectionValueGetter(info);
						}
					}

					return null;
				}
			}
		}

		/// <summary>
		/// Gets the property info for the property with the <paramref name="propertyName"/>.
		/// </summary>
		/// <param name="type">Type that has the property.</param>
		/// <param name="propertyName">Name of the property.</param>
		/// <returns></returns>
		protected static PropertyInfo GetPropertyInfo(Type type, string propertyName)
		{
			PropertyInfo info = null;

			try
			{
				info = type.GetProperty(propertyName, ResolveFlagsToUse(type));
			}
			catch (AmbiguousMatchException amex)
			{
				// This is kind of an edge case, so tried it the normal way first,
				// seems to happen if you override a generic property

				if (logger.IsDebugEnabled)
				{
					logger.DebugFormat(amex, "Retrieving property {0} on type {1} raised a {2}. Maybe it is generic and overriden. Will try to get the most specific property now.",
						propertyName, type, amex.GetType().Name);
				}

				// Try again on instance only, loop through base type hierarchy
				Type baseType = type;
				while (info == null)
				{
					info = baseType.GetProperty(propertyName, propertyFlagsDeclaredOnly);

					baseType = baseType.BaseType;
					if (baseType == typeof(object) || baseType == null)
					{
						break;
					}
				}
			}

			return info;
		}

		private static BindingFlags ResolveFlagsToUse(Type type)
		{
			if (type.Assembly.FullName.StartsWith("DynamicAssemblyProxyGen") || type.Assembly.FullName.StartsWith("DynamicProxyGenAssembly2"))
			{
				return propertyFlagsDeclaredOnly;
			}

			return PropertyFlags;
		}

		#endregion

		#region FormScopeInfo

		/// <summary>
		/// Groups validation enabled configuration for a target set
		/// </summary>
		protected class FormScopeInfo
		{
			private readonly string target;
			private readonly bool isValidationEnabled;

			/// <summary>
			/// Initializes a new instance of the <see cref="FormScopeInfo"/> class.
			/// </summary>
			/// <param name="target">The target.</param>
			/// <param name="isValidationEnabled">if set to <c>true</c> [is validation enabled].</param>
			public FormScopeInfo(string target, bool isValidationEnabled)
			{
				this.target = target;
				this.isValidationEnabled = isValidationEnabled;
			}

			/// <summary>
			/// Gets the root target.
			/// </summary>
			/// <value>The root target.</value>
			public string RootTarget
			{
				get { return target; }
			}

			/// <summary>
			/// Gets a value indicating whether this instance is validation enabled.
			/// </summary>
			/// <value>
			/// 	<c>true</c> if this instance is validation enabled; otherwise, <c>false</c>.
			/// </value>
			public bool IsValidationEnabled
			{
				get { return isValidationEnabled; }
			}
		}

		#endregion
	}
}
