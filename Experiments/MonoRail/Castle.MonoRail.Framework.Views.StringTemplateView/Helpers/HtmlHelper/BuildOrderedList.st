$!
	HtmlHelper/BuildOrderedList(elements, styleClass?, itemClass?)
!$
<ol $if(styleClass)$class="$styleClass$"$endif$>
	$elements:{element|BuildListItem(item=element, itemClass=itemClass)}; separator="\n"$
</ol>
