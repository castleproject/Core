require 'madeleine'
require 'madeleine/automatic'
require 'madeleine/zmarshal'
require 'singleton'
require 'yaml'

class MadeleineService
  include Madeleine::Automatic::Interceptor

  @@storage_path = self.name.downcase + "_storage"

  class << self
    def storage_path
      @@storage_path
    end
  
    def storage_path=(storage_path)
      @@storage_path = storage_path
    end

    def instance
      @system = MadeleineServer.new(self).system if @system.nil?
      @system
    end
    
    def restart
      MadeleineServer.clean_storage(self)
      @system = MadeleineServer.new(self).system
    end
  end
end

class MadeleineServer
  SNAPSHOT_INTERVAL   = 30  * 60 * 24 # Each day
  AUTOMATIC_SNAPSHOTS = true

  # Clears all the command_log and snapshot files located in the storage directory, so the
  # database is essentially dropped and recreated as blank. Used in tests.
  def self.clean_storage(service)
    require 'fileutils'
    if (File.directory?(service.storage_path))
      FileUtils.rm_rf(Dir[service.storage_path + '/*.command_log'])
      FileUtils.rm_rf(Dir[service.storage_path + '/*.snapshot'])
    else
      FileUtils.mkdir_p(service.storage_path)
    end
  end

  def initialize(service)
    marshaller = Madeleine::ZMarshal.new()
    @server = Madeleine::Automatic::AutomaticSnapshotMadeleine.new(service.storage_path, 
        marshaller) { service.new }
    start_snapshot_thread if AUTOMATIC_SNAPSHOTS
  end

  def system
    @server.system
  end

  def start_snapshot_thread
    Thread.new(@server) {
      while true
        sleep(SNAPSHOT_INTERVAL)
        @server.take_snapshot
      end
    }
  end
end