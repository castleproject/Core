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

namespace Castle.ActiveRecord.Framework
{
	/// <summary>
	/// Determines the default flushing behaviour of <see cref="Castle.ActiveRecord.SessionScope"/>
	/// and <see cref="Castle.ActiveRecord.TransactionScope"/>
	/// </summary>
	public enum DefaultFlushType
	{
		/// <summary>
		/// Classic flushing behaviour like in RC3 and before. <see cref="Castle.ActiveRecord.SessionScope"/>
		/// flushes automatically, <see cref="Castle.ActiveRecord.TransactionScope"/> flushes on Disposal if
		/// the transaction was committed.
		/// </summary>
		Classic,

		/// <summary>
		/// New recommended behaviour. Both types of scope flush automatically, consolidating behaviour between
		/// scoped and non-scoped code.
		/// </summary>
		Auto,

		/// <summary>
		/// Both scope types do only flush on disposal.
		/// </summary>
		Leave,

		/// <summary>
		/// NH2.0-alike behaviour. The <see cref="Castle.ActiveRecord.SessionScope"/> won't flush at all unless
		/// called manually. <see cref="Castle.ActiveRecord.TransactionScope"/> flushes automatically. This
		/// allows to use the scopes like the NH-ISession-ITransaction-block.
		/// </summary>
		Transaction
	}
}
