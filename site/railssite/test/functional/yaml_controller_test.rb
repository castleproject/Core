require File.dirname(__FILE__) + '/../test_helper'
require 'yaml_controller'

# Re-raise errors caught by the controller.
class YamlController; def rescue_action(e) raise e end; end

class YamlControllerTest < Test::Unit::TestCase
  def setup
    @controller = YamlController.new
    @request, @response = ActionController::TestRequest.new, ActionController::TestResponse.new
  end

  # Replace this with your real tests
  def test_truth
    assert true
  end
end
