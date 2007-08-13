// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.ActiveRecordSupport
{
	using System;
	using System.Collections;
	using System.Reflection;
	using Castle.ActiveRecord;
	using Castle.ActiveRecord.Framework.Internal;
	using Castle.Components.Binder;
	using Iesi.Collections;
	using NHibernate.Property;
	using Iesi.Collections.Generic;
	using System.Collections.Generic;

	/// <summary>
	/// Extends <see cref="DataBinder"/> class with some 
	/// ActiveRecord specific functionality.
	/// <seealso cref="AutoLoadBehavior"/>
	/// <seealso cref="ARDataBindAttribute"/>
	/// </summary>
	/// <remarks>
	/// Autoload can be turned <i>on</i> on the parameter, see <see cref="AutoLoadBehavior"/>.
	/// </remarks>
	public class ARDataBinder : DataBinder
	{
		protected internal static readonly object[] EmptyArg = new object[0];

		private AutoLoadBehavior autoLoad;
		private bool persistchanges;
		private string[] expectCollPropertiesList;
		private Stack<ActiveRecordModel> modelStack = new Stack<ActiveRecordModel>();

		/// <summary>
		/// Gets or sets a value indicating if the changes should be persisted.
		/// </summary>
		/// <value><c>true</c> if the changes should be persisted; otherwise, <c>false</c>.</value>
		public bool PersistChanges
		{
			get { return persistchanges; }
			set { persistchanges = value; }
		}

		/// <summary>
		/// Gets or sets the <see cref="AutoLoadBehavior"/>.
		/// </summary>
		/// <value>The auto load behavior.</value>
		public AutoLoadBehavior AutoLoad
		{
			get { return autoLoad; }
			set { autoLoad = value; }
		}

		protected override void PushInstance(object instance, string prefix)
		{
			ActiveRecordModel model = ActiveRecordModel.GetModel(instance.GetType());

			if (model == null && modelStack.Count != 0)
			{
				foreach(NestedModel nestedModel in CurrentARModel.Components)
				{
					if (string.Compare(nestedModel.Property.Name, prefix, true) == 0)
					{
						model = nestedModel.Model;
						break;
					}
				}
			}

			if (model != null)
			{
				modelStack.Push(model);
			}

			base.PushInstance(instance, prefix);
		}

		protected override void PopInstance(object instance, string prefix)
		{
			ActiveRecordModel model = ActiveRecordModel.GetModel(instance.GetType());

			if (model == null && CurrentARModel != null && CurrentARModel.IsNestedType)
			{
				modelStack.Pop();
			}

			if (model != null)
			{
				ActiveRecordModel actualModel = modelStack.Pop();
				
				if (actualModel != model)
				{
					throw new BindingException("Unexpected ARModel on the stack: found {0}, expecting {1}", 
						actualModel.ToString(), model.ToString());
				}
			}

			base.PopInstance(instance, prefix);
		}

		/// <summary>
		/// Gets the current AR model.
		/// </summary>
		/// <value>The current AR model.</value>
		protected ActiveRecordModel CurrentARModel
		{
			get
			{
				return modelStack.Count == 0 ? null : modelStack.Peek();
			}
		}

		public object BindObject(Type targetType, string prefix, string exclude, string allow, string expect,
								 CompositeNode treeRoot)
		{
			expectCollPropertiesList = CreateNormalizedList(expect);

			return BindObject(targetType, prefix, exclude, allow, treeRoot);
		}

		protected override object CreateInstance(Type instanceType, String paramPrefix, Node node)
		{
			if (node == null)
			{
				throw new BindingException(
					"Nothing found for the given prefix. Are you sure the form fields are using the prefix " +
					paramPrefix + "?");
			}

			if (node.NodeType != NodeType.Composite)
			{
				throw new BindingException("Unexpected node type. Expecting Composite, found " + node.NodeType);
			}

			CompositeNode cNode = (CompositeNode) node;

			object instance;

			bool shouldLoad = autoLoad != AutoLoadBehavior.Never;

			if (autoLoad == AutoLoadBehavior.OnlyNested)
			{
				shouldLoad = StackDepth != 0;
			}

			if (shouldLoad)
			{
				if (instanceType.IsArray)
				{
					throw new BindingException("ARDataBinder AutoLoad does not support arrays");
				}

				ActiveRecordModel model = ActiveRecordModel.GetModel(instanceType);

				PrimaryKeyModel pkModel;

				object id = ObtainPrimaryKeyValue(model, cNode, paramPrefix, out pkModel);

				if (IsValidKey(id))
				{
					instance = ActiveRecordMediator.FindByPrimaryKey(instanceType, id, true);
				}
				else
				{
					if (autoLoad == AutoLoadBehavior.NewInstanceIfInvalidKey ||
						(autoLoad == AutoLoadBehavior.NewRootInstanceIfInvalidKey && StackDepth == 0))
					{
						instance = base.CreateInstance(instanceType, paramPrefix, node);
					}
					else if (autoLoad == AutoLoadBehavior.NullIfInvalidKey ||
							 autoLoad == AutoLoadBehavior.OnlyNested ||
							 (autoLoad == AutoLoadBehavior.NewRootInstanceIfInvalidKey && StackDepth != 0))
					{
						instance = null;
					}
					else
					{
						throw new BindingException(string.Format(
													   "Could not find primary key '{0}' for '{1}'",
													   pkModel.Property.Name, instanceType.FullName));
					}
				}
			}
			else
			{
				instance = base.CreateInstance(instanceType, paramPrefix, node);
			}

			return instance;
		}

		/// <summary>
		/// for joined subclasses HasAndBelongsToMany properties doesn't include the ones of the parent class
		/// so we need to check them recursively
		/// </summary>
		protected bool FindPropertyInHasAndBelongsToMany(ActiveRecordModel model, string propertyName,
														 ref Type foundType, ref ActiveRecordModel foundModel)
		{
			foreach(HasAndBelongsToManyModel hasMany2ManyModel in model.HasAndBelongsToMany)
			{
				// Inverse=true relations will be ignored
				if (hasMany2ManyModel.Property.Name == propertyName && !hasMany2ManyModel.HasManyAtt.Inverse)
				{
					foundType = hasMany2ManyModel.HasManyAtt.MapType;
					foundModel = ActiveRecordModel.GetModel(foundType);
					return true;
				}
			}
			if (model.IsJoinedSubClass || model.IsDiscriminatorSubClass)
			{
				return FindPropertyInHasAndBelongsToMany(model.Parent, propertyName, ref foundType, ref foundModel);
			}
			return false;
		}

		/// <summary>
		/// for joined subclasses HasMany properties doesn't include the ones of the parent class
		/// so we need to check them recursively
		/// </summary>
		protected bool FindPropertyInHasMany(ActiveRecordModel model, string propertyName,
											 ref Type foundType, ref ActiveRecordModel foundModel)
		{
			foreach(HasManyModel hasManyModel in model.HasMany)
			{
				// Inverse=true relations will be ignored
				if (hasManyModel.Property.Name == propertyName && !hasManyModel.HasManyAtt.Inverse)
				{
					foundType = hasManyModel.HasManyAtt.MapType;
					foundModel = ActiveRecordModel.GetModel(foundType);
					return true;
				}
			}
			if (model.IsJoinedSubClass || model.IsDiscriminatorSubClass)
			{
				return FindPropertyInHasMany(model.Parent, propertyName, ref foundType, ref foundModel);
			}
			return false;
		}

		protected override object BindSpecialObjectInstance(Type instanceType, string prefix, Node node,
															out bool succeeded)
		{
			succeeded = false;

			ActiveRecordModel model = CurrentARModel;

			if (model == null)
			{
				return null;
			}

			object container = CreateContainer(instanceType);

			bool found = false;
			Type targetType = null;
			ActiveRecordModel targetModel = null;

			found = FindPropertyInHasAndBelongsToMany(model, prefix, ref targetType, ref targetModel);

			if (!found)
			{
				found = FindPropertyInHasMany(model, prefix, ref targetType, ref targetModel);
			}

			if (found)
			{
				succeeded = true;

				ClearContainer(container);

				if (node.NodeType == NodeType.Indexed)
				{
					IndexedNode indexNode = (IndexedNode) node;

					Array collArray = Array.CreateInstance(targetType, indexNode.ChildrenCount);

					collArray = (Array) InternalBindObject(collArray.GetType(), prefix, node);

					foreach(object item in collArray)
					{
						AddToContainer(container, item);
					}
				}
				else if (node.NodeType == NodeType.Leaf)
				{
					PrimaryKeyModel pkModel = targetModel.PrimaryKey;
					Type pkType = pkModel.Property.PropertyType;

					LeafNode leafNode = (LeafNode) node;

					bool convSucceeded;

					if (leafNode.IsArray) // Multiples values found
					{
						foreach(object element in (Array) leafNode.Value)
						{
							object keyConverted = Converter.Convert(pkType, leafNode.ValueType.GetElementType(),
																	element, out convSucceeded);

							if (convSucceeded)
							{
								object item = ActiveRecordMediator.FindByPrimaryKey(targetType, keyConverted, true);
								AddToContainer(container, item);
							}
						}
					}
					else // Single value found
					{
						object keyConverted = Converter.Convert(pkType, leafNode.ValueType.GetElementType(),
																leafNode.Value, out convSucceeded);

						if (convSucceeded)
						{
							object item = ActiveRecordMediator.FindByPrimaryKey(targetType, keyConverted, true);
							AddToContainer(container, item);
						}
					}
				}
			}

			return container;
		}

		protected override bool IsSpecialType(Type instanceType)
		{
			return IsContainerType(instanceType);
		}

		protected override void SetPropertyValue(object instance, PropertyInfo prop, object value)
		{
			object[] attributes = prop.GetCustomAttributes(typeof(WithAccessAttribute), false);

			if (attributes.Length == 0)
			{
				base.SetPropertyValue(instance, prop, value);
				return;
			}

			WithAccessAttribute accessAttribute = (WithAccessAttribute) attributes[0];
			IPropertyAccessor propertyAccessor;

			switch(accessAttribute.Access)
			{
				case PropertyAccess.Property:
					propertyAccessor = PropertyAccessorFactory.GetPropertyAccessor("property");
					break;
				case PropertyAccess.Field:
					propertyAccessor = PropertyAccessorFactory.GetPropertyAccessor("field");
					break;
				case PropertyAccess.FieldCamelcase:
					propertyAccessor = PropertyAccessorFactory.GetPropertyAccessor("field.camelcase");
					break;
				case PropertyAccess.FieldCamelcaseUnderscore:
					propertyAccessor = PropertyAccessorFactory.GetPropertyAccessor("field.camelcase-underscore");
					break;
				case PropertyAccess.FieldPascalcaseMUnderscore:
					propertyAccessor = PropertyAccessorFactory.GetPropertyAccessor("field.pascalcase-m-underscore");
					break;
				case PropertyAccess.FieldLowercaseUnderscore:
					propertyAccessor = PropertyAccessorFactory.GetPropertyAccessor("field.lowercase-underscore");
					break;
				case PropertyAccess.NosetterCamelcase:
					propertyAccessor = PropertyAccessorFactory.GetPropertyAccessor("nosetter.camelcase");
					break;
				case PropertyAccess.NosetterCamelcaseUnderscore:
					propertyAccessor = PropertyAccessorFactory.GetPropertyAccessor("nosetter.camelcase-underscore");
					break;
				case PropertyAccess.NosetterPascalcaseMUndersc:
					propertyAccessor = PropertyAccessorFactory.GetPropertyAccessor("nosetter.pascalcase-m-underscore");
					break;
				case PropertyAccess.NosetterLowercaseUnderscore:
					propertyAccessor = PropertyAccessorFactory.GetPropertyAccessor("nosetter.lowercase-underscore");
					break;
				case PropertyAccess.NosetterLowercase:
					propertyAccessor = PropertyAccessorFactory.GetPropertyAccessor("nosetter.lowercase");
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			propertyAccessor.GetSetter(instance.GetType(), prop.Name).Set(instance, value);
		}

		/// <summary>
		/// for joined subclasses BelongsTo properties doesn't include the ones of the parent class
		/// so we need to check them recursively
		/// </summary>
		protected bool IsBelongsToRef(ActiveRecordModel arModel, string prefix)
		{
			foreach(BelongsToModel model in arModel.BelongsTo)
			{
				if (model.Property.Name == prefix)
				{
					return true;
				}
			}
			if (arModel.IsJoinedSubClass || arModel.IsDiscriminatorSubClass)
			{
				return IsBelongsToRef(arModel.Parent, prefix);
			}
			return false;
		}

		protected override bool ShouldRecreateInstance(object value, Type type, string prefix, Node node)
		{
			if (IsContainerType(type))
			{
				return true;
			}

			if (node != null && CurrentARModel != null)
			{
				// If it's a belongsTo ref, we need to recreate it 
				// instead of overwrite its properties, otherwise NHibernate will complain
				if (IsBelongsToRef(CurrentARModel, prefix))
				{
					return true;
				}
			}

			return base.ShouldRecreateInstance(value, type, prefix, node);
		}

		protected override void BeforeBindingProperty(object instance, PropertyInfo prop, string prefix,
													  CompositeNode node)
		{
			base.BeforeBindingProperty(instance, prop, prefix, node);

			if (IsPropertyExpected(prop, node))
			{
				ClearExpectedCollectionProperties(instance, prop);
			}
		}

		private bool IsPropertyExpected(PropertyInfo prop, CompositeNode node)
		{
			string propId = string.Format("{0}.{1}", node.FullName, prop.Name);

			if (expectCollPropertiesList != null)
			{
				return Array.BinarySearch(expectCollPropertiesList, propId, CaseInsensitiveComparer.Default) >= 0;
			}

			return false;
		}

		private void ClearExpectedCollectionProperties(object instance, PropertyInfo prop)
		{
			object value = prop.GetValue(instance, null);

			ClearContainer(value);
		}

		#region helpers

		private object ObtainPrimaryKeyValue(ActiveRecordModel model, CompositeNode node, String prefix,
											 out PrimaryKeyModel pkModel)
		{
			pkModel = ObtainPrimaryKey(model);

			String pkPropName = pkModel.Property.Name;

			Node idNode = node.GetChildNode(pkPropName);

			if (idNode != null && idNode.NodeType != NodeType.Leaf)
			{
				throw new BindingException("Expecting leaf node to contain id for ActiveRecord class. " +
										   "Prefix: {0} PK Property Name: {1}", prefix, pkPropName);
			}

			LeafNode lNode = (LeafNode) idNode;

			if (lNode == null)
			{
				throw new BindingException("ARDataBinder autoload failed as element {0} " +
										   "doesn't have a primary key {1} value", prefix, pkPropName);
			}

			bool conversionSuc;

			return Converter.Convert(pkModel.Property.PropertyType, lNode.ValueType, lNode.Value, out conversionSuc);
		}

		private static PrimaryKeyModel ObtainPrimaryKey(ActiveRecordModel model)
		{
			if (model.IsJoinedSubClass || model.IsDiscriminatorSubClass)
			{
				return ObtainPrimaryKey(model.Parent);
			}
			return model.PrimaryKey;
		}

		private bool IsValidKey(object id)
		{
			if (id != null)
			{
				if (id.GetType() == typeof(String))
				{
					return id.ToString() != String.Empty;
				}
				else if (id.GetType() == typeof(Guid))
				{
					return Guid.Empty != ((Guid) id);
				}
				else
				{
					return Convert.ToInt64(id) != 0;
				}
			}

			return false;
		}

		private bool IsContainerType(Type type)
		{
			bool isContainerType = type == typeof(IList) || type == typeof(ISet);

			if (!isContainerType && type.IsGenericType)
			{
				Type[] genericArgs = type.GetGenericArguments();

				Type genType = typeof(ICollection<>).MakeGenericType(genericArgs);

				isContainerType = genType.IsAssignableFrom(type);
			}

			return isContainerType;
		}

		private object CreateContainer(Type type)
		{
			if (type.IsGenericType)
			{
				if (type.GetGenericTypeDefinition() == typeof(ISet<>))
				{
					Type[] genericArgs = type.GetGenericArguments();
					Type genericType = typeof(HashedSet<>).MakeGenericType(genericArgs);
					return Activator.CreateInstance(genericType);
				}
				else if (type.GetGenericTypeDefinition() == typeof(IList<>))
				{
					Type[] genericArgs = type.GetGenericArguments();
					Type genericType = typeof(List<>).MakeGenericType(genericArgs);
					return Activator.CreateInstance(genericType);
				}
			}
			else
			{
				if (type == typeof(IList))
				{
					return new ArrayList();
				}
				else if (type == typeof(ISet))
				{
					return new HashedSet();
				}
			}
			return null;
		}

		private void ClearContainer(object instance)
		{
			if (instance is IList)
			{
				(instance as IList).Clear();
			}
			else if (instance is ISet)
			{
				(instance as ISet).Clear();
			}
		}

		private void AddToContainer(object container, object item)
		{
			if (container is IList)
			{
				(container as IList).Add(item);
			}
			else if (container is ISet)
			{
				(container as ISet).Add(item);
			}
			else if (container != null)
			{
				Type itemType = item.GetType();

				Type collectionType = typeof(ICollection<>).MakeGenericType(itemType);

				if (collectionType.IsAssignableFrom(container.GetType()))
				{
					MethodInfo addMethod = container.GetType().GetMethod("Add");

					addMethod.Invoke(container, new object[] {item});
				}
			}
		}

		#endregion
	}
}
