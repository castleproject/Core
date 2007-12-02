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

namespace Castle.ActiveRecord.Framework.Internal
{
	using System;
	using System.Collections;

	/// <summary>
	/// This model of a full Active Record persistent class.
	/// </summary>
	[Serializable]
	public class ActiveRecordModel : IVisitable
	{
		/// <summary>
		/// The mapping between a type and a model
		/// </summary>
		protected internal static IDictionary type2Model = Hashtable.Synchronized(new Hashtable());

		/// <summary>
		/// Whatever Active Record will generate debug information or not
		/// </summary>
		protected internal static bool isDebug = false;

		/// <summary>
		/// Whatever types that does not explicitly state that they are lazy should be lazy.
		/// </summary>
		protected internal static bool isLazyByDefault = false;

		/// <summary>
		/// Whether the default inferred table name is plural
		/// </summary>
		protected internal static bool pluralizeTableNames = false;

		private readonly Type type;

		private bool isJoinedSubClassBase;
		private bool isDiscriminatorBase;
		private bool isDiscriminatorSubClass;
		private bool isJoinedSubClass;
		private bool isNestedType;
		private bool isNestedCompositeType;

		private ActiveRecordAttribute arAtt;
		private ActiveRecordModel parent;
		private PrimaryKeyModel primaryKey;
		private CompositeKeyModel compositeKey;
		private KeyModel key;
		private TimestampModel timestamp;
		private VersionModel version;
		private NestedModel parentNested;

		private readonly IList imports = new ArrayList();
		private readonly IList hasManyToAny = new ArrayList();
		private readonly IList anys = new ArrayList();
		private readonly IList properties = new ArrayList();
		private readonly IList fields = new ArrayList();
		private readonly IList componentParent = new ArrayList();
		private readonly IList classes = new ArrayList();
		private readonly IList joinedclasses = new ArrayList();
		private readonly IList components = new ArrayList();
		private readonly IList belongsTo = new ArrayList();
		private readonly IList hasMany = new ArrayList();
		private readonly IList hasAndBelongsToMany = new ArrayList();
		private readonly IList oneToOne = new ArrayList();
		private readonly IList collectionIds = new ArrayList();
		private readonly IList hilos = new ArrayList();
		private readonly IList notMappedProperties = new ArrayList();
		private readonly IList validators = new ArrayList();
		private readonly IList compositeUserType = new ArrayList();

		/// <summary>
		/// Initializes a new instance of the <see cref="ActiveRecordModel"/> class.
		/// </summary>
		/// <param name="type">The type.</param>
		public ActiveRecordModel(Type type)
		{
			this.type = type;
		}

		/// <summary>
		/// Gets or sets the parent model
		/// </summary>
		/// <value>The parent.</value>
		public ActiveRecordModel Parent
		{
			get { return parent; }
			set { parent = value; }
		}

		/// <summary>
		/// Gets the type for this model
		/// </summary>
		/// <value>The type.</value>
		public Type Type
		{
			get { return type; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance is joined sub class base.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is joined sub class base; otherwise, <c>false</c>.
		/// </value>
		public bool IsJoinedSubClassBase
		{
			get { return isJoinedSubClassBase; }
			set { isJoinedSubClassBase = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance is discriminator base.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is discriminator base; otherwise, <c>false</c>.
		/// </value>
		public bool IsDiscriminatorBase
		{
			get { return isDiscriminatorBase; }
			set { isDiscriminatorBase = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance is discriminator sub class.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is discriminator sub class; otherwise, <c>false</c>.
		/// </value>
		public bool IsDiscriminatorSubClass
		{
			get { return isDiscriminatorSubClass; }
			set { isDiscriminatorSubClass = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance is joined sub class.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is joined sub class; otherwise, <c>false</c>.
		/// </value>
		public bool IsJoinedSubClass
		{
			get { return isJoinedSubClass; }
			set { isJoinedSubClass = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance is nested type.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is nested type; otherwise, <c>false</c>.
		/// </value>
		public bool IsNestedType
		{
			get { return isNestedType; }
			set { isNestedType = value; }
		}

		/// <summary>
		/// Gets or sets the parent nested.
		/// </summary>
		/// <value>The parent nested.</value>
		public NestedModel ParentNested
		{
			get { return parentNested; }
			set { parentNested = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance is nested type.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is nested type; otherwise, <c>false</c>.
		/// </value>
		public bool IsNestedCompositeType
		{
			get { return isNestedCompositeType; }
			set { isNestedCompositeType = value; }
		}

		/// <summary>
		/// Gets or sets the active record attribute
		/// </summary>
		/// <value>The active record att.</value>
		public ActiveRecordAttribute ActiveRecordAtt
		{
			get { return arAtt; }
			set { arAtt = value; }
		}

		/// <summary>
		/// Used only by joined subclasses
		/// </summary>
		public KeyModel Key
		{
			get { return key; }
			set { key = value; }
		}

		/// <summary>
		/// Gets or sets the timestamp model
		/// </summary>
		/// <value>The timestamp.</value>
		public TimestampModel Timestamp
		{
			get { return timestamp; }
			set { timestamp = value; }
		}

		/// <summary>
		/// Gets or sets the version model
		/// </summary>
		/// <value>The version.</value>
		public VersionModel Version
		{
			get { return version; }
			set { version = value; }
		}

		/// <summary>
		/// Gets all the imports
		/// </summary>
		/// <value>The imports.</value>
		public IList Imports
		{
			get { return imports; }
		}

		/// <summary>
		/// Gets all the properties
		/// </summary>
		/// <value>The properties.</value>
		public IList Properties
		{
			get { return properties; }
		}

		/// <summary>
		/// Gets all the fields
		/// </summary>
		/// <value>The fields.</value>
		public IList Fields
		{
			get { return fields; }
		}

		/// <summary>
		/// If the object is a component, will return the objects declared parent property.
		/// There should only be one, but implemented as a list
		/// </summary>
		public IList ComponentParent
		{
			get { return componentParent; }
		}

		/// <summary>
		/// Gets the list of [has many to any] models
		/// </summary>
		/// <value>The has many to any.</value>
		public IList HasManyToAny
		{
			get { return hasManyToAny; }
		}

		/// <summary>
		/// Gets the list of [any] model
		/// </summary>
		/// <value>The anys.</value>
		public IList Anys
		{
			get { return anys; }
		}

		/// <summary>
		/// Gets the list of the derived classes
		/// </summary>
		/// <value>The classes.</value>
		public IList Classes
		{
			get { return classes; }
		}

		/// <summary>
		/// Gets the list of derived joined classes.
		/// </summary>
		/// <value>The joined classes.</value>
		public IList JoinedClasses
		{
			get { return joinedclasses; }
		}

		/// <summary>
		/// Gets the list of components.
		/// </summary>
		/// <value>The components.</value>
		public IList Components
		{
			get { return components; }
		}

		/// <summary>
		/// Gets the list of [belongs to] models
		/// </summary>
		/// <value>The belongs to.</value>
		public IList BelongsTo
		{
			get { return belongsTo; }
		}

		/// <summary>
		/// Gets the list of [has many] models
		/// </summary>
		/// <value>The has many.</value>
		public IList HasMany
		{
			get { return hasMany; }
		}

		/// <summary>
		/// Gets the list of [has and belongs to many] models
		/// </summary>
		/// <value>The has and belongs to many.</value>
		public IList HasAndBelongsToMany
		{
			get { return hasAndBelongsToMany; }
		}

		/// <summary>
		/// Gets the list of [one to one] models
		/// </summary>
		/// <value>The one to ones.</value>
		public IList OneToOnes
		{
			get { return oneToOne; }
		}

		/// <summary>
		/// Gets the list of [collection id] models
		/// </summary>
		/// <value>The collection I ds.</value>
		public IList CollectionIDs
		{
			get { return collectionIds; }
		}

		/// <summary>
		/// For unique Primary keys
		/// </summary>
		public PrimaryKeyModel PrimaryKey
		{
			get { return primaryKey; }
			set { primaryKey = value; }
		}

		/// <summary>
		/// For Composite Primary keys
		/// </summary>
		public CompositeKeyModel CompositeKey
		{
			get { return compositeKey; }
			set { compositeKey = value; }
		}

		/// <summary>
		/// Gets the list of [hilo] models
		/// </summary>
		/// <value>The hilos.</value>
		public IList Hilos
		{
			get { return hilos; }
		}

		/// <summary>
		/// Gets the list of  properties not mapped .
		/// </summary>
		/// <value>The not mapped properties.</value>
		public IList NotMappedProperties
		{
			get { return notMappedProperties; }
		}

		/// <summary>
		/// Gets the validators.
		/// </summary>
		/// <value>The validators.</value>
		public IList Validators
		{
			get { return validators; }
		}

		/// <summary>
		/// Gets a value indicating whether to use auto import
		/// </summary>
		/// <value><c>true</c> if should use auto import; otherwise, <c>false</c>.</value>
		public bool UseAutoImport
		{
			get
			{
				if (arAtt != null)
				{
					return arAtt.UseAutoImport;
				}

				return true;
			}
		}

		/// <summary>
		/// Gets the composite user types properties.
		/// </summary>
		/// <value>The type of the composite user.</value>
		public IList CompositeUserType
		{
			get { return compositeUserType; }
		}

		/// <summary>
		/// Used internally register an association between a type and its model
		/// </summary>
		/// <param name="arType"></param>
		/// <param name="model"></param>
		internal static void Register(Type arType, ActiveRecordModel model)
		{
			type2Model[arType] = model;
		}

		/// <summary>
		/// Gets the <see cref="Framework.Internal.ActiveRecordModel"/> for a given ActiveRecord class.
		/// </summary>
		public static ActiveRecordModel GetModel(Type arType)
		{
			arType = GetNonProxy(arType);
			return (ActiveRecordModel) type2Model[arType];
		}

		/// <summary>
		/// Gets an array containing the <see cref="Framework.Internal.ActiveRecordModel"/> for every registered ActiveRecord class.
		/// </summary>
		public static ActiveRecordModel[] GetModels()
		{
			ActiveRecordModel[] modelArray = new ActiveRecordModel[type2Model.Values.Count];

			type2Model.Values.CopyTo(modelArray, 0);

			return modelArray;
		}

		/// <summary>
		/// Get the base type is the object is lazy
		/// </summary>
		private static Type GetNonProxy(Type type)
		{
			if (type.GetField("__interceptor") != null ||//Dynamic Proxy 1.0
				type.GetField("__interceptors")!=null) //Dynamic Proxy 2.0
			{
				type = type.BaseType;
			}
			return type;
		}

		#region IVisitable Members

		/// <summary>
		/// Accepts the specified visitor and call the relevant IVisitor.Visit***() method
		/// </summary>
		/// <param name="visitor">The visitor.</param>
		public void Accept(IVisitor visitor)
		{
			visitor.VisitModel(this);
		}

		#endregion
	}
}
