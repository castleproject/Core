Dependencies.mechanism = :require
ActionController::Base.consider_all_requests_local = true

require 'fileutils'
FileUtils.mkdir_p(RAILS_ROOT + "/log")

unless defined? TEST_LOGGER
  timestamp = Time.now.strftime('%Y%m%d%H%M%S')
  log_name = RAILS_ROOT + "/log/instiki_test.#{timestamp}.log"
  $stderr.puts "To see the Rails log:\n    less #{log_name}"
  
  TEST_LOGGER = ActionController::Base.logger = Logger.new(log_name)
  ActionController::Base.logger.level = Logger::DEBUG
  
  WikiService.storage_path = RAILS_ROOT + '/storage/test/'
end
