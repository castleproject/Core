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

[System.Reflection.DefaultMember("Item")]
class FakeRequest(IRequest):
	[Property(Headers)]
	headers as NameValueCollection
	
	[Property(Files)]
	files as IDictionary
	
	[Property(Params)]
	params as NameValueCollection
	
	[Property(IsLocal)]
	isLocal as bool
	
	[Property(Uri)]
	uri as Uri
	
	def BinaryRead(count as int) as (byte):
		pass
		
	Item(key as string) as string:
		get:
			return null
	
	def ReadCookie( name as string) as string:
		return null
	
	def ValidateInput():
		pass
	
	[Property(QueryString)]
	queryString as NameValueCollection
	
	[Property(Form)]
	form as NameValueCollection
	
	[Property(UserLanguages)]
	userLanguages as (string)
