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

	public class BreadthFirstWalker : ITreeWalker
	{
		public void Walk(Folder folder, Anakia.Act act)
		{
			Queue queue = new Queue();
			
			queue.Enqueue(folder);
			
			WalkQueue(queue, act);
		}

		private void WalkQueue(Queue queue, Act act)
		{
			while(queue.Count != 0)
			{
				Folder node = (Folder) queue.Dequeue();
				
				foreach(DocumentNode n in node.Documents)
				{
					act(n);
				}
								
				foreach(Folder child in node.Folders)
				{
					queue.Enqueue(child);
				}
			}
		}
	}
}
