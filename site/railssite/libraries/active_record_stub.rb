# This project uses Railties, which has an external dependency on ActiveRecord
# Since ActiveRecord may not be present in Instiki runtime environment, this 
# file provides a stub replacement for it

unless defined? ActiveRecord::Base

module ActiveRecord
  class Base
    
      # dependency in railties/lib/dispatcher.rb
     def self.reset_column_information_and_inheritable_attributes_for_all_subclasses
        # noop
      end
      
      # dependency in actionpack/lib/action_controller/benchmarking.rb
      def self.connected?
        false
      end
      
    end
  end

end