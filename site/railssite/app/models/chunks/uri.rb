require 'chunks/chunk'

# This wiki chunk matches arbitrary URIs, using patterns from the Ruby URI modules.
# It parses out a variety of fields that could be used by renderers to format
# the links in various ways (shortening domain names, hiding email addresses)
# It matches email addresses and host.com.au domains without schemes (http://)
# but adds these on as required.
#
# The heuristic used to match a URI is designed to err on the side of caution.
# That is, it is more likely to not autolink a URI than it is to accidently
# autolink something that is not a URI. The reason behind this is it is easier
# to force a URI link by prefixing 'http://' to it than it is to escape and
# incorrectly marked up non-URI.
#
# I'm using a part of the [ISO 3166-1 Standard][iso3166] for country name suffixes.
# The generic names are from www.bnoack.com/data/countrycode2.html)
#   [iso3166]: http://geotags.com/iso3166/
class URIChunk < Chunk::Abstract
  include URI::REGEXP::PATTERN

  # this condition is to get rid of pesky warnings in tests
  unless defined? URIChunk::INTERNET_URI_REGEXP

    GENERIC = '(?:aero|biz|com|coop|edu|gov|info|int|mil|museum|name|net|org)'
    COUNTRY = '(?:au|at|be|ca|ch|de|dk|fr|hk|in|ir|it|jp|nl|no|pt|ru|se|sw|tv|tw|uk|us)'
  
    # These are needed otherwise HOST will match almost anything
    TLDS = "(?:#{GENERIC}|#{COUNTRY})"
    
    # Redefine USERINFO so that it must have non-zero length
    USERINFO = "(?:[#{UNRESERVED};:&=+$,]|#{ESCAPED})+"
  
    # unreserved_no_ending = alphanum | mark, but URI_ENDING [)!] excluded
    UNRESERVED_NO_ENDING = "-_.~*'(#{ALNUM}"  

    # this ensures that query or fragment do not end with URI_ENDING
    # and enable us to use a much simpler self.pattern Regexp

    # uric_no_ending = reserved | unreserved_no_ending | escaped
    URIC_NO_ENDING = "(?:[#{UNRESERVED_NO_ENDING}#{RESERVED}]|#{ESCAPED})"
    # query = *uric
    QUERY = "#{URIC_NO_ENDING}*"
    # fragment = *uric
    FRAGMENT = "#{URIC_NO_ENDING}*"

    # DOMLABEL is defined in the ruby uri library, TLDS is defined above
    INTERNET_HOSTNAME = "(?:#{DOMLABEL}\\.)+#{TLDS}" 

    # Correct a typo bug in ruby 1.8.x lib/uri/common.rb 
    PORT = '\\d*'

    INTERNET_URI =
        "(?:(#{SCHEME}):/{0,2})?" + # Optional scheme:        (\1)
        "(?:(#{USERINFO})@)?" +     # Optional userinfo@      (\2)
        "(#{INTERNET_HOSTNAME})" +  # Mandatory hostname      (\3)
        "(?::(#{PORT}))?" +         # Optional :port          (\4)
        "(#{ABS_PATH})?"  +         # Optional absolute path  (\5)
        "(?:\\?(#{QUERY}))?" +      # Optional ?query         (\6)
        "(?:\\#(#{FRAGMENT}))?"     # Optional #fragment      (\7)

    TEXTILE_SYNTAX_PREFIX = '(!)?'
  
    INTERNET_URI_REGEXP = Regexp.new(TEXTILE_SYNTAX_PREFIX + INTERNET_URI, Regexp::EXTENDED, 'N')

  end

  def URIChunk.pattern
    INTERNET_URI_REGEXP
  end

  attr_reader :user, :host, :port, :path, :query, :fragment, :link_text
  
  def self.apply_to(content)
    content.gsub!( self.pattern ) do |matched_text|
      chunk = self.new($~)
      if chunk.textile_url? or chunk.textile_image?
        # do not substitute
        matched_text
      else
        content.chunks << chunk
        chunk.mask(content)
      end
    end
  end

  def initialize(match_data)
    super(match_data)
    @link_text = match_data[0]
    @textile_prefix, @original_scheme, @user, @host, @port, @path, @query, @fragment = 
        match_data[1..-1]
    treat_trailing_character
  end

  def textile_url?
    @textile_prefix == '":'
  end

  def textile_image?
    @textile_prefix == '!' and @trailing_punctuation == '!'
  end

  def treat_trailing_character
    # If the last character matched by URI pattern is in ! or ), this may be part of the markup,
    # not a URL. We should handle it as such. It is possible to do it by a regexp, but 
    # much easier to do programmatically
    last_char = @link_text[-1..-1]
    if last_char == ')' or last_char == '!'
      @trailing_punctuation = last_char
      @link_text.chop!
      [@original_scheme, @user, @host, @port, @path, @query, @fragment].compact.last.chop!
    end
  end

  # If the text should be escaped then don't keep this chunk.
  # Otherwise only keep this chunk if it was substituted back into the
  # content.
  def unmask(content)
    return nil if escaped_text
    return self if content.sub!(mask(content), "<a href=\"#{uri}\">#{link_text}</a>")
  end

  # If there is no hostname in the URI, do not render it
  # It's probably only contains the scheme, eg 'something:' 
  def escaped_text() ( host.nil? ? @uri : nil )  end

  def scheme
    @original_scheme or (@user ? 'mailto' : 'http')
  end

  def scheme_delimiter
    scheme == 'mailto' ? ':' : '://'
  end

  def user_delimiter
     '@' unless @user.nil?
  end

  def port_delimiter
     ':' unless @port.nil?
  end

  def query_delimiter
     '?' unless @query.nil?
  end

  def uri
    [scheme, scheme_delimiter, user, user_delimiter, host, port_delimiter, port, path, 
      query_delimiter, query].compact.join
  end

end

# uri with mandatory scheme but less restrictive hostname, like
# http://localhost:2500/blah.html
class LocalURIChunk < URIChunk

  unless defined? LocalURIChunk::LOCAL_URI_REGEXP
    # hostname can be just a simple word like 'localhost'
    ANY_HOSTNAME = "(?:#{DOMLABEL}\\.)*#{TOPLABEL}\\.?"
    
    # The basic URI expression as a string
    # Scheme and hostname are mandatory
    LOCAL_URI =
           "(?:(#{SCHEME})://)+" +  # Mandatory scheme://     (\1)
           "(?:(#{USERINFO})@)?" +  # Optional userinfo@      (\2)
           "(#{ANY_HOSTNAME})" +    # Mandatory hostname      (\3)
           "(?::(#{PORT}))?" +      # Optional :port          (\4)
           "(#{ABS_PATH})?"  +      # Optional absolute path  (\5)
           "(?:\\?(#{QUERY}))?" +   # Optional ?query         (\6)
           "(?:\\#(#{FRAGMENT}))?"  # Optional #fragment      (\7)
  
    LOCAL_URI_REGEXP = Regexp.new(TEXTILE_SYNTAX_PREFIX + LOCAL_URI, Regexp::EXTENDED, 'N')
  end

  def LocalURIChunk.pattern
    LOCAL_URI_REGEXP
  end

end
