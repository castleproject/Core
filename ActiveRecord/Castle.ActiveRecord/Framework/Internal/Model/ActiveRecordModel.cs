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

namespace Castle.ActiveRecord.Framework.Internal
{
	using System;
	using System.Collections;

	[Serializable]
	public class ActiveRecordModel : IModelNode
	{
		protected internal static IDictionary type2Model = Hashtable.Synchronized(new Hashtable());

		private readonly Type type;

		private bool isJoinedSubClassBase;
		private bool isDiscriminatorBase;
		private bool isDiscriminatorSubClass;
		private bool isJoinedSubClass;
		private bool isNestedType;
		private ActiveRecordAttribute arAtt;
		private ActiveRecordModel parent;
		private IList imports = new ArrayList();
		private IList hasManyToAny = new ArrayList();
		private IList anys = new ArrayList();
		private IList ids = new ArrayList();
		private IList properties = new ArrayList();
		private IList fields = new ArrayList();
		private IList classes = new ArrayList();
		private IList joinedclasses = new ArrayList();
		private IList components = new ArrayList();
		private IList belongsTo = new ArrayList();
		private IList hasMany = new ArrayList();
		private IList hasAndBelongsToMany = new ArrayList();
		private IList oneToOne = new ArrayList();
		private IList collectionIds = new ArrayList();
		private IList hilos = new ArrayList();
		private IList notMappedProperties = new ArrayList();
		private IList validators = new ArrayList();
		private KeyModel key;
		private TimestampModel timestamp;
		private VersionModel version;

		public ActiveRecordModel(Type type)
		{
			this.type = type;
		}

		public ActiveRecordModel Parent
		{
			get { return parent; }
			set { parent = value; }
		}

		public Type Type
		{
			get { return type; }
		}

		public bool IsJoinedSubClassBase
		{
			get { return isJoinedSubClassBase; }
			set { isJoinedSubClassBase = value; }
		}

		public bool IsDiscriminatorBase
		{
			get { return isDiscriminatorBase; }
			set { isDiscriminatorBase = value; }
		}

		public bool IsDiscriminatorSubClass
		{
			get { return isDiscriminatorSubClass; }
			set { isDiscriminatorSubClass = value; }
		}

		public bool IsJoinedSubClass
		{
			get { return isJoinedSubClass; }
			set { isJoinedSubClass = value; }
		}

		public bool IsNestedType
		{
			get { return isNestedType; }
			set { isNestedType = value; }
		}

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

		public TimestampModel Timestamp
		{
			get { return timestamp; }
			set { timestamp = value; }
		}

		public VersionModel Version
		{
			get { return version; }
			set { version = value; }
		}

		public IList Imports
		{
			get { return imports; }
		}

		public IList Properties
		{
			get { return properties; }
		}

		public IList Fields
		{
			get { return fields; }
		}

		public IList HasManyToAny
		{
			get { return hasManyToAny; }
		}

		public IList Anys
		{
			get { return anys; }
		}

		public IList Classes
		{
			get { return classes; }
		}

		public IList JoinedClasses
		{
			get { return joinedclasses; }
		}

		public IList Components
		{
			get { return components; }
		}

		public IList BelongsTo
		{
			get { return belongsTo; }
		}

		public IList HasMany
		{
			get { return hasMany; }
		}

		public IList HasAndBelongsToMany
		{
			get { return hasAndBelongsToMany; }
		}

		public IList OneToOnes
		{
			get { return oneToOne; }
		}

		public IList CollectionIDs
		{
			get { return collectionIds; }
		}

		public IList Ids
		{
			get { return ids; }
		}

		public IList Hilos
		{
			get { return hilos; }
		}

		public IList NotMappedProperties
		{
			get { return notMappedProperties; }
		}

		public IList Validators
		{
			get { return validators; }
		}

		/// <summary>
		/// Internally used
		/// </summary>
		/// <param name="arType"></param>
		/// <param name="model"></param>
		internal static void Register(Type arType, Framework.Internal.ActiveRecordModel model)
		{
			type2Model[arType] = model;
		}

		/// <summary>
		/// Internally used
		/// </summary>
		/// <param name="arType"></param>
		/// <returns></returns>
		public static Framework.Internal.ActiveRecordModel GetModel(Type arType)
		{
			return (Framework.Internal.ActiveRecordModel) type2Model[arType];
		}

		#region IVisitable Members

		public void Accept(IVisitor visitor)
		{
			visitor.VisitModel(this);
		}

		#endregion
	}
}