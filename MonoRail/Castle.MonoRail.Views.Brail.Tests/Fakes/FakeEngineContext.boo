// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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
namespace Castle.MonoRail.Views.Brail.Tests.Fakes

import System
import System.Collections
import Castle.MonoRail.Framework
import System.Collections.Specialized
import System.Web.Caching
import System.Security.Principal
import Castle.MonoRail.Framework.Internal

class FakeEngineContext(IRailsEngineContext):
	
	def constructor(url as string):
		self(url,"GET")
	
	def constructor(url as string, requestType as  string):
		self.url = url
		self.requestType = requestType
		
	def Trace(message as String):
		pass

	def Transfer(path as String, preserveForm as bool):
		pass
	
	[Property(RequestType)]
	requestType as String 
	
	[Property(Url)]
	url as String 
	
	[Property(UrlReferrer)]
	urlReferrer as String 
	
	[Property(UnderlyingContext)]
	underlyingContext as object 
	
	[Property(Params)]
	params as NameValueCollection = NameValueCollection()
	
	[Property(Session)]
	session as IDictionary = Hashtable()
	
	[Property(Request)]
	request as IRequest = FakeRequest()
	
	[Property(Response)]
	response as IResponse = FakeResponse()
	
	[Property(Cache)]
	cache as Cache
	
	[Property(Flash)]
	flash as IDictionary = Hashtable()
	
	[Property(CurrentUser)]
	currentUser as IPrincipal
	
	[Property(LastException)]
	lastException as Exception
	
	[Property(ApplicationPath)]
	applicationPath as string 
	
	[Property(UrlInfo)]
	urlInfo as UrlInfo 
	
	[Property(Server)]
	server as IServerUtility = FakeServiceUtility()
	
	Output:
		get:
			return response.Output.ToString()
