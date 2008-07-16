// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Views.AspView
{
	using Components.DictionaryAdapter;
    using Framework.Helpers;

	public interface IHelpersAccesor
	{
		/// <summary>
		/// Gets the AjaxHelper instance
		/// </summary>
		[DictionaryKeySubstitution("Ajax", "AjaxHelper")]
		AjaxHelper Ajax { get; }

		/// <summary>
		/// Gets the BehaviourHelper instance
		/// </summary>
		[DictionaryKeySubstitution("Behaviour", "BehaviourHelper")]
		BehaviourHelper Behaviour { get; }

		/// <summary>
		/// Gets the DateFormatHelper instance
		/// </summary>
		[DictionaryKeySubstitution("DateFormat", "DateFormatHelper")]
		DateFormatHelper DateFormat { get; }

		/// <summary>
		/// Gets the DictHelper instance
		/// </summary>
		[DictionaryKeySubstitution("Dict", "DictHelper")]
		DictHelper Dict { get; }

		/// <summary>
		/// Gets the EffectsFatHelper instance
		/// </summary>
		[DictionaryKeySubstitution("EffectsFat", "EffectsFatHelper")]
		EffectsFatHelper EffectsFat { get; }

		/// <summary>
		/// Gets the FormHelper instance
		/// </summary>
		[DictionaryKeySubstitution("Form", "FormHelper")]
		FormHelper Form { get; }

		/// <summary>
		/// Gets the HtmlHelper instance
		/// </summary>
		[DictionaryKeySubstitution("Html", "HtmlHelper")]
		HtmlHelper Html { get; }

		/// <summary>
		/// Gets the PaginationHelper instance
		/// </summary>
		[DictionaryKeySubstitution("Pagination", "PaginationHelper")]
		PaginationHelper Pagination { get; }

		/// <summary>
		/// Gets the ScriptaculousHelper instance
		/// </summary>
		[DictionaryKeySubstitution("Scriptaculous", "ScriptaculousHelper")]
		ScriptaculousHelper Scriptaculous { get; }

		/// <summary>
		/// Gets the UrlHelper instance
		/// </summary>
		[DictionaryKeySubstitution("Url", "UrlHelper")]
		UrlHelper Url { get; }

		/// <summary>
		/// Gets the ValidationHelper instance
		/// </summary>
		[DictionaryKeySubstitution("Validation", "ValidationHelper")]
		ValidationHelper Validation { get; }

		/// <summary>
		/// Gets the WizardHelper instance
		/// </summary>
		[DictionaryKeySubstitution("Wizard", "WizardHelper")]
		WizardHelper Wizard { get; }

		/// <summary>
		/// Gets the ZebdaHelper instance
		/// </summary>
		[DictionaryKeySubstitution("Zebda", "ZebdaHelper")]
		ZebdaHelper Zebda { get; }
	}
}