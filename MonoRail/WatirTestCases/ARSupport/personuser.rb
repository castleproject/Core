## Copyright 2004-2006 Castle Project - http:##www.castleproject.org/
## 
## Licensed under the Apache License, Version 2.0 (the "License");
## you may not use this file except in compliance with the License.
## You may obtain a copy of the License at
## 
##	 http:##www.apache.org/licenses/LICENSE-2.0
## 
## Unless required by applicable law or agreed to in writing, software
## distributed under the License is distributed on an "AS IS" BASIS,
## WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
## See the License for the specific language governing permissions and
## limitations under the License.

require '../common'

class PersonUser

	class << self

		include Test::Unit::Assertions

		def create(ie, first, middle, last, username, password)

			ie.goto("#{$base_url}/personuser/new.castle")
			
			send_create(ie, first, middle, last, username, password)
		end

		def edit(ie, id, first, middle, last, username, password)

			ie.goto("#{$base_url}/personuser/edit.castle?id=#{id}")
			
			send_edit(ie, id, first, middle, last, username, password)
		end
		
		def delete(ie, id)

			ie.goto("#{$base_url}/personuser/removeconfirm.castle?id=#{id}")

			fail('Looks like removeConfirm.castle didnt load instance') unless ie.contains_text("Confirm removal of #{id}?")
			
			ie.button(:id, 'button').click

			fail('Error removing instance') unless ie.contains_text('Removed')
			fail('Unexpected exception') if ie.contains_text('Exception')
			
		end
		
		private
		
		def send_create(ie, first, middle, last, username, password)
		
			ie.text_field(:id, "user_first").set(first)
			ie.text_field(:id, "user_middle").set(middle)
			ie.text_field(:id, "user_last").set(last)
			ie.text_field(:id, "user_username").set(username)
			ie.text_field(:id, "user_password").set(password)
			
			ie.button(:id, 'insertbutton').click

			# Check for errors
			
			fail('There were databind errors') if ie.contains_text('binding error')
			fail('Unexpected exception') if ie.contains_text('Exception')
			
			assert_equal(first, ie.span(:id, 'first').text)
			assert_equal(middle, ie.span(:id, 'middle').text)
			assert_equal(last, ie.span(:id, 'last').text)
			assert_equal(username, ie.span(:id, 'username').text)
			assert_equal(password, ie.span(:id, 'password').text)

			# returns new created id
			
			ie.span(:id, 'newid').text
		end
		
		def send_edit(ie, id, first, middle, last, username, password)
		
			ie.text_field(:id, "user_first").set(first)
			ie.text_field(:id, "user_middle").set(middle)
			ie.text_field(:id, "user_last").set(last)
			ie.text_field(:id, "user_username").set(username)
			ie.text_field(:id, "user_password").set(password)
			
			ie.button(:id, 'updatebutton').click

			# Check for errors
			
			fail('There were databind errors') if ie.contains_text('binding error')
			fail('Unexpected exception') if ie.contains_text('Exception')
			
			assert_equal(first, ie.span(:id, 'first').text)
			assert_equal(middle, ie.span(:id, 'middle').text)
			assert_equal(last, ie.span(:id, 'last').text)
			assert_equal(username, ie.span(:id, 'username').text)
			assert_equal(password, ie.span(:id, 'password').text)
		end

	end

end
