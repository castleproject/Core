require File.dirname(__FILE__) + '/../test_helper'
require 'management_controller'

# Re-raise errors caught by the controller.
class ManagementController; def rescue_action(e) raise e end; end

class ManagementControllerTest < Test::Unit::TestCase
  def setup
    @controller = ManagementController.new
    @request, @response = ActionController::TestRequest.new, ActionController::TestResponse.new
  end

  # Replace this with your real tests
  def test_truth
    assert true
  end
end
