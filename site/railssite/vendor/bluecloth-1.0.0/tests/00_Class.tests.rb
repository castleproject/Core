#!/usr/bin/ruby
#
# Unit test for the BlueCloth class object 
# $Id: 00_Class.tests.rb,v 1.1 2005/01/07 23:01:51 alexeyv Exp $
#
# Copyright (c) 2004 The FaerieMUD Consortium.
# 

if !defined?( BlueCloth ) || !defined?( BlueCloth::TestCase )
	basedir = File::dirname( __FILE__ )
	require File::join( basedir, 'bctestcase' )
end


### This test case tests ...
class BlueClothClassTestCase < BlueCloth::TestCase

	TestString = "foo"

	def test_00_class_constant
		printTestHeader "BlueCloth: Class Constant"

		assert Object::constants.include?( "BlueCloth" ),
			"No BlueCloth constant in Object"
		assert_instance_of Class, BlueCloth
	end

	def test_01_instantiation
		printTestHeader "BlueCloth: Instantiation"
		rval = nil
		
		# With no argument... ("")
		assert_nothing_raised {
			rval = BlueCloth::new
		}
		assert_instance_of BlueCloth, rval
		assert_kind_of String, rval
		assert_equal "", rval

		# String argument
		assert_nothing_raised {
			rval = BlueCloth::new TestString
		}
		assert_instance_of BlueCloth, rval
		assert_kind_of String, rval
		assert_equal TestString, rval

		addSetupBlock {
			debugMsg "Creating a new BlueCloth"
			@obj = BlueCloth::new( TestString )
		}
		addTeardownBlock {
			@obj = nil
		}
	end

	def test_02_duplication
		printTestHeader "BlueCloth: Duplication"
		rval = nil
		
		assert_nothing_raised {
			rval = @obj.dup
		}
		assert_instance_of BlueCloth, rval
		assert_kind_of String, rval
		assert_equal TestString, rval
	end


end

