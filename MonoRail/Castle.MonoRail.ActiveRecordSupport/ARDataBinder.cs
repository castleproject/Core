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

namespace Castle.MonoRail.ActiveRecordSupport
{
	using System;
	using System.Collections;

	using Castle.ActiveRecord;
	using Castle.ActiveRecord.Framework.Internal;
	using Castle.Components.Binder;
	
	using Iesi.Collections;

	/// <summary>
	/// Extends <see cref="DataBinder"/> class with some 
	/// ActiveRecord specific functionality.
	/// <seealso cref="AutoLoadBehavior"/>
	/// <seealso cref="ARDataBindAttribute"/>
	/// </summary>
	/// <remarks>
	/// Autoload can be turned on on the parameter, see <see cref="AutoLoadBehavior"/>.
	/// </remarks>
	public class ARDataBinder : DataBinder
	{
		protected internal static readonly object[] EmptyArg = new object[0];

		private AutoLoadBehavior autoLoad;
		private bool persistchanges;

		public ARDataBinder() : base()
		{
		}

		public bool PersistChanges
		{
			get { return persistchanges; }
			set { persistchanges = value; }
		}

		public AutoLoadBehavior AutoLoad
		{
			get { return autoLoad; }
			set { autoLoad = value; }
		}

		protected override object CreateInstance(Type instanceType, String paramPrefix, Node node)
		{
			if (node == null)
			{
				throw new BindingException("Nothing found for the given prefix. Are you sure the form fields are using the prefix " + paramPrefix + "?");
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
				shouldLoad =  StackDepth != 0;
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
					if (autoLoad == AutoLoadBehavior.NewInstanceIfInvalidKey)
					{
						instance = base.CreateInstance(instanceType, paramPrefix, node);
					}
					else if (autoLoad == AutoLoadBehavior.NullIfInvalidKey || 
					         autoLoad == AutoLoadBehavior.OnlyNested)
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

		protected override object BindSpecialObjectInstance(Type instanceType, string prefix, Node node, out bool succeeded)
		{
			succeeded = false;
			
			object stackInstance = InstanceOnStack;
			
			ActiveRecordModel model = ActiveRecordModel.GetModel(stackInstance.GetType());
			
			if (model == null)
			{
				return null;
			}
			
			object container = CreateContainer(instanceType);
			
			bool found = false;
			Type targetType = null;
			ActiveRecordModel targetModel = null;

			foreach(HasAndBelongsToManyModel hasMany2ManyModel in model.HasAndBelongsToMany)
			{
				// Inverse=true relations will be ignored
				if (hasMany2ManyModel.Property.Name == prefix && !hasMany2ManyModel.HasManyAtt.Inverse)
				{
					targetType = hasMany2ManyModel.HasManyAtt.MapType;

					targetModel = ActiveRecordModel.GetModel(targetType);

					found = true;
					break;
				}
			}
			
			if (!found)
			{
				foreach(HasManyModel hasManyModel in model.HasMany)
				{
					// Inverse=true relations will be ignored
					if (hasManyModel.Property.Name == prefix && !hasManyModel.HasManyAtt.Inverse)
					{
						targetType = hasManyModel.HasManyAtt.MapType;

						targetModel = ActiveRecordModel.GetModel(targetType);

						found = true;
						break;
					}
				}
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
						foreach(object element in (Array)leafNode.Value)
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

		protected override bool ShouldRecreateInstance(object value, Type type, string prefix, Node node)
		{
			if (IsContainerType(type))
			{
				return true;
			}
			
			return base.ShouldRecreateInstance(value, type, prefix, node);
		}

		#region helpers
		
		private object ObtainPrimaryKeyValue(ActiveRecordModel model, CompositeNode node, String prefix, out PrimaryKeyModel pkModel)
		{

			if (model.IsJoinedSubClass)
			{
				pkModel = model.Parent.PrimaryKey;
			}
			else
			{
				pkModel = model.PrimaryKey;
			}
			
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
#if DOTNET2
			if (!isContainerType && type.IsGenericType)
			{
				Type[] genericArgs = type.GetGenericArguments();

				Type genType = typeof(System.Collections.Generic.ICollection<>).MakeGenericType(genericArgs);

				isContainerType = genType.IsAssignableFrom(type);
			}
#endif
			return isContainerType;
		}

		private object CreateContainer(Type type)
		{
			if (type == typeof(IList))
			{
				return new ArrayList();
			}
			else if (type == typeof(ISet))
			{
				return new HashedSet();
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
#if DOTNET2
				Type itemType = item.GetType();

				Type collectionType = typeof(System.Collections.Generic.ICollection<>).MakeGenericType(itemType);

				if (collectionType.IsAssignableFrom(container.GetType()))
				{
					MethodInfo addMethod = container.GetType().GetMethod("Add");

					addMethod.Invoke(container, new object[] {item});
				}
#endif
			}
		}
		
		#endregion
	}
}
