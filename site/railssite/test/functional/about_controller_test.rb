require File.dirname(__FILE__) + '/../test_helper'
require 'about_controller'

# Re-raise errors caught by the controller.
class AboutController; def rescue_action(e) raise e end; end

class AboutControllerTest < Test::Unit::TestCase
  def setup
    @controller = AboutController.new
    @request, @response = ActionController::TestRequest.new, ActionController::TestResponse.new
  end

  # Replace this with your real tests
  def test_truth
    assert true
  end
end
