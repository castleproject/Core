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

namespace Castle.MonoRail.Framework.Views.StringTemplateView.Configuration
{
	using System;

	public sealed class ConfigConstants
	{
		private ConfigConstants() { throw new InvalidOperationException(); }
	
		public static readonly char[] PATH_SEPARATOR_CHARS = new char[] { '\\', '/' };

		public static readonly string STGROUP_NAME_PREFIX			= "STView_Views";
		public static readonly string HELPER_RESOURCE_NAMESPACE		= "Castle.MonoRail.Framework.Views.StringTemplateView.Helpers";

		public static readonly string LAYOUTS_DIR					= "layouts";
		public static readonly string SHARED_DIR					= "shared";

		public static readonly string COMPONENT_BODY_KEY			= "body";

		public static readonly string CONTEXT_ATTRIB_KEY			= "context";
		public static readonly string CHILD_CONTENT_ATTRIB_KEY		= "childContent";
		public static readonly string REQUEST_ATTRIB_KEY			= "request";
		public static readonly string RESPONSE_ATTRIB_KEY			= "response";
		public static readonly string SESSION_ATTRIB_KEY			= "session";
		public static readonly string SITEROOT_ATTRIB_KEY			= "siteRoot";
		public static readonly string CONTROLLER_ATTRIB_KEY			= "controller";

		public static readonly string ELEMENT_stview_sectionname	= "STViewEngine";
		public static readonly string ATTR_template_lexer_type		= "template-lexer";
		public static readonly string ATTR_template_writer_type		= "template-writer";

		public static readonly string ELEMENT_attrib_renderers		= "attribute-renderers";
		public static readonly string ELEMENT_attrib_renderer		= "attribute-renderer";
		public static readonly string ATTR_renderer_area			= "area";
		public static readonly string ATTR_renderer_type			= "renderer-type";
		public static readonly string ATTR_renderer_target_type		= "attribute-type";
	}
}
