#!/usr/bin/env ruby

$VERBOSE = true

$: << ".."

require 'rubyunit'
require 'zip/ioextras'

include IOExtras

class FakeIOTest < RUNIT::TestCase
  class FakeIOUsingClass
    include FakeIO
  end

  def test_kind_of?
    obj = FakeIOUsingClass.new
    
    assert(obj.kind_of?(Object))
    assert(obj.kind_of?(FakeIOUsingClass))
    assert(obj.kind_of?(IO))
    assert(!obj.kind_of?(Fixnum))
    assert(!obj.kind_of?(String))
  end
end

class AbstractInputStreamTest < RUNIT::TestCase
  # AbstractInputStream subclass that provides a read method
  
  TEST_LINES = [ "Hello world#{$/}", 
    "this is the second line#{$/}", 
    "this is the last line"]
  TEST_STRING = TEST_LINES.join
  class TestAbstractInputStream 
    include AbstractInputStream
    def initialize(aString)
      super()
      @contents = aString
      @readPointer = 0
    end

    def read(charsToRead)
      retVal=@contents[@readPointer, charsToRead]
      @readPointer+=charsToRead
      return retVal
    end

    def produce_input
      read(100)
    end

    def input_finished?
      @contents[@readPointer] == nil
    end
  end

  def setup
    @io = TestAbstractInputStream.new(TEST_STRING)
  end
  
  def test_gets
    assert_equals(TEST_LINES[0], @io.gets)
    assert_equals(1, @io.lineno)
    assert_equals(TEST_LINES[1], @io.gets)
    assert_equals(2, @io.lineno)
    assert_equals(TEST_LINES[2], @io.gets)
    assert_equals(3, @io.lineno)
    assert_equals(nil, @io.gets)
    assert_equals(4, @io.lineno)
  end

  def test_getsMultiCharSeperator
    assert_equals("Hell", @io.gets("ll"))
    assert_equals("o world#{$/}this is the second l", @io.gets("d l"))
  end

  def test_each_line
    lineNumber=0
    @io.each_line {
      |line|
      assert_equals(TEST_LINES[lineNumber], line)
      lineNumber+=1
    }
  end

  def test_readlines
    assert_equals(TEST_LINES, @io.readlines)
  end

  def test_readline
    test_gets
    begin
      @io.readline
      fail "EOFError expected"
      rescue EOFError
    end
  end
end

class AbstractOutputStreamTest < RUNIT::TestCase
  class TestOutputStream
    include AbstractOutputStream

    attr_accessor :buffer

    def initialize
      @buffer = ""
    end

    def << (data)
      @buffer << data
      self
    end
  end

  def setup
    @outputStream = TestOutputStream.new

    @origCommaSep = $,
    @origOutputSep = $\
  end

  def teardown
    $, = @origCommaSep
    $\ = @origOutputSep
  end

  def test_write
    count = @outputStream.write("a little string")
    assert_equals("a little string", @outputStream.buffer)
    assert_equals("a little string".length, count)

    count = @outputStream.write(". a little more")
    assert_equals("a little string. a little more", @outputStream.buffer)
    assert_equals(". a little more".length, count)
  end
  
  def test_print
    $\ = nil # record separator set to nil
    @outputStream.print("hello")
    assert_equals("hello", @outputStream.buffer)

    @outputStream.print(" world.")
    assert_equals("hello world.", @outputStream.buffer)
    
    @outputStream.print(" You ok ",  "out ", "there?")
    assert_equals("hello world. You ok out there?", @outputStream.buffer)

    $\ = "\n"
    @outputStream.print
    assert_equals("hello world. You ok out there?\n", @outputStream.buffer)

    @outputStream.print("I sure hope so!")
    assert_equals("hello world. You ok out there?\nI sure hope so!\n", @outputStream.buffer)

    $, = "X"
    @outputStream.buffer = ""
    @outputStream.print("monkey", "duck", "zebra")
    assert_equals("monkeyXduckXzebra\n", @outputStream.buffer)

    $\ = nil
    @outputStream.buffer = ""
    @outputStream.print(20)
    assert_equals("20", @outputStream.buffer)
  end
  
  def test_printf
    @outputStream.printf("%d %04x", 123, 123) 
    assert_equals("123 007b", @outputStream.buffer)
  end
  
  def test_putc
    @outputStream.putc("A")
    assert_equals("A", @outputStream.buffer)
    @outputStream.putc(65)
    assert_equals("AA", @outputStream.buffer)
  end

  def test_puts
    @outputStream.puts
    assert_equals("\n", @outputStream.buffer)

    @outputStream.puts("hello", "world")
    assert_equals("\nhello\nworld\n", @outputStream.buffer)

    @outputStream.buffer = ""
    @outputStream.puts("hello\n", "world\n")
    assert_equals("hello\nworld\n", @outputStream.buffer)
    
    @outputStream.buffer = ""
    @outputStream.puts(["hello\n", "world\n"])
    assert_equals("hello\nworld\n", @outputStream.buffer)

    @outputStream.buffer = ""
    @outputStream.puts(["hello\n", "world\n"], "bingo")
    assert_equals("hello\nworld\nbingo\n", @outputStream.buffer)

    @outputStream.buffer = ""
    @outputStream.puts(16, 20, 50, "hello")
    assert_equals("16\n20\n50\nhello\n", @outputStream.buffer)
  end
end


# Copyright (C) 2002-2004 Thomas Sondergaard
# rubyzip is free software; you can redistribute it and/or
# modify it under the terms of the ruby license.
