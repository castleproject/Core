require 'abstract_application'

class FacilityController < AbstractApplicationController
  helper :facilities

  layout 'layouts/facilitylayout'

  def index
    redirect "../facilities/"
  end

  def aspectsharp
    
  end

end
