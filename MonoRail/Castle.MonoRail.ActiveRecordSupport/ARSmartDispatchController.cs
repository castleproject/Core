// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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
	using System.Reflection;
	
	using Castle.ActiveRecord;

	using Castle.MonoRail.Framework;

	/// <summary>
	/// Extends the SmartDipatchController with ActiveRecord specific functionality
	/// </summary>
	public class ARSmartDispatchController : SmartDispatcherController
	{
		public ARSmartDispatchController () : base ()
		{
		}
		
		protected override void Initialize()
		{
			binder = new ARDataBinder();
		}	
		
		/// <summary>
		/// Check if any method argument is associated with an ARDataBinderAttribute
		/// that has the Validate property set to true,
		/// and perform automatic validation for it (as long it inherit from ActiveRecordValidationBase)
		/// </summary>
		protected override object[] BuildMethodArguments( ParameterInfo[] parameters, IRequest request )
		{
			object[] args = base.BuildMethodArguments( parameters, request );

			for( int i=0; i < args.Length; i++)
			{
				ParameterInfo param	= parameters[i];
				
				object[] bindAttributes	= param.GetCustomAttributes( typeof(ARDataBindAttribute), false );
				
				if ( bindAttributes.Length > 0 )
				{
					ARDataBindAttribute dba = bindAttributes[0] as ARDataBindAttribute;							
					if( dba.AutoPersist )
					{
						PersistInstances (args[i]);
					}
					else if( dba.Validate )
					{
						ValidateInstances (args[i], param);
					}
				}
			}
			
			return args;
		}

		private void PersistInstances( object instances )
		{
			Type instanceType = instances.GetType();		
			ActiveRecordBase[] records = null;
			
			if( instanceType.IsArray )
			{
				records = instances as ActiveRecordBase[];
			}
			else if( typeof(ActiveRecordBase).IsAssignableFrom( instanceType ) )
			{
				records = new ActiveRecordBase[] { (ActiveRecordBase) instances };
			}

			if( records != null )
			{
				foreach(ActiveRecordBase record in records)
				{								
					record.Save();
				}
			}
		}

		private void ValidateInstances( object instances, ParameterInfo param )
		{
			Type instanceType = instances.GetType();		
			ActiveRecordValidationBase[] records = null;
			
			if( instanceType.IsArray )
			{
				records = instances as ActiveRecordValidationBase[];
			}
			else if( typeof(ActiveRecordValidationBase).IsAssignableFrom( instanceType ) )
			{
				records = new ActiveRecordValidationBase[] { (ActiveRecordValidationBase) instances };
			}

			if( records != null )
			{
				foreach(ActiveRecordValidationBase record in records)
				{								
					if( !record.IsValid() )
					{
						throw new RailsException( "ARSmartDispatchController: Error validating {0} {1}\n{2}",
							param.ParameterType.FullName, param.Name, string.Join( "\n", record.ValidationErrorMessages) );
					}
				}
			}			
		}
	}
}
