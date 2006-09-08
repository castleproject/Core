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

#
# Common operations for IE drivers
#
module CommonIEDriver

	def assert_no_exception(ie)
		fail('There were databind errors') if ie.contains_text('binding error')
		fail('Unexpected exception') if ie.contains_text('Exception')
		fail('Server exception') if ie.contains_text('Server')
	end

	def get_id(ie)
		ie.span(:id, 'newid').text
	end

	def assert_valid_remove_confirmation(ie, id)
		fail('Looks like removeConfirm.castle didnt load instance') unless ie.contains_text("Confirm removal of #{id}?")
	end

	def assert_removed(ie)
		fail('Error removing instance') unless ie.contains_text('Removed')
		fail('Unexpected exception') if ie.contains_text('Exception')
	end

end