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
import System.IO
import System.Collections
import Castle.MonoRail.Framework
import System.Collections.Specialized
import System.Web
import System.Web.Caching
import System.Security.Principal
import Castle.MonoRail.Framework.Internal

class FakeResponse(IResponse):

	headers = NameValueCollection()

	def AppendHeader(name as string, value as string):
		headers.Add(name,value)
	
	[Property(Output)]
	output as TextWriter = StringWriter()
	
	[Property(StatusCode)]
	statusCode as int

	[Property(Charset)]
	charset as string		
	
	[Property(CachePolicy)]		
	cachePolicy as HttpCachePolicy
	
	[Property(CacheControlHeader)]
	cacheControlHeader as string
	
	OutputStream:
		get:
			raise NotImplementedException()
	
	ContentType:
		get:
			val = headers.GetValues("Content-Type")
			if val and val[0].Length>0:
				return val[0]
			return "text/html"
		set:
			if headers.GetValues("Content-Type") is not null:
				headers.Remove("Content-Type")
			headers.Add("Content-Type",value)
	
	def Write(s as string):
		output.Write(s)
		
	def Write(obj as object):
		output.Write(obj)
	
	def Write(ch as char):
		output.Write(ch)
	
	def Write(buffer as (char), index as int, count as int):
		output.Write(buffer,index,count)
		
	def WriteFile(fileName as string):
		raise NotImplementedException()
		
	def Redirect(controller as string, action as string):
		raise NotImplementedException()
	
	def Redirect(area as string, controller as string, action as string):
		raise NotImplementedException()
	
	def Redirect(url as string):
		raise NotImplementedException()
	
	def Redirect(url as string, endProcess as bool):
		raise NotImplementedException()
	
	def CreateCookie(name as string, value as string):
		raise NotImplementedException()
	
	def CreateCookie(name as string, value as string, expiration as DateTime):
		raise NotImplementedException()
		