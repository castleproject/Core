require 'abstract_application'

class FacilitiesController < AbstractApplicationController
  helper :facilities

  def index
    @facilities = FacilitiesList.new()
  end

end
