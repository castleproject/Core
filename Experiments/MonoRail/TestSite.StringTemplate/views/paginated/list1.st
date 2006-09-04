$list1:{item|
$item 
}$
<p>
Showing $list1.FirstItem$ - $list1.LastItem$ of $list1.TotalItems$
</p>
<p>
HasFirst: $list1.HasFirst$ <br/>
HasPrevious: $list1.HasPrevious$ <br/>
HasNext: $list1.HasNext$ <br/>
HasLast: $list1.HasLast$ <br/>
</p>
<p>
$PaginationHelper/CreatePageLink( page=1, text="first" )$
$PaginationHelper/CreatePageLink( page=list1.PreviousIndex, text="prev" )$
$PaginationHelper/CreatePageLink( page=list1.NextIndex, text="next" )$
$PaginationHelper/CreatePageLink( page=list1.LastIndex, text="last" )$
<p>

