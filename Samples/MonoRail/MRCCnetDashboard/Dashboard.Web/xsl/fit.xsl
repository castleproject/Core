<?xml version="1.0"?>
<xsl:stylesheet
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">

    <xsl:output method="html"/>
    
    <xsl:variable name="hrefnode" select="/cruisecontrol/build/buildresults//task[@name='fit']/message[starts-with(text(), 'results')]"/>
    <xsl:variable name="resultsnode" select="/cruisecontrol/build/buildresults//task[@name='fit']/message[contains(text(), 'fixtures run')]"/>
    <xsl:variable name="ahref" select="substring-after($hrefnode, ':')"/>

    <xsl:template match="/">
    	<xsl:if test="count($hrefnode) > 0">
    		<table cellpadding="2" cellspacing="0" border="0" width="98%">
    		<tr>
				<td class="sectionheader" colspan="2">
				     Acceptance Tests: <xsl:value-of select="$resultsnode"/>
			    </td>
			    
            </tr>
            <tr>
            <td><a href="{$ahref}">test results</a></td>            
            </tr>
    		</table>
    	</xsl:if>
    </xsl:template>
</xsl:stylesheet>