require 'chunks/uri'
require 'chunks/match'
require 'test/unit'

class URITest < Test::Unit::TestCase
  include ChunkMatch

  def test_non_matches
    assert_no_match(URIChunk.pattern, 'There is no URI here') 
    assert_no_match(URIChunk.pattern, 'One gemstone is the garnet:reddish in colour, like ruby') 
  end

  def test_simple_uri
    match(URIChunk, 'http://www.example.com',
		:scheme =>'http', :host =>'www.example.com', :path => nil,
		:link_text => 'http://www.example.com'
	)
    match(URIChunk, 'http://www.example.com/',
		:scheme =>'http', :host =>'www.example.com', :path => '/',
		:link_text => 'http://www.example.com/'
	)
    match(URIChunk, 'www.example.com', 
		:scheme =>'http', :host =>'www.example.com', :link_text => 'www.example.com'
	)
    match(URIChunk, 'example.com', 
		:scheme =>'http',:host =>'example.com', :link_text => 'example.com'
    )
    match(URIChunk, 'http://example.com.au/', 
		:scheme =>'http', :host =>'example.com.au', :link_text => 'http://example.com.au/'
	)
    match(URIChunk, 'example.com.au',  
		:scheme =>'http', :host =>'example.com.au', :link_text => 'example.com.au'
	)
    match(URIChunk, 'http://www.example.co.uk/',
		:scheme =>'http', :host =>'www.example.co.uk',
		:link_text => 'http://www.example.co.uk/'
	)
    match(URIChunk, 'example.co.uk',
		:scheme =>'http', :host =>'example.co.uk', :link_text => 'example.co.uk'
	)
	match(URIChunk, 'http://moinmoin.wikiwikiweb.de/HelpOnNavigation',
		:scheme => 'http', :host => 'moinmoin.wikiwikiweb.de', :path => '/HelpOnNavigation',
		:link_text => 'http://moinmoin.wikiwikiweb.de/HelpOnNavigation'
	)
	match(URIChunk, 'moinmoin.wikiwikiweb.de/HelpOnNavigation',
		:scheme => 'http', :host => 'moinmoin.wikiwikiweb.de', :path => '/HelpOnNavigation',
		:link_text => 'moinmoin.wikiwikiweb.de/HelpOnNavigation'
	)
  end

  def test_email_uri
	match(URIChunk, 'mail@example.com', 
		:user => 'mail', :host => 'example.com', :link_text => 'mail@example.com'
	)
  end

  def test_non_email
	# The @ is part of the normal text, but 'example.com' is marked up.
	match(URIChunk, 'Not an email: @example.com', :user => nil, :uri => 'http://example.com')
  end

  def test_non_uri
	assert_no_match(URIChunk.pattern, 'httpd.conf')
	assert_no_match(URIChunk.pattern, 'libproxy.so')
  end

  def test_uri_in_text
    match(URIChunk, 'Go to: http://www.example.com/', :host => 'www.example.com', :path =>'/')
    match(URIChunk, 'http://www.example.com/ is a link.', :host => 'www.example.com')
    match(URIChunk, 
      'Email david@loudthinking.com', 
      :scheme =>'mailto', :user =>'david', :host =>'loudthinking.com'
    )
  end

  def test_uri_in_parentheses
    match(URIChunk, 'URI (http://brackets.com.de) in brackets', :host => 'brackets.com.de')
    match(URIChunk, 'because (as shown at research.net) the results', :host => 'research.net')
    match(URIChunk, 
      'A wiki (http://wiki.org/wiki.cgi?WhatIsWiki) page', 
      :scheme => 'http', :host => 'wiki.org', :path => '/wiki.cgi', :query => 'WhatIsWiki'
    )
  end
  
  def test_uri_list_item
    match(
      URIChunk, 
      '* http://www.btinternet.com/~mail2minh/SonyEricssonP80xPlatform.sis', 
      :path => '/~mail2minh/SonyEricssonP80xPlatform.sis'
    )
  end
end
