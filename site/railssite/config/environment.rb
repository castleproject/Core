if RUBY_VERSION < '1.8.1' 
  puts 'Instiki requires Ruby 1.8.1+'
  exit
end

# Enable UTF-8 support
$KCODE = 'u'
require 'jcode'

RAILS_ROOT = File.expand_path(File.dirname(__FILE__) + '/../') unless defined? RAILS_ROOT
RAILS_ENV  = ENV['RAILS_ENV'] || 'production' unless defined? RAILS_ENV

unless defined? ADDITIONAL_LOAD_PATHS
# Mocks first.
  ADDITIONAL_LOAD_PATHS = ["#{RAILS_ROOT}/test/mocks/#{RAILS_ENV}"]

# Then model subdirectories.
  ADDITIONAL_LOAD_PATHS.concat(Dir["#{RAILS_ROOT}/app/models/[_a-z]*"]) 

# Followed by the standard includes.
  ADDITIONAL_LOAD_PATHS.concat %w(
    app
    app/models
    app/controllers
    app/helpers
    config
    libraries
  ).map { |dir| "#{File.expand_path(File.join(RAILS_ROOT, dir))}" }

# Third party vendors
  ADDITIONAL_LOAD_PATHS.concat %w(
    vendor/bluecloth-1.0.0/lib
    vendor/madeleine-0.7.1/lib
    vendor/redcloth-2.0.11/lib
    vendor/rubyzip-0.5.6
    vendor/actionpack/lib
    vendor/activesupport/lib
    vendor/railties/lib
  ).map { |dir| 
    "#{File.expand_path(File.join(RAILS_ROOT, dir))}" 
  }.delete_if { |dir| not File.exist?(dir) }

# Prepend to $LOAD_PATH
  ADDITIONAL_LOAD_PATHS.reverse.each { |dir| $:.unshift(dir) if File.directory?(dir) }
end

require 'action_controller'
require 'active_record_stub'
require 'instiki_errors'

unless defined? RAILS_DEFAULT_LOGGER
  RAILS_DEFAULT_LOGGER = Logger.new(STDERR)
  ActionController::Base.logger ||= RAILS_DEFAULT_LOGGER
  if defined? INSTIKI_DEBUG_LOG and INSTIKI_DEBUG_LOG
    RAILS_DEFAULT_LOGGER.level = Logger::DEBUG
    ActionController::Base.logger.level = Logger::DEBUG
  else
    RAILS_DEFAULT_LOGGER.level = Logger::INFO
    ActionController::Base.logger.level = Logger::INFO
  end
end

# Environment-specific configuration.
require "environments/#{RAILS_ENV}"
require 'wiki_service'

Socket.do_not_reverse_lookup = true

ActionController::Base.template_root ||= "#{RAILS_ROOT}/app/views/"
