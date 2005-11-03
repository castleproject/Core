<?xml version="1.0"?>
<xsl:stylesheet
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">

    <xsl:output method="html"/>
    
	<xsl:variable name="testcase.list" select="//test-case"/>
	
    <xsl:template match="/">
        <table cellpadding="2" cellspacing="0" border="0" width="98%">

            <!-- Unit Tests -->
            <tr>
                <td class="sectionheader" colspan="2">
                    Test Timing
                </td>
            </tr>

            <xsl:apply-templates select="$testcase.list">
                <xsl:sort select="@time" order="descending" data-type="number" />
            </xsl:apply-templates>

            <tr><td colspan="2"> </td></tr>
        </table>
    </xsl:template>

    <xsl:template match="test-case">
        <tr>
            <xsl:if test="position() mod 2 = 0">
                <xsl:attribute name="class">unittests-oddrow</xsl:attribute>
            </xsl:if>
            <td class="section-data">
                <xsl:value-of select="./@name"/>
            </td>
            <td class="section-data">
            	<xsl:choose>
            		<xsl:when test="@executed[.='True']">
            		    <xsl:value-of select="./@time"/>
            		</xsl:when>
            		<xsl:otherwise>
            			not run
            		</xsl:otherwise>
            	</xsl:choose>
            </td>
        </tr>
    </xsl:template>

</xsl:stylesheet>

  