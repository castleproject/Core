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

namespace PetStore.Web.Components
{
	using System;
	using System.IO;

	using Castle.MonoRail.Framework;


	public class ImageLocationComponent : ViewComponent
	{
		private String productImagesDir;

		public ImageLocationComponent(String productImagesDir)
		{
			this.productImagesDir = productImagesDir;
		}

		public override void Render()
		{
			String appPath = RailsContext.ApplicationPath;

			appPath = Path.Combine("/" + appPath, productImagesDir);

			String image = (String) ComponentParams["image"];
			String border = (String) ComponentParams["border"];
			String width = (String) ComponentParams["width"];
			String height = (String) ComponentParams["height"];

			image = Path.Combine(appPath, image);

			if (border == null) border = "0";

			if (width == null && height == null)
			{
				RenderText(String.Format("<img src=\"{0}\" border=\"{1}\" />", 
					image, border));
			}
			else
			{
				RenderText(String.Format("<img src=\"{0}\" border=\"{1}\" width=\"{2}\" height=\"{3}\" />", 
					image, border, width, height));
			}
		}
	}
}
