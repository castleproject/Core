require File.dirname(__FILE__) + '/../test_helper'
require 'dynamicproxy_controller'

# Re-raise errors caught by the controller.
class DynamicproxyController; def rescue_action(e) raise e end; end

class DynamicproxyControllerTest < Test::Unit::TestCase
  def setup
    @controller = DynamicproxyController.new
    @request, @response = ActionController::TestRequest.new, ActionController::TestResponse.new
  end

  # Replace this with your real tests
  def test_truth
    assert true
  end
end
