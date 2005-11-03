<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns="http://www.w3.org/TR/xhtml1/strict">
<xsl:output method="html"/>
	<xsl:template match="/">
		<xsl:apply-templates select="//coverage[count(module) != 0]" />					
	</xsl:template>
	<xsl:template match="coverage">
	
	        <table class="section-table" cellpadding="2" cellspacing="0" border="0" width="98%">
	
	            <tr>
	                <td class="sectionheader" colspan="2">
	           		<!-- Test Coverage Summary -->
	            	
				NCover - Test Coverage:
				<xsl:value-of select=
					"round((count(//coverage/module/method/seqpnt[@visitcount > 0]) div count(//coverage/module/method/seqpnt)) * 100)"/>%
                        </td>
	            </tr>
	            <tr>
		        <td>
		    		<div style="color: maroon; font-size: 10pt; font-weight: bold;">
					Untested Code:
				</div> 
		    	</td>
		    </tr>
	            <tr>
		        <td>
				<xsl:apply-templates select="module"/>
			</td>
		    </tr>
		  
		</table>
	</xsl:template>
	
	<xsl:template match="module">
		<br />
		<xsl:apply-templates select="method[./seqpnt[@visitcount = 0]]"/>
	</xsl:template>
	
	<xsl:template match="method">
		<div style="color: maroon; font-size: 10pt; font-weight: bold;">
			<xsl:value-of select="@class"/>.<xsl:value-of select="@name"/>
		</div> 
		<table border="1" cellpadding="3" cellspacing="0" bordercolor="black" width="100%">
			<xsl:apply-templates select="seqpnt[@visitcount = 0]"/>
		</table>
		<p/>
	</xsl:template>
	
	<xsl:template match="seqpnt">
		<tr>
			<xsl:if test="@line != 16707566">

				<td bgcolor="#FFCCCC" width="15%">
					Line: <xsl:value-of select="@line"/>
				</td>
				<td bgcolor="#FFFFEE">
				<xsl:choose>
				   <!-- if this build is off the default CCNet working directory, shorten the path to show only the bits in version control. -->
				   <xsl:when test="contains(@document, '\WorkingDirectory\')">
					<xsl:value-of select="substring-after(@document, '\WorkingDirectory\')"/>
				   </xsl:when>
				   <xsl:otherwise>
				   	<xsl:value-of select="@document"/>
				   </xsl:otherwise>
				</xsl:choose>
				</td>
			</xsl:if>
		</tr>
	</xsl:template>
</xsl:stylesheet>
