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

namespace PerformanceTest
{
	using System;
	using System.Net;
	using System.Text;


	public class App
	{
		public static void Main()
		{
			// First request, just a sanity check

			SendRequestAndAssert("http://localhost/perf/perf1/index.rails", "This is a static content render by my controller");

			// Standard controller test

			long time1 = PerfCheck(1000, "http://localhost/perf/perf1/index.rails", "This is a static content render by my controller");

			// Smart controller test 1

			long time2 = PerfCheck(1000, "http://localhost/perf/perf2/index.rails", "This is a static content render by my controller");

			// Smart controller test 2

			long time3 = PerfCheck(1000, "http://localhost/perf/perf2/save.rails?id=2&name=something&address=something2", "Save method invoked");

			Console.WriteLine("time1 {0} ticks", time1);
			Console.WriteLine("time2 {0} ticks", time2);
			Console.WriteLine("time3 {0} ticks", time3);
		}

		private static long PerfCheck(int times, string url, string expected)
		{
			long start = DateTime.Now.Ticks;

			for(int i=0; i <= times; i++)
			{
				SendRequestAndAssert(url, expected);
			}

			long end = DateTime.Now.Ticks;

			return end - start;
		}

		private static void SendRequestAndAssert(string url, string expected)
		{
			HttpWebResponse response = null;

			try
			{
				HttpWebRequest myReq = (HttpWebRequest) 
					WebRequest.Create(url);

				response = (HttpWebResponse) myReq.GetResponse();
			}
			catch(Exception ex)
			{
				
			}

			AssertContents(expected, response);
		}

		protected static void AssertContents(String expected, HttpWebResponse response)
		{
			int size = expected.Length;
			byte[] contentsArray = new byte[size];
			response.GetResponseStream().Read(contentsArray, 0, size);
			Encoding encoding = Encoding.Default;
			String contents = encoding.GetString(contentsArray);
			if ( !expected.Equals(contents) )
			{
				throw new Exception("Expected " + expected + " but was " + contents);
			}
		}
	}
}
