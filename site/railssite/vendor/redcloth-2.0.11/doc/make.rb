require 'yaml'
require 'redcloth'

def a_name( phrase )
    phrase.downcase.
           gsub( /\W+/, '-' )
end

file_name = ARGV.shift
case file_name
when 'README'
    puts <<-HTML
    <!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN"
            "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
    <html xmlns="http://www.w3.org/1999/xhtml" lang="en" xml:lang="en">
    <head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>RedCloth [Textile Humane Web Text for Ruby]</title>
    <style type="text/css">
    BODY {
        margin: 10px 60px 40px 60px;
        font-family: georgia, serif;
        font-size: 12pt;
    }
    TABLE {
        padding: 15px;
        width: 250px;
    }
    TH {
        text-align: left;
    }
    TD {
        border-top: solid 1px #eee;
    }
    H4 {
        border: solid 1px #caa;
        margin: 10px 15px 5px 10px;
        padding: 5px;
        color: white;
        background-color: #333;
        font-weight: bold;
        font-size: 12pt;
    }
    P {
        margin: 10px 15px 5px 15px;
    }
    P.example1 {
        background-color: #FEE;
        font-weight: bold;
        font-size: 9pt;
        padding: 5px;
    }
    P.example2 {
        border: solid 1px #DDD;
        background-color: #EEE;
        font-size: 9pt;
        padding: 5px;
    }
    .big {
        font-size: 15pt;
    }
    #big-red {
        font-size: 15pt;
        color: red;
    }
    #big-red2 {
        font-size: 15pt;
        color: red;
    }
    #sidebar {
        float: right;
        font-family: verdana, arial, sans-serif;
        font-size: 10pt;
        border-left: solid 1px #999;
        margin-left: 10px;
        width: 200px;
    }
    /* VARIATION BUTTON STYLING (v2.0) - SIZABLE SIZE */
    #css-buttons ul{list-style: none;margin: 0 0 10px 0;padding: 0;}
    #css-buttons li{border: 1px solid #999; margin: 5px 0 0 20px; width:9.0em;}
    head:first-child+body #css-buttons li{padding-right:2px;}
    #css-buttons li a{color: #333; text-decoration: none;}

    .css-button {
        display:block;
        font: 0.8em verdana, arial, sans-serif;
        padding: 2px 0 2px 0px; border: 1px solid white;
        text-decoration: none; width:100%;
        background: #ddd;color: #333;
    }

    .css-button span {
        font: bold 1.0em verdana, arial, sans-serif;
        padding: 2px 3px 2px 3px; color: #fff;
    }  

    /* BUTTON LOGO STYLING */
    .rss span{background:#f60;}
    .w3c span {background: #fff; color: #069; font: bold 1.1em helvetica, arial, Sans-Serif;}
    .w3c2{background: #fc6;color: black !important;}
    </style>
    </head>
    <body>
    HTML
    puts RedCloth.new( File.open( file_name ).read ).to_html
    puts "</body>"
    puts "</html>"
when 'QUICK-REFERENCE'
    YAML::add_private_type( "example" ) do |type, val|
        esc = val.dup
        esc.htmlesc!( :NoQuotes )
        [ :example, esc.gsub( /\n/, '<br />' ),
          RedCloth.new( val ).to_html ]
    end

    content = YAML::load( File.open( 'REFERENCE' ) )

    sections = content.collect { |c| c.keys.first }
    sections.shift

    puts <<-HTML
    <!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN"
         "DTD/xhtml1-transitional.dtd">
    <html xmlns="http://www.w3.org/1999/xhtml" lang="en" xml:lang="en">
    <head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Textile Quick Reference</title>
    <style type="text/css">
    BODY {
        margin: 5px;
    }
    TABLE {
        font-family: georgia, serif;
        font-size: 10pt;
        padding: 5px;
        width: 200px;
    }
    TH {
        padding-top: 10px;
    }
    TD {
        border-top: solid 1px #eee;
    }
    H1 {
        font-size: 18pt;
    }
    H4 {
        color: #999;
        background-color: #fee;
        border: solid 1px #caa;
        margin: 10px 15px 5px 10px;
        padding: 5px;
    }
    P {
        margin: 2px 5px 2px 5px;
    }
    TD.example1 PRE {
        background-color: #FEE;
        font-family: georgia, serif;
        font-size: 8pt;
        padding: 5px;
        padding: 5px;
    }
    TD.example3 {
        border: solid 1px #DDD;
        background-color: #EEE;
        font-size: 8pt;
        padding: 5px;
    }
    .big {
        font-size: 12pt;
    }
    #big-red {
        font-size: 12pt;
        color: red;
    }
    #big-red2 {
        font-size: 15pt;
        color: red;
    }
    </style>
    </head>
    <body>
    <table>
    <tr><th colspan='3'><h1>Textile Quick Reference</h1></th></tr>
    <tr><th colspan='3'>Sections: #{ sections.collect { |s| "<a href='##{ a_name( s ) }'>#{ s.gsub( /\s/, '&nbsp;' ) }</a>" }.join( ' | ' ) }</th></tr>
    HTML

    ct = 0
    content.each do |section|
        section.each do |header, parags|
            puts "<tr><th colspan='5'><a name='#{ a_name( header ) }'>#{ header }</a></th></tr>" if ct.nonzero?
            parags.each do |p|
                if p.is_a?( Array ) and p[0] == :example
                    puts "<tr><td class='example1'><pre>#{ p[1] }</pre></td><td>&rarr;</td>" +
                             "<td class='example2'>#{ p[2] }</td></tr>"
                end
            end
        end
        ct += 1
    end
    puts "</table>"
    puts "</body>"
    puts "</html>"

when 'REFERENCE'
    YAML::add_private_type( "example" ) do |type, val|
        esc = val.dup
        esc.htmlesc!( :NoQuotes )
        [ esc.gsub( /\n/, '<br />' ),
          RedCloth.new( val ).to_html.
                   gsub( /;(\w)/, '; \1' ).
                   htmlesc!( :NoQuotes ).
                   gsub( /\n/, '<br />' ),
          RedCloth.new( val ).to_html ]
    end

    content = YAML::load( File.open( file_name ) )

    sections = content.collect { |c| c.keys.first }
    sections.shift

    puts <<-HTML
    <!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN"
         "DTD/xhtml1-transitional.dtd">
    <html xmlns="http://www.w3.org/1999/xhtml" lang="en" xml:lang="en">
    <head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Textile Reference</title>
    <style type="text/css">
    BODY {
        margin: 10px 30px;
    }
    TABLE {
        font-family: georgia, serif;
        font-size: 11pt;
        padding: 15px;
    }
    TH {
        border-bottom: solid 1px black;
        font-size: 24pt;
        font-weight: bold;
        padding-top: 30px;
    }
    H1 {
        font-size: 42pt;
    }
    H4 {
        color: #666;
        background-color: #fee;
        border: solid 1px #caa;
        margin: 10px 0px 5px 0px;
        padding: 5px;
    }
    P {
        margin: 10px 15px 5px 15px;
    }
    TD.sections {
        background: black;
        color: white;
        font-family: georgia, serif;
        font-weight: bold;
        font-size: 9pt;
        padding: 5px;
    }
    TD.sections A         { color: #CCEEFF; }
    TD.sections A:link    { color: #CCEEFF; }
    TD.sections A:visited { color: #CCEEFF; }
    TD.sections A:active  { color: #EEEEEE; }
    TD.sections A:hover   { color: #EEEEEE; }

    TD.example1 PRE {
        background-color: #B30;
        color: white;
        font-family: georgia, serif;
        font-weight: bold;
        font-size: 9pt;
        padding: 5px;
    }
    TD.example2 P {
        border: solid 1px #DDD;
        background-color: #EEE;
        font-family: georgia, serif;
        font-size: 9pt;
        padding: 5px;
    }
    TD.example3 {
        border: solid 1px #EED;
        background-color: #FFE;
        padding: 5px;
    }
    .big {
        font-size: 15pt;
    }
    #big-red {
        font-size: 15pt;
        color: red
    }
    </style>
    <script type="text/javascript">
        function quickRedReference() {
            window.open( 
                "quick.html",
                "redRef",
                "height=600,width=550,channelmode=0,dependent=0," +
                "directories=0,fullscreen=0,location=0,menubar=0," +
                "resizable=0,scrollbars=1,status=1,toolbar=0"
            );
        }
    </script>
    </head>
    <body>
    <table>
    HTML

    ct = 0
    content.each do |section|
        section.each do |header, parags|
            if ct.zero?
                puts "<tr><th colspan='3'><h1>#{ header }</h1></th></tr>"
                puts "<tr><td class='sections' colspan='3'>Sections: #{ sections.collect { |s| "<a href='##{ a_name( s ) }'>#{ s.gsub( /\s/, '&nbsp;' ) }</a>" }.join( ' | ' ) }</td></tr>"
            else
                puts "<tr><th colspan='3'><a name='#{ a_name( header ) }'><small>#{ ct }.</small></a><br />#{ header }</th></tr>"
            end
            parags.each do |p|
                if p.is_a? Array
                    puts "<tr><td class='example1' valign='top'><pre>#{ p[0] }</pre></td><td>&rarr;</td>" +
                             "<td class='example2'><p>#{ p[1] }</p></td></tr><tr><td colspan='2'></td>" +
                             "<td class='example3'>#{ p[2] }</td></tr>"
                else
                    puts "<tr><td class='explain' colspan='3'>"
                    puts RedCloth.new( p ).to_html
                    puts "</td></tr>"
                end
            end
            unless ct.zero?
                puts "<tr><td colspan='5' style='border-bottom: solid 1px #eee;'></td></tr>"
            end
        end
        ct += 1
    end
    puts "</table>"
    puts "</body>"
    puts "</html>"
end
