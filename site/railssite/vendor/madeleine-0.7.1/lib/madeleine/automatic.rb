require 'yaml'
require 'madeleine/zmarshal'
require 'soap/marshal'

module Madeleine

# Automatic commands for Madeleine
#
# Author::    Stephen Sykes <sds@stephensykes.com>
# Copyright:: Copyright (C) 2003-2004
# Version::   0.41
#
# This module provides a way of automatically generating command objects for madeleine to
# store.  It works by making a proxy object for all objects of any classes in which it is included.
# Method calls to these objects are intercepted, and stored as a command before being
# passed on to the real receiver.  The command objects remember which object the command was
# destined for by using a pair of internal ids that are contained in each of the proxy objects.
#
# There is also a mechanism for specifying which methods not to intercept calls to by using
# automatic_read_only, and its opposite automatic_read_write.
#
# Should you require it, the snapshots can be stored as yaml, and can be compressed.  Just pass 
# the marshaller you want to use as the second argument to AutomaticSnapshotMadeleine.new.  
# If the passed marshaller did not successfully deserialize the latest snapshot, the system 
# will try to automatically detect and read either Marshal, YAML, SOAP, or their corresponding
# compressed versions.
#
# This module is designed to work correctly in the case there are multiple madeleine systems in use by
# a single program, and is also safe to use with threads.
#
# Usage:
#
#  require 'madeleine'
#  require 'madeleine/automatic'
#
#  class A
#    include Madeleine::Automatic::Interceptor
#    attr_reader :foo
#    automatic_read_only :foo
#    def initialize(param1, ...)
#      ...
#    end
#    def some_method(paramA, ...)
#      ...
#    end
#    automatic_read_only
#    def bigfoo
#      foo.upcase
#    end
#  end
#
#  mad = AutomaticSnapshotMadeleine.new("storage_directory") { A.new(param1, ...) }
#
#  mad.system.some_method(paramA, ...) # logged as a command by madeleine
#  print mad.foo                       # not logged
#  print mad.bigfoo                    # not logged
#  mad.take_snapshot
#

  module Automatic
#
# This module should be included (at the top) in any classes that are to be persisted.
# It will intercept method calls and make sure they are converted into commands that are logged by Madeleine.
# It does this by returning a Prox object that is a proxy for the real object.
#
# It also handles automatic_read_only and automatic_read_write, allowing user specification of which methods
# should be made into commands
#
    module Interceptor
#
# When included, redefine new so that we can return a Prox object instead, and define methods to handle
# keeping track of which methods are read only
#
      def self.included(klass)
        class <<klass
          alias_method :_old_new, :new

          def new(*args, &block)
            Prox.new(_old_new(*args, &block))
          end
#
# Called when a method added - remember symbol if read only 
# This is a good place to add in any superclass's read only methods also
#
          def method_added(symbol)
            self.instance_eval {
              @read_only_methods ||= []
              @auto_read_only_flag ||= false
              @read_only_methods << symbol if @auto_read_only_flag
              c = self
              while (c = c.superclass)
                if (c.instance_eval {instance_variables.include? "@read_only_methods"})
                  @read_only_methods |= c.instance_eval {@read_only_methods}
                end
              end
            }
          end
#
# Set the read only flag, or add read only methods
#
          def automatic_read_only(*list)
            if (list == [])
              self.instance_eval {@auto_read_only_flag = true}
            else
              list.each {|s| self.instance_eval {@read_only_methods ||= []; @read_only_methods << s}}
            end
          end
#
# Clear the read only flag, or remove read only methods
#
          def automatic_read_write(*list)
            if (list == [])
              self.instance_eval {@auto_read_only_flag = false}
            else
              list.each {|s| self.instance_eval {@read_only_methods ||= []; @read_only_methods.delete(s)}}
            end
          end

        end
      end
#
# Return the list of read only methods so Automatic_proxy#method_missing can find what to and what not to make into a command
#
      def read_only_methods
        self.class.instance_eval {@read_only_methods}
      end
    end

#
# A Command object is automatically created for each method call to an object within the system that comes from without.
# These objects are recorded in the log by Madeleine.
# 
    class Command
      def initialize(symbol, myid, *args)
        @symbol = symbol
        @myid = myid
        @args = args
      end
#
# Called by madeleine when the command is done either first time, or when restoring the log
#
      def execute(system)
        Thread.current[:system].myid2ref(@myid).thing.send(@symbol, *@args)
      end
    end
#
# This is a little class to pass to SnapshotMadeleine.  This is used for snapshots only. 
# It acts as the marshaller, and just passes marshalling requests on to the user specified
# marshaller.  This defaults to Marshal, but could be YAML or another.
# After we have done a restore, the ObjectSpace is searched for instances of Prox to
# add new objects to the list in AutomaticSnapshotMadeleine
#
    class Automatic_marshaller #:nodoc:
      def Automatic_marshaller.load(io)
        restored_obj = Deserialize.load(io, Thread.current[:system].marshaller)
        ObjectSpace.each_object(Prox) {|o| Thread.current[:system].restore(o) if (o.sysid == restored_obj.sysid)}
        restored_obj
      end
      def Automatic_marshaller.dump(obj, io = nil)
        Thread.current[:system].marshaller.dump(obj, io)
      end
    end
#
# A Prox object is generated and returned by Interceptor each time a system object is created.
#
    class Prox #:nodoc:
      attr_accessor :thing, :myid, :sysid
      
      def initialize(thing)
        if (thing)
          raise "App object created outside of app" unless Thread.current[:system]
          @sysid = Thread.current[:system].sysid
          @myid = Thread.current[:system].add(self)
          @thing = thing
        end
      end
#
# This automatically makes and executes a new Command if a method is called from 
# outside the system.
#
      def method_missing(symbol, *args, &block)
#      print "Sending #{symbol} to #{@thing.to_s}, myid=#{@myid}, sysid=#{@sysid}\n"
        raise NoMethodError, "Undefined method" unless @thing.respond_to?(symbol)
        if (Thread.current[:system])
          @thing.send(symbol, *args, &block)
        else
          raise "Cannot make command with block" if block_given?
          Thread.current[:system] = AutomaticSnapshotMadeleine.systems[@sysid]
          begin
            if (@thing.read_only_methods.include?(symbol))
              result = Thread.current[:system].execute_query(Command.new(symbol, @myid, *args))
            else
              result = Thread.current[:system].execute_command(Command.new(symbol, @myid, *args))
            end
          ensure
            Thread.current[:system] = false
          end
          result
        end
      end
#
# Custom marshalling - this adds the internal id (myid) and the system id to a marshal
# of the object we are the proxy for.
# We take care to not marshal the same object twice, so circular references will work.
# We ignore Thread.current[:system].marshaller here - this is only called by Marshal, and
# marshal is always used for Command objects
#
      def _dump(depth)
        if (Thread.current[:snapshot_memory])
          if (Thread.current[:snapshot_memory][self])
            [@myid.to_s, @sysid].pack("A8A30")
          else
            Thread.current[:snapshot_memory][self] = true
            [@myid.to_s, @sysid].pack("A8A30") + Marshal.dump(@thing, depth)
          end
        else
          [@myid.to_s, @sysid].pack("A8A30")  # never marshal a prox object in a command, just ref
        end
      end

#
# Custom marshalling for Marshal - restore a Prox object.
#
      def Prox._load(str)
        x = Prox.new(nil)
        a = str.unpack("A8A30a*")
        x.myid = a[0].to_i
        x.sysid = a[1]
        x = Thread.current[:system].restore(x)
        x.thing = Marshal.load(a[2]) if (a[2] > "")
        x
      end

    end

#
# The AutomaticSnapshotMadeleine class contains an instance of the persister
# (default is SnapshotMadeleine) and provides additional automatic functionality.
#
# The class is instantiated the same way as SnapshotMadeleine:
# madeleine_sys = AutomaticSnapshotMadeleine.new("storage_directory") { A.new(param1, ...) }
# The second initialisation parameter is the persister.  Supported persisters are:
#
# * Marshal  (default)
# * YAML
# * SOAP::Marshal
# * Madeleine::ZMarshal.new(Marshal)
# * Madeleine::ZMarshal.new(YAML)
# * Madeleine::ZMarshal.new(SOAP::Marshal)
#
# The class keeps a record of all the systems that currently exist.
# Each instance of the class keeps a record of Prox objects in that system by internal id (myid).
#
# We also add functionality to take_snapshot in order to set things up so that the custom Prox object 
# marshalling will work correctly.
#
    class AutomaticSnapshotMadeleine
      attr_accessor :marshaller
      attr_reader :list, :sysid

      def initialize(directory_name, marshaller=Marshal, persister=SnapshotMadeleine, &new_system_block)
        @sysid ||= Time.now.to_f.to_s + Thread.current.object_id.to_s # Gererate a new sysid
        @myid_count = 0
        @list = {}
        Thread.current[:system] = self # during system startup system should not create commands
        Thread.critical = true
        @@systems ||= {}  # holds systems by sysid
        @@systems[@sysid] = self
        Thread.critical = false
        @marshaller = marshaller # until attrb
        begin
          @persister = persister.new(directory_name, Automatic_marshaller, &new_system_block)
          @list.delete_if {|k,v|  # set all the prox objects that now exist to have the right sysid
            begin
              ObjectSpace._id2ref(v).sysid = @sysid
              false
            rescue RangeError
              true # Id was to a GC'd object, delete it
            end
          }
        ensure
          Thread.current[:system] = false
        end
      end
#
# Add a proxy object to the list, return the myid for that object
#
      def add(proxo)  
        @list[@myid_count += 1] = proxo.object_id
        @myid_count
      end
#
# Restore a marshalled proxy object to list - myid_count is increased as required.
# If the object already exists in the system then the existing object must be used.
#
      def restore(proxo)  
        if (@list[proxo.myid])
          proxo = myid2ref(proxo.myid)
        else
          @list[proxo.myid] = proxo.object_id
          @myid_count = proxo.myid if (@myid_count < proxo.myid)
        end
        proxo
      end
#
# Returns a reference to the object indicated by the internal id supplied.
#
      def myid2ref(myid)
        raise "Internal id #{myid} not found" unless objid = @list[myid]
        ObjectSpace._id2ref(objid)
      end
#
# Take a snapshot of the system.
#
      def take_snapshot
        begin
          Thread.current[:system] = self
          Thread.current[:snapshot_memory] = {}
          @persister.take_snapshot
        ensure
          Thread.current[:snapshot_memory] = nil
          Thread.current[:system] = false
        end
      end
#
# Returns the hash containing the systems. 
#
      def AutomaticSnapshotMadeleine.systems
        @@systems
      end
#
# Close method changes the sysid for Prox objects so they can't be mistaken for real ones in a new 
# system before GC gets them
#
      def close
        begin
          @list.each_key {|k| myid2ref(k).sysid = nil}
        rescue RangeError
          # do nothing
        end
        @persister.close
      end

#
# Pass on any other calls to the persister
#
      def method_missing(symbol, *args, &block)
        @persister.send(symbol, *args, &block)
      end
    end


    module Deserialize #:nodoc:
#
# Detect format of an io stream. Leave it rewound.
#
      def Deserialize.detect(io)
        c = io.getc
        c1 = io.getc
        io.rewind
        if (c == Marshal::MAJOR_VERSION && c1 <= Marshal::MINOR_VERSION)
          Marshal
        elsif (c == 31 && c1 == 139) # gzip magic numbers
          ZMarshal
        else
          while (s = io.gets)
            break if (s !~ /^\s*$/) # ignore blank lines
          end
          io.rewind
          if (s && s =~ /^\s*<\?[xX][mM][lL]/) # "<?xml" begins an xml serialization
            SOAP::Marshal
          else
            while (s = io.gets)
              break if (s !~ /^\s*#/ && s !~ /^\s*$/) # ignore blank and comment lines
            end
            io.rewind
            if (s && s =~ /^\s*---/) # "---" is the yaml header
              YAML
            else
              nil # failed to detect
            end
          end
        end
      end
#
# Try to deserialize object.  If there was an error, try to detect marshal format, 
# and return deserialized object using the right marshaller
# If detection didn't work, raise up the exception
#
      def Deserialize.load(io, marshaller=Marshal)
        begin
          marshaller.load(io)
        rescue Exception => e
          io.rewind
          detected_marshaller = detect(io)
          if (detected_marshaller == ZMarshal)
            zio = Zlib::GzipReader.new(io)
            detected_zmarshaller = detect(zio)
            zio.finish
            io.rewind
            if (detected_zmarshaller)
              ZMarshal.new(detected_zmarshaller).load(io)
            else
              raise e
            end
          elsif (detected_marshaller)
            detected_marshaller.load(io)
          else
            raise e
          end
        end
      end
    end

  end
end

AutomaticSnapshotMadeleine = Madeleine::Automatic::AutomaticSnapshotMadeleine
