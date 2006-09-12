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

require 'accountpermission'
require 'productlicense'
require 'account'
require 'category'

# Test cases for MonoRail ActiveRecord support

## -- Account Permission --
##
## Creates three Account Permissions
##
class AccountPermissionTestCase < Test::Unit::TestCase

	def test_crud()
		id = AccountPermission.create($ie, 'account permission 1')
		
		AccountPermission.edit($ie, id, 'new name')
		
		AccountPermission.delete($ie, id)
	end

end

class ProductLicenseTestCase < Test::Unit::TestCase

	def test_crud()
		id = ProductLicense.create($ie, '1/2/2006', '2/2/2007')
		
		ProductLicense.edit($ie, id, '1/3/2006', '2/4/2007')
		
		ProductLicense.delete($ie, id)
	end
	
end

class AccountTestCase < Test::Unit::TestCase

	def test_crud()
		ap1 = AccountPermission.create($ie, 'account permission 1')
		ap2 = AccountPermission.create($ie, 'account permission 2')
		pl1 = ProductLicense.create($ie, '1/2/2006', '2/2/2007')
		
		# Uses the standard approach to send multiple values (indexed node)
		id = Account.create($ie, 'account name', 'hammett@gmail.com', '123', '123', pl1, ap1, ap2)
		
		Account.edit($ie, id, 'new account name', 'hammett@apache.org', 'xpto', 'xpto', pl1, ap2)
		
		Account.delete($ie, id)
		
		# Uses a different approach to send multiple values (leafnode with array)
		id = Account.create2($ie, 'account name', 'hammett@gmail.com', '123', '123', pl1, ap1, ap2)
		
		Account.delete($ie, id)
	end
	
end

class CategoryTestCase < Test::Unit::TestCase

	def test_crud()
		id = Category.create($ie, 'name')
		
		Category.edit($ie, id, 'new name')
		
		Category.delete($ie, id)
	end
	
end

class ProductLicenseOneToManyTestCase < Test::Unit::TestCase

	def test_crud()

		# Uses the standard approach to send multiple values (indexed node)
		id1 = Account.create($ie, 'account name', 'hammett@gmail.com', '123', '123', '0')
		id2 = Account.create($ie, 'account name', 'hammett@gmail.com', '123', '123', '0')

		pl1 = ProductLicense.create_with_accounts($ie, '1/2/2006', '2/2/2007', id1, id2)
		
		Account.delete($ie, id1)
		Account.delete($ie, id2)
		
		ProductLicense.delete($ie, pl1)
		
	end
	
end


class CastleTests
	def self.suite
		suite = Test::Unit::TestSuite.new
		suite << AccountPermissionTestCase.suite
		suite << ProductLicenseTestCase.suite
		suite << AccountTestCase.suite
		suite << CategoryTestCase.suite
		suite << ProductLicenseOneToManyTestCase.suite
		return suite
	end
end

$base_url = "http://localhost:89"

$ie = IE.new()
$ie.set_fast_speed
Test::Unit::UI::Console::TestRunner.run(CastleTests)
$ie.close

