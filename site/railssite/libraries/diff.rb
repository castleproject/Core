# heavily based off difflib.py - see that file for documentation
# ported from Python by Bill Atkins

# This does not support all features offered by difflib; it 
# implements only the subset of features necessary
# to support a Ruby version of HTML Differ.  You're welcome to finish this off.

# By default, String#each iterates by line.  This isn't really appropriate
# for diff, so often a string will be split by // to get an array of one-
# character strings.

# Some methods in Diff are untested and are not guaranteed to work.  The
# methods in HTMLDiff and any methods it calls should work quite well.

# changes by DenisMertz
# * main change:
# ** get the tag soup away
#    the tag soup problem was first reported with <p> tags, but it appeared also with
#    <li>, <ul> etc... tags 
#   this version should mostly fix these problems
# ** added a Builder class to manage the creation of the final htmldiff
# * minor changes:
# ** use symbols instead of string to represent opcodes 
# ** small fix to html2list
#

module Enumerable
  def reduce(init)
    result = init
    each { |item| result = yield(result, item) }
    result
  end
end

module Diff

  class SequenceMatcher
    def initialize(a=[''], b=[''], isjunk=nil, byline=false)
      a = (!byline and a.kind_of? String) ? a.split(//) : a
      b = (!byline and b.kind_of? String) ? b.split(//) : b
      @isjunk = isjunk || proc {}
      set_seqs a, b
    end

    def set_seqs(a, b)
      set_seq_a a
      set_seq_b b
    end

    def set_seq_a(a)
      @a = a
      @matching_blocks = @opcodes = nil
    end

    def set_seq_b(b)
      @b = b
      @matching_blocks = @opcodes = nil
      chain_b
    end

    def chain_b
      @fullbcount = nil
      @b2j = {}
      pophash = {}
      junkdict = {}
      
      @b.each_with_index do |elt, i|
        if @b2j.has_key? elt
          indices = @b2j[elt]
          if @b.length >= 200 and indices.length * 100 > @b.length
            pophash[elt] = 1
            indices.clear
          else
            indices.push i
          end
        else
          @b2j[elt] = [i]
        end
      end
        
      pophash.each_key { |elt| @b2j.delete elt }
      
      junkdict = {}
        
      unless @isjunk.nil?
        [pophash, @b2j].each do |d|
          d.each_key do |elt|
            if @isjunk.call(elt)
              junkdict[elt] = 1
              d.delete elt
            end
          end
        end
      end
      
      @isbjunk = junkdict.method(:has_key?)
      @isbpopular = junkdict.method(:has_key?)
    end
    
    def find_longest_match(alo, ahi, blo, bhi)
      besti, bestj, bestsize = alo, blo, 0
      
      j2len = {}
      
      (alo..ahi).step do |i|
        newj2len = {}
        (@b2j[@a[i]] || []).each do |j|
          if j < blo
            next
          end
          if j >= bhi
            break
          end
          
          k = newj2len[j] = (j2len[j - 1] || 0) + 1
          if k > bestsize
            besti, bestj, bestsize = i - k + 1, j - k + 1, k
          end
        end
        j2len = newj2len
      end
      
      while besti > alo and bestj > blo and
          not @isbjunk.call(@b[bestj-1]) and
          @a[besti-1] == @b[bestj-1]
        besti, bestj, bestsize = besti-1, bestj-1, bestsize+1
      end
      
      while besti+bestsize < ahi and bestj+bestsize < bhi and
          not @isbjunk.call(@b[bestj+bestsize]) and
          @a[besti+bestsize] == @b[bestj+bestsize]
        bestsize += 1
      end
      
      while besti > alo and bestj > blo and
          @isbjunk.call(@b[bestj-1]) and
          @a[besti-1] == @b[bestj-1]
        besti, bestj, bestsize = besti-1, bestj-1, bestsize+1
      end
      
      while besti+bestsize < ahi and bestj+bestsize < bhi and
          @isbjunk.call(@b[bestj+bestsize]) and
          @a[besti+bestsize] == @b[bestj+bestsize]
        bestsize += 1
      end
      
      [besti, bestj, bestsize]
    end
    
    def get_matching_blocks
      return @matching_blocks unless @matching_blocks.nil? or 
        @matching_blocks.empty?
      
      @matching_blocks = []
      la, lb = @a.length, @b.length
      match_block_helper(0, la, 0, lb, @matching_blocks)
      @matching_blocks.push [la, lb, 0]
    end
    
    def match_block_helper(alo, ahi, blo, bhi, answer)
      i, j, k = x = find_longest_match(alo, ahi, blo, bhi)
      if not k.zero?
        if alo < i and blo < j
          match_block_helper(alo, i, blo, j, answer)
        end
        answer.push x
        if i + k < ahi and j + k < bhi
          match_block_helper(i + k, ahi, j + k, bhi, answer)
        end
      end
    end
    
    def get_opcodes
      unless @opcodes.nil? or @opcodes.empty?
        return @opcodes
      end

      i = j = 0
      @opcodes = answer = []
      get_matching_blocks.each do |ai, bj, size|
        tag = if i < ai and j < bj
                :replace
              elsif i < ai
                :delete
              elsif j < bj 
                :insert
              end

        answer.push [tag, i, ai, j, bj] if tag
        
        i, j = ai + size, bj + size
        
        answer.push [:equal, ai, i, bj, j] unless size.zero?
        
      end
      return answer
    end

    # XXX: untested
    def get_grouped_opcodes(n=3)
      codes = get_opcodes
      if codes[0][0] == :equal
        tag, i1, i2, j1, j2 = codes[0]
        codes[0] = tag, [i1, i2 - n].max, i2, [j1, j2-n].max, j2
      end
      
      if codes[-1][0] == :equal
        tag, i1, i2, j1, j2 = codes[-1]
        codes[-1] = tag, i1, min(i2, i1+n), j1, min(j2, j1+n)
      end
      nn = n + n
      group = []
      codes.each do |tag, i1, i2, j1, j2|
        if tag == :equal and i2-i1 > nn
          group.push [tag, i1, [i2, i1 + n].min, j1, [j2, j1 + n].min]
          yield group
          group = []
          i1, j1 = [i1, i2-n].max, [j1, j2-n].max
          group.push [tag, i1, i2, j1 ,j2]
        end
      end
      if group and group.length != 1 and group[0][0] == :equal
        yield group
      end
    end

    def ratio
      matches = get_matching_blocks.reduce(0) do |sum, triple|
        sum + triple[-1]
      end
      Diff.calculate_ratio(matches, @a.length + @b.length)
    end

    def quick_ratio
      if @fullbcount.nil? or @fullbcount.empty?
        @fullbcount = {}
        @b.each do |elt|
          @fullbcount[elt] = (@fullbcount[elt] || 0) + 1
        end
      end
      
      avail = {}
      matches = 0
      @a.each do |elt|
        if avail.has_key? elt
          numb = avail[elt]
        else
          numb = @fullbcount[elt] || 0
        end
        avail[elt] = numb - 1
        if numb > 0
          matches += 1
        end
      end
      Diff.calculate_ratio matches, @a.length + @b.length
    end

    def real_quick_ratio
      la, lb = @a.length, @b.length
      Diff.calculate_ratio([la, lb].min, la + lb)
    end

    protected :chain_b, :match_block_helper
  end # end class SequenceMatcher

  def self.calculate_ratio(matches, length)
    return 1.0 if length.zero?
    2.0 * matches / length
  end

  # XXX: untested
  def self.get_close_matches(word, possibilities, n=3, cutoff=0.6)
    unless n > 0
      raise "n must be > 0: #{n}"
    end
    unless 0.0 <= cutoff and cutoff <= 1.0
      raise "cutoff must be in (0.0..1.0): #{cutoff}"
    end

    result = []
    s = SequenceMatcher.new
    s.set_seq_b word
    possibilities.each do |x|
      s.set_seq_a x
      if s.real_quick_ratio >= cutoff and
          s.quick_ratio >= cutoff and
          s.ratio >= cutoff
        result.push [s.ratio, x]
      end
    end
    
    unless result.nil? or result.empty?
      result.sort
      result.reverse!
      result = result[-n..-1]
    end
    result.collect { |score, x| x }
  end

  def self.count_leading(line, ch)
    i, n = 0, line.length
    while i < n and line[i].chr == ch
      i += 1
    end
    i
  end
end


module HTMLDiff
  include Diff
  class Builder
    VALID_METHODS = [:replace, :insert, :delete, :equal]
    def initialize(a, b)
      @a = a
      @b = b
      @content = []
    end

    def do_op(opcode)
      @opcode = opcode
      op = @opcode[0]
      VALID_METHODS.include?(op) or raise(NameError, "Invalid opcode #{op}")
      self.method(op).call
    end

    def result
      @content.join('')
    end

    #this methods have to be called via do_op(opcode) so that @opcode is set properly
    private

    def replace
      delete("diffmod")
      insert("diffmod")
    end
    
    def insert(tagclass="diffins")
      op_helper("ins", tagclass, @b[@opcode[3]...@opcode[4]])
    end
    
    def delete(tagclass="diffdel")
       op_helper("del", tagclass, @a[@opcode[1]...@opcode[2]])
    end
    
    def equal
      @content += @b[@opcode[3]...@opcode[4]]
    end
  
    # using this as op_helper would be equivalent to the first version of diff.rb by Bill Atkins
    def op_helper_simple(tagname, tagclass, to_add)
      @content << "<#{tagname} class=\"#{tagclass}\">" 
      @content += to_add
      @content << "</#{tagname}>"
    end
    
    # this tries to put <p> tags or newline chars before the opening diff tags (<ins> or <del>)
    # or after the ending diff tags
    # as a result the diff tags should be the "more inside" possible.
    # this seems to work nice with html containing only paragraphs
    # but not sure it works if there are other tags (div, span ... ? ) around
    def op_helper(tagname, tagclass, to_add)
      @content << to_add.shift while ( HTMLDiff.is_newline(to_add.first) or 
                                         HTMLDiff.is_p_close_tag(to_add.first) or
                                         HTMLDiff.is_p_open_tag(to_add.first) )
      @content << "<#{tagname} class=\"#{tagclass}\">" 
      @content += to_add
      last_tags = []
      last_tags.unshift(@content.pop) while ( HTMLDiff.is_newline(@content.last) or 
                                         HTMLDiff.is_p_close_tag(@content.last) or
                                         HTMLDiff.is_p_open_tag(@content.last) )
      last_tags.unshift "</#{tagname}>"
      @content += last_tags
      remove_empty_diff(tagname, tagclass)
    end

    def remove_empty_diff(tagname, tagclass)
      if @content[-2] == "<#{tagname} class=\"#{tagclass}\">" and @content[-1] == "</#{tagname}>" then
        @content.pop
        @content.pop
      end
    end

  end
  
  def self.is_newline(x)
    (x == "\n") or (x == "\r") or (x == "\t")
  end

  def self.is_p_open_tag(x)
    x =~ /\A<(p|li|ul|ol|dir|dt|dl)/
  end

  def self.is_p_close_tag(x)
    x =~ %r!\A</(p|li|ul|ol|dir|dt|dl)!
  end

  def self.diff(a, b)
    a, b = a.split(//), b.split(//) if a.kind_of? String and b.kind_of? String
    a, b = html2list(a), html2list(b)

    out = Builder.new(a, b)
    s = SequenceMatcher.new(a, b)

    s.get_opcodes.each do |opcode|
      out.do_op(opcode)
    end

    out.result 
  end
  
  def self.html2list(x, b=false)
    mode = 'char'
    cur = ''
    out = []
    
    x = x.split(//) if x.kind_of? String
    
    x.each do |c|
      if mode == 'tag'
        if c == '>'
          if b
            cur += ']'
          else
            cur += c
          end
          out.push(cur)
          cur = ''
          mode = 'char'
        else
          cur += c
        end
      elsif mode == 'char'
        if c == '<'
          out.push cur
          if b
            cur = '['
          else
            cur = c
          end
          mode = 'tag'
        elsif /\s/.match c
          out.push cur + c
          cur = ''
        else
          cur += c
        end
      end
    end
    
    out.push cur
    # TODO: make something better here
    out.each{|x| x.chomp! unless is_newline(x)}
    out.find_all { |x| x != '' }
  end
  
  
end

if __FILE__ == $0

  require 'pp'
  # a = "<p>this is the original string</p>" # \n<p>but around the world</p>"
  # b = "<p>this is the original </p><p>other parag</p><p>string</p>"
  a = "<ul>\n\t<li>one</li>\n\t<li>two</li>\n</ul>"
  b = "<ul>\n\t<li>one</li>\n\t<li>two\n\t<ul><li>abc</li></ul></li>\n</ul>"
  puts a
  pp HTMLDiff.html2list(a)
  puts
  puts b
  pp HTMLDiff.html2list(b)
 puts
  puts HTMLDiff.diff(a, b)
end