// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

// This supporting service was inspired by
// http://www.codeproject.com/cs/database/dal.asp
// by Deyan Petrov

namespace Castle.Services.Transaction
{
	using System;

	/// <summary>
	/// The supported transaction mode for the components.
	/// </summary>
	public enum TransactionMode 
	{ 
		Unspecified,
		/// <summary>
		/// no transaction context will be created
		/// </summary>
		Disabled = 0,
		/// <summary>
		/// transaction context will be created 
		/// managing internally a connection, no 
		/// transaction is opened though
		/// </summary>
		NotSupported,
		/// <summary>
		/// transaction context will be created if not present 
		/// </summary>
		Required,// 
		/// <summary>
		/// a new transaction context will be created 
		/// </summary>
		RequiresNew,// 
		/// <summary>
		/// an existing appropriate transaction context 
		/// will be joined if present
		/// </summary>
		Supported
	}
    
	/// <summary>
	/// The supported isolation modes.
	/// </summary>
	public enum IsolationMode
	{
		Unspecified,
		Chaos = 0,        
		ReadCommitted,    
		ReadUncommitted,
		RepeatableRead,
		Serializable
	}

	/// <summary>
	/// Indicates that the target class wants to use
	/// the transactional services.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class TransactionalAttribute : System.Attribute 
	{
		public TransactionalAttribute()
		{
		}
	}

	/// <summary>
	/// Indicates the transaction support for a method.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class TransactionAttribute : System.Attribute 
	{
		private TransactionMode _transactionMode;
		private IsolationMode _isolationMode;

		/// <summary>
		/// Declares unspecified values for transaction and isolation, which
		/// means that the transaction manager will use the default values
		/// for them
		/// </summary>
		public TransactionAttribute() : this(TransactionMode.Unspecified, IsolationMode.Unspecified)
		{
		}

		/// <summary>
		/// Declares the transaction mode, but omits the isolation, 
		/// which means that the transaction manager should use the
		/// default value for it.
		/// </summary>
		/// <param name="transactionMode"></param>
		public TransactionAttribute( TransactionMode transactionMode ) : this(transactionMode, IsolationMode.Unspecified)
		{
		}

		/// <summary>
		/// Declares both the transaction mode and isolation 
		/// desired for this method. The transaction manager should
		/// obey the declaration.
		/// </summary>
		/// <param name="transactionMode"></param>
		/// <param name="isolationMode"></param>
		public TransactionAttribute( TransactionMode transactionMode, IsolationMode isolationMode )
		{
			_transactionMode = transactionMode;
			_isolationMode = isolationMode;
		}

		/// <summary>
		/// Returns the <see cref="TransactionMode"/>
		/// </summary>
		public TransactionMode TransactionMode
		{
			get { return _transactionMode; }
		}

		/// <summary>
		/// Returns the <see cref="IsolationMode"/>
		/// </summary>
		public IsolationMode IsolationMode
		{
			get { return _isolationMode; }
		}
	}
}
