#!/usr/local/bin/ruby -w
#
# Copyright(c) 2003 Anders Bengtsson
#
# PersistenceTest is based on the unit tests from Prevayler,
# Copyright(c) 2001-2003 Klaus Wuestefeld.
#

$LOAD_PATH.unshift("lib")

require 'madeleine'
require 'test/unit'

class AddingSystem
  attr_reader :total

  def initialize
    @total = 0
  end

  def add(value)
    @total += value
    @total
  end
end


class Addition

  attr_reader :value

  def initialize(value)
    @value = value
  end

  def execute(system)
    system.add(@value)
  end
end


class PersistenceTest < Test::Unit::TestCase

  def setup
    @madeleine = nil
  end

  def teardown
    delete_prevalence_files(prevalence_base)
    Dir.delete(prevalence_base)
  end

  def verify(expected_total)
    assert_equal(expected_total, prevalence_system().total(), "Total")
  end

  def prevalence_system
    @madeleine.system
  end

  def prevalence_base
    "PrevalenceBase"
  end

  def clear_prevalence_base
    @madeleine.close unless @madeleine.nil?
    delete_prevalence_files(prevalence_base())
  end

  def delete_prevalence_files(directory_name)
    return unless File.exist?(directory_name)
    Dir.foreach(directory_name) {|file_name|
      next if file_name == '.'
      next if file_name == '..'
      file_name.untaint
      assert(File.delete(directory_name + File::SEPARATOR + file_name) == 1,
                  "Unable to delete #{file_name}")
    }
  end

  def crash_recover
    @madeleine.close unless @madeleine.nil? 
    @madeleine = create_madeleine()
  end

  def create_madeleine
   SnapshotMadeleine.new(prevalence_base()) { AddingSystem.new }
  end

  def snapshot
    @madeleine.take_snapshot
  end

  def add(value, expected_total)
    total = @madeleine.execute_command(Addition.new(value))
    assert_equal(expected_total, total, "Total")
  end

  def verify_snapshots(expected_count)
    count = 0
    Dir.foreach(prevalence_base) {|name|
      if name =~ /\.snapshot$/
        count += 1
      end
    }
    assert_equal(expected_count, count, "snapshots")
  end

  def test_main
    clear_prevalence_base

    # There is nothing to recover at first.
    # A new system will be created.
    crash_recover

    crash_recover
    add(40,40)
    add(30,70)
    verify(70)

    crash_recover
    verify(70)

    add(20,90)
    add(15,105)
    verify_snapshots(0)
    snapshot
    verify_snapshots(1)
    snapshot
    verify_snapshots(2)
    verify(105)

    crash_recover
    snapshot
    add(10,115)
    snapshot
    add(5,120)
    add(4,124)
    verify(124)

    crash_recover
    add(3,127)
    verify(127)

    verify_snapshots(4)

    clear_prevalence_base
    snapshot

    crash_recover
    add(10,137)
    add(2,139)
    crash_recover
    verify(139)
  end

  def test_main_in_safe_level_one
    thread = Thread.new {
      $SAFE = 1
      test_main
    }
    thread.join
  end
end


def add_persistence_tests(suite)
  suite << PersistenceTest.suite
end
