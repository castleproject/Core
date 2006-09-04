$!
	HtmlHelper/BuildUnorderedList(elements, styleClass?, itemClass?)
!$
<ul $if(styleClass)$class="$styleClass$"$endif$>
	$elements:{element|BuildListItem(item=element, itemClass=itemClass)}; separator="\n"$
</ul>
