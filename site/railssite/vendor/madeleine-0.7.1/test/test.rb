#!/usr/bin/env ruby
#

$LOAD_PATH.unshift("lib")
$LOAD_PATH.unshift("test")

require 'madeleine'
require 'test/unit'


class Append
  def initialize(value)
    @value = value
  end

  def execute(system)
    system << @value
  end
end


module TestUtils
  def delete_directory(directory_name)
    return unless File.exists?(directory_name)
    Dir.foreach(directory_name) do |file|
      next if file == "."
      next if file == ".."
      assert(File.delete(directory_name + File::SEPARATOR + file) == 1,
             "Unable to delete #{file}")
    end
    Dir.delete(directory_name)
  end
end


class SnapshotMadeleineTest < Test::Unit::TestCase
  include TestUtils

  def teardown
    delete_directory(persistence_base)
  end

  def persistence_base
    "closing-test"
  end

  def test_closing
    madeleine = SnapshotMadeleine.new(persistence_base) { "hello" }
    madeleine.close
    assert_raises(RuntimeError) do
      madeleine.execute_command(Append.new("world"))
    end
  end
end

class NumberedFileTest < Test::Unit::TestCase

  def test_main
    target = Madeleine::NumberedFile.new(File::SEPARATOR + "foo", "bar", 321)
    assert_equal(File::SEPARATOR + "foo" + File::SEPARATOR +
                 "000000000000000000321.bar",
                 target.name)
  end
end


class LoggerTest < Test::Unit::TestCase
  include TestUtils

  def teardown
    delete_directory("whoah")
  end

  def test_creation
    @log = Object.new
    def @log.store(command)
      unless defined? @commands
        @commands = []
      end
      @commands << command
    end
    def @log.commands
      @commands
    end

    log_factory = self
    target = Madeleine::Logger.new("whoah", log_factory)
    target.store(:foo)
    assert(@log.commands.include?(:foo))
  end

  # Self-shunt
  def create_log(directory_name)
    @log
  end
end

class CommandVerificationTest < Test::Unit::TestCase

  def teardown
    Dir.delete("foo")
  end

  def test_broken_command
    target = SnapshotMadeleine.new("foo") { :a_system }
    assert_raises(Madeleine::InvalidCommandException) do
      target.execute_command(:not_a_command)
    end
  end
end


class CustomMarshallerTest < Test::Unit::TestCase
  include TestUtils

  def teardown
    delete_directory(prevalence_base)
  end

  def prevalence_base
    "custom-marshaller-test"
  end

  def madeleine_class
    SnapshotMadeleine
  end

  def test_changing_marshaller
    @log = ""
    marshaller = self
    target = madeleine_class.new(prevalence_base, marshaller) { "hello world" }
    target.take_snapshot
    assert_equal("dump ", @log)
    target = nil

    madeleine_class.new(prevalence_base, marshaller) { flunk() }
    assert_equal("dump load ", @log)
  end

  def load(io)
    @log << "load "
    assert_equal("dump data", io.read())
  end

  def dump(system, io)
    @log << "dump "
    assert_equal("hello world", system)
    io.write("dump data")
  end
end


class ErrorRaisingCommand
  def execute(system)
    raise "this is an exception from a command"
  end
end

class ErrorHandlingTest < Test::Unit::TestCase
  include TestUtils

  def teardown
    delete_directory(prevalence_base)
  end

  def prevalence_base
    "error-handling-base"
  end

  def test_exception_in_command
    madeleine = SnapshotMadeleine.new(prevalence_base) { "hello" }
    assert_raises(RuntimeError) do
      madeleine.execute_command(ErrorRaisingCommand.new)
    end
    madeleine.close
    madeleine = SnapshotMadeleine.new(prevalence_base) { "hello" }
    madeleine.close
  end
end

class QueryTest < Test::Unit::TestCase
  include TestUtils

  def teardown
    delete_directory(prevalence_base)
  end

  def prevalence_base
    "query-base"
  end

  def test_querying
    madeleine = SnapshotMadeleine.new(prevalence_base) { "hello" }
    query = Object.new
    def query.execute(system)
      system.size
    end
    # 'query' is an un-marshallable singleton, so we implicitly test
    # that querys aren't stored.
    assert_equal(5, madeleine.execute_query(query))
    # TODO: assert that no logging was done
    # TODO: assert that lock was held
  end
end


class TimeOptimizingLoggerTest < Test::Unit::TestCase
  include TestUtils

  def setup
    @target = Madeleine::Logger.new("some_directory", self)
    @log = []
    def @log.store(command)
      self << command
    end
  end

  def teardown
    delete_directory("some_directory")
  end

  def test_optimizing_ticks
    assert_equal(0, @log.size)
    @target.store(Madeleine::Clock::Tick.new(Time.at(3)))
    assert_equal(0, @log.size)
    @target.store(Madeleine::Clock::Tick.new(Time.at(22)))
    assert_equal(0, @log.size)
    @target.store(Addition.new(100))
    assert_kind_of(Madeleine::Clock::Tick, @log[0])
    assert_equal(22, value_of_tick(@log[0]))
    assert_equal(100, @log[1].value)
    assert_equal(2, @log.size)
  end

  def value_of_tick(tick)
    @clock = Object.new
    def @clock.forward_to(time)
      @value = time.to_i
    end
    def @clock.value
      @value
    end
    tick.execute(self)
    @clock.value
  end

  # Self-shunt
  def create_log(directory_name)
    assert_equal("some_directory", directory_name)
    @log
  end

  # Self-shunt
  def clock
    @clock
  end
end


class SharedLockQueryTest < Test::Unit::TestCase
  include TestUtils

  def prevalence_base
    "shared_lock_test"
  end

  def teardown
    delete_directory(prevalence_base)
  end

  def test_query
    madeleine = SnapshotMadeleine.new(prevalence_base) { "hello" }
    lock = Object.new
    madeleine.instance_eval { @lock = lock } # FIXME: The horror, the horror

    $shared = false
    $was_shared = false
    def lock.synchronize_shared(&block)
      $shared = true
      block.call
      $shared = false
    end
    query = Object.new
    def query.execute(system)
      $was_shared = $shared
    end
    madeleine.execute_query(query)
    assert($was_shared)
  end
end

suite = Test::Unit::TestSuite.new("Madeleine")

suite << SnapshotMadeleineTest.suite
suite << NumberedFileTest.suite
require 'test_command_log'
suite << CommandLogTest.suite
suite << LoggerTest.suite
suite << CommandVerificationTest.suite
suite << CustomMarshallerTest.suite
suite << ErrorHandlingTest.suite
suite << QueryTest.suite
suite << TimeOptimizingLoggerTest.suite
suite << SharedLockQueryTest.suite
require 'test_executer'
suite << ExecuterTest.suite

require 'test_clocked'
add_clocked_tests(suite)
require 'test_automatic'
add_automatic_tests(suite)
require 'test_persistence'
add_persistence_tests(suite)
require 'test_platforms'
add_platforms_tests(suite)
require 'test_zmarshal'
add_zmarshal_tests(suite)

require 'test/unit/ui/console/testrunner'
Test::Unit::UI::Console::TestRunner.run(suite)
