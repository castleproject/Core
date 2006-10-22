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

namespace Anakia
{
	using System;
	using System.Collections;
	using System.IO;
	using System.Xml;
	using Anakia.DocData;
	using Commons.Collections;
	using NAnt.Core;
	using NAnt.Core.Attributes;
	using NVelocity;
	using NVelocity.App;
	using NVelocity.Context;

	[TaskName("doctransform")]
	public class NDocTransformTask : Task
	{
		private VelocityEngine velocity;
		private Template template;
		private DirectoryInfo targetDir;
		private string nodcxmlfile;
		private string templateFile;
		private string inheritsFrom;
		private string restrictToNs;
		private string typelist;
		private XmlDocument projectDom;
		private ArrayList types = new ArrayList();
		private ArrayList enums = new ArrayList();
		private int counter;
		private int startOrder;
		private bool ignoresuperclasses;
		private SimpleHelper helper = new SimpleHelper();

		[TaskAttribute("templatefile")]
		public String TemplateFile
		{
			get { return templateFile; }
			set { templateFile = value; }
		}

		[TaskAttribute("ndocxmlfile")]
		public String NDocXmlFile
		{
			get { return nodcxmlfile; }
			set { nodcxmlfile = value; }
		}
		
		[TaskAttribute("targetdir")]
		public DirectoryInfo TargetDir
		{
			get { return targetDir; }
			set { targetDir = value; }
		}
		
		[TaskAttribute("inheritsfrom")]
		public String InheritsFrom
		{
			get { return inheritsFrom; }
			set { inheritsFrom = value; }
		}

		[TaskAttribute("startorder")]
		public int StartOrder
		{
			get { return startOrder; }
			set { startOrder = value; }
		}

		[TaskAttribute("restrictToNs")]
		public String RestrictToNs
		{
			get { return restrictToNs; }
			set { restrictToNs = value; }
		}

		[TaskAttribute("typelist")]
		public string TypeList
		{
			get { return typelist; }
			set { typelist = value; }
		}

		[TaskAttribute("ignoresuperclasses")]
		public bool IgnoreSuperClasses
		{
			get { return ignoresuperclasses; }
			set { ignoresuperclasses = value; }
		}

		#region overrides

		protected override void InitializeTask(XmlNode taskNode)
		{
			base.InitializeTask(taskNode);

			// Initializes NVelocity

			velocity = new VelocityEngine();

			ExtendedProperties props = new ExtendedProperties();
			velocity.Init(props);

			template = velocity.GetTemplate(templateFile);

			// TODO: validate all arguments are present
			
			counter = startOrder + 1;
		}

		protected override void ExecuteTask()
		{
			targetDir.Create();
			
			projectDom = new XmlDocument();
			projectDom.Load(NDocXmlFile);
			
			PerformReplaces(projectDom);

			try
			{
				if (inheritsFrom != null)
				{
					XmlNode baseElemNode = projectDom.SelectSingleNode("//hierarchyType[@id='" + InheritsFrom + "']"); 
								
					BuildDocData((XmlElement)baseElemNode, baseElemNode.ChildNodes, true);
				}
				else if (typelist != null)
				{
					String[] list = typelist.Split(',');
					
					foreach(String type in list)
					{
						String typeName = type.Trim();
						
						XmlNode baseElemNode = projectDom.SelectSingleNode("//hierarchyType[@id='" + typeName + "']"); 
						
						if (baseElemNode == null)
						{
							throw new Exception("Could not find hierarchyType for " + typeName);
						}
								
						BuildDocData((XmlElement)baseElemNode, baseElemNode.ChildNodes, false);
					}
				}
				
				foreach(ClassDocData classDoc in types)
				{
					String targetFileName = "Generated_" + classDoc.name + ".xml";
					String targetFullname = Path.Combine(targetDir.FullName, targetFileName);
					
					File.Delete(targetFullname);
					
					IContext context = CreateContext(classDoc);
					
					using(StreamWriter writer = new StreamWriter(targetFullname, false))
					{
						template.Merge(context, writer);
					}
				}
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex);
				Console.Read();
			}
		}

		#endregion

		private IContext CreateContext(ClassDocData doc)
		{
			VelocityContext context = new VelocityContext();
			
			context.Put("doc", doc);
			context.Put("counter", counter++);
			context.Put("helper", helper);
						
			return context;
		}

		private void BuildDocData(XmlElement node, XmlNodeList nodes, bool recurse)
		{
			String id = node.GetAttribute("id");
			
			Console.WriteLine(id);
			
			XmlNode classNode = projectDom.SelectSingleNode("//class[@id='" + id + "']");
			
			if (classNode != null)
			{
				ClassDocData classDoc = new ClassDocData(ClassType.Class);
				
				PopulateClass(classDoc, (XmlElement) classNode);
				
				types.Add(classDoc);
			}
			
			XmlNode interfaceNode = projectDom.SelectSingleNode("//interface[@id='" + id + "']");
			
			if (interfaceNode != null)
			{
				ClassDocData classDoc = new ClassDocData(ClassType.Interface);
				
				PopulateClass(classDoc, (XmlElement) interfaceNode);
				
				types.Add(classDoc);
			}
			
			XmlNode enumNode = projectDom.SelectSingleNode("//enumeration[@id='" + id + "']");
			
			if (enumNode != null)
			{
				EnumDocData enumDoc = new EnumDocData();
				
				PopulateEnum(enumDoc, (XmlElement) enumNode);
				
				enums.Add(enumDoc);
			}
			
			if (recurse)
			{
				foreach(XmlElement elem in nodes)
				{
					BuildDocData(elem, elem.ChildNodes, recurse);
				}
			}
		}

		private void PopulateClass(ClassDocData doc, XmlElement node)
		{			
			String name = node.GetAttribute("name");
			String id = node.GetAttribute("id");
			String access = node.GetAttribute("access");
			String baseType = node.GetAttribute("baseType");
			
			doc.name = name;
			doc.id = id;
			doc.access = (Visibility) Enum.Parse(typeof(Visibility), access);
		
			PopulateCommonDoc(doc, node);

			// Constructors
			
			doc.constructors = CreateConstructors(node);
			
			// Properties
			
			doc.properties = CreateProperties(node);
			
			// Methods
			
			doc.methods = CreateMethods(node);
			
			// Events
			
			doc.events = CreateEvents(node);
		}

		private ConstructorDocData[] CreateConstructors(XmlElement node)
		{
			ArrayList constructorsCollected = new ArrayList();
			
			foreach(XmlElement constrElem in node.SelectNodes("constructor"))
			{
				String name = constrElem.GetAttribute("name");
				String id = constrElem.GetAttribute("id");
				Visibility access = (Visibility) Enum.Parse(typeof(Visibility), constrElem.GetAttribute("access"));

				ConstructorDocData constr = new ConstructorDocData(name, id, access);
				
				PopulateCommonDoc(constr, constrElem);
				
				XmlNodeList parameters = constrElem.SelectNodes("parameter");
				
				constr.parameters = CreateParameters(parameters, constrElem.SelectSingleNode("documentation"));
				
				constructorsCollected.Add(constr);
			}
			
			return (ConstructorDocData[]) constructorsCollected.ToArray(typeof(ConstructorDocData));
		}

		private PropertyDocData[] CreateProperties(XmlElement node)
		{
			ArrayList propertiesCollected = new ArrayList();
			
			foreach(XmlElement propElem in node.SelectNodes("property"))
			{
				if (IsFrameworkType(propElem.GetAttribute("declaringType")))
				{
					continue;
				}
				if (ignoresuperclasses)
				{
					String declaringType = propElem.GetAttribute("declaringType");
					
					if (declaringType != String.Empty)
					{
						continue;
					}
				}
				
				XmlNodeList parameters = propElem.SelectNodes("parameter");
				
				ParameterDocData[] paramsDoc = CreateParameters(parameters, propElem.SelectSingleNode("documentation"));
				
				String name = propElem.GetAttribute("name");
				String id = propElem.GetAttribute("id");
				String type = propElem.GetAttribute("type");
				Visibility access = (Visibility) Enum.Parse(typeof(Visibility), propElem.GetAttribute("access"));
				
				PropertyDocData prop = new PropertyDocData(name, id, type, access, paramsDoc);
				
				PopulateCommonDoc(prop, propElem);

				propertiesCollected.Add(prop);
			}
			
			return (PropertyDocData[]) propertiesCollected.ToArray(typeof(PropertyDocData));
		}

		private MethodDocData[] CreateMethods(XmlElement node)
		{
			ArrayList methodsCollected = new ArrayList();
			
			foreach(XmlElement methodElem in node.SelectNodes("method"))
			{
				if (IsFrameworkType(methodElem.GetAttribute("declaringType")))
				{
					continue;
				}
				if (ignoresuperclasses)
				{
					String declaringType = methodElem.GetAttribute("declaringType");
					
					if (declaringType != String.Empty)
					{
						continue;
					}
				}
				
				XmlNodeList parameters = methodElem.SelectNodes("parameter");
				
				ParameterDocData[] paramsDoc = CreateParameters(parameters, methodElem.SelectSingleNode("documentation"));
				
				String name = methodElem.GetAttribute("name");
				String id = methodElem.GetAttribute("id");
				String returnType = methodElem.GetAttribute("returnType");
				Visibility access = (Visibility) Enum.Parse(typeof(Visibility), methodElem.GetAttribute("access"));
				
				MethodDocData method = new MethodDocData(name, id, access, returnType, paramsDoc);
				
				PopulateCommonDoc(method, methodElem);

				methodsCollected.Add(method);
			}
			
			return (MethodDocData[]) methodsCollected.ToArray(typeof(MethodDocData));
		}

		private EventDocData[] CreateEvents(XmlElement node)
		{
			ArrayList eventsCollected = new ArrayList();
			
			foreach(XmlElement eventElem in node.SelectNodes("event"))
			{
				if (IsFrameworkType(eventElem.GetAttribute("declaringType")))
				{
					continue;
				}
				if (ignoresuperclasses)
				{
					String declaringType = eventElem.GetAttribute("declaringType");
					
					if (declaringType != String.Empty)
					{
						continue;
					}
				}
												
				String name = eventElem.GetAttribute("name");
				String id = eventElem.GetAttribute("id");
				String type = eventElem.GetAttribute("type");
				Visibility access = (Visibility) Enum.Parse(typeof(Visibility), eventElem.GetAttribute("access"));
				
				// Find delegate
				
				XmlElement delegateNode = (XmlElement) 
					eventElem.ParentNode.SelectSingleNode("//delegate[@id='T:" + type + "']");
				
				String delegateName = delegateNode != null ? delegateNode.GetAttribute("name") : type;
				String returnType = delegateNode != null ? delegateNode.GetAttribute("returnType") : "System.Void";
				XmlNodeList parameters = delegateNode != null ? delegateNode.SelectNodes("parameter") : null;

				ParameterDocData[] paramsDoc = CreateParameters(parameters, eventElem.SelectSingleNode("documentation"));

				EventDocData eventDocData = new EventDocData(name, delegateName, id, access, returnType, paramsDoc);
				
				PopulateCommonDoc(eventDocData, eventElem);

				eventsCollected.Add(eventDocData);
			}
			
			return (EventDocData[]) eventsCollected.ToArray(typeof(EventDocData));
		}

		private bool IsFrameworkType(string type)
		{
			return type.StartsWith("System.");
		}

		private ParameterDocData[] CreateParameters(XmlNodeList parameters, XmlNode docNode)
		{			
			if (parameters == null)
			{
				return new ParameterDocData[0];
			}
		
			ArrayList parametersCollected = new ArrayList();

			foreach(XmlElement paramNode in parameters)
			{
				// name="table" type="System.String"
				
				String name = paramNode.GetAttribute("name");
				String type = paramNode.GetAttribute("type");
				XmlElement paramDocNode = null;
				
				if (docNode != null)
				{
					// <param name="table"></param>
					
					paramDocNode = (XmlElement) 
					               docNode.SelectSingleNode("param[@name='" + name + "']");
				}
				
				parametersCollected.Add(new ParameterDocData(name, type, paramDocNode));
			}
			
			return (ParameterDocData[]) parametersCollected.ToArray(typeof(ParameterDocData));
		}

		private void PopulateEnum(EnumDocData doc, XmlElement node)
		{
			PopulateCommonDoc(doc, node);
			
			String name = node.GetAttribute("name");
			String id = node.GetAttribute("id");
			String access = node.GetAttribute("access");
			String baseType = node.GetAttribute("baseType");
			
			doc.name = name;
			doc.id = id;
			doc.access = (Visibility) Enum.Parse(typeof(Visibility), access);
			
			// Fields
		}

		private void PopulateCommonDoc(CommonDocData doc, XmlElement node)
		{
			XmlNode docNode = node.SelectSingleNode("documentation");
			
			if (docNode != null)
			{
				foreach(XmlNode child in docNode.ChildNodes)
				{
					if (child.Name == "summary")
					{
						doc.summary = (XmlElement) child;
					}
					else if (child.Name == "remarks")
					{
						doc.remarks = (XmlElement) child;
					}
					else if (child.Name == "example")
					{
						doc.example = (XmlElement) child;
					}
				}
			}
		}

		/// <summary>
		/// Replaces documentation nodes by their counter part
		/// para -> p
		/// code -> pre
		/// see -> tt
		/// seealso is removed
		/// </summary>
		/// <param name="dom"></param>
		private void PerformReplaces(XmlDocument dom)
		{
			XmlNodeList list = dom.DocumentElement.SelectNodes("//para");
			
			foreach(XmlNode paraNode in list)
			{
				XmlElement newPelem = dom.CreateElement("p");
				
				newPelem.InnerXml = paraNode.InnerXml;
				
				paraNode.ParentNode.ReplaceChild(newPelem, paraNode);
			}
			
//			list = dom.DocumentElement.SelectNodes("//example");
//			
//			foreach(XmlNode example in list)
//			{
//				foreach(XmlNode child in example.ChildNodes)
//				{
//					if (child.NodeType == XmlNodeType.Text)
//					{
//						int i = child.ChildNodes.Count;
//						
//						XmlElement pElem = dom.CreateElement("p");
//						pElem.InnerXml = child.Value;
//						child.ParentNode.ReplaceChild(pElem, child);
//					}
//					if (child.NodeType == XmlNodeType.Element &&
//						child.Name == "code")
//					{
//						XmlElement preElem = dom.CreateElement("pre");
//						preElem.SetAttribute("format", "cs");
//						preElem.InnerXml = child.InnerXml;
//						child.ParentNode.ReplaceChild(preElem, child);
//					}
//				}
//			}
			
			list = dom.DocumentElement.SelectNodes("//code");
			
			foreach(XmlNode codeNode in list)
			{
				if (codeNode.PreviousSibling != null && 
				    codeNode.PreviousSibling.NodeType == XmlNodeType.Text)
				{
					XmlDocumentFragment fragment = dom.CreateDocumentFragment();
					XmlElement pElem = dom.CreateElement("p");
					pElem.InnerXml = codeNode.PreviousSibling.Value;
					
					fragment.AppendChild(pElem);
					
					XmlElement preElem = dom.CreateElement("pre");
					preElem.SetAttribute("format", "cs");
					preElem.InnerXml = Environment.NewLine + codeNode.InnerXml;
					
					fragment.AppendChild(preElem);
					
					codeNode.ParentNode.ReplaceChild(fragment, codeNode.PreviousSibling);
					codeNode.ParentNode.RemoveChild(codeNode);
				}
				else
				{
					XmlElement newPelem = dom.CreateElement("pre");
					
					newPelem.SetAttribute("format", "cs");
				
					newPelem.InnerXml = Environment.NewLine + codeNode.InnerXml;
					
					codeNode.ParentNode.ReplaceChild(newPelem, codeNode);
				}				
			}
			
			list = dom.DocumentElement.SelectNodes("//seealso");

			foreach(XmlElement seeNode in list)
			{
				seeNode.ParentNode.RemoveChild(seeNode);
			}
			
			list = dom.DocumentElement.SelectNodes("//see");
			
			foreach(XmlElement seeNode in list)
			{
				XmlElement newPelem = dom.CreateElement("tt");
								
				if (seeNode.HasAttribute("langword"))
				{
					newPelem.InnerXml = seeNode.GetAttribute("langword");
				}
				else if (seeNode.HasAttribute("cref"))
				{
					newPelem.InnerXml = NormalizeCRef(seeNode.GetAttribute("cref"));
				}
					
				seeNode.ParentNode.ReplaceChild(newPelem, seeNode);
			}
			
			list = dom.DocumentElement.SelectNodes("//paramref");
			
			foreach(XmlElement seeNode in list)
			{
				XmlElement newPelem = dom.CreateElement("tt");
								
				if (seeNode.HasAttribute("name"))
				{
					newPelem.InnerXml = seeNode.GetAttribute("name");
				}
					
				seeNode.ParentNode.ReplaceChild(newPelem, seeNode);
			}
			
			list = dom.DocumentElement.SelectNodes("//c");
			
			foreach(XmlElement cNode in list)
			{
				XmlElement newPelem = dom.CreateElement("tt");
							
				newPelem.InnerXml = cNode.InnerXml;
					
				cNode.ParentNode.ReplaceChild(newPelem, cNode);
			}
		}

		private string NormalizeCRef(string cref)
		{
			return cref.Substring(2);
		}
	}
}
