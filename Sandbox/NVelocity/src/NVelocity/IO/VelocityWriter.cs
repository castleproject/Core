using System;
using System.IO;
using System.Text;

//package org.apache.velocity.io;
//
//
//import java.io.IOException;
//import java.io.OutputStreamWriter;
//import java.io.Writer;

namespace NVelocity.IO {

    /**
     * Implementation of a fast Writer. It was originally taken from JspWriter
     * and modified to have less syncronization going on.
     *
     * @author <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
     * @author <a href="mailto:jon@latchkey.com">Jon S. Stevens</a>
     * @author Anil K. Vijendran
     * @version $Id: VelocityWriter.cs,v 1.4 2004/12/27 06:12:33 corts Exp $
     */
    // TODO: class was final and it appears that readonly and final are not the same thing - most of the methods were final as well
    public class VelocityWriter : TextWriter {
	/**
	 * constant indicating that the Writer is not buffering output
	 */
	public static readonly int	NO_BUFFER = 0;

	/**
	 * constant indicating that the Writer is buffered and is using the 
	 * implementation default buffer size
	 */
	public static readonly int	DEFAULT_BUFFER = -1;

	/**
	 * constant indicating that the Writer is buffered and is unbounded; 
	 * this is used in BodyContent
	 */
	public static readonly int	UNBOUNDED_BUFFER = -2;

	protected int bufferSize;
	protected Boolean autoFlush;

	private TextWriter writer;

	private char[] cb;
	private int nextChar;

	private static int defaultCharBufferSize = 8 * 1024;

	private Boolean flushed = false;

	/**
	 * Create a buffered character-output stream that uses a default-sized
	 * output buffer.
	 *
	 * @param  response  A Servlet Response
	 */
	public VelocityWriter(TextWriter writer):this(writer, defaultCharBufferSize, true) {}

	/**
	 * private constructor.
	 */
	private VelocityWriter(int bufferSize, Boolean autoFlush) {
	    this.bufferSize = bufferSize;
	    this.autoFlush  = autoFlush;
	}

	/**
	 * This method returns the size of the buffer used by the JspWriter.
	 *
	 * @return the size of the buffer in bytes, or 0 is unbuffered.
	 */
	public int getBufferSize() { return bufferSize; }

	/**
	 * This method indicates whether the JspWriter is autoFlushing.
	 *
	 * @return if this JspWriter is auto flushing or throwing IOExceptions on 
	 *         buffer overflow conditions
	 */
	public Boolean isAutoFlush() { return autoFlush; }

	/**
	 * Create a new buffered character-output stream that uses an output
	 * buffer of the given size.
	 *
	 * @param  response A Servlet Response
	 * @param  sz   	Output-buffer size, a positive integer
	 *
	 * @exception  IllegalArgumentException  If sz is <= 0
	 */
	public VelocityWriter(TextWriter writer, int sz, Boolean autoFlush):this(sz,autoFlush) {
	    if (sz < 0)
		throw new ArgumentOutOfRangeException("Buffer size <= 0");
	    this.writer = writer;
	    cb = sz == 0 ? null : new char[sz];
	    nextChar = 0;
	}

	private void init( TextWriter writer, int sz, Boolean autoFlush ) {
	    this.writer= writer;
	    if( sz > 0 && ( cb == null || sz > cb.Length ) )
		cb=new char[sz];
	    nextChar = 0;
	    this.autoFlush=autoFlush;
	    this.bufferSize=sz;
	}

	/**
	 * Flush the output buffer to the underlying character stream, without
	 * flushing the stream itself.  This method is non-private only so that it
	 * may be invoked by PrintStream.
	 */
	private void flushBuffer() { //throws IOException
	    if (bufferSize == 0)
		return;
	    flushed = true;
	    if (nextChar == 0)
		return;
	    writer.Write(cb, 0, nextChar);
	    nextChar = 0;
	}

	/**
	 * Discard the output buffer.
	 */
	public void clear() {
	    nextChar = 0;
	}

	private void bufferOverflow() { // throws IOException
	    throw new IOException("overflow");
	}

	/**
	 * Flush the stream.
	 *
	 */
	public void flush() { // throws IOException
	    flushBuffer();
	    if (writer != null) {
		writer.Flush();
	    }
	}

	/**
	 * Close the stream.
	 *
	 */
	public void close() { //throws IOException {
	    if (writer == null)
		return;
	    flush();
	}

	/**
	 * @return the number of bytes unused in the buffer
	 */
	public int getRemaining() {
	    return bufferSize - nextChar;
	}

	/**
	 * Write a single character.
	 *
	 */
	public void write(int c) { //throws IOException {
	    if (bufferSize == 0) {
		writer.Write(c);
	    } else {
		if (nextChar >= bufferSize)
		    if (autoFlush)
			flushBuffer();
		    else
			bufferOverflow();
		cb[nextChar++] = (char) c;
	    }
	}

	/**
	 * Our own little min method, to avoid loading
	 * <code>java.lang.Math</code> if we've run out of file
	 * descriptors and we're trying to print a stack trace.
	 */
	private int min(int a, int b) {
	    return (a < b ? a : b);
	}

	/**
	 * Write a portion of an array of characters.
	 *
	 * <p> Ordinarily this method stores characters from the given array into
	 * this stream's buffer, flushing the buffer to the underlying stream as
	 * needed.  If the requested length is at least as large as the buffer,
	 * however, then this method will flush the buffer and write the characters
	 * directly to the underlying stream.  Thus redundant
	 * <code>DiscardableBufferedWriter</code>s will not copy data unnecessarily.
	 *
	 * @param  cbuf  A character array
	 * @param  off   Offset from which to start reading characters
	 * @param  len   Number of characters to write
	 *
	 */
	public void write(char[] cbuf, int off, int len) {
	    //throws IOException    {
	    if (bufferSize == 0) {
		writer.Write(cbuf, off, len);
		return;
	    }

	    if (len == 0) {
		return;
	    }

	    if (len >= bufferSize) {
		/* If the request length exceeds the size of the output buffer,
		flush the buffer and then write the data directly.  In this
		way buffered streams will cascade harmlessly. */
		if (autoFlush)
		    flushBuffer();
		else
		    bufferOverflow();
		writer.Write(cbuf, off, len);
		return;
	    }

	    int b = off, t = off + len;
	    while (b < t) {
		int d = min(bufferSize - nextChar, t - b);
		Array.Copy(cbuf, b, cb, nextChar, d);
		b += d;
		nextChar += d;
		if (nextChar >= bufferSize)
		    if (autoFlush)
			flushBuffer();
		    else
			bufferOverflow();
	    }
	}

	/**
	 * Write an array of characters.  This method cannot be inherited from the
	 * Writer class because it must suppress I/O exceptions.
	 */
	public void write(char[] buf) { //throws IOException {
	    write(buf, 0, buf.Length);
	}

	/**
	 * Write a portion of a String.
	 *
	 * @param  s     String to be written
	 * @param  off   Offset from which to start reading characters
	 * @param  len   Number of characters to be written
	 *
	 */
	public void write(String s, int off, int len) { //throws IOException {
	    if (bufferSize == 0) {
		writer.Write(s, off, len);
		return;
	    }
	    int b = off, t = off + len;
	    while (b < t) {
		int d = min(bufferSize - nextChar, t - b);
		Array.Copy(s.ToCharArray(), b, cb, nextChar, b+d);
		b += d;
		nextChar += d;
		if (nextChar >= bufferSize)
		    if (autoFlush)
			flushBuffer();
		    else
			bufferOverflow();
	    }
	}

	/**
	 * Write a string.  This method cannot be inherited from the Writer class
	 * because it must suppress I/O exceptions.
	 */
	public void write(String s) { //throws IOException {
	    if (s != null) {
		write(s, 0, s.Length);
	    }
	}

	/**
	 * resets this class so that it can be reused
	 *
	 */
	public void recycle( TextWriter writer) {
	    this.writer = writer;
	    flushed = false;
	    clear();
	}

	// needed to extend TextWriter
	public override Encoding Encoding {
	    get { return writer.Encoding; }
	}

    }
}
