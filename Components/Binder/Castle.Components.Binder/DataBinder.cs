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

namespace Castle.Components.Binder
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Reflection;
	using System.Web;
	using Castle.Components.Validator;
	using Castle.Core;
	using Castle.Core.Logging;

	/// <summary>
	/// </summary>
	[Serializable]
	public class DataBinder : MarshalByRefObject, IDataBinder, IServiceEnabledComponent
	{
		protected internal static readonly BindingFlags PropertiesBindingFlags =
			BindingFlags.Instance | BindingFlags.Public;

		private IConverter converter = new DefaultConverter();

		private ValidatorRunner validator;

		/// <summary>Collect the databind errors</summary>
		protected IList errors;

		/// <summary>Holds a sorted array of properties names that should be ignored</summary>
		private string[] excludedPropertyList;

		/// <summary>Holds a sorted array of properties names that are on the white list</summary>
		private string[] allowedPropertyList;

		private Stack instanceStack;
		private Stack<string> prefixStack;

		private IDictionary<object, ErrorSummary> validationErrorSummaryPerInstance = new Dictionary<object, ErrorSummary>();

		private IBinderTranslator binderTranslator;

		private ILogger logger = NullLogger.Instance;

		public event BinderHandler OnBeforeBinding;

		public event BinderHandler OnAfterBinding;

		/// <summary>
		/// Initializes a new instance of the <see cref="DataBinder"/> class.
		/// </summary>
		public DataBinder()
		{
		}

		#region IServiceEnabledComponent

		public void Service(IServiceProvider provider)
		{
			ILoggerFactory loggerFactory = (ILoggerFactory) provider.GetService(typeof(ILoggerFactory));

			if (loggerFactory != null)
			{
				logger = loggerFactory.Create(typeof(DataBinder));
			}
		}

		#endregion

		#region IDataBinder

		public bool CanBindParameter(Type desiredType, String paramName, CompositeNode treeRoot)
		{
			bool canConvert;

			Node childNode = treeRoot.GetChildNode(paramName);

			if (childNode != null)
			{
				canConvert = true;
			}
			else if (desiredType == typeof(DateTime))
			{
				TrySpecialDateTimeBinding(desiredType, treeRoot, paramName, out canConvert);
			}
			else
			{
				canConvert = false;
			}

			return canConvert;
		}

		public bool CanBindObject(Type targetType, String prefix, CompositeNode treeRoot)
		{
			Node childNode = treeRoot.GetChildNode(prefix);

			return childNode != null;
		}

		public object BindParameter(Type desiredType, String paramName, CompositeNode treeRoot)
		{
			bool conversionSucceeded;
			object result;

			try
			{
				if (desiredType.IsArray)
				{
					Node childNode = treeRoot.GetChildNode(paramName);

					result = ConvertToArray(desiredType, paramName, childNode, out conversionSucceeded);
				}
				else
				{
					result = ConvertToSimpleValue(desiredType, paramName, treeRoot, out conversionSucceeded);
				}
			}
			catch(Exception ex)
			{
				// Something unexpected during convertion
				// throw new exception with paramName specified

				throw new BindingException(
					"Exception converting param '" + paramName + "' to " + desiredType + ". Check inner exception for details", ex);
			}

			return result;
		}

		public object BindObject(Type targetType, string prefix, CompositeNode treeRoot)
		{
			return BindObject(targetType, prefix, null, null, treeRoot);
		}

		public object BindObject(Type targetType, string prefix, string excludedProperties, string allowedProperties,
		                         CompositeNode treeRoot)
		{
			if (targetType == null)
			{
				throw new ArgumentNullException("targetType");
			}
			if (prefix == null)
			{
				throw new ArgumentNullException("prefix");
			}
			if (treeRoot == null)
			{
				throw new ArgumentNullException("treeRoot");
			}

			errors = new ArrayList();
			instanceStack = new Stack();
			prefixStack = new Stack<string>();

			excludedPropertyList = CreateNormalizedList(excludedProperties);
			allowedPropertyList = CreateNormalizedList(allowedProperties);

			return InternalBindObject(targetType, prefix, treeRoot.GetChildNode(prefix));
		}

		public void BindObjectInstance(object instance, string prefix, CompositeNode treeRoot)
		{
			BindObjectInstance(instance, prefix, null, null, treeRoot);
		}

		public void BindObjectInstance(object instance, string prefix, string excludedProperties, string allowedProperties,
		                               CompositeNode treeRoot)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			if (prefix == null)
			{
				throw new ArgumentNullException("prefix");
			}
			if (treeRoot == null)
			{
				throw new ArgumentNullException("treeRoot");
			}

			errors = new ArrayList();
			instanceStack = new Stack();
			prefixStack = new Stack<string>();

			excludedPropertyList = CreateNormalizedList(excludedProperties);
			allowedPropertyList = CreateNormalizedList(allowedProperties);

			InternalRecursiveBindObjectInstance(instance, prefix, treeRoot.GetChildNode(prefix));
		}

		/// <summary>
		/// Represents the databind errors
		/// </summary>
		public ErrorList ErrorList
		{
			get { return new ErrorList(errors); }
		}

		public IConverter Converter
		{
			get { return converter; }
			set { converter = value; }
		}

		public ValidatorRunner Validator
		{
			get { return validator; }
			set { validator = value; }
		}

		public IBinderTranslator Translator
		{
			get { return binderTranslator; }
			set { binderTranslator = value; }
		}

		/// <summary>
		/// Gets the validation error summary.
		/// </summary>
		/// <param name="instance">The instance.</param>
		public ErrorSummary GetValidationSummary(object instance)
		{
			return validationErrorSummaryPerInstance.ContainsKey(instance) ? validationErrorSummaryPerInstance[instance] : null;
		}

		#endregion

		#region Implementation

		protected object InternalBindObject(Type instanceType, String paramPrefix, Node node)
		{
			bool succeeded;
			return InternalBindObject(instanceType, paramPrefix, node, out succeeded);
		}

		protected object InternalBindObject(Type instanceType, String paramPrefix, Node node, out bool succeeded)
		{
			succeeded = false;

			if (IsSpecialType(instanceType))
			{
				return BindSpecialObjectInstance(instanceType, paramPrefix, node, out succeeded);
			}

			if (ShouldIgnoreType(instanceType))
			{
				return null;
			}

			if (instanceType.IsArray)
			{
				return InternalBindObjectArray(instanceType, paramPrefix, node, out succeeded);
			}
			else if (IsGenericList(instanceType))
			{
				return InternalBindGenericList(instanceType, paramPrefix, node, out succeeded);
			}
			else
			{
				succeeded = true;
				object instance = CreateInstance(instanceType, paramPrefix, node);
				InternalRecursiveBindObjectInstance(instance, paramPrefix, node);
				return instance;
			}
		}

		protected void InternalRecursiveBindObjectInstance(object instance, String prefix, Node node)
		{
			if (node == null)
			{
				return;
			}

			if (node.NodeType != NodeType.Composite && node.NodeType != NodeType.Indexed)
			{
				throw new BindingException(
					"Non-composite node passed to InternalRecursiveBindObjectInstance while binding {0} with prefix {1}", instance,
					prefix);
			}

			InternalRecursiveBindObjectInstance(instance, prefix, (CompositeNode) node);
		}

		protected void InternalRecursiveBindObjectInstance(object instance, String prefix, CompositeNode node)
		{
			if (node == null || instance == null)
			{
				return;
			}

			BeforeBinding(instance, prefix, node);

			if (PerformCustomBinding(instance, prefix, node))
			{
				return;
			}

			PushInstance(instance, prefix);

			ErrorSummary summary = new ErrorSummary();

			validationErrorSummaryPerInstance[instance] = summary;

			Type instanceType = instance.GetType();

			PropertyInfo[] props = instanceType.GetProperties(PropertiesBindingFlags);

			string nodeFullName = node.FullName;

			foreach(PropertyInfo prop in props)
			{
				if (ShouldIgnoreProperty(prop, nodeFullName))
				{
					continue;
				}

				Type propType = prop.PropertyType;
				String paramName = prop.Name;

				String translatedParamName = Translate(instanceType, paramName);

				if (translatedParamName == null)
				{
					continue;
				}

				bool isSimpleProperty = IsSimpleProperty(propType);

				// There are some caveats by running the validators here. 
				// We should follow the validator's execution order...
				if (isSimpleProperty)
				{
					if (CheckForValidationFailures(instance, instanceType, prop, node, translatedParamName, prefix, summary))
					{
						continue;
					}
				}

				BeforeBindingProperty(instance, prop, prefix, node);

				try
				{
					bool conversionSucceeded;

					if (isSimpleProperty)
					{
						object value = ConvertToSimpleValue(propType, translatedParamName, node, out conversionSucceeded);

						if (conversionSucceeded)
						{
							SetPropertyValue(instance, prop, value);
						}
					}
					else
					{
						// if the property is an object, we look if it is already instanciated
						object value = prop.GetValue(instance, null);

						Node nestedNode = node.GetChildNode(paramName);

						if (nestedNode != null)
						{
							if (ShouldRecreateInstance(value, propType, paramName, nestedNode))
							{
								value = InternalBindObject(propType, paramName, nestedNode, out conversionSucceeded);

								if (conversionSucceeded)
								{
									SetPropertyValue(instance, prop, value);
								}
							}
							else
							{
								InternalRecursiveBindObjectInstance(value, paramName, nestedNode);
							}
						}

						CheckForValidationFailures(instance, instanceType, prop, value, translatedParamName, prefix, summary);
					}
				}
				catch(Exception ex)
				{
					errors.Add(new DataBindError(prefix, prop.Name, ex));
				}
			}

			PopInstance(instance, prefix);

			AfterBinding(instance, prefix, node);
		}

		/// <summary>
		/// Sets the property value of the object we are binding.
		/// Databinders that require different ways to access properties
		/// can override this method.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="prop"></param>
		/// <param name="value"></param>
		protected virtual void SetPropertyValue(object instance, PropertyInfo prop, object value)
		{
			if (!prop.CanWrite)
			{
				return;
			}
			prop.SetValue(instance, value, null);
		}

		protected bool CheckForValidationFailures(object instance, Type instanceType,
		                                          PropertyInfo prop, CompositeNode node,
		                                          string name, string prefix,
		                                          ErrorSummary summary)
		{
			object value = null;

			if (validator == null)
			{
				return false;
			}

			IValidator[] validators = validator.GetValidators(instanceType, prop);

			if (validators.Length != 0)
			{
				Node valNode = node.GetChildNode(name);

				if (valNode != null && valNode.NodeType == NodeType.Leaf)
				{
					value = ((LeafNode)valNode).Value;
				}

				if (value == null && IsDateTimeType(prop.PropertyType))
				{
					bool conversionSucceeded;
					value = TryGetDateWithUTCFormat(node, name, out conversionSucceeded);
				}

				if (value == null && valNode == null)
				{
					// Value was not present on the data source. Skip validation
					return false;
				}
			}

			return CheckForValidationFailures(instance, instanceType, prop, value, name, prefix, summary);
		}

		protected bool CheckForValidationFailures(object instance, Type instanceType,
		                                          PropertyInfo prop, object value,
		                                          string name, string prefix,
		                                          ErrorSummary summary)
		{
			bool hasFailure = false;

			if (validator == null)
			{
				return false;
			}

			IValidator[] validators = validator.GetValidators(instanceType, prop);

			foreach(IValidator validatorItem in validators)
			{
				if (!validatorItem.IsValid(instance, value))
				{
					string propName = validatorItem.FriendlyName ?? validatorItem.Name;

					errors.Add(new DataBindError(prefix, prop.Name, validatorItem.ErrorMessage));

					summary.RegisterErrorMessage(propName, validatorItem.ErrorMessage);
					
					hasFailure = true;
				}
			}

			return hasFailure;
		}

		protected int StackDepth
		{
			get { return instanceStack.Count; }
		}

		private string Translate(Type instanceType, string paramName)
		{
			if (binderTranslator != null)
			{
				return binderTranslator.Translate(instanceType, paramName);
			}

			return paramName;
		}

		private object InternalBindObjectArray(Type instanceType, String paramPrefix, Node node, out bool succeeded)
		{
			succeeded = false;

			if (node == null)
			{
				return Array.CreateInstance(instanceType.GetElementType(), 0);
			}

			return ConvertToArray(instanceType, paramPrefix, node, out succeeded);
		}

		internal static bool IsGenericList(Type instanceType)
		{
			if (!instanceType.IsGenericType)
			{
				return false;
			}
			if (typeof(IList).IsAssignableFrom(instanceType))
			{
				return true;
			}

			Type[] genericArgs = instanceType.GetGenericArguments();

			if (genericArgs.Length == 0)
			{
				return false;
			}
			Type listType = typeof(IList<>).MakeGenericType(genericArgs[0]);
			return listType.IsAssignableFrom(instanceType);
		}

		private object InternalBindGenericList(Type instanceType, string paramPrefix, Node node, out bool succeeded)
		{
			succeeded = false;

			if (node == null)
			{
				return CreateInstance(instanceType, paramPrefix, node);
			}

			return ConvertToGenericList(instanceType, paramPrefix, node, out succeeded);
		}

		#endregion

		#region CreateInstance

		protected virtual object CreateInstance(Type instanceType, String paramPrefix, Node node)
		{
			const BindingFlags creationFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

			return Activator.CreateInstance(instanceType, creationFlags, null, null, null);
		}

		#endregion

		#region Overridables

		protected virtual void AfterBinding(object instance, String prefix, Node node)
		{
			if (OnAfterBinding != null)
			{
				OnAfterBinding(instance, prefix, node);
			}
		}

		protected virtual void BeforeBinding(object instance, String prefix, Node node)
		{
			if (OnBeforeBinding != null)
			{
				OnBeforeBinding(instance, prefix, node);
			}
		}

		protected virtual void BeforeBindingProperty(object instance, PropertyInfo prop, string prefix, CompositeNode node)
		{
		}

		protected virtual bool ShouldRecreateInstance(object value, Type type, String prefix, Node node)
		{
			return value == null || type.IsArray || IsGenericList(type);
		}

		protected virtual bool ShouldIgnoreType(Type instanceType)
		{
			bool ignore = instanceType.IsAbstract || instanceType.IsInterface;

			if (ignore && instanceType.IsGenericType)
			{
				ignore = !IsGenericList(instanceType);
			}

			return ignore;
		}

		protected virtual bool PerformCustomBinding(object instance, string prefix, Node node)
		{
			return false;
		}

		/// <summary>
		/// Implementations will bound the instance itself.
		/// <seealso cref="IsSpecialType"/>
		/// </summary>
		/// <remarks>
		/// <seealso cref="IsSpecialType"/>
		/// </remarks>
		/// <param name="instanceType"></param>
		/// <param name="prefix"></param>
		/// <param name="node"></param>
		/// <param name="succeeded"></param>
		protected virtual object BindSpecialObjectInstance(Type instanceType, String prefix,
		                                                   Node node, out bool succeeded)
		{
			succeeded = false;

			return null;
		}

		/// <summary>
		/// Invoked during object binding to allow 
		/// subclasses to have a chance of binding the types itself.
		/// If the implementation returns <c>true</c>
		/// the binder will invoke <see cref="BindSpecialObjectInstance"/>
		/// </summary>
		/// <param name="instanceType">Type about to be bound</param>
		/// <returns><c>true</c> if subclass wants to handle binding</returns>
		protected virtual bool IsSpecialType(Type instanceType)
		{
			return false;
		}

		#endregion

		protected ILogger Logger
		{
			get { return logger; }
		}

		protected object ConvertLeafNode(Type desiredType, LeafNode lNode, out bool conversionSucceeded)
		{
			return Converter.Convert(desiredType, lNode.ValueType, lNode.Value, out conversionSucceeded);
		}

		private object ConvertToSimpleValue(Type desiredType, string key, CompositeNode parent, out bool conversionSucceeded)
		{
			conversionSucceeded = false;

			Node childNode = parent.GetChildNode(key);

			if (childNode == null && IsDateTimeType(desiredType))
			{
				return TrySpecialDateTimeBinding(desiredType, parent, key, out conversionSucceeded);
			}
			else if (childNode == null)
			{
				return null;
			}
			else if (childNode.NodeType == NodeType.Leaf)
			{
				return ConvertLeafNode(desiredType, (LeafNode) childNode, out conversionSucceeded);
			}
			else
			{
				throw new BindingException("Could not convert param as the node related " +
				                           "to the param is not a leaf node. Param {0} parent node: {1}", key, parent.Name);
			}
		}

		private object ConvertToArray(Type desiredType, String key, Node node, out bool conversionSucceeded)
		{
			Type elemType = desiredType.GetElementType();

			if (node == null)
			{
				conversionSucceeded = false;
				return Array.CreateInstance(elemType, 0);
			}
			else if (node.NodeType == NodeType.Leaf)
			{
				LeafNode leafNode = node as LeafNode;

				return Converter.Convert(desiredType, leafNode.ValueType, leafNode.Value, out conversionSucceeded);
			}
			else if (node.NodeType == NodeType.Indexed)
			{
				IndexedNode indexedNode = node as IndexedNode;

				if (IsSimpleProperty(elemType))
				{
					return ConvertFlatNodesToArray(desiredType, indexedNode.ChildNodes, out conversionSucceeded);
				}
				else
				{
					return ConvertComplexNodesToArray(desiredType, indexedNode, out conversionSucceeded);
				}
			}
			else
			{
				throw new BindingException("Could not convert param to array as the node related " +
				                           "to the param is not a leaf node nor an indexed node. Key {0}", key);
			}
		}

		private object ConvertComplexNodesToArray(Type desiredType, IndexedNode parent, out bool conversionSucceeded)
		{
			Type arrayElemType = desiredType.GetElementType();

			ArrayList validItems = ConvertComplexNodesToList(arrayElemType, parent, out conversionSucceeded);

			return conversionSucceeded ? validItems.ToArray(arrayElemType) : Array.CreateInstance(arrayElemType, 0);
		}

		private ArrayList ConvertComplexNodesToList(Type elemType, IndexedNode parent, out bool conversionSucceeded)
		{
			conversionSucceeded = true;

			ArrayList validItems = new ArrayList();

			foreach(Node node in parent.ChildNodes)
			{
				if (node.NodeType == NodeType.Composite)
				{
					CompositeNode lnode = node as CompositeNode;

					validItems.Add(InternalBindObject(elemType, parent.Name, lnode, out conversionSucceeded));

					if (!conversionSucceeded)
					{
						break;
					}
				}
			}

			return validItems;
		}

		private object ConvertFlatNodesToArray(Type desiredType, Node[] nodes, out bool conversionSucceeded)
		{
			Type arrayElemType = desiredType.GetElementType();

			ArrayList validItems = ConvertFlatNodesToList(arrayElemType, nodes, out conversionSucceeded);

			return conversionSucceeded ? validItems.ToArray(arrayElemType) : Array.CreateInstance(arrayElemType, 0);
		}

		private ArrayList ConvertFlatNodesToList(Type elemType, Node[] nodes, out bool conversionSucceeded)
		{
			conversionSucceeded = true;

			ArrayList validItems = new ArrayList();

			foreach(Node node in nodes)
			{
				if (node.Name != String.Empty)
				{
					throw new BindingException("Unexpected non-flat node found: {0}", node.Name);
				}

				if (node.NodeType == NodeType.Leaf)
				{
					LeafNode lnode = node as LeafNode;

					validItems.Add(ConvertLeafNode(elemType, lnode, out conversionSucceeded));

					if (!conversionSucceeded)
					{
						break;
					}
				}
			}

			return validItems;
		}

		private object ConvertToGenericList(Type desiredType, String key, Node node, out bool conversionSucceeded)
		{
			Type[] genericArgs = desiredType.GetGenericArguments();

			if (genericArgs.Length == 0)
			{
				throw new BindingException("Can't infer the Generics placeholders (type parameters). Key {0}.", key);
			}

			Type elemType = genericArgs[0];

			if (node == null)
			{
				conversionSucceeded = false;
				return CreateInstance(desiredType, key, node);
			}
			else if (node.NodeType == NodeType.Leaf)
			{
				LeafNode leafNode = node as LeafNode;

				return Converter.Convert(desiredType, leafNode.ValueType, leafNode.Value, out conversionSucceeded);
			}
			else if (node.NodeType == NodeType.Indexed)
			{
				IndexedNode indexedNode = node as IndexedNode;

				IList convertedNodes;

				if (IsSimpleProperty(elemType))
				{
					convertedNodes = ConvertFlatNodesToList(elemType, indexedNode.ChildNodes, out conversionSucceeded);
				}
				else
				{
					convertedNodes = ConvertComplexNodesToList(elemType, indexedNode, out conversionSucceeded);
				}

				Type desiredImplType = desiredType.IsInterface
				                       	? typeof(List<>).MakeGenericType(elemType)
				                       	: desiredType;
				IList target = (IList)CreateInstance(desiredImplType, key, node);

				foreach(object elem in convertedNodes)
				{
					if(elem!=null)
						target.Add(elem);
				}

				return target;
			}
			else
			{
				throw new BindingException("Could not convert param to generic list as the node related " +
				                           "to the param is not a leaf node nor an indexed node. Key {0}", key);
			}
		}

		private string TryGetDateWithUTCFormat(CompositeNode treeRoot, string paramName, out bool conversionSucceeded)
		{
			// YYYY-MM-DDThh:mm:ss.sTZD (eg 1997-07-16T19:20:30.45+01:00)
			string fullDateTime = "";
			conversionSucceeded = false;

			Node dayNode = treeRoot.GetChildNode(paramName + "day");
			Node monthNode = treeRoot.GetChildNode(paramName + "month");
			Node yearNode = treeRoot.GetChildNode(paramName + "year");

			Node hourNode = treeRoot.GetChildNode(paramName + "hour");
			Node minuteNode = treeRoot.GetChildNode(paramName + "minute");
			Node secondNode = treeRoot.GetChildNode(paramName + "second");

			if (dayNode != null)
			{
				int day = (int) RelaxedConvertLeafNode(typeof(int), dayNode, 0);
				int month = (int) RelaxedConvertLeafNode(typeof(int), monthNode, 0);
				int year = (int) RelaxedConvertLeafNode(typeof(int), yearNode, 0);

				fullDateTime = string.Format("{0:0000}-{1:00}-{2:00}", year, month, day);
				conversionSucceeded = true;
			}

			if (hourNode != null)
			{
				int hour = (int) RelaxedConvertLeafNode(typeof(int), hourNode, 0);
				int minute = (int) RelaxedConvertLeafNode(typeof(int), minuteNode, 0);
				int second = (int) RelaxedConvertLeafNode(typeof(int), secondNode, 0);

				fullDateTime += string.Format("T{0:00}:{1:00}:{2:00}", hour, minute, second);
				conversionSucceeded = true;
			}

			return fullDateTime == "" ? null : fullDateTime;
		}

		private object TrySpecialDateTimeBinding(Type desiredType, CompositeNode treeRoot,
		                                         String paramName, out bool conversionSucceeded)
		{
			string dateUtc = TryGetDateWithUTCFormat(treeRoot, paramName, out conversionSucceeded);

			if (dateUtc != null)
			{
				conversionSucceeded = true;

				DateTime dt = DateTime.Parse(dateUtc);

				if (desiredType.Name == "NullableDateTime")
				{
					TypeConverter typeConverter = TypeDescriptor.GetConverter(desiredType);

					return typeConverter.ConvertFrom(dateUtc);
				}
				else
				{
					return DateTime.Parse(dateUtc);
				}
			}

			conversionSucceeded = false;
			return null;
		}

		private object RelaxedConvertLeafNode(Type desiredType, Node node, object defaultValue)
		{
			if (node == null)
			{
				return defaultValue;
			}

			if (node.NodeType != NodeType.Leaf)
			{
				throw new BindingException("Expected LeafNode, found {0} named {1}", node.NodeType, node.Name);
			}

			bool conversionSucceeded;

			object result = ConvertLeafNode(desiredType, (LeafNode) node, out conversionSucceeded);

			return conversionSucceeded ? result : defaultValue;
		}

		private bool IsDateTimeType(Type desiredType)
		{
			if (desiredType == typeof(DateTime))
			{
				return true;
			}
			else if (desiredType.Name == "NullableDateTime")
			{
				return true;
			}
			else if (desiredType.Name == "Nullable`1")
			{
				Type[] args = desiredType.GetGenericArguments();

				return (args.Length == 1 && args[0] == typeof(DateTime));
			}

			return false;
		}

		#region Support methods

		protected object InstanceOnStack
		{
			get
			{
				if (instanceStack.Count == 0)
				{
					return null;
				}

				return instanceStack.Peek();
			}
		}

		protected string PrefixOnStack
		{
			get
			{
				if (prefixStack.Count == 0)
				{
					return null;
				}

				return prefixStack.Peek();
			}
		}

		protected string ParentPrefixOnStack
		{
			get
			{
				if (prefixStack.Count < 2)
				{
					return null;
				}

				return prefixStack.ToArray()[prefixStack.Count - 2];
			}
		}

		protected String[] CreateNormalizedList(String csv)
		{
			if (csv == null || csv.Trim() == String.Empty)
			{
				return null;
			}
			else
			{
				String[] list = csv.Split(',');
				NormalizeList(list);
				return list;
			}
		}

		protected virtual void PushInstance(object instance, string prefix)
		{
			instanceStack.Push(instance);
			prefixStack.Push(prefix);
		}

		protected virtual void PopInstance(object instance, string prefix)
		{
			object actual = instanceStack.Pop();

			if (actual != instance)
			{
				throw new BindingException("Unexpected item on the stack: found {0}, expecting {1}", actual, instance);
			}

			string actualPrefix = prefixStack.Pop();

			if (actualPrefix != prefix)
			{
				throw new BindingException("Unexpected prefix on the stack: found {0}, expecting {1}", actualPrefix, prefix);
			}
		}

		private void NormalizeList(String[] list)
		{
			for(int i = 0; i < list.Length; i++)
			{
				list[i] = "root." + list[i].Trim();
			}

			Array.Sort(list, CaseInsensitiveComparer.Default);
		}

		private bool ShouldIgnoreProperty(PropertyInfo prop, string nodeFullName)
		{
			bool allowed = true;
			bool disallowed = false;

			string propId = string.Format("{0}.{1}", nodeFullName, prop.Name);

			if (allowedPropertyList != null)
			{
				allowed = Array.BinarySearch(allowedPropertyList, propId, CaseInsensitiveComparer.Default) >= 0;
			}
			if (excludedPropertyList != null)
			{
				disallowed = Array.BinarySearch(excludedPropertyList, propId, CaseInsensitiveComparer.Default) >= 0;
			}

			return (!allowed) || (disallowed);
		}

		private bool IsSimpleProperty(Type propType)
		{
			if (propType.IsArray)
			{
				return false;
			}

			bool isSimple = propType.IsPrimitive ||
			                propType.IsEnum ||
			                propType == typeof(String) ||
			                propType == typeof(Guid) ||
			                propType == typeof(DateTime) ||
			                propType == typeof(Decimal) ||
			                propType == typeof(HttpPostedFile);

			if (isSimple)
			{
				return true;
			}

			TypeConverter tconverter = TypeDescriptor.GetConverter(propType);

			return tconverter.CanConvertFrom(typeof(String));
		}

		#endregion
	}
}
