# Batched writes for Madeleine
#
# Copyright(c) Håkan Råberg 2003
#
# 
# This is an experimental implementation of batched log writes to mininize 
# calls to fsync. It uses a Shared/Exclusive-Lock, implemented in sync.rb, 
# which is included in Ruby 1.8.
#
# Writes are batched for a specified amount of time, before written to disk and
# then executed.
#
# For a detailed discussion about the problem, see
#  http://www.prevayler.org/wiki.jsp?topic=OvercomingTheWriteBottleneck
#
#
# Usage is identical to normal SnapshotMadeleine, and it can also be used as
# persister for AutomaticSnapshotMadeleine. (One difference: the log isn't 
# visible on disk until any commands are executed.)
#
# You can also use the execute_query method for shared synchronzied queries,
# for eaay coarse-grained locking of the system.
#
# The exclusive lock is only locked during the actual execution of commands and
# while closing.
#
# Keeping both log writes and executes of commands in the originating thread 
# is needed by AutomaticSnapshotPrevayler. Hence the strange SimplisticPipe
# class.
#
# Todo: 
#    - It seems like Sync (sync.rb) prefers shared locks. This should probably
#       be changed.
#
#
# Madeleine - Ruby Object Prevalence
#
# Copyright(c) Anders Bengtsson 2003
#

require 'madeleine'
require 'madeleine/clock'

include Madeleine::Clock

module Madeleine
  module Batch
    class BatchedSnapshotMadeleine < SnapshotMadeleine

      def initialize(directory_name, marshaller=Marshal, &new_system_block)
        super(directory_name, marshaller, &new_system_block)
        @log_actor = LogActor.launch(self)
      end

      def execute_command(command)
        verify_command_sane(command)
        queued_command = QueuedCommand.new(command)
        @lock.synchronize(:SH) do
          raise "closed" if @closed
          @logger.store(queued_command)
        end
        queued_command.wait_for
      end

      def execute_query(query)
        verify_command_sane(query)
        @lock.synchronize(:SH) do
          execute_without_storing(query)
        end
      end

      def close
        @log_actor.destroy
        @lock.synchronize do
          @logger.close
          @closed = true
        end
      end

      def flush
        @lock.synchronize do
          @logger.flush
        end
      end

      def take_snapshot
        @lock.synchronize(:SH) do
          @lock.synchronize do
            @logger.close
          end
          Snapshot.new(@directory_name, system, @marshaller).take
          @logger.reset
        end
      end

      private

      def create_lock
        Sync.new
      end

      def create_logger(directory_name, log_factory)
        BatchedLogger.new(directory_name, log_factory, self.system)
      end

      def log_factory
        BatchedLogFactory.new
      end
    end

    private

    class LogActor
      def self.launch(madeleine, delay=0.01)
        result = new(madeleine, delay)
        result
      end

      def destroy
        @is_destroyed = true
        if @thread.alive?
          @thread.wakeup
          @thread.join
        end
      end

      private

      def initialize(madeleine, delay)
        @is_destroyed = false

        madeleine.flush
        @thread = Thread.new {
          until @is_destroyed
            sleep(delay)
            madeleine.flush
          end
        }
      end
    end

    class BatchedLogFactory
      def create_log(directory_name)
        BatchedLog.new(directory_name)
      end
    end

    class BatchedLogger < Logger
      def initialize(directory_name, log_factory, system)
        super(directory_name, log_factory)
        @buffer = []
        @system = system
      end

      def store(queued_command)
        @buffer << queued_command
      end

      def close
        return if @log.nil?
        flush
        @log.close
        @log = nil
      end

      def flush
        return if @buffer.empty?

        open_new_log if @log.nil?

        if @system.kind_of?(ClockedSystem)
          @buffer.unshift(QueuedTick.new)
        end

        @buffer.each do |queued_command|
          queued_command.store(@log)
        end

        @log.flush

        @buffer.each do |queued_command|
          queued_command.execute(@system)
        end

        @buffer.clear
      end
    end

    class BatchedLog < CommandLog
      def store(command)
        Marshal.dump(command, @file)
      end

      def flush
        @file.flush
        @file.fsync
      end
    end

    class QueuedCommand
      def initialize(command)
        @command = command
        @pipe = SimplisticPipe.new
      end

      def store(log)
        @pipe.write(log)
      end

      def execute(system)
        @pipe.write(system)
      end

      def wait_for
        @pipe.read do |log|
          log.store(@command)
        end

        @pipe.read do |system|          
          return @command.execute(system)
        end
      end
    end

    class QueuedTick
      def initialize
        @tick = Tick.new(Time.now)
      end

      def store(log)
        log.store(@tick)
      end

      def execute(system)
        @tick.execute(system)
      end
    end

    class SimplisticPipe
      def initialize 
        @receive_lock = Mutex.new.lock
        @consume_lock = Mutex.new.lock
        @message = nil
      end

      def read
        begin
          wait_for_message_received

          if block_given?
            yield @message
          else
            return @message
          end

        ensure
          message_consumed
        end
      end

      def write(message)
        raise WriteBlockedException unless can_write?

        @message = message
        message_received
        wait_for_message_consumed
        @message = nil
      end

      def can_write?
        @message.nil?
      end

      private
 
      def message_received
        @receive_lock.unlock
      end

      def wait_for_message_received
        @receive_lock.lock
      end

      def message_consumed
        @consume_lock.unlock
      end

      def wait_for_message_consumed
        @consume_lock.lock
      end
    end

    class WriteBlockedException < Exception
    end
  end
end

BatchedSnapshotMadeleine = Madeleine::Batch::BatchedSnapshotMadeleine
