#!/usr/local/bin/ruby -w
#
# Copyright(c) 2003 Håkan Råberg
#
# This test is based on Prevaylers TransactionTestRun,
# Copyright(c) 2001-2003 Klaus Wuestefeld.
#

$LOAD_PATH.unshift("../lib")

require 'madeleine'
require 'madeleine/clock'
require 'batched'

module ScalabilityTest

  class TransactionTestRun
    MIN_THREADS = 20
    MAX_THREADS = 20
    NUMBER_OF_OBJECTS = 100000
    ROUND_DURATION = 20
    DIR = "ScalabilityBase"

    def initialize
      @system = TransactionSystem.new
      @madeleine = BatchedSnapshotMadeleine.new(DIR) { @system }
      
      @system.replace_all_records(create_records(NUMBER_OF_OBJECTS))

      @is_round_finished = false

      @best_round_ops_per_s = 0
      @best_round_threads = 0
      @operation_count = 0
      @last_operation = 0
      @active_round_threads = 0
      
      @half_of_the_objects = NUMBER_OF_OBJECTS / 2

      @connection_cache = []
      @connection_cache_lock = Mutex.new

      ObjectSpace.garbage_collect

      puts "========= Running " + name + " (" + (MAX_THREADS - MIN_THREADS + 1).to_s + " rounds). Subject: " + subject_name + "..."
      puts "Each round will take approx. " + ROUND_DURATION.to_s + " seconds to run..."
      perform_test
      puts "----------- BEST ROUND: " + result_string(@best_round_ops_per_s, @best_round_threads)

      @madeleine.close
      delete_directory(DIR)
    end
      
    def name
      "Transaction Test"
    end

    def subject_name
      "Madeleine"
    end

    def result_string(ops_per_s, threads)
      ops_per_s.to_s + " operations/second (" + threads.to_s + " threads)"
    end

    def perform_test
      for threads in MIN_THREADS..MAX_THREADS
        ops_per_s = perform_round(threads)
        if ops_per_s > @best_round_ops_per_s
          @best_round_ops_per_s = ops_per_s
          @best_round_threads = threads
        end
      end
    end

    def perform_round(threads)
      initial_operation_count = @operation_count
      start_time = Time.now.to_f

      start_threads(threads)
      sleep(ROUND_DURATION)
      stop_threads

      seconds_ellapsed = Time.now.to_f - start_time
      ops_per_second = (@operation_count - initial_operation_count) / seconds_ellapsed

      puts
      puts "Seconds ellapsed: " + seconds_ellapsed.to_s
      puts "--------- Round Result: " + result_string(ops_per_second, threads)

      ops_per_second
    end

    def start_threads(threads)
      @is_round_finished = false
      for i in 1..threads
        start_thread(@last_operation + i, threads)
      end
    end

    def start_thread(starting_operation, operation_increment)
      Thread.new {
        connection = accquire_connection

        operation = starting_operation
        while not @is_round_finished
          # puts "Operation " + operation.to_s
          execute_operation(connection, operation)
          operation += operation_increment
        end

        @connection_cache_lock.synchronize do
          @connection_cache << connection
          @operation_count += (operation - starting_operation) / operation_increment
          @last_operation = operation if @last_operation < operation
          @active_round_threads -= 1
        end
      }      
      @active_round_threads += 1
    end

    def execute_operation(connection, operation)
      record_to_insert = Record.new(NUMBER_OF_OBJECTS + operation)
      id_to_delete = spread_id(operation)
      record_to_update = Record.new(@half_of_the_objects + id_to_delete)
      
      connection.perform_transaction(record_to_insert, record_to_update, id_to_delete)
    end

    def spread_id(id) 
      (id / @half_of_the_objects) * @half_of_the_objects + ((id * 16807) % @half_of_the_objects)
    end

    def create_test_connection
      TransactionConnection.new(@madeleine)
    end

    def accquire_connection
      @connection_cache_lock.synchronize do
        return @connection_cache.empty? ? create_test_connection : @connection_cache.shift
      end
    end

    def stop_threads
      @is_round_finished = true
      while @active_round_threads != 0
        sleep(0.001)
      end
    end

    def create_records(number_of_objects)
      result = []
      for i in 0..number_of_objects
        result << Record.new(i)
      end
      result
    end

  
    def delete_directory(directory_name)
      Dir.foreach(directory_name) do |file|
        next if file == "."
        next if file == ".."
        File.delete(directory_name + File::SEPARATOR + file)
      end
      Dir.delete(directory_name)
    end
  end

  class TransactionSystem
    include Madeleine::Clock::ClockedSystem

    def initialize
      @records_by_id = Hash.new
      @transaction_lock = Mutex.new
    end

    def perform_transaction(record_to_insert, record_to_update, id_to_delete)
      @transaction_lock.synchronize do
        put(record_to_insert)
        put(record_to_update)
        @records_by_id.delete(id_to_delete)
      end
    end

    def put(new_record)
      @records_by_id[new_record.id] = new_record
    end

    def replace_all_records(new_records)
      @records_by_id.clear
      new_records.each do |record|
        put(record)
      end
    end
  end

  class TransactionConnection
    def initialize(madeleine)
      @madeleine = madeleine
    end

    def perform_transaction(record_to_insert, record_to_update, id_to_delete)
      @madeleine.execute_command(TestTransaction.new(record_to_insert, record_to_update, id_to_delete))
    end
  end

  class TestTransaction
    def initialize(record_to_insert, record_to_update, id_to_delete)
      @record_to_insert = record_to_insert
      @record_to_update = record_to_update
      @id_to_delete = id_to_delete
    end
    
    def execute(system)
      system.perform_transaction(@record_to_insert, @record_to_update, @id_to_delete)
    end
  end

  class Record
    attr_reader :id, :name, :string_1, :date_1, :date_2
    
    def initialize(id)
      @id = id
      @name = "NAME" + (id % 10000).to_s
      @string_1 = (id % 10000).to_s == 0 ? Record.large_string + id : nil;
      @date_1 = Record.random_date
      @date_2 = Record.random_date
    end

    def self.large_string
      [].fill("A", 1..980).to_s
    end

    def self.random_date
      rand(10000000)
    end
  end
end

if __FILE__ == $0
  puts "Madeleine Scalability Test"
  puts "Based on Prevaylers Scalability Test"
  puts
  
  Thread.abort_on_exception = true
  ScalabilityTest::TransactionTestRun.new
end
