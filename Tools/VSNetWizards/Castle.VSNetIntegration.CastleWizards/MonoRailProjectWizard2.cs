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

namespace Castle.VSNetIntegration.CastleWizards
{
	using System;
	using System.IO;
	using System.Runtime.InteropServices;
	using System.Text;
	using System.Windows.Forms;
	
	using EnvDTE;


	[Guid("43C9796F-E6C8-460D-B722-204A7121A510")]
	[ProgIdAttribute("Castle.MonoRailProjectWizard")]
	public class MonoRailProjectWizard : AbstractProjectWizard, IDTWizard, IWin32Window
	{
		private int owner;
		private bool enableWindsorIntegration = false;
		private bool createTestProject = false;
		private bool useNVelocity, useBrail;
		private bool useARIntegration = false;
		private bool useNHIntegration = false;
		private bool useATMIntegration = false;
		private bool useLoggingIntegration = false;

		StringBuilder sb = new StringBuilder();

		public void Execute(object Application, int hwndOwner, ref object[] ContextParams, ref object[] CustomParams, ref wizardResult retval)
		{
			dteInstance = (DTE) Application;
			owner = hwndOwner;

			projectName = (String) ContextParams[1];
			localProjectPath = (String) ContextParams[2];
//			String installationDirectory = (String) ContextParams[3];
			bool exclusive = (bool) ContextParams[4];
//			String solutionName = (String) ContextParams[5];
//			bool silent = (bool) ContextParams[6];

			if (exclusive)
			{
				vsPromptResult promptResult = dteInstance.ItemOperations.PromptToSave;

				if (promptResult == vsPromptResult.vsPromptResultCancelled)
				{
					retval = wizardResult.wizardResultCancel;
					return;
				}
			}

			using(MRNewProjectDialog dlg = new MRNewProjectDialog())
			{
				dlg.ShowDialog(this);
				retval = dlg.WizardResult;

				if (retval == wizardResult.wizardResultSuccess)
				{
					enableWindsorIntegration = dlg.EnableWindsorIntegration;
					createTestProject = dlg.CreateTestProject;
					useNVelocity = dlg.UseNVelocity;
					useBrail = dlg.UseBrail;

					foreach(String item in dlg.facilities.CheckedItems)
					{
						if (item == "ActiveRecord Integration")
						{
							useARIntegration = true;
						}
						else if (item == "NHibernate Integration")
						{
							useNHIntegration = true;
						}
						else if (item == "Automatic Transaction Management")
						{
							useATMIntegration = true;
						}
						else if (item == "Logging")
						{
							useLoggingIntegration = true;
						}
					}
				}
				else
				{
					retval = wizardResult.wizardResultCancel;
					return;
				}
			}

			try
			{

			String projectFile = GetTemplateFileName(@"CSharp\MRProject\MRProject.csproj");

			EnsureDirExists(localProjectPath);

			Project project = 
				dteInstance.Solution.AddFromTemplate(projectFile, localProjectPath, projectName + ".csproj", exclusive);

			project.Properties.Item("DefaultNamespace").Value = projectName;

			PerformReplacesOn(project, projectName, "Controllers\\HomeController.cs");
			PerformReplacesOn(project, projectName, "global.asax");

			AddGlobalApplication(project);
			AddView(project);

			ModifyWebConfig(project);
			
			if (enableWindsorIntegration)
			{
				AddContainerFile(project);
				AddControllerConfigFile(project);
				AddFacilityConfigFile(project);
			}

			UpdateReferences(project);

			UpdateProjectToUseCassini(project);

			if (createTestProject)
			{
				Project testProject = CreateTestProject(localProjectPath, projectName);

				testProject.Properties.Item("DefaultNamespace").Value = projectName + ".Tests";

				AddReference(testProject, project);

				AddCommonPostBuildEvent(testProject);
			}
			}
			catch(Exception ex)
			{
				MessageBox.Show(String.Format("Message: {0}\r\n\r\n{1}", ex.Message, ex.StackTrace));
			}
		}

		private void UpdateProjectToUseCassini(Project project)
		{
			ConfigurationManager confMng = project.ConfigurationManager;

			for (int i = 1; i <= confMng.Count; i++)
			{
				Configuration configuration = confMng.Item(i,".NET");

				configuration.Properties.Item("StartAction").Value = 
					VSLangProj.prjStartAction.prjStartActionProgram;
				
				configuration.Properties.Item("StartWorkingDirectory").Value = localProjectPath;
				
				configuration.Properties.Item("StartProgram").Value = CassiniLocation;

				configuration.Properties.Item("StartArguments").Value = 
					String.Format("{0} {1} {2}", localProjectPath, 81, "/");
			}
		}

		private void AddView(Project project)
		{
			String viewTemplateFile = null;
			String viewFile = null;

			if (useNVelocity)
			{
				viewFile = "index.vm";
				viewTemplateFile = GetTemplateFileName(@"CSharp\MRProject\index.vm");
			}
			else if (useBrail)
			{
				viewFile = "index.boo";
				viewTemplateFile = GetTemplateFileName(@"CSharp\MRProject\index.boo");
			}
			else
			{
				viewFile = "index.aspx";
				viewTemplateFile = GetTemplateFileName(@"CSharp\MRProject\index.aspx");
			}

			project.ProjectItems.Item("Views").
				ProjectItems.Item("Home").
					ProjectItems.AddFromTemplate(viewTemplateFile, viewFile);
		}

		private void AddContainerFile(Project project)
		{
			String projectFile = GetTemplateFileName(@"CSharp\MRProject\WebContainer.cs");

			project.ProjectItems.AddFromTemplate(projectFile, "WebContainer.cs");
			PerformReplacesOn(project, projectName, "WebContainer.cs");
		}

		private void AddGlobalApplication(Project project)
		{
			String globalAppFile = null;

			if (enableWindsorIntegration)
			{
				globalAppFile = GetTemplateFileName(@"CSharp\MRProject\ContainerGlobalApplication.cs");
			}
			else
			{
				globalAppFile = GetTemplateFileName(@"CSharp\MRProject\SimpleGlobalApplication.cs");
			}

			project.ProjectItems.AddFromTemplate(globalAppFile, "GlobalApplication.cs");
			PerformReplacesOn(project, projectName, "GlobalApplication.cs");
		}

		private void UpdateReferences(Project project)
		{
			if (enableWindsorIntegration)
			{
				AddReference(project, "Castle.DynamicProxy.dll");
				AddReference(project, "Castle.MicroKernel.dll");
				AddReference(project, "Castle.Model.dll");
				AddReference(project, "Castle.Windsor.dll");
				AddReference(project, "Castle.MonoRail.WindsorExtension.dll");

				if (useARIntegration || useNHIntegration)
				{
					AddReference(project, "NHibernate.dll");
					AddReference(project, "Nullables.dll");
					AddReference(project, "Nullables.NHibernate.dll");
					AddReference(project, "Iesi.Collections.dll");
				}

				if (useARIntegration || useNHIntegration || useLoggingIntegration)
				{
					AddReference(project, "log4net.dll");
				}

				if (useARIntegration)
				{
					AddReference(project, "Castle.ActiveRecord.dll");
					AddReference(project, "Castle.Facilities.ActiveRecordIntegration.dll");
				}
				if (useNHIntegration)
				{
					AddReference(project, "Castle.Facilities.NHibernateIntegration.dll");
				}
				if (useATMIntegration)
				{
					AddReference(project, "Castle.Facilities.AutomaticTransactionManagement.dll");
				}
				if (useLoggingIntegration)
				{
					AddReference(project, "Castle.Facilities.Logging.dll");
					AddReference(project, "Castle.Services.Logging.Log4netIntegration.dll");
					AddReference(project, "Castle.Services.Logging.dll");
				}
			}

			if (useNVelocity)
			{
				AddReference(project, "Castle.MonoRail.Framework.Views.NVelocity.dll");
				AddReference(project, "NVelocity.dll");
			}

			if (useBrail)
			{
				AddReference(project, "anrControls.Markdown.NET.dll");
				AddReference(project, "Boo.Lang.Compiler.dll");
				AddReference(project, "Boo.Lang.dll");
				AddReference(project, "Boo.Lang.Parser.dll");
				AddReference(project, "Castle.MonoRail.Views.Brail");
			}
		}

		private void AddFacilityConfigFile(Project project)
		{
			String projectFile = GetTemplateFileName(@"CSharp\MRProject\facilities.config");

			ProjectItem item = project.ProjectItems.AddFromTemplate(projectFile, "facilities.config");

			Window codeWindow = item.Open(Constants.vsViewKindCode);
	
			codeWindow.Activate();

			if (useARIntegration)
			{
				ReplaceToken(codeWindow, "!AR!", "<facility id=\"arintegration\" \r\n\t\t\t" + 
					"type=\"Castle.Facilities.ActiveRecordIntegration.ActiveRecordFacility, " + 
					"Castle.Facilities.ActiveRecordIntegration\"\r\n\t\t\tisWeb=\"true\" >\r\n\r\n\t\t\t" + 
					"<assemblies>\r\n\t\t\t\t" + 
					"<item>Name.Of.Your.ActiveRecord.Assembly</item>\r\n\t\t\t</assemblies>\r\n\r\n\t\t\t" + 
					"<config>\r\n\t\t\t\t" + 
					"<add key=\"hibernate.connection.driver_class\" value=\"NHibernate.Driver.SqlClientDriver\" />\r\n\t\t\t\t" + 
					"<add key=\"hibernate.dialect\" value=\"NHibernate.Dialect.MsSql2000Dialect\" />\r\n\t\t\t\t" + 
					"<add key=\"hibernate.connection.provider\" value=\"NHibernate.Connection.DriverConnectionProvider\" />\r\n\t\t\t\t" + 
					"<add key=\"hibernate.connection.connection_string\" value=\"Data Source=.;Initial Catalog=yourdatabase;Integrated Security=True;\" />\r\n\t\t\t" + 
					"</config>\r\n\t\t</facility>" );
			}
			else
			{
				ReplaceToken(codeWindow, "!AR!", "" );
			}

			if (useNHIntegration)
			{
				ReplaceToken(codeWindow, "!NH!", "<facility id=\"nhibernatefacility\" \r\n\t\t\t" +
					"type=\"Castle.Facilities.NHibernateIntegration.NHibernateFacility, Castle.Facilities.NHibernateIntegration\">\r\n" + 
					"\t\t\t<factory id=\"sessionFactory1\">\r\n" +
					"\t\t\t\t<settings>\r\n" +
					"\t\t\t\t\t<item key=\"hibernate.connection.provider\">NHibernate.Connection.DriverConnectionProvider</item>\r\n" +
					"\t\t\t\t\t<item key=\"hibernate.connection.driver_class\">NHibernate.Driver.SqlClientDriver</item>\r\n" +
					"\t\t\t\t\t<item key=\"hibernate.connection.connection_string\">Server=(local);initial catalog=test;Integrated Security=SSPI</item>\r\n" + 
					"\t\t\t\t\t<item key=\"hibernate.dialect\">NHibernate.Dialect.MsSql2000Dialect</item>\r\n" +
					"\t\t\t\t</settings>\r\n" +
					"\t\t\t\t<resources>\r\n" +
					"\t\t\t\t\t<resource name=\"relative or absolute path to your file.hbm.xml\" />\r\n" +
					"\t\t\t\t\t<resource name=\"relative or absolute path to other file.hbm.xml\" />\r\n" +
					"\t\t\t\t</resources>\r\n" +
					"\t\t\t</factory>\r\n" +
					"\t\t</facility>" );
			}
			else
			{
				ReplaceToken(codeWindow, "!NH!", "" );
			}

			if (useATMIntegration)
			{
				ReplaceToken(codeWindow, "!ATM!", "<facility id=\"transaction\"\r\n\t\t\t" + 
					"type=\"Castle.Facilities.AutomaticTransactionManagement.TransactionFacility, " + 
					"Castle.Facilities.AutomaticTransactionManagement\" />" );
			}
			else
			{
				ReplaceToken(codeWindow, "!ATM!", "" );
			}

			if (useLoggingIntegration)
			{
				ReplaceToken(codeWindow, "!LOGGING!", "<facility id=\"logging.facility\"\r\n\t\t\t" + 
					"type=\"Castle.Facilities.Logging.LoggingFacility, Castle.Facilities.Logging\"\r\n\t\t\t" + 
					"loggingApi=\"Log4net\">\r\n\t\t</facility>" );
			}
			else
			{
				ReplaceToken(codeWindow, "!LOGGING!", "" );
			}

			codeWindow.Close(vsSaveChanges.vsSaveChangesYes);
		}

		private void AddControllerConfigFile(Project project)
		{
			String projectFile = GetTemplateFileName(@"CSharp\MRProject\controllers.config");

			project.ProjectItems.AddFromTemplate(projectFile, "controllers.config");

			PerformReplacesOn(project, projectName, "controllers.config");
		}

		private void ModifyWebConfig(Project project)
		{
			ProjectItem item = project.ProjectItems.Item("web.config");

			Window codeWindow = item.Open(Constants.vsViewKindCode);
	
			codeWindow.Activate();
	
			ReplaceToken(codeWindow, "!ENABLEWINDSOR!", enableWindsorIntegration.ToString().ToLower() );
			ReplaceToken(codeWindow, "!SECTIONS!", CreateSectionsConfigNode() );
			ReplaceToken(codeWindow, "!MONORAILNODE!", CreateMonoRailConfigNode() );
			ReplaceToken(codeWindow, "!CASTLENODE!", CreateCastleConfigNode() );

			codeWindow.Close(vsSaveChanges.vsSaveChangesYes);
		}

		private string CreateSectionsConfigNode()
		{
			sb.Length = 0;

			if (enableWindsorIntegration)
			{
				sb.Append("\t\t<section \r\n" +
					"\t\t\tname=\"castle\" \r\n" +
					"\t\t\ttype=\"Castle.Windsor.Configuration.AppDomain.CastleSectionHandler, Castle.Windsor\" />");
			}

			if (useBrail)
			{
				sb.Append("\t\t<section \r\n" +
					"\t\t\tname=\"Brail\" \r\n" +
					"\t\t\ttype=\"Castle.MonoRail.Views.Brail.BrailConfigurationSection, Castle.MonoRail.Views.Brail\" />");
			}

			return sb.ToString();
		}

		private string CreateMonoRailConfigNode()
		{
			sb.Length = 0;

			if (!enableWindsorIntegration)
			{
				sb.Append("\t\t<controllers>\r\n");
				sb.AppendFormat("\t\t\t<assembly>{0}</assembly>\r\n", projectName);
				sb.Append("\t\t</controllers>\r\n");
			}

			sb.Append("\r\n");

			if (useNVelocity || useBrail)
			{
				sb.Append("\t\t<viewEngine\r\n");
				sb.Append("\t\t\tviewPathRoot=\"views\"\r\n");
				sb.AppendFormat("\t\t\tcustomEngine=\"{0}\" />\r\n", useNVelocity ? 
					"Castle.MonoRail.Framework.Views.NVelocity.NVelocityViewEngine, Castle.MonoRail.Framework.Views.NVelocity" : 
					"Castle.MonoRail.Views.Brail.BooViewEngine, Castle.MonoRail.Views.Brail");
			}

			sb.Append("\r\n");

			return sb.ToString();
		}

		private string CreateCastleConfigNode()
		{
			sb.Length = 0;

			if (useBrail)
			{
				sb.Append("\t<Brail \r\n");
				sb.Append("\t\tdebug=\"false\"\r\n");
				sb.Append("\t\tsaveToDisk=\"false\"\r\n");
				sb.Append("\t\tsaveDirectory=\"Brail_Generated_Code\"\r\n");
				sb.Append("\t\tbatch=\"true\"\r\n");
				sb.Append("\t\tcommonScriptsDirectory=\"CommonScripts\" >\r\n");
				sb.Append("\t\t<reference assembly=\"Castle.MonoRail.Framework\"/>\r\n");
				sb.Append("\t</Brail>\r\n\r\n");
			}

			if (enableWindsorIntegration)
			{
				sb.Append("\t<castle>\r\n");
				sb.Append("\t\t<include uri=\"file://facilities.config\" />\r\n\r\n");
				sb.Append("\t\t<include uri=\"file://controllers.config\" />\r\n\r\n");
				sb.Append("\t\t<!-- <include uri=\"file://components.config\" /> -->\r\n\r\n");
				sb.Append("\t\t<!-- <include uri=\"file://services.config\" /> -->\r\n");
				sb.Append("\t</castle>\r\n");
			}

			return sb.ToString();
		}

		private Project CreateTestProject(string localProjectPath, string projectName)
		{
			String localTestProjectPath = Path.Combine(localProjectPath, @"..\" + projectName + ".Tests");
			localTestProjectPath = new DirectoryInfo(localTestProjectPath).FullName;

			String testProjectFile = GetTemplateFileName(@"CSharp\MRProjectTest\MRProjectTest.csproj");
			EnsureDirExists(localTestProjectPath);

			Project testProject = 
				dteInstance.Solution.AddFromTemplate(testProjectFile, localTestProjectPath, projectName + ".Tests.csproj", false);

			PerformReplacesOn(testProject, projectName, "HomeControllerTestCase.cs");
			PerformReplacesOn(testProject, projectName, "App.config");

			return testProject;
		}

		public IntPtr Handle
		{
			get { return new IntPtr(owner); }
		}	
	}
}
