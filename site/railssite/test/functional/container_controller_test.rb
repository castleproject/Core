require File.dirname(__FILE__) + '/../test_helper'
require 'container_controller'

# Re-raise errors caught by the controller.
class ContainerController; def rescue_action(e) raise e end; end

class ContainerControllerTest < Test::Unit::TestCase
  def setup
    @controller = ContainerController.new
    @request, @response = ActionController::TestRequest.new, ActionController::TestResponse.new
  end

  # Replace this with your real tests
  def test_truth
    assert true
  end
end
