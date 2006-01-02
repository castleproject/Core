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

namespace WizardSampleSite.Controllers
{
	using System;
	using System.Collections;

	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Internal;

	using WizardSampleSite.Model;


	[DynamicActionProvider( typeof(WizardActionProvider) )]
	public class SimpleWizardController : Controller, IWizardController
	{
		public WizardStepPage[] GetSteps(IRailsEngineContext context)
		{
			return new WizardStepPage[]
				{
					new IntroductionStep(), 
					new MainInfoStep(), 
					new SubscribeStep(), 
					new ConfirmationStep(), 
					new ResultStep()
				};
		}

		public void OnWizardStart()
		{
		}

		public bool OnBeforeStep(string wizardName, string stepName, WizardStepPage step)
		{
			return true;
		}

		public void OnAfterStep(string wizardName, string stepName, WizardStepPage step)
		{
		}
	}

	// Please note that we put the steps on the same file
	// for brevity's sake


	/// <summary>
	/// Presents a small introduction
	/// </summary>
	class IntroductionStep : WizardStepPage
	{
	}

	class MainInfoStep : WizardStepPage
	{
		// protected override void Reset()
		// {
		//   Session.Remove("account");
		// }

		public void Save()
		{
			Account account = GetAccountFromSession(Session);

			account.Name = Params["name"];
			account.Username = Params["username"];
			account.Email = Params["email"];
			account.Pwd = Params["pwd"];
			account.PwdConfirmation = Params["pwdconfirmation"];

			// Some naive validation

			IList errors = ValidateAccount(account);

			if (errors.Count != 0)
			{
				// Not good

				Flash["errors"] = errors;

				// User can't go to the next step yet

				RedirectToAction( ActionName );
			}
			else
			{
				DoNavigate();
			}
		}

		private IList ValidateAccount(Account account)
		{
			IList errors = new ArrayList();
	
			if (account.Name == null || account.Name.Length == 0)
			{
				errors.Add("Full name field must be filled");
			}
			if (account.Username == null || account.Username.Length == 0)
			{
				errors.Add("User name field must be filled");
			}
			if (account.Email == null || account.Email.Length == 0)
			{
				errors.Add("E-mail field must be filled");
			}
			if (account.Pwd != account.PwdConfirmation)
			{
				errors.Add("Password don't match with confirmation");
			}

			return errors;
		}

		/// <summary>
		/// Note that you can override 
		/// the Show method and render a different 
		/// view (the default behavior is to render 
		/// the step name, i.e. MainInfoStep)
		/// </summary>
		protected override void RenderWizardView()
		{
			PropertyBag.Add("account", GetAccountFromSession(Session));

			base.RenderWizardView ();
		}

		internal static Account GetAccountFromSession(IDictionary session)
		{
			Account account = session["stored.account"] as Account;

			if (account == null)
			{
				account = new Account();

				session["stored.account"] = account;
			}

			return account;
		}
	}

	class SubscribeStep : WizardStepPage
	{
		public void Save()
		{
			Account account = MainInfoStep.GetAccountFromSession(Session);

			String[] interests = (String[]) ConvertUtils.Convert( 
				typeof(String[]), Params["interests"], "interests", null, Params );

			account.Interests = interests;

			DoNavigate();
		}
	}

	class ConfirmationStep : WizardStepPage
	{
		protected override void RenderWizardView()
		{
			PropertyBag.Add("account", MainInfoStep.GetAccountFromSession(Session));

			base.RenderWizardView();
		}
	}

	class ResultStep : WizardStepPage
	{
		protected override void RenderWizardView()
		{
			Account account = MainInfoStep.GetAccountFromSession(Session);
		
			// AccountService.Create(account);
			// Session.Remove("account");

			base.RenderWizardView();
		}
	}
}
