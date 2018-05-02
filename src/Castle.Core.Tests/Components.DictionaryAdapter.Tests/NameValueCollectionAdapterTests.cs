// Copyright 2004-2014 Castle Project - http://www.castleproject.org/
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

namespace Castle.Components.DictionaryAdapter.Tests
{
    using System.Collections.Specialized;

    using NUnit.Framework;

    [TestFixture]
    public class NameValueCollectionAdapterTests
    {

        [SetUp]
        public void SetUp()
        {
            nameValueCollection = new NameValueCollection();
        }

        private NameValueCollection nameValueCollection;

        [Test]
        public void Contains_IsCaseInsensitive()
        {
            var adapter = new NameValueCollectionAdapter(nameValueCollection);
            adapter["a key"] = "a value";

            Assert.IsTrue(adapter.Contains("A Key"));
        }

        [Test]
        public void Contains_IsCorrectWhenValueIsNull()
        {
            var adapter = new NameValueCollectionAdapter(nameValueCollection);
            adapter["a key"] = null;

            Assert.IsTrue(adapter.Contains("A Key"));
        }
    }
}
