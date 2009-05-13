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

namespace Castle.VSNetIntegration.CastleWizards
{
	using System;
	using System.Collections;
	using System.IO;
	using System.Runtime.InteropServices;
	using System.Text;
	using System.Xml;

	using Castle.VSNetIntegration.CastleWizards.Dialogs.Panels;
	using Castle.VSNetIntegration.CastleWizards.Shared;
	using Castle.VSNetIntegration.CastleWizards.Shared.Dialogs;
	using Castle.VSNetIntegration.Shared;

	using EnvDTE;
	using Constants=Castle.VSNetIntegration.CastleWizards.Shared.Constants;
	using EnvConstants = EnvDTE.Constants;

	[Guid("9FF77D9F-E4FC-47EE-8E8B-0079FC2F2478")]
	[ProgId("Castle.MonoRailProjectWizardVS8")]
	[ComDefaultInterface(typeof(IDTWizard))]
	[ComVisibleAttribute(true)]
	public class MonoRailProjectWizard : BaseProjectWizard
	{
		private MRPanel optionsPanel = new MRPanel();
		private ContainerIntegrationPanel integrationPanel = new ContainerIntegrationPanel();

		protected override void AddPanels(WizardDialog dlg)
		{
			dlg.AddPanel(optionsPanel);
			dlg.AddPanel(integrationPanel);
		}

		protected override String WizardTitle
		{
			get { return "New MonoRail project"; }
		}

		protected override void AddExtensions(IList extensions)
		{
			extensions.Add(new TestProjectExtension());
			extensions.Add(new ATMExtension());
			extensions.Add(new ARIntegrationExtension());
			extensions.Add(new NHIntegrationExtension());
			extensions.Add(new LoggingIntegrationExtension());
		}

		protected override void AddProjects(ExtensionContext context)
		{
			String projectFile = context.GetTemplateFileName(@"CSharp\MRProject\MRProject.csproj");

			Utils.EnsureDirExists(LocalProjectPath);

			Project project = 
				context.DteInstance.Solution.AddFromTemplate(projectFile, LocalProjectPath, ProjectName + ".csproj", Exclusive);

			project.Properties.Item("DefaultNamespace").Value = NameSpace;

			Utils.PerformReplacesOn(project, NameSpace, LocalProjectPath, "Controllers\\BaseController.cs");
			Utils.PerformReplacesOn(project, NameSpace, LocalProjectPath, "Controllers\\HomeController.cs");
			Utils.PerformReplacesOn(project, NameSpace, LocalProjectPath, "Controllers\\LoginController.cs");
			Utils.PerformReplacesOn(project, NameSpace, LocalProjectPath, "Controllers\\ContactController.cs");
			Utils.PerformReplacesOn(project, NameSpace, LocalProjectPath, "Models\\ContactInfo.cs");
			Utils.PerformReplacesOn(project, NameSpace, LocalProjectPath, "Models\\Country.cs");
			Utils.PerformReplacesOn(project, NameSpace, LocalProjectPath, "global.asax");

			context.Projects.Add(Constants.ProjectMain, project);

			AddGlobalApplication(project);
			AddHomeViews(project);
			AddLoginViews(project);
			AddContactViews(project);
			AddLayout(project);
			AddRescue(project);
			
			CreateXmlDomForConfig(project, MRConfigConstants.Web);
			
			UpdateReferences(project);
			UpdateProjectToUseCassini(project);
			
			base.AddProjects(context);
		}
		
		protected override void PostProcess(ExtensionContext context)
		{
			ProcessWebConfig();

			base.PostProcess(context);

			PersistWebConfig();
		}

		private void ProcessWebConfig()
		{
			Project project = Context.Projects[Constants.ProjectMain];
			XmlDocument webConfigDoc = (XmlDocument) Context.Properties[MRConfigConstants.Web];
			XmlElement mrNode = (XmlElement) webConfigDoc.SelectSingleNode("configuration/monorail");

			if (!HasEnabledWindsorIntegration)
			{
				XmlElement controllersElem = webConfigDoc.CreateElement("controllers");
				XmlElement assemblyElem = webConfigDoc.CreateElement("assembly");

				controllersElem.AppendChild(assemblyElem);
				assemblyElem.AppendChild(webConfigDoc.CreateTextNode(ProjectName));

				mrNode.AppendChild(controllersElem);
			}
			else
			{
				mrNode.SetAttribute("useWindsorIntegration", "true");

				XmlNode configSectionsNode = webConfigDoc.SelectSingleNode("configuration/configSections");
				XmlElement castleSectionElem = webConfigDoc.CreateElement("section");

				castleSectionElem.SetAttribute("name", "castle");
				castleSectionElem.SetAttribute("type", "Castle.Windsor.Configuration.AppDomain.CastleSectionHandler, Castle.Windsor");

				configSectionsNode.AppendChild(castleSectionElem);

				XmlElement castleElem = webConfigDoc.CreateElement("castle");

				XmlElement includePropElem = webConfigDoc.CreateElement("include");
				XmlElement includeFacElem = webConfigDoc.CreateElement("include");
				XmlElement includeControllersElem = webConfigDoc.CreateElement("include");
				XmlElement includeComponentsElem = webConfigDoc.CreateElement("include");

				includePropElem.SetAttribute("uri", "file://Config/properties.config");
				includeFacElem.SetAttribute("uri", "file://Config/facilities.config");
				includeControllersElem.SetAttribute("uri", "file://Config/controllers.config");
				includeComponentsElem.SetAttribute("uri", "file://Config/components.config");

				castleElem.AppendChild(includePropElem);
				castleElem.AppendChild(includeFacElem);
				castleElem.AppendChild(includeControllersElem);
				castleElem.AppendChild(includeComponentsElem);

				webConfigDoc.DocumentElement.AppendChild(webConfigDoc.CreateComment("Container configuration. For more information see http://www.castleproject.org/container/documentation/trunk/manual/windsorconfigref.html"));
				webConfigDoc.DocumentElement.AppendChild(webConfigDoc.CreateComment("and http://www.castleproject.org/container/documentation/trunk/usersguide/compparams.html"));
				webConfigDoc.DocumentElement.AppendChild(castleElem);

				ProjectItem configItem = project.ProjectItems.AddFolder("config", EnvConstants.vsProjectItemKindPhysicalFolder);

				// Controllers.config
				String projectFile = Context.GetTemplateFileName(@"CSharp\MRProject\Config\controllers.config");
				configItem.ProjectItems.AddFromTemplate(projectFile, MRConfigConstants.Controllers);

				// Facilities.config
				projectFile = Context.GetTemplateFileName(@"CSharp\MRProject\Config\facilities.config");
				configItem.ProjectItems.AddFromTemplate(projectFile, MRConfigConstants.Facilities);

				// Properties.config
				projectFile = Context.GetTemplateFileName(@"CSharp\MRProject\Config\properties.config");
				configItem.ProjectItems.AddFromTemplate(projectFile, MRConfigConstants.Properties);

				// Components.config
				projectFile = Context.GetTemplateFileName(@"CSharp\MRProject\Config\components.config");
				configItem.ProjectItems.AddFromTemplate(projectFile, MRConfigConstants.Components);

				Utils.CreateXmlDomForConfig(Context, project, configItem.ProjectItems.Item(MRConfigConstants.Properties), 
					MRConfigConstants.PropertiesConfig);

				Utils.CreateXmlDomForConfig(Context, project, configItem.ProjectItems.Item(MRConfigConstants.Facilities),
					MRConfigConstants.FacilitiesConfig);

				Utils.CreateXmlDomForConfig(Context, project, configItem.ProjectItems.Item(MRConfigConstants.Components),
					MRConfigConstants.ComponentsConfig);

				XmlDocument controllersDom =
					Utils.CreateXmlDomForConfig(Context, project, configItem.ProjectItems.Item(MRConfigConstants.Controllers),
						MRConfigConstants.ControllersConfig);
				
				RegisterControllers(controllersDom);
			}

			AddViewEngineConfiguration(webConfigDoc, mrNode);
			AddRoutingConfiguration(webConfigDoc, mrNode);
		}

		private void RegisterControllers(XmlDocument dom)
		{
			RegisterController(dom, "home.controller", "HomeController");
			RegisterController(dom, "login.controller", "LoginController");
			RegisterController(dom, "contact.controller", "ContactController");
		}

		private void RegisterController(XmlDocument dom, string name, string ns)
		{
			XmlElement compElem = dom.CreateElement("component");

			compElem.SetAttribute("id", name);
			compElem.SetAttribute("type", String.Format("{0}.Controllers.{1}, {0}", Context.ProjectName, ns));

			dom.DocumentElement.SelectSingleNode("components").AppendChild(compElem);
		}

		private void AddRoutingConfiguration(XmlDocument webConfigDoc, XmlElement mrNode)
		{
			if (!optionsPanel.EnableRouting) return;

			XmlElement routingElem = webConfigDoc.CreateElement("routing");
			XmlElement rule1 = webConfigDoc.CreateElement("rule");
			XmlElement rule1pattern = webConfigDoc.CreateElement("pattern");
			XmlElement rule1replace = webConfigDoc.CreateElement("replace");

			routingElem.AppendChild(rule1);

			rule1.AppendChild(rule1pattern);
			rule1.AppendChild(rule1replace);

			rule1pattern.AppendChild( webConfigDoc.CreateTextNode(@"(/blog/posts/)(\d+)/(\d+)/(.)*$") );
			rule1replace.AppendChild( webConfigDoc.CreateCDataSection(" /blog/view.castle?year=$2&month=$3 ") );

			mrNode.AppendChild(webConfigDoc.CreateComment("For more information on routing see http://www.castleproject.org/monorail/documentation/trunk/advanced/routing.html"));
			mrNode.AppendChild(routingElem);
			
			// Select Single Node: configuration/system.web/httpModules
			XmlElement httpModules = (XmlElement) webConfigDoc.DocumentElement.SelectSingleNode("/configuration/system.web/httpModules");
			
			// Add <add name="routing" type="Castle.MonoRail.Framework.RoutingModule, Castle.MonoRail.Framework" />
			XmlElement addRoutingElem = webConfigDoc.CreateElement("add");
			
			addRoutingElem.SetAttribute("name", "routing");
			addRoutingElem.SetAttribute("type", "Castle.MonoRail.Framework.RoutingModule, Castle.MonoRail.Framework");
			
			httpModules.InsertBefore(addRoutingElem, httpModules.FirstChild);
		}

		private void AddViewEngineConfiguration(XmlDocument webConfigDoc, XmlElement mrNode)
		{
			if (!optionsPanel.VeWebForms)
			{
				XmlElement viewEngineElem = webConfigDoc.CreateElement("viewEngines");
				viewEngineElem.SetAttribute("viewPathRoot", "Views");

				XmlElement addElem = webConfigDoc.CreateElement("add");
				addElem.SetAttribute("xhtml", "false");
				
				if (optionsPanel.VeNVelocity)
				{
					addElem.SetAttribute("type",
						"Castle.MonoRail.Framework.Views.NVelocity.NVelocityViewEngine, Castle.MonoRail.Framework.Views.NVelocity");
				}
				else if (optionsPanel.VeBrail)
				{
					addElem.SetAttribute("type",
						"Castle.MonoRail.Views.Brail.BooViewEngine, Castle.MonoRail.Views.Brail");

					AddBrailCommonConfiguration(webConfigDoc);
				}
				else if (optionsPanel.VeWebForms)
				{
					addElem.SetAttribute("type",
						"Castle.MonoRail.Framework.Views.Aspx.WebFormsViewEngine, Castle.MonoRail.Framework");
				}

				viewEngineElem.AppendChild(addElem);
				mrNode.AppendChild(viewEngineElem);
			}
		}

		private void AddBrailCommonConfiguration(XmlDocument webConfigDoc)
		{
			XmlNode configSectionsNode = webConfigDoc.SelectSingleNode("configuration/configSections");
			XmlElement castleSectionElem = webConfigDoc.CreateElement("section");

			castleSectionElem.SetAttribute("name", "brail");
			castleSectionElem.SetAttribute("type", "Castle.MonoRail.Views.Brail.BrailConfigurationSection, Castle.MonoRail.Views.Brail");

			configSectionsNode.AppendChild(castleSectionElem);

			XmlComment comment = webConfigDoc.CreateComment("For more on Brail " +
				"configuration see http://www.castleproject.org/monorail/documentation/trunk/viewengines/brail/index.html");
			XmlElement brailElem = webConfigDoc.CreateElement("brail");

			brailElem.SetAttribute("debug", "false");
			brailElem.SetAttribute("saveToDisk", "false");
			brailElem.SetAttribute("saveDirectory", "Brail_Generated_Code");
			brailElem.SetAttribute("batch", "true");
			brailElem.SetAttribute("commonScriptsDirectory", "CommonScripts");

			XmlElement refElem = webConfigDoc.CreateElement("reference");
			refElem.SetAttribute("assembly", "Castle.MonoRail.Framework");

			brailElem.AppendChild(refElem);

			webConfigDoc.DocumentElement.AppendChild(comment);
			webConfigDoc.DocumentElement.AppendChild(brailElem);
		}

		private void AddGlobalApplication(Project project)
		{
			String globalAppFile;

			if (HasEnabledWindsorIntegration)
			{
				globalAppFile = Context.GetTemplateFileName(@"CSharp\MRProject\ContainerGlobalApplication.cs");
			}
			else
			{
				globalAppFile = Context.GetTemplateFileName(@"CSharp\MRProject\SimpleGlobalApplication.cs");
			}

			project.ProjectItems.AddFromTemplate(globalAppFile, "GlobalApplication.cs");
			
			Utils.PerformReplacesOn(project, NameSpace, LocalProjectPath, "GlobalApplication.cs");
		}

		private void UpdateProjectToUseCassini(Project project)
		{
			ConfigurationManager confMng = project.ConfigurationManager;

			for (int i = 1; i <= confMng.Count; i++)
			{
				Configuration configuration = confMng.Item(i,".NET");

				configuration.Properties.Item("StartAction").Value = 
					VSLangProj.prjStartAction.prjStartActionProgram;
				
				configuration.Properties.Item("StartWorkingDirectory").Value = LocalProjectPath;
				
				configuration.Properties.Item("StartProgram").Value = Context.CassiniLocation;

				configuration.Properties.Item("StartArguments").Value = 
					String.Format("\"{0}\" {1} {2}", LocalProjectPath, 81, "/");
			}
		}

		private void AddHomeViews(Project project)
		{
			AddView(project, "Home", "Index");
		}

		private void AddLoginViews(Project project)
		{
			AddView(project, "Login", "Index");
			AddView(project, "Login", "authenticate");
		}

		private void AddContactViews(Project project)
		{
			AddView(project, "Contact", "index");
			AddView(project, "Contact", "confirmation");
		}

		private void AddView(Project project, string controller, string action)
		{
			String fileExtension = ViewFileExtension();

			String viewFile = String.Format("{0}.{1}", action, fileExtension);
			String viewTemplateFile = Context.GetTemplateFileName(String.Format(@"CSharp\MRProject\ViewsPlaceHolder\{0}\{1}", 
				controller, viewFile));

			project.ProjectItems.Item("Views").ProjectItems.Item(controller).ProjectItems.AddFromTemplate(viewTemplateFile, viewFile);
		}
		
		private string ViewFileExtension()
		{
			if (optionsPanel.VeNVelocity)
			{
				return "vm";
			}
			else if (optionsPanel.VeBrail)
			{
				return "brail";
			}
			else
			{
				return "aspx";
			}
		}

		private void AddRescue(Project project)
		{
			String viewTemplateFile;
			String viewFile;

			if (optionsPanel.VeNVelocity)
			{
				viewFile = "generalerror.vm";
				viewTemplateFile = Context.GetTemplateFileName(@"CSharp\MRProject\rescue_default.vm");
			}
			else if (optionsPanel.VeBrail)
			{
				viewFile = "generalerror.brail";
				viewTemplateFile = Context.GetTemplateFileName(@"CSharp\MRProject\rescue_default.brail");
			}
			else
			{
				viewFile = "generalerror.aspx";
				viewTemplateFile = Context.GetTemplateFileName(@"CSharp\MRProject\rescue_default.aspx");
			}

			project.ProjectItems.Item("Views").
				ProjectItems.Item("rescues").
				ProjectItems.AddFromTemplate(viewTemplateFile, viewFile);
		}

		private void AddLayout(Project project)
		{
			String viewTemplateFile;
			String viewFile;

			if (optionsPanel.VeNVelocity)
			{
				viewFile = "default.vm";
				viewTemplateFile = Context.GetTemplateFileName(@"CSharp\MRProject\ViewsPlaceHolder\layout_default.vm");
			}
			else if (optionsPanel.VeBrail)
			{
				viewFile = "default.brail";
				viewTemplateFile = Context.GetTemplateFileName(@"CSharp\MRProject\ViewsPlaceHolder\layout_default.brail");
			}
			else
			{
				viewFile = "default.aspx";
				viewTemplateFile = Context.GetTemplateFileName(@"CSharp\MRProject\ViewsPlaceHolder\layout_default.aspx");
			}

			project.ProjectItems.Item("Views").
				ProjectItems.Item("layouts").
				ProjectItems.AddFromTemplate(viewTemplateFile, viewFile);
		}

		private void UpdateReferences(Project project)
		{
			if (optionsPanel.VeNVelocity)
			{
				Utils.AddReference(project, "Castle.MonoRail.Framework.Views.NVelocity.dll");
				Utils.AddReference(project, "NVelocity.dll");
			}
			else if (optionsPanel.VeBrail)
			{
				// Utils.AddReference(project, "anrControls.Markdown.NET.dll");
				// Utils.AddReference(project, "Boo.Lang.Compiler.dll");
				// Utils.AddReference(project, "Boo.Lang.dll");
				// Utils.AddReference(project, "Boo.Lang.Parser.dll");	
				Utils.AddReference(project, "Castle.MonoRail.Views.Brail");
			}

			if (HasEnabledWindsorIntegration)
			{
				Utils.AddReference(project, "Castle.DynamicProxy.dll");
				Utils.AddReference(project, "Castle.MicroKernel.dll");
				Utils.AddReference(project, "Castle.Core.dll");
				Utils.AddReference(project, "Castle.Windsor.dll");
				Utils.AddReference(project, "Castle.MonoRail.WindsorExtension.dll");
			}
		}

		private XmlDocument CreateXmlDomForConfig(Project project, String file)
		{
			return Utils.CreateXmlDomForConfig(Context, project, file);
		}

		private void PersistWebConfig()
		{
			Project project = Context.Projects[Constants.ProjectMain];

			IList configFiles = (IList) Context.Properties[Constants.ConfigFileList];

			foreach(String file in configFiles)
			{
				ProjectItem item;

				if (file.IndexOf("\\") > -1)
				{
					string[] p = file.Split('\\');
					item = project.ProjectItems.Item(p[0]);

					for (int i = 1; i < p.Length; i++)
					{
						item = item.ProjectItems.Item(p[i]);
					}
				}
				else
				{
					item = project.ProjectItems.Item(file);
				}

				Window codeWindow = item.Open(EnvConstants.vsViewKindCode);
	
				codeWindow.Activate();

				TextDocument objTextDoc = ( ( EnvDTE.TextDocument )(
					codeWindow.Document.Object( "TextDocument" ) ) );

				EditPoint objEditPt = objTextDoc.StartPoint.CreateEditPoint();
				objEditPt.StartOfDocument();
				objEditPt.Delete(objTextDoc.EndPoint);

				XmlDocument webConfigDoc = (XmlDocument) Context.Properties[file];

				String tempFile = Path.GetTempFileName();

				XmlTextWriter writer = new XmlTextWriter(tempFile, Encoding.UTF8);
				writer.Formatting = Formatting.Indented;
				writer.Indentation = 1;
				writer.IndentChar = '\t';

				webConfigDoc.Save(writer);
				writer.Close();

				objEditPt.InsertFromFile(tempFile);

				codeWindow.Close(vsSaveChanges.vsSaveChangesYes);

				File.Delete(tempFile);
			}
		}

		private bool HasEnabledWindsorIntegration
		{
			get { return ((bool) Context.Properties["enableWindsorIntegration"]); }
		}
	}
}
