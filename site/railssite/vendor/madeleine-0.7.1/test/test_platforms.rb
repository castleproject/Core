
class AddCommand
  def initialize(obj)
    @obj = obj
  end
  
  def execute(system)
    system[@obj.myid] = @obj
  end
end

class Foo  
  attr_accessor :myid
end


# Checks for a strange marshalling or IO bug observed in the
# native win32-port of Ruby on WinXP.
#
# Test case provided by Steve Conover.

class WierdWin32CorruptionTest < Test::Unit::TestCase
  include TestUtils

  def teardown
    (1..5).each {|i|
      delete_directory("corruption_test#{i}")
    }
  end

  def doCorruptionTest(idstr, storagenumber)
    m = SnapshotMadeleine.new("corruption_test" + storagenumber) { Hash.new() }
  
    f = Foo.new()
    f.myid = idstr

    m.execute_command(AddCommand.new(f))
    m.close()
    m = SnapshotMadeleine.new("corruption_test" + storagenumber) { Hash.new() }
  end

  def testErrorOne
    doCorruptionTest("123456789012345678901", "1")
  end
  
  def testErrorTwo
    doCorruptionTest("aaaaaaaaaaaaaaaaaaaaa", "2")
  end
  
  def testNoErrorOne
    doCorruptionTest("12345678901234567890", "3")
  end
  
  def testNoErrorTwo
    doCorruptionTest("1234567890123456789012", "4")
  end

  def testWhiteSpace
    doCorruptionTest("\n\r\t \r\n", "5")
  end
end

def add_platforms_tests(suite)
  suite << WierdWin32CorruptionTest.suite
end
