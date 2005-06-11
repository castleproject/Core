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

namespace AspectSharp.Core.Matchers
{
	using System;
	using System.Collections;
	using AspectSharp.Lang.AST;

	/// <summary>
	/// Summary description for DefaultAspectMatcher.
	/// </summary>
	[Serializable]
	public class DefaultAspectMatcher : IAspectMatcher
	{
		private IDictionary _customMatcherCache = new Hashtable();

		#region IAspectMatcher Members

		public AspectDefinition[] Match(Type targetType, AspectDefinitionCollection aspects)
		{
			// TODO: think about caches here...

			ArrayList list = new ArrayList();

			foreach (AspectDefinition aspect in aspects)
			{
				IClassMatcher matcher = ObtainClassMatcher(aspect);

				if (matcher.Match(targetType, aspect))
				{
					list.Add(aspect);
				}
			}

			return (AspectDefinition[]) list.ToArray(typeof (AspectDefinition));
		}

		#endregion

		protected virtual IClassMatcher ObtainClassMatcher(AspectDefinition aspect)
		{
			switch (aspect.TargetType.TargetStrategy)
			{
				case TargetStrategyEnum.SingleType:
					return SingleTypeMatcher.Instance;
				case TargetStrategyEnum.Assignable:
					return AssignableMatcher.Instance;
				case TargetStrategyEnum.Namespace:
					return NamespaceMatcher.Instance;
				case TargetStrategyEnum.Custom:
					return ObtainCustomMatcher(aspect.TargetType);
			}
			// There is no way we can get here - hopefully
			return null;
		}

		protected virtual IClassMatcher ObtainCustomMatcher(TargetTypeDefinition target)
		{
			Type customType = target.CustomMatcherType.ResolvedType;

			IClassMatcher matcher = GetCustomMatcherFromCache(customType);

			try
			{
				matcher = (IClassMatcher) Activator.CreateInstance(customType);
			}
			catch (InvalidCastException ex)
			{
				throw new MatcherException("Error trying to cast your custom class matcher to IClassMatcher", ex);
			}
			catch (Exception ex)
			{
				throw new MatcherException("Error trying to instantiate your custom class matcher", ex);
			}

			RegisterMatcherInCache(matcher, customType);

			return matcher;
		}

		private void RegisterMatcherInCache(IClassMatcher matcher, Type customType)
		{
			_customMatcherCache[customType] = matcher;
		}

		private IClassMatcher GetCustomMatcherFromCache(Type customType)
		{
			return (IClassMatcher) _customMatcherCache[customType];
		}
	}
}