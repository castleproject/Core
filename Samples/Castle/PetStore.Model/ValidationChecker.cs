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

namespace PetStore.Model
{
	using Castle.ActiveRecord;
	using Castle.Components.Validator;
	using NHibernate.Event;

	[EventListener]
	public class ValidationChecker : 
		IPreUpdateEventListener, IPreInsertEventListener
	{
		public bool OnPreUpdate(PreUpdateEvent @event)
		{
			ThrowIfInvalid(@event.Entity);
			return false;
		}

		public bool OnPreInsert(PreInsertEvent @event)
		{
			ThrowIfInvalid(@event.Entity);
			return false;
		}

		private static IValidatorRegistry registry = 
			new CachedValidationRegistry();
		private IValidatorRunner runner = 
			new ValidatorRunner(registry);

		private void ThrowIfInvalid(object o)
		{
			if (!runner.IsValid(o))
			{
				throw new ValidationException(
					string.Format(
						"Validation of {0} has failed", o));
			}
		}
	}
}