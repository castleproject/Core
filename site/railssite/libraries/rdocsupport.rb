begin
  require "rdoc/markup/simple_markup"
  require 'rdoc/markup/simple_markup/to_html'
rescue LoadError
  # use old version if available
  require 'markup/simple_markup'
  require 'markup/simple_markup/to_html'
end

module RDocSupport

# A simple +rdoc+ markup class which recognizes some additional
# formatting commands suitable for Wiki use.
class RDocMarkup < SM::SimpleMarkup
  def initialize
    super()

    pre = '(?:\\s|^|\\\\)'

    # links of the form
    # [[<url> description with spaces]]
    add_special(/((\\)?\[\[\S+?\s+.+?\]\])/,:TIDYLINK)

    # and external references
    add_special(/((\\)?(link:|anchor:|http:|mailto:|ftp:|img:|www\.)\S+\w\/?)/,
                :HYPERLINK)

    # <br/>
    add_special(%r{(#{pre}<br/>)}, :BR)

    # and <center> ... </center>
    add_html("center", :CENTER)
  end

  def convert(text, handler)
    super.sub(/^<p>\n/, '').sub(/<\/p>$/, '')
  end
end

# Handle special hyperlinking requirments for RDoc formatted
# entries. Requires RDoc

class HyperLinkHtml < SM::ToHtml

  # Initialize the HyperLinkHtml object.
  # [path] location of the node
  # [site] object representing the whole site (typically of class
  #             +Site+)
  def initialize
    super()
    add_tag(:CENTER, "<center>", "</center>")
  end

  # handle <br/>
  def handle_special_BR(special)
    return "&lt;br/&gt" if special.text[0,1] == '\\'
    special.text
  end

  # We're invoked with a potential external hyperlink.
  # [mailto:]   just gets inserted.
  # [http:]     links are checked to see if they
  #             reference an image. If so, that image gets inserted
  #             using an <img> tag. Otherwise a conventional <a href>
  #             is used.
  # [img:] insert a <tt><img></tt> tag
  # [link:] used to insert arbitrary <tt><a></tt> references
  # [anchor:] used to create an anchor
  def handle_special_HYPERLINK(special)
      text = special.text.strip
      return text[1..-1] if text[0,1] == '\\'
      url = special.text.strip
      if url =~ /([A-Za-z]+):(.*)/
        type = $1
        path = $2
      else
        type = "http"
        path = url
        url  = "http://#{url}"
      end

      case type 
      when "http"
        if url =~ /\.(gif|png|jpg|jpeg|bmp)$/
          "<img src=\"#{url}\"/>"
        else
          "<a href=\"#{url}\">#{url.sub(%r{^\w+:/*}, '')}</a>"
        end
      when "img"
        "<img src=\"#{path}\"/>"
      when "link"
        "<a href=\"#{path}\">#{path}</a>"
      when "anchor"
        "<a name=\"#{path}\"></a>"
      else
        "<a href=\"#{url}\">#{url.sub(%r{^\w+:/*}, '')}</a>"
      end
    end

    # Here's a hyperlink where the label is different to the URL
    #  [[url label that may contain spaces]]
    #

    def handle_special_TIDYLINK(special)
      text = special.text.strip
      return text[1..-1] if text[0,1] == '\\'
      unless text =~ /\[\[(\S+?)\s+(.+?)\]\]/
        return text
      end
      url   = $1
      label = $2
      label = RDocFormatter.new(label).to_html
      label = label.split.select{|x| x =~ /\S/}.
        map{|x| x.chomp}.join(' ')

      case url
      when /link:(\S+)/
        return %{<a href="#{$1}">#{label}</a>}
      when /img:(\S+)/
        return %{<img src="http://#{$1}" alt="#{label}" />}
      when /rubytalk:(\S+)/
        return %{<a href="http://ruby-talk.org/blade/#{$1}">#{label}</a>}
      when /rubygarden:(\S+)/
        return %{<a href="http://www.rubygarden.org/ruby?#{$1}">#{label}</a>}
      when /c2:(\S+)/
        return %{<a href="http://c2.com/cgi/wiki?#{$1}">#{label}</a>}
      when /isbn:(\S+)/
        return %{<a href="http://search.barnesandnoble.com/bookSearch/} +
          %{isbnInquiry.asp?isbn=#{$1}">#{label}</a>}
      end

      unless url =~ /\w+?:/
        url = "http://#{url}"
      end

      "<a href=\"#{url}\">#{label}</a>"
    end
end

class RDocFormatter
  def initialize(text)
    @text = text
  end

  def to_html
    markup = RDocMarkup.new
    h = HyperLinkHtml.new
    markup.convert(@text, h)
  end
end

end