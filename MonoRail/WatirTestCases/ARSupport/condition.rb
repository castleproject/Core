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
## See the License for the specific language governing comments and
## limitations under the License.

require '../common'

class Condition

	class << self

		include Test::Unit::Assertions

		def create(ie, name, type_id, *comments)

			ie.goto("#{$base_url}/condition/new.castle")
			
			ie.text_field(:id, "condition_name").set(name)
			ie.select_list(:id, "condition_ConditionType_id").select_value(type_id.to_s)
			
			comments.each { |value|
				ie.checkbox(:id, 'condition_comments', value.to_s).set
			}
			
			ie.button(:id, 'insertbutton').click

			# Check for errors
			
			fail('There were databind errors') if ie.contains_text('binding error')
			fail('Unexpected exception') if ie.contains_text('Exception')
			
			assert_equal(name, ie.span(:id, 'name').text)
			assert_equal(type_id, ie.span(:id, 'type').text) unless type_id == "0"
			assert_equal(comments.sort!, ie.span(:id, 'comments').text.split(',').sort!)

			# returns new created id
			
			ie.span(:id, 'newid').text
		end

		def edit(ie, id, name, type_id, *comments)

			ie.goto("#{$base_url}/condition/edit.castle?id=#{id}")
			
			ie.text_field(:id, "condition_name").set(name)
			ie.select_list(:id, "condition_ConditionType_id").select_value(type_id.to_s)
			
			# ie.checkboxes.each { |check| check.clear() }
			
			# comments.each { |value|
			#	ie.checkbox(:id, 'condition_comments', value.to_s).set
			# }
			
			ie.button(:id, 'updatebutton').click

			# Check for errors
			
			fail('There were databind errors') if ie.contains_text('binding error')
			fail('Unexpected exception') if ie.contains_text('Exception')
			
			assert_equal(name, ie.span(:id, 'name').text)
			assert_equal(type_id, ie.span(:id, 'type').text) unless type_id == "0"
			assert_equal(comments.sort!, ie.span(:id, 'comments').text.split(',').sort!)
		end
		
	end

end
