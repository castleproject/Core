#
# Madeleine - Ruby Object Prevalence
#
# Author::    Anders Bengtsson <ndrsbngtssn@yahoo.se>
# Copyright:: Copyright (c) 2003-2004
#
# Usage:
#
#  require 'madeleine'
#
#  madeleine = SnapshotMadeleine.new("my_example_storage") {
#    SomeExampleApplication.new()
#  }
#
#  madeleine.execute_command(command)
#

module Madeleine

  require 'thread'
  require 'sync'
  require 'madeleine/files'

  MADELEINE_VERSION = "0.7.1"

  class SnapshotMadeleine

    # Builds a new Madeleine instance. If there is a snapshot available
    # then the system will be created from that, otherwise
    # <tt>new_system</tt> will be used. The state of the system will
    # then be restored from the command logs.
    #
    # You can provide your own snapshot marshaller, for instance using
    # YAML or SOAP, instead of Ruby's built-in marshaller. The
    # <tt>snapshot_marshaller</tt> must respond to
    # <tt>load(stream)</tt> and <tt>dump(object, stream)</tt>. You
    # must use the same marshaller every time for a system.
    #
    # See: DefaultSnapshotMadeleine
    #
    # * <tt>directory_name</tt> - Storage directory to use. Will be created if needed.
    # * <tt>snapshot_marshaller</tt> - Marshaller to use for system snapshots. (Optional)
    # * <tt>new_system_block</tt> - Block to create a new system (if no stored system was found).
    def self.new(directory_name, snapshot_marshaller=Marshal, &new_system_block)
      log_factory = DefaultLogFactory.new
      logger = Logger.new(directory_name,
                          log_factory)
      snapshotter = Snapshotter.new(directory_name,
                                    snapshot_marshaller)
      lock = DefaultLock.new
      recoverer = Recoverer.new(directory_name,
                                snapshot_marshaller)
      system = recoverer.recover_snapshot(new_system_block)
      executer = Executer.new(system)
      recoverer.recover_logs(executer)
      DefaultSnapshotMadeleine.new(system, logger, snapshotter, lock, executer)
    end
  end

  class DefaultSnapshotMadeleine

    # The prevalent system
    attr_reader :system

    def initialize(system, logger, snapshotter, lock, executer)
      @system = system
      @logger = logger
      @snapshotter = snapshotter
      @lock = lock
      @executer = executer

      @closed = false
    end

    # Execute a command on the prevalent system.
    #
    # Commands must have a method <tt>execute(aSystem)</tt>.
    # Otherwise an error, <tt>Madeleine::InvalidCommandException</tt>,
    # will be raised.
    #
    # The return value from the command's <tt>execute()</tt> method is returned.
    #
    # * <tt>command</tt> - The command to execute on the system.
    def execute_command(command)
      verify_command_sane(command)
      @lock.synchronize {
        raise "closed" if @closed
        @logger.store(command)
        @executer.execute(command)
      }
    end

    # Execute a query on the prevalent system.
    #
    # Only differs from <tt>execute_command</tt> in that the command/query isn't logged, and
    # therefore isn't allowed to modify the system. A shared lock is held, preventing others
    # from modifying the system while the query is running.
    #
    # * <tt>query</tt> - The query command to execute
    def execute_query(query)
      @lock.synchronize_shared {
        @executer.execute(query)
      }
    end

    # Take a snapshot of the current system.
    #
    # You need to regularly take a snapshot of a running system,
    # otherwise the logs will grow big and restarting the system will take a
    # long time. Your backups must also be done from the snapshot files,
    # since you can't make a consistent backup of a live log.
    #
    # A practical way of doing snapshots is a timer thread:
    #
    #  Thread.new(madeleine) {|madeleine|
    #    while true
    #      sleep(60 * 60 * 24) # 24 hours
    #      madeleine.take_snapshot
    #    end
    #  }
    def take_snapshot
      @lock.synchronize {
        @logger.close
        @snapshotter.take(@system)
        @logger.reset
      }
    end

    # Close the system.
    #
    # The log file is closed and no new commands can be received
    # by this Madeleine.
    def close
      @lock.synchronize {
        @logger.close
        @closed = true
      }
    end

    private

    def verify_command_sane(command)
      unless command.respond_to?(:execute)
        raise InvalidCommandException.new("Commands must have an 'execute' method")
      end
    end
  end

  class InvalidCommandException < Exception
  end

  #
  # Internal classes below
  #

  FILE_COUNTER_SIZE = 21 #:nodoc:

  class DefaultLock #:nodoc:

    def initialize
      @lock = Sync.new
    end

    def synchronize(&block)
      @lock.synchronize(&block)
    end

    def synchronize_shared(&block)
      @lock.synchronize(:SH, &block)
    end
  end

  class Executer #:nodoc:

    def initialize(system)
      @system = system
      @in_recovery = false
    end

    def execute(command)
      begin
        command.execute(@system)
      rescue
        raise unless @in_recovery
      end
    end

    def recovery
      begin
        @in_recovery = true
        yield
      ensure
        @in_recovery = false
      end
    end
  end

  class Recoverer #:nodoc:

    def initialize(directory_name, marshaller)
      @directory_name, @marshaller = directory_name, marshaller
    end

    def recover_snapshot(new_system_block)
      system = nil
      id = SnapshotFile.highest_id(@directory_name)
      if id > 0
        snapshot_file = SnapshotFile.new(@directory_name, id).name
        open(snapshot_file, "rb") {|snapshot|
          system = @marshaller.load(snapshot)
        }
      else
        system = new_system_block.call
      end
      system
    end

    def recover_logs(executer)
      executer.recovery {
        CommandLog.log_file_names(@directory_name, FileService.new).each {|file_name|
          open(@directory_name + File::SEPARATOR + file_name, "rb") {|log|
            recover_log(executer, log)
          }
        }
      }
    end

    private

    def recover_log(executer, log)
      while ! log.eof?
        command = Marshal.load(log)
        executer.execute(command)
      end
    end
  end

  class NumberedFile #:nodoc:

    def initialize(path, name, id)
      @path, @name, @id = path, name, id
    end

    def name
      result = @path
      result += File::SEPARATOR
      result += sprintf("%0#{FILE_COUNTER_SIZE}d", @id)
      result += '.'
      result += @name
    end
  end

  class CommandLog #:nodoc:

    def self.log_file_names(directory_name, file_service)
      return [] unless file_service.exist?(directory_name)
      result = file_service.dir_entries(directory_name).select {|name|
        name =~ /^\d{#{FILE_COUNTER_SIZE}}\.command_log$/
      }
      result.each {|name| name.untaint }
      result.sort!
      result
    end

    def initialize(path, file_service)
      id = self.class.highest_log(path, file_service) + 1
      numbered_file = NumberedFile.new(path, "command_log", id)
      @file = file_service.open(numbered_file.name, 'wb')
    end

    def close
      @file.close
    end

    def store(command)
      Marshal.dump(command, @file)
      @file.flush
      @file.fsync
    end

    def self.highest_log(directory_name, file_service)
      highest = 0
      log_file_names(directory_name, file_service).each {|file_name|
        match = /^(\d{#{FILE_COUNTER_SIZE}})/.match(file_name)
        n = match[1].to_i
        if n > highest
          highest = n
        end
      }
      highest
    end
  end

  class DefaultLogFactory #:nodoc:
    def create_log(directory_name)
      CommandLog.new(directory_name, FileService.new)
    end
  end

  class Logger #:nodoc:

    def initialize(directory_name, log_factory)
      @directory_name = directory_name
      @log_factory = log_factory
      @log = nil
      @pending_tick = nil
      ensure_directory_exists
    end

    def ensure_directory_exists
      if ! File.exist?(@directory_name)
        Dir.mkdir(@directory_name)
      end
    end

    def reset
      close
      delete_log_files
    end

    def store(command)
      if command.kind_of?(Madeleine::Clock::Tick)
        @pending_tick = command
      else
        if @pending_tick
          internal_store(@pending_tick)
          @pending_tick = nil
        end
        internal_store(command)
      end
    end

    def internal_store(command)
      if @log.nil?
        open_new_log
      end
      @log.store(command)
    end

    def close
      return if @log.nil?
      @log.close
      @log = nil
    end

    private

    def delete_log_files
      Dir.glob(@directory_name + File::SEPARATOR + "*.command_log").each {|name|
        name.untaint
        File.delete(name)
      }
    end

    def open_new_log
      @log = @log_factory.create_log(@directory_name)
    end
  end

  class SnapshotFile < NumberedFile #:nodoc:

    def self.highest_id(directory_name)
      return 0 unless File.exist?(directory_name)
      suffix = "snapshot"
      highest = 0
      Dir.foreach(directory_name) {|file_name|
        match = /^(\d{#{FILE_COUNTER_SIZE}}\.#{suffix}$)/.match(file_name)
        next unless match
        n = match[1].to_i
        if n > highest
          highest = n
        end
      }
      highest
    end

    def self.next(directory_name)
      new(directory_name, highest_id(directory_name) + 1)
    end

    def initialize(directory_name, id)
      super(directory_name, "snapshot", id)
    end
  end

  class Snapshotter #:nodoc:

    def initialize(directory_name, marshaller)
      @directory_name, @marshaller = directory_name, marshaller
    end

    def take(system)
      numbered_file = SnapshotFile.next(@directory_name)
      name = numbered_file.name
      open(name + '.tmp', 'wb') {|snapshot|
        @marshaller.dump(system, snapshot)
        snapshot.flush
        snapshot.fsync
      }
      File.rename(name + '.tmp', name)
    end
  end

  module Clock #:nodoc:
    class Tick #:nodoc:

      def initialize(time)
        @time = time
      end

      def execute(system)
        system.clock.forward_to(@time)
      end
    end
  end
end

SnapshotMadeleine = Madeleine::SnapshotMadeleine

