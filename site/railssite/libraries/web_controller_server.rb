require 'webrick'
include WEBrick

# Terminology:
# * Static controller: Is a controller that has all it's show_* and do_* methods mounted by the server
# at server start, so the server will handle request rejection of invalid requests
# * Dynamic controller: Is a controller that's mounted with a responsibility to handle all the request mapping
# itself. Hence, the server will never reject a request that's below the controller in the path.
class WebControllerServer
  STATIC_MOUNTING  = 1
  DYNAMIC_MOUNTING = 2

  MOUNT_TYPE = DYNAMIC_MOUNTING

  SERVER_TYPE = (RUBY_PLATFORM =~ /mswin/ ? SimpleServer : Daemon)

  def initialize(port, server_type, controller_path)
    @server = WEBrick::HTTPServer::new(:Port => port, :ServerType => server_type || SERVER_TYPE)
    @server.logger.info "Your WEBrick server is now running on http://localhost:#{port}"
    @controller_path = controller_path
    mount_controllers
    start_server
  end
  
  private
    # Mounts all the controllers in the controller_path according to the mount type
    def mount_controllers
      require_controller_files

      case MOUNT_TYPE
        when STATIC_MOUNTING  then controller_names.each { |name| mount_static_controller(name)  }
        when DYNAMIC_MOUNTING then controller_names.each { |name| mount_dynamic_controller(name) }
      end
    end

    # Mounts the controller specified by <tt>controller_name</tt> to accept all requests below it's path.
    # If more than one controller is mounted by this WebControllerServer, the controller is mounted by name,
    # such that fx WikiController would get the requests of "/wiki/something" and "/wiki/some/thing/else".
    # If only one controller is mounted, all requests to this WebControllerServer is handled by that controller
    def mount_dynamic_controller(controller_name)
      mount_path = mounting_multiple_controllers? ? "/#{controller_name}" : "/"
      @server.mount(mount_path, controller_class(controller_name))
    end

    # Mount all public show_* and do_* methods as actions tied to the controller specified by <tt>controller_name</tt>.
    # If more than one controller is mounted by this WebControllerServer, the actions are mounted below the controller
    # in the path, such as "/wiki/page" or "/wiki/list". If only one controller is mounted, they're mounted directly on 
    # the root, such as "/page" or "/list"
    def mount_static_controller(controller_name)
      controller_class(controller_name).public_instance_methods(false).each { |method|
        mount_path = (mounting_multiple_controllers? ? "/#{controller_name}" : "") + "/#{method}"
        @server.mount(mount_path, controller_class(controller_name))
      }
    end

    # Requires all the controller files that are present in the controller_path
    def require_controller_files
      controller_names.each { |controller| require @controller_path + controller }
    end

    # Returns true if more than one controller exists in the controller_path
    def mounting_multiple_controllers?
      controller_names.length > 1
    end

    # Returns a list of controller names in lower-case from the controller path
    def controller_names
      Dir.entries(@controller_path).delete_if { |file| file !~ /rb$/ }.collect{ |c| c[0..-4] }
    end

    # Returns the class of the controller specified by the <tt>controller_name</tt>
    def controller_class(controller_name)
      eval(controller_name.capitalize + "Controller")
    end

    # Start accepting requests to the defined mount points
    def start_server
      trap("INT") { @server.shutdown }
      @server.start
    end
end