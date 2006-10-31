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

namespace Castle.Windsor.Configuration.Interpreters.XmlProcessor.ElementProcessors
{
	using System;
	using System.Collections;
	using System.Xml;

	enum StatementState
	{
		Init,
		Collect,
		Finished
	}

	public class IfProcessingInstructionProcessor : AbstractXmlNodeProcessor
	{
		private static readonly XmlNodeType[] acceptNodes = new XmlNodeType[] {XmlNodeType.ProcessingInstruction};

		private static readonly String IfPiName = "if";
		private static readonly String EndPiName = "end";
		private static readonly String ElsePiName = "else";
		private static readonly String ElsifPiName = "elsif";

		public IfProcessingInstructionProcessor()
		{
		}

		public override String Name
		{
			get { return IfPiName; }
		}

		public override XmlNodeType[] AcceptNodeTypes
		{
			get { return acceptNodes; }
		}

		public override void Process(IXmlProcessorNodeList nodeList, IXmlProcessorEngine engine)
		{
			XmlProcessingInstruction node = nodeList.Current as XmlProcessingInstruction;

			AssertData(node, true);

			StatementState state = engine.HasFlag(node.Data) ? StatementState.Collect : StatementState.Init;

			ArrayList nodesToProcess = new ArrayList();
			int nestedLevels = 0;

			RemoveItSelf(nodeList.Current);

			while(nodeList.MoveNext())
			{
				if (nodeList.Current.NodeType == XmlNodeType.ProcessingInstruction)
				{
					XmlProcessingInstruction pi = nodeList.Current as XmlProcessingInstruction;

					if (pi.Name == EndPiName)
					{
						nestedLevels--;

						if (nestedLevels < 0)
						{
							RemoveItSelf(nodeList.Current);
							break;
						}
					}
					else if (pi.Name == IfPiName)
					{
						nestedLevels++;
					}
					else if (nestedLevels == 0)
					{
						if (pi.Name == ElsePiName || pi.Name == ElsifPiName)
						{
							ProcessElseElement(pi, engine, ref state);
							continue;
						}
					}
				}

				if (state == StatementState.Collect)
				{
					nodesToProcess.Add(nodeList.Current);
				}
				else
				{
					RemoveItSelf(nodeList.Current);
				}
			}

			if (nestedLevels != -1)
			{
				throw new XmlProcessorException("Unbalanced pi if element");
			}

			if (nodesToProcess.Count > 0)
			{
				engine.DispatchProcessAll(new DefaultXmlProcessorNodeList(nodesToProcess));
			}
		}

		private void ProcessElseElement(XmlProcessingInstruction pi, IXmlProcessorEngine engine, ref StatementState state)
		{
			AssertData(pi, pi.Name == ElsifPiName);

			if (state == StatementState.Collect)
			{
				state = StatementState.Finished;
			}
			else if (pi.Name == ElsePiName || engine.HasFlag(pi.Data))
			{
				if (state == StatementState.Init)
				{
					state = StatementState.Collect;
				}
			}

			RemoveItSelf(pi);
			return;
		}

		private void AssertData(XmlProcessingInstruction pi, bool requireData)
		{
			String data = pi.Data.Trim();

			if (data == "" && requireData)
			{
				throw new XmlProcessorException("Element '{0}' must have a flag attribute", pi.Name);
			}
			else if (data != "")
			{
				if (!requireData)
				{
					throw new XmlProcessorException("Element '{0}' cannot have any attributes", pi.Name);
				}
			}
		}
	}
}