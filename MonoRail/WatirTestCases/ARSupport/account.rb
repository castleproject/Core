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

class Account

	class << self

		include Test::Unit::Assertions

		def create(ie, name, email, password, confirmation, prod_lic_id, *permissions)

			ie.goto("#{$base_url}/account/new.castle")
			
			send_create(ie, name, email, password, confirmation, prod_lic_id, permissions)
		end

		def create2(ie, name, email, password, confirmation, prod_lic_id, *permissions)

			ie.goto("#{$base_url}/account/new2.castle")

			send_create(ie, name, email, password, confirmation, prod_lic_id, permissions)
		end
		
		def edit(ie, id, name, email, password, confirmation, prod_lic_id, *permissions)

			ie.goto("#{$base_url}/account/edit.castle?id=#{id}")
			
			send_edit(ie, id, name, email, password, confirmation, prod_lic_id, permissions)
		end
		
		def delete(ie, id)

			ie.goto("#{$base_url}/account/removeconfirm.castle?id=#{id}")

			fail('Looks like removeConfirm.castle didnt load instance') unless ie.contains_text("Confirm removal of #{id}?")
			
			ie.button(:id, 'button').click

			fail('Error removing instance') unless ie.contains_text('Removed')
			fail('Unexpected exception') if ie.contains_text('Exception')
			
		end
		
		private
		
		def send_create(ie, name, email, password, confirmation, prod_lic_id, permissions)
		
			ie.text_field(:id, "account_name").set(name)
			ie.text_field(:id, "account_email").set(email)
			ie.text_field(:id, "account_password").set(password)
			ie.text_field(:id, "account_confirmationpassword").set(confirmation)
			ie.select_list(:id, "account_ProductLicense_id").select_value(prod_lic_id.to_s)
			
			permissions.each { |value|
				ie.checkbox(:id, 'account_permissions', value.to_s).set
			}
			
			ie.button(:id, 'insertbutton').click

			# Check for errors
			
			fail('There were databind errors') if ie.contains_text('binding error')
			fail('Unexpected exception') if ie.contains_text('Exception')
			
			assert_equal(name, ie.span(:id, 'name').text)
			assert_equal(email, ie.span(:id, 'email').text)
			assert_equal(password, ie.span(:id, 'password').text)
			assert_equal(prod_lic_id, ie.span(:id, 'pl').text) unless prod_lic_id == "0"
			assert_equal(permissions.sort!, ie.span(:id, 'permissions').text.split(',').sort!)

			# returns new created id
			
			ie.span(:id, 'newid').text
		end
		
		def send_edit(ie, id, name, email, password, confirmation, prod_lic_id, permissions)
		
			ie.text_field(:id, "account_name").set(name)
			ie.text_field(:id, "account_email").set(email)
			ie.text_field(:id, "account_password").set(password)
			ie.text_field(:id, "account_confirmationpassword").set(confirmation)
			ie.select_list(:id, "account_ProductLicense_id").select_value(prod_lic_id.to_s)
			
			ie.checkboxes.each { |check| check.clear() }
			
			permissions.each { |value|
				ie.checkbox(:id, 'account_permissions', value.to_s).set
			}
			
			ie.button(:id, 'updatebutton').click

			# Check for errors
			
			fail('There were databind errors') if ie.contains_text('binding error')
			fail('Unexpected exception') if ie.contains_text('Exception')
			
			assert_equal(name, ie.span(:id, 'name').text)
			assert_equal(email, ie.span(:id, 'email').text)
			assert_equal(password, ie.span(:id, 'password').text)
			assert_equal(prod_lic_id, ie.span(:id, 'pl').text) unless prod_lic_id == "0"
			assert_equal(permissions.sort!, ie.span(:id, 'permissions').text.split(',').sort!)
		end

	end

end
