require File.dirname(__FILE__) + '/../test_helper'
require 'castleonrails_controller'

# Re-raise errors caught by the controller.
class CastleonrailsController; def rescue_action(e) raise e end; end

class CastleonrailsControllerTest < Test::Unit::TestCase
  def setup
    @controller = CastleonrailsController.new
    @request, @response = ActionController::TestRequest.new, ActionController::TestResponse.new
  end

  # Replace this with your real tests
  def test_truth
    assert true
  end
end
