#!/usr/bin/env ruby

$VERBOSE = true

$: << ".."

require 'rubyunit'
require 'zip/ziprequire'

$: << 'rubycode.zip' << 'rubycode2.zip'

class ZipRequireTest < RUNIT::TestCase
  def test_require
    assert(require('notzippedruby'))
    assert(!require('notzippedruby'))

    assert(require('zippedruby1'))
    assert(!require('zippedruby1'))

    assert(require('zippedruby2'))
    assert(!require('zippedruby2'))

    assert(require('zippedruby3'))
    assert(!require('zippedruby3'))

    c1 = NotZippedRuby.new
    assert(c1.returnTrue)
    assert(ZippedRuby1.returnTrue)
    assert(!ZippedRuby2.returnFalse)
    assert_equals(4, ZippedRuby3.multiplyValues(2, 2))
  end

  def test_get_resource
    get_resource("aResource.txt") {
      |f|
      assert_equals("Nothing exciting in this file!", f.read)
    }
  end
end

# Copyright (C) 2002 Thomas Sondergaard
# rubyzip is free software; you can redistribute it and/or
# modify it under the terms of the ruby license.
