/*
 * Metadata - jQuery plugin for parsing metadata from elements
 *
 * Copyright (c) 2006 John Resig, Yehuda Katz, Jörn Zaefferer
 *
 * Dual licensed under the MIT and GPL licenses:
 *   http://www.opensource.org/licenses/mit-license.php
 *   http://www.gnu.org/licenses/gpl.html
 *
 * Revision: $Id: metadata.js 1915 2007-05-18 14:25:48Z joern.zaefferer $
 *
 */

/**
 * Sets the type of metadata to use. Metadata is encoded in JSON, and each property
 * in the JSON will become a property of the element itself.
 *
 * There are three supported types of metadata storage:
 *
 *   attr:  Inside an attribute. The name parameter indicates *which* attribute.
 *          
 *   class: Inside the class attribute, wrapped in curly braces: { }
 *   
 *   elem:  Inside a child element (e.g. a script tag). The
 *          name parameter indicates *which* element.
 *          
 * The metadata for an element is loaded the first time the element is accessed via jQuery.
 *
 * As a result, you can define the metadata type, use $(expr) to load the metadata into the elements
 * matched by expr, then redefine the metadata type and run another $(expr) for other elements.
 * 
 * @name $.meta.setType
 *
 * @example <p id="one" class="some_class {item_id: 1, item_label: 'Label'}">This is a p</p>
 * @before $.meta.setType("class")
 * @after $("#one").data().item_id == 1; $("#one")[0].item_label == "Label"
 * @desc Reads metadata from the class attribute
 * 
 * @example <p id="one" class="some_class" data="{item_id: 1, item_label: 'Label'}">This is a p</p>
 * @before $.meta.setType("attr", "data")
 * @after $("#one").data().item_id == 1; $("#one")[0].item_label == "Label"
 * @desc Reads metadata from a "data" attribute
 * 
 * @example <p id="one" class="some_class"><script>{item_id: 1, item_label: 'Label'}</script>This is a p</p>
 * @before $.meta.setType("elem", "script")
 * @after $("#one").data().item_id == 1; $("#one")[0].item_label == "Label"
 * @desc Reads metadata from a nested script element
 * 
 * @param String type The encoding type
 * @param String name The name of the attribute to be used to get metadata (optional)
 * @cat Plugins/Metadata
 * @descr Sets the type of encoding to be used when loading metadata for the first time
 * @type undefined
 * @see data()
 */

(function($) {
	// settings
	$.meta = {
	  type: "class",
	  name: "metadata",
	  setType: function(type,name){
	    this.type = type;
	    this.name = name;
	  },
	  cre: /({.*})/,
	  single: 'metadata'
	};
	
	// reference to original setArray()
	var setArray = $.fn.setArray;
	
	// define new setArray()
	$.fn.setArray = function(arr){
	    return setArray.apply( this, arguments ).each(function(){
	      if ( this.nodeType == 9 || $.isXMLDoc(this) || this.metaDone ) return;
	      
	      var data = "{}";
	      
	      if ( $.meta.type == "class" ) {
	        var m = $.meta.cre.exec( this.className );
	        if ( m )
	          data = m[1];
	      } else if ( $.meta.type == "elem" ) {
	      	if( !this.getElementsByTagName ) return;
	        var e = this.getElementsByTagName($.meta.name);
	        if ( e.length )
	          data = $.trim(e[0].innerHTML);
	      } else if ( this.getAttribute != undefined ) {
	        var attr = this.getAttribute( $.meta.name );
	        if ( attr )
	          data = attr;
	      }
	      
	      if ( !/^{/.test( data ) )
	        data = "{" + data + "}";
	
	      eval("data = " + data);
	
	      if ( $.meta.single )
	        this[ $.meta.single ] = data;
	      else
	        $.extend( this, data );
	      
	      this.metaDone = true;
	    });
	};
	
	/**
	 * Returns the metadata object for the first member of the jQuery object.
	 *
	 * @name data
	 * @descr Returns element's metadata object
	 * @type jQuery
	 * @cat Plugins/Metadata
	 */
	$.fn.data = function() {
	  return this[0][$.meta.single];
	};
})(jQuery);