## Copyright 2004-2006 Castle Project - http:##www.castleproject.org/
## 
## Licensed under the Apache License, Version 2.0 (the "License");
## you may not use this file except in compliance with the License.
## You may obtain a copy of the License at
## 
##     http:##www.apache.org/licenses/LICENSE-2.0
## 
## Unless required by applicable law or agreed to in writing, software
## distributed under the License is distributed on an "AS IS" BASIS,
## WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
## See the License for the specific language governing permissions and
## limitations under the License.

require 'rubygems'
require 'watir'
require 'test/unit'
require 'test/unit/ui/console/testrunner'

include Watir

require '../common'

# Test cases for MonoRail AjaxHelper

class ObserveFieldTestCase < Test::Unit::TestCase
	include CommonIEDriver

	def test_fieldchange()
		$ie.goto("#{$base_url}/ajax/observefield.rails")

		content = 'hello!';
		
		$ie.text_field(:id, "zip").set(content)
		
		sleep 2
		
		assert_equal("Address #{content}", $ie.div(:id, 'address').text)
	end

end

class ObserveFormTestCase < Test::Unit::TestCase
	include CommonIEDriver

	def test_fieldchange()
		$ie.goto("#{$base_url}/ajax/observeform.rails")

		name = 'hammett';
		address = 'sao paulo';
		
		$ie.text_field(:id, "name").set(name)
		$ie.text_field(:id, "addressf").set(address)
		
		sleep 2
		
		assert_equal("name #{name} address #{address}", $ie.div(:id, 'message').text)
	end

end

class PeriodicallyCallRemoteTestCase < Test::Unit::TestCase
	include CommonIEDriver

	def test_fieldchange()
		$ie.goto("#{$base_url}/ajax/periodicallycall.rails")

		$ie.text_field(:id, "value").set('1')
		
		sleep 2
		
		assert_equal("2", $ie.div(:id, 'newValue').text)

		$ie.text_field(:id, "value").set('4')
		
		sleep 2
		
		assert_equal("8", $ie.div(:id, 'newValue').text)
	end

end

class JsProxiesTestCase < Test::Unit::TestCase
	include CommonIEDriver

	def test_parameterless_remote_method()
		$ie.goto("#{$base_url}/ajax/jsproxies.rails")

		$ie.button(:id, 'invocableMethodButton').click
		
		sleep 1
		
		assert_equal("Success", $ie.div(:id, 'result').text)
	end
	
	def test_parameterful_remote_method()
		$ie.goto("#{$base_url}/ajax/jsproxies.rails")

		$ie.button(:id, 'anotherMethodButton').click
		
		sleep 1
		
		assert_equal("Success something", $ie.div(:id, 'result').text)
	end

end

class FormRemoteTagTestCase < Test::Unit::TestCase
	include CommonIEDriver

	def test_submit_form_async()
		$ie.goto("#{$base_url}/ajax/formremotetag.rails")

		$ie.text_field(:id, "name").set('hammett')
		$ie.text_field(:id, "email").set('hammett@apache.org')

		$ie.button(:id, 'sub').click
		
		sleep 1
		
		assert_equal("hammett", $ie.span(:id, 'hammett_name').text)
		assert_equal("hammett@apache.org", $ie.span(:id, 'hammett_email').text)
	end

end

class CastleTests
	def self.suite
		suite = Test::Unit::TestSuite.new
		suite << ObserveFieldTestCase.suite
		suite << ObserveFormTestCase.suite
		suite << PeriodicallyCallRemoteTestCase.suite
		suite << JsProxiesTestCase.suite
		suite << FormRemoteTagTestCase.suite
		return suite
	end
end

$base_url = "http://localhost:88"

$ie = IE.new()
#$ie.set_fast_speed
Test::Unit::UI::Console::TestRunner.run(CastleTests)
#$ie.close

