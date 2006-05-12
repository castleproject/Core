$!
	HtmlHelper/BuildList(tag, elements, styleClass?, itemClass?)
!$
<$tag$ $if(styleClass)$class="$styleClass$"$endif$>
	$elements:{element|BuildListItem(item=element, itemClass=itemClass)}; separator="\n"$
</$tag$>
