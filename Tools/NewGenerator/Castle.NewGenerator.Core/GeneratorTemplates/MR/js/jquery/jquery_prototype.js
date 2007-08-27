function Extend(dest, newPrototype) 
{
	// Copia prototype de mais derivada para destino, 
	// criando um copia especializada
	for(var property in newPrototype) 
	{
		dest[property] = newPrototype[property];
	}
	return dest;
}

AbstractInsertion = function(adjacency) { this.adjacency = adjacency; }

AbstractInsertion.prototype = {
  initialize: function(element, content) 
  {
    this.element = element;
    this.content = content;

    if (this.adjacency && this.element.insertAdjacentHTML) 
    {
      try 
      {
        this.element.insertAdjacentHTML(this.adjacency, this.content);
      } 
      catch (e) 
      {
        var tagName = this.element.tagName.toUpperCase();
        
        if (tagName == 'TBODY' || tagName == 'TR')
        {
          this.insertContent(this.contentFromAnonymousTable());
        }
        else
        {
          throw e;
        }
      }
    } 
    else 
    {
      this.range = this.element.ownerDocument.createRange();
      if (this.initializeRange) this.initializeRange();
      this.insertContent([this.range.createContextualFragment(this.content)]);
    }
  },

  contentFromAnonymousTable: function() {
    var div = document.createElement('div');
    div.innerHTML = '<table><tbody>' + this.content + '</tbody></table>';
    return div.childNodes[0].childNodes[0].childNodes;
  }
}

var InsertionBottom = function() { this.initialize.apply(this, arguments); } 

InsertionBottom.prototype = Extend(new AbstractInsertion('beforeEnd'), {
  initializeRange: function() {
    this.range.selectNodeContents(this.element);
    this.range.collapse(this.element);
  },

  insertContent: function(fragments) 
  {
	for(var i=0; i < fragments.length; i++)
	{
		this.element.appendChild(fragments[i]);
	}
  }
});

