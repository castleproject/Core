<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns="http://www.w3.org/TR/xhtml1/strict">
<xsl:output method="html"/>
	<xsl:template match="/">
		<xsl:apply-templates select="//report/data/all[./coverage/@value != '']" />					
	</xsl:template>
	<xsl:template match="all">
	
	        <table class="section-table" cellpadding="2" cellspacing="0" border="0" width="98%">
	
	            <tr>
	                <td class="unittests-sectionheader" colspan="2">
	           		<!-- Test Coverage Summary -->
	            	
				EMMA - Test Coverage:
				<xsl:value-of select="substring-before(./coverage/@value, '%')"/>%
                        </td>
	            </tr>
	            <tr>
		        <td>
		    		<div style="color: maroon; font-size: 10pt; font-weight: bold;">
					Package breakdown:
				</div> 
		    	</td>
		    </tr>
	            <tr>
		        <td>
			      <br />
			      <table border="1" cellpadding="3" cellspacing="0" bordercolor="black" width="100%">
		  
		 		 <tr>
		 		    <td>Line Coverage %</td>
				    <td>Package</td>
				 </tr>
		  		 
		  		 <xsl:apply-templates select="package/coverage[@type = 'line, %']">
		  		 	<xsl:sort data-type="number" select="number(substring-before(@value, '%'))"/>
		  		 </xsl:apply-templates>
		  		 
		  	      </table>
			</td>
		    </tr>
		  
		</table>
	</xsl:template>	
	
	<xsl:template match="coverage">
		<tr>
			
			<xsl:choose>
			     <xsl:when test="substring-before(@value, '%') &lt; 80">
				<td bgcolor="#FFCCCC" width="20%">
			   		<xsl:value-of select="substring-before(@value, '%')"/> 
				</td>
			     </xsl:when>
			     <xsl:otherwise>
				<td bgcolor="#ccffcc" width="20%">
					<xsl:value-of select="substring-before(@value, '%')"/> 
				</td>
			     </xsl:otherwise>
			</xsl:choose>
			
			<td bgcolor="#FFFFEE">
			   <xsl:value-of select="../@name"/>
			</td>

		</tr>
	</xsl:template>
</xsl:stylesheet>