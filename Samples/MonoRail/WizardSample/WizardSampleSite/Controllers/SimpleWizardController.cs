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
		protected override void Reset()
		{
			Session.Remove("account");
		}

		public void Save([DataBind("account")] Account accountFromForm)
		{
			Account account = WizSessionUtil.GetAccountFromSession(Session);

			// Update the account on session with the data from the form
			
			account.Name = accountFromForm.Name;
			account.Username = accountFromForm.Username;
			account.Email = accountFromForm.Email;
			account.Pwd = accountFromForm.Pwd;
			account.PwdConfirmation = accountFromForm.PwdConfirmation;

			// Some naive validation

			IList errors = ValidateAccount(account);

			if (errors.Count != 0)
			{
				// Not good

				Flash["errors"] = errors;

				// User can't go to the next step yet

				RedirectToStep("MainInfoStep");
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
		/// this method and render a different 
		/// view (the default behavior is to render 
		/// the step name, i.e. MainInfoStep in this step)
		/// </summary>
		protected override void RenderWizardView()
		{
			PropertyBag.Add("account", WizSessionUtil.GetAccountFromSession(Session));

			base.RenderWizardView();
		}
	}

	class SubscribeStep : WizardStepPage
	{
		/// <summary>
		/// You can also use this to give the view some data.
		/// Here we send the presaved (if any) selections
		/// </summary>
		protected override void RenderWizardView()
		{
			PropertyBag.Add("source", new String[] { "Sports", "Science", "Nature", "History" });
			
			PropertyBag.Add("account", WizSessionUtil.GetAccountFromSession(Session));
			
			base.RenderWizardView();
		}
		
		public void Save([DataBind("account")] Account accountFromForm)
		{
			Account account = WizSessionUtil.GetAccountFromSession(Session);

			account.Interests = accountFromForm.Interests;

			DoNavigate();
		}
	}

	class ConfirmationStep : WizardStepPage
	{
		/// <summary>
		/// This is an example of a pre-condition check.
		/// </summary>
		protected override bool IsPreConditionSatisfied(IRailsEngineContext context)
		{
			Account account = WizSessionUtil.GetAccountFromSession(Session);
			
			if (account.IsEmpty)
			{
				Flash.Add("error", "No account information found. Please enter the data");
				RedirectToStep("MainInfoStep");
				return false;
			}
			
			return true;
		}

		protected override void RenderWizardView()
		{
			PropertyBag.Add("account", WizSessionUtil.GetAccountFromSession(Session));

			base.RenderWizardView();
		}
	}

	class ResultStep : WizardStepPage
	{
		protected override void RenderWizardView()
		{
			Account account = WizSessionUtil.GetAccountFromSession(Session);
		
			// Here you would take the proper actions like creating
			// the account in the database.
			
			// AccountService.Create(account);
			// Session.Remove("account");

			base.RenderWizardView();
		}
	}
	
	class WizSessionUtil
	{
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
}
