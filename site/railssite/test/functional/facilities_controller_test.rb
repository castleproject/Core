require File.dirname(__FILE__) + '/../test_helper'
require 'facilities_controller'

# Re-raise errors caught by the controller.
class FacilitiesController; def rescue_action(e) raise e end; end

class FacilitiesControllerTest < Test::Unit::TestCase
  def setup
    @controller = FacilitiesController.new
    @request, @response = ActionController::TestRequest.new, ActionController::TestResponse.new
  end

  # Replace this with your real tests
  def test_truth
    assert true
  end
end
