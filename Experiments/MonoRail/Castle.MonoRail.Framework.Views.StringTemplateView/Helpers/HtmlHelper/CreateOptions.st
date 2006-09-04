$!
	HtmlHelper/CreateOptions(elems, textProperty, valueProperty, selectedProperty)
	
	If "selectedProperty" is either non-null or evaluates to the boolean value 'true'
	for an element in the collection 'elems', that element is marked as "selected".
!$
$if(elems)$$elems:{elem | <option $if(elem.(selectedProperty))$selected="true"$endif$ $if(elem.(valueProperty))$value="$elem.($valueProperty$)$"$endif$>$elem.($textProperty$)$</option>}; separator="\n"$$\n$$endif$