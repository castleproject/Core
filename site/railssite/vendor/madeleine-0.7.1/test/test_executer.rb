
unless $LOAD_PATH.include?("lib")
  $LOAD_PATH.unshift("lib")
end
unless $LOAD_PATH.include?("test")
  $LOAD_PATH.unshift("test")
end

require 'test/unit'
require 'madeleine'

class ExecuterTest < Test::Unit::TestCase

  def test_executer
    system = Object.new
    command = self
    executer = Madeleine::Executer.new(system)
    @executed_with = nil
    executer.execute(command)
    assert_same(system, @executed_with)
  end

  # Self-shunt
  def execute(system)
    @executed_with = system
  end

  def test_execute_with_exception
    system = Object.new
    command = Object.new
    def command.execute(system)
      raise "this is an exception from a command"
    end
    executer = Madeleine::Executer.new(system)
    assert_raises(RuntimeError) {
      executer.execute(command)
    }
  end

  def test_exception_in_recovery
    system = Object.new
    command = Object.new
    def command.execute(system)
      raise "this is an exception from a command"
    end
    executer = Madeleine::Executer.new(system)
    executer.recovery {
      executer.execute(command)
    }
    assert_raises(RuntimeError) {
      executer.execute(command)
    }
  end
end
