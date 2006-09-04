$!
	HtmlHelper/InputCheckbox(name, value, isChecked, attrib)
	
	@param name			checkbox name
	@param value		checkbox value
	@param isChecked	checked or unchecked?
	@param attrib		IDictionary of attributes
!$
<input type="checkbox" name="$name$" id="$if(id)$$id$$else$$name$$endif$" value="$value$" $if(isChecked)$checked$endif$ $if(attrib)$$AbstractHelper/GetAttributes(attrib=attrib)$$endif$ />