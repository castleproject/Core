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

namespace Castle.MicroKernel
{
	using System;
	using System.Collections;
	using System.Runtime.Serialization;
	using ComponentActivator;
	using Core;

#if !SILVERLIGHT
	public partial class DefaultKernel : MarshalByRefObject, IKernel, IKernelEvents, IDeserializationCallback
#else
	public partial class DefaultKernel : IKernel, IKernelEvents
#endif
	{
		public virtual void AddComponent(String key, Type classType)
		{
			AddComponent(key, classType, classType);
		}

		/// <summary>
		/// Adds a concrete class
		/// as a component with the specified <paramref name="lifestyle"/>.
		/// </summary>
		/// <param name="key">The key with which to index the component.</param>
		/// <param name="classType">The <see cref="Type"/> of the component.</param>
		/// <param name="lifestyle">The specified <see cref="LifestyleType"/> for the component.</param>
		/// <remarks>
		/// If you have indicated a lifestyle for the specified <paramref name="classType"/> using
		/// attributes, this method will not overwrite that lifestyle. To do that, use the
		/// <see cref="IKernel.AddComponent(string,Type,LifestyleType,bool)"/> method.
		/// </remarks>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="key"/> or <paramref name="classType"/>
		/// are <see langword="null"/>.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="lifestyle"/> is <see cref="LifestyleType.Undefined"/>.
		/// </exception>
		public void AddComponent(string key, Type classType, LifestyleType lifestyle)
		{
			AddComponent(key, classType, classType, lifestyle);
		}

		/// <summary>
		/// Adds a concrete class
		/// as a component with the specified <paramref name="lifestyle"/>.
		/// </summary>
		/// <param name="key">The key with which to index the component.</param>
		/// <param name="classType">The <see cref="Type"/> of the component.</param>
		/// <param name="lifestyle">The specified <see cref="LifestyleType"/> for the component.</param>
		/// <param name="overwriteLifestyle">
		/// If <see langword="true"/>, then ignores all other configurations
		/// for lifestyle and uses the value in the <paramref name="lifestyle"/> parameter.
		/// </param>
		/// <remarks>
		/// If you have indicated a lifestyle for the specified <paramref name="classType"/> using
		/// attributes, this method will not overwrite that lifestyle. To do that, use the
		/// <see cref="IKernel.AddComponent(string,Type,Type,LifestyleType,bool)"/> method.
		/// </remarks>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="key"/> or <paramref name="classType"/>
		/// are <see langword="null"/>.
		/// </exception>
		/// <exception cref="ArgumentException" />
		/// Thrown if <paramref name="lifestyle"/> is <see cref="LifestyleType.Undefined"/>.
		public void AddComponent(string key, Type classType, LifestyleType lifestyle, bool overwriteLifestyle)
		{
			AddComponent(key, classType, classType, lifestyle, overwriteLifestyle);
		}

		public virtual void AddComponent(String key, Type serviceType, Type classType)
		{
			AddComponent(key, serviceType, classType, LifestyleType.Singleton);
		}

		/// <summary>
		/// Adds a concrete class and an interface 
		/// as a component with the specified <paramref name="lifestyle"/>.
		/// </summary>
		/// <param name="key">The key with which to index the component.</param>
		/// <param name="serviceType">The service <see cref="Type"/> that this component implements.</param>
		/// <param name="classType">The <see cref="Type"/> of the component.</param>
		/// <param name="lifestyle">The specified <see cref="LifestyleType"/> for the component.</param>
		/// <remarks>
		/// If you have indicated a lifestyle for the specified <paramref name="classType"/> using
		/// attributes, this method will not overwrite that lifestyle. To do that, use the
		/// <see cref="AddComponent(string,Type,Type,LifestyleType,bool)"/> method.
		/// </remarks>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="key"/>, <paramref name="serviceType"/>, or <paramref name="classType"/>
		/// are <see langword="null"/>.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="lifestyle"/> is <see cref="LifestyleType.Undefined"/>.
		/// </exception>
		public void AddComponent(string key, Type serviceType, Type classType, LifestyleType lifestyle)
		{
			AddComponent(key, serviceType, classType, lifestyle, false);
		}

		/// <summary>
		/// Adds a concrete class and an interface 
		/// as a component with the specified <paramref name="lifestyle"/>.
		/// </summary>
		/// <param name="key">The key with which to index the component.</param>
		/// <param name="serviceType">The service <see cref="Type"/> that this component implements.</param>
		/// <param name="classType">The <see cref="Type"/> of the component.</param>
		/// <param name="lifestyle">The specified <see cref="LifestyleType"/> for the component.</param>
		/// <param name="overwriteLifestyle">
		/// If <see langword="true"/>, then ignores all other configurations
		/// for lifestyle and uses the value in the <paramref name="lifestyle"/> parameter.
		/// </param>
		/// <remarks>
		/// If you have indicated a lifestyle for the specified <paramref name="classType"/> using
		/// attributes, this method will not overwrite that lifestyle. To do that, use the
		/// <see cref="AddComponent(string,Type,Type,LifestyleType,bool)"/> method.
		/// </remarks>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="key"/>, <paramref name="serviceType"/>, or <paramref name="classType"/>
		/// are <see langword="null"/>.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="lifestyle"/> is <see cref="LifestyleType.Undefined"/>.
		/// </exception>
		public void AddComponent(string key, Type serviceType, Type classType, LifestyleType lifestyle,
								 bool overwriteLifestyle)
		{
			if (key == null) throw new ArgumentNullException("key");
			if (serviceType == null) throw new ArgumentNullException("serviceType");
			if (classType == null) throw new ArgumentNullException("classType");
			if (LifestyleType.Undefined == lifestyle)
				throw new ArgumentException("The specified lifestyle must be Thread, Transient, or Singleton.", "lifestyle");
			ComponentModel model = ComponentModelBuilder.BuildModel(key, serviceType, classType, null);

			if (overwriteLifestyle || LifestyleType.Undefined == model.LifestyleType)
			{
				model.LifestyleType = lifestyle;
			}

			RaiseComponentModelCreated(model);

			IHandler handler = HandlerFactory.Create(model);
			RegisterHandler(key, handler);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="classType"></param>
		/// <param name="parameters"></param>
		public virtual void AddComponentWithExtendedProperties(String key, Type classType, IDictionary parameters)
		{
			if (key == null) throw new ArgumentNullException("key");
			if (parameters == null) throw new ArgumentNullException("parameters");
			if (classType == null) throw new ArgumentNullException("classType");

			ComponentModel model = ComponentModelBuilder.BuildModel(key, classType, classType, parameters);
			RaiseComponentModelCreated(model);
			IHandler handler = HandlerFactory.Create(model);
			RegisterHandler(key, handler);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="serviceType"></param>
		/// <param name="classType"></param>
		/// <param name="parameters"></param>
		public virtual void AddComponentWithExtendedProperties(String key, Type serviceType, Type classType,
															   IDictionary parameters)
		{
			if (key == null) throw new ArgumentNullException("key");
			if (parameters == null) throw new ArgumentNullException("parameters");
			if (serviceType == null) throw new ArgumentNullException("serviceType");
			if (classType == null) throw new ArgumentNullException("classType");

			ComponentModel model = ComponentModelBuilder.BuildModel(key, serviceType, classType, parameters);
			RaiseComponentModelCreated(model);
			IHandler handler = HandlerFactory.Create(model);
			RegisterHandler(key, handler);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="model"></param>
		public virtual void AddCustomComponent(ComponentModel model)
		{
			if (model == null) throw new ArgumentNullException("model");

			RaiseComponentModelCreated(model);
			IHandler handler = HandlerFactory.Create(model);

			object skipRegistration = model.ExtendedProperties[ComponentModel.SkipRegistration];

			if (skipRegistration != null)
			{
				RegisterHandler(model.Name, handler, (bool)skipRegistration);
			}
			else
			{
				RegisterHandler(model.Name, handler);
			}
		}

		/// <summary>
		/// Used mostly by facilities. Adds an instance
		/// to be used as a component.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="instance"></param>
		public void AddComponentInstance(String key, object instance)
		{
			if (key == null) throw new ArgumentNullException("key");
			if (instance == null) throw new ArgumentNullException("instance");

			Type classType = instance.GetType();

			ComponentModel model = new ComponentModel(key, classType, classType);
			model.LifestyleType = LifestyleType.Singleton;
			model.CustomComponentActivator = typeof(ExternalInstanceActivator);
			model.ExtendedProperties["instance"] = instance;

			RaiseComponentModelCreated(model);
			IHandler handler = HandlerFactory.Create(model);
			RegisterHandler(key, handler);
		}

		/// <summary>
		/// Used mostly by facilities. Adds an instance
		/// to be used as a component.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="serviceType"></param>
		/// <param name="instance"></param>
		public void AddComponentInstance(String key, Type serviceType, object instance)
		{
			AddComponentInstance(key, serviceType, instance.GetType(), instance);
		}

		public void AddComponentInstance(string key, Type serviceType, Type classType, object instance)
		{
			if (key == null) throw new ArgumentNullException("key");
			if (serviceType == null) throw new ArgumentNullException("serviceType");
			if (instance == null) throw new ArgumentNullException("instance");
			if (classType == null) throw new ArgumentNullException("classType");

			ComponentModel model = new ComponentModel(key, serviceType, classType);
			model.LifestyleType = LifestyleType.Singleton;
			model.CustomComponentActivator = typeof(ExternalInstanceActivator);
			model.ExtendedProperties["instance"] = instance;

			RaiseComponentModelCreated(model);
			IHandler handler = HandlerFactory.Create(model);
			RegisterHandler(key, handler);
		}

		/// <summary>
		/// Adds a concrete class as a component
		/// </summary>
		public void AddComponent<T>()
		{
			Type classType = typeof(T);
			AddComponent(classType.FullName, classType);
		}

		/// <summary>
		/// Adds a concrete class
		/// as a component with the specified <paramref name="lifestyle"/>.
		/// </summary>
		/// <param name="lifestyle">The specified <see cref="LifestyleType"/> for the component.</param>
		/// <remarks>
		/// If you have indicated a lifestyle for the specified T using
		/// attributes, this method will not overwrite that lifestyle. To do that, use the
		/// <see cref="AddComponent(string,Type,LifestyleType,bool)"/> method.
		/// </remarks>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="lifestyle"/> is <see cref="LifestyleType.Undefined"/>.
		/// </exception>
		public void AddComponent<T>(LifestyleType lifestyle)
		{
			Type classType = typeof(T);
			AddComponent(classType.FullName, classType, lifestyle);
		}

		/// <summary>
		/// Adds a concrete class
		/// as a component with the specified <paramref name="lifestyle"/>.
		/// </summary>
		/// <param name="lifestyle">The specified <see cref="LifestyleType"/> for the component.</param>
		/// <param name="overwriteLifestyle">If <see langword="true"/>, then ignores all other configurations
		/// for lifestyle and uses the value in the <paramref name="lifestyle"/> parameter.</param>
		/// <remarks>
		/// If you have indicated a lifestyle for the specified T using
		/// attributes, this method will not overwrite that lifestyle. To do that, use the
		/// <see cref="AddComponent(string,Type,LifestyleType,bool)"/> method.
		/// </remarks>
		/// <exception cref="ArgumentException"/>
		/// Thrown if 
		/// <paramref name="lifestyle"/>
		///  is 
		/// <see cref="LifestyleType.Undefined"/>
		/// .
		public void AddComponent<T>(LifestyleType lifestyle, bool overwriteLifestyle)
		{
			Type classType = typeof(T);
			AddComponent(classType.FullName, classType, lifestyle, overwriteLifestyle);
		}

		/// <summary>
		/// Adds a concrete class and an interface
		/// as a component
		/// </summary>
		/// <param name="serviceType">The service <see cref="Type"/> that this component implements.</param>
		public void AddComponent<T>(Type serviceType)
		{
			Type classType = typeof(T);
			AddComponent(classType.FullName, serviceType, classType);
		}

		/// <summary>
		/// Adds a concrete class and an interface
		/// as a component with the specified <paramref name="lifestyle"/>.
		/// </summary>
		/// <param name="serviceType">The service <see cref="Type"/> that this component implements.</param>
		/// <param name="lifestyle">The specified <see cref="LifestyleType"/> for the component.</param>
		/// <remarks>
		/// If you have indicated a lifestyle for the specified T using
		/// attributes, this method will not overwrite that lifestyle. To do that, use the
		/// <see cref="AddComponent(string,Type,Type,LifestyleType,bool)"/> method.
		/// </remarks>
		/// <exception cref="ArgumentNullException">
		/// are <see langword="null"/>.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="lifestyle"/> is <see cref="LifestyleType.Undefined"/>.
		/// </exception>
		public void AddComponent<T>(Type serviceType, LifestyleType lifestyle)
		{
			Type classType = typeof(T);
			AddComponent(classType.FullName, serviceType, classType, lifestyle);
		}

		/// <summary>
		/// Adds a concrete class and an interface
		/// as a component with the specified <paramref name="lifestyle"/>.
		/// </summary>
		/// <param name="serviceType">The service <see cref="Type"/> that this component implements.</param>
		/// <param name="lifestyle">The specified <see cref="LifestyleType"/> for the component.</param>
		/// <param name="overwriteLifestyle">If <see langword="true"/>, then ignores all other configurations
		/// for lifestyle and uses the value in the <paramref name="lifestyle"/> parameter.</param>
		/// <remarks>
		/// attributes, this method will not overwrite that lifestyle. To do that, use the
		/// <see cref="AddComponent(string,Type,Type,LifestyleType,bool)"/> method.
		/// </remarks>
		/// <exception cref="ArgumentNullException">
		/// are <see langword="null"/>.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="lifestyle"/> is <see cref="LifestyleType.Undefined"/>.
		/// </exception>
		public void AddComponent<T>(Type serviceType, LifestyleType lifestyle, bool overwriteLifestyle)
		{
			Type classType = typeof(T);
			AddComponent(classType.FullName, serviceType, classType, lifestyle, overwriteLifestyle);
		}

		/// <summary>
		/// Used mostly by facilities. Adds an instance
		/// to be used as a component.
		/// </summary>
		/// <param name="instance"></param>
		public void AddComponentInstance<T>(object instance)
		{
			Type serviceType = typeof(T);
			AddComponentInstance(serviceType.FullName, serviceType, instance);
		}

		/// <summary>
		/// Used mostly by facilities. Adds an instance
		/// to be used as a component.
		/// </summary>
		/// <param name="serviceType"></param>
		/// <param name="instance"></param>
		public void AddComponentInstance<T>(Type serviceType, object instance)
		{
			Type classType = typeof(T);
			AddComponentInstance(classType.FullName, serviceType, classType, instance);
		}
	}
}
