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

require '../common'

class ConditionType

	class << self

		include Test::Unit::Assertions
		include CommonIEDriver

		def create(ie, name)

			ie.goto("#{$base_url}/conditiontype/new.castle")

			ie.text_field(:id, "condtype_name").set(name)
			
			ie.button(:id, 'insertbutton').click

			assert_no_exception(ie)
			
			assert_equal(name, ie.span(:id, 'name').text, 'Insert: name was not set')

			# returns new id
			
			get_id(ie)
			
		end
		
	end

end
