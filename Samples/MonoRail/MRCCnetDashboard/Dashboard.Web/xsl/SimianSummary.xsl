<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns="http://schemas.microsoft.com/intellisense/ie5">

	<xsl:output method="html"/>

	<xsl:variable name="simian.root" select="//simian"/>
	<xsl:variable name="simian.version" select="$simian.root/@version" />

	<xsl:template match="/">

			<div id="simian-summary">
				<table class="section-table" cellSpacing="0" cellPadding="2" width="98%" border="0">
					<tr>
						<td class="sectionheader" colSpan="4">Simian <xsl:value-of select="$simian.version" /> Summary</td>
					</tr>
					<tr>
						<td><b><xsl:text>Configuration</xsl:text></b></td>
					</tr>
					<xsl:apply-templates select="$simian.root//check"/>					
				</table>
			</div>
	</xsl:template>
	
	<xsl:template match="check">
		<xsl:for-each select="./@*" >
			<tr>
				<td colspan="2" class="section-data">
					<xsl:value-of select="name()"/>
				</td>
				<td colspan="2" class="section-data">
					<xsl:value-of select="."/>
				</td>
			</tr>
		</xsl:for-each>
		<xsl:apply-templates select="./summary"/>					
	</xsl:template>
	
	<!-- Reports rules relating specifically to namespaces -->
	<xsl:template match="summary">
		<tr>
			<td><b><xsl:text>Results</xsl:text></b></td>
		</tr>
		<xsl:for-each select="./@*" >
			<tr>
				<td colspan="2" class="section-data">
					<xsl:value-of select="name()"/>
				</td>
				<td colspan="2" class="section-data">
					<xsl:value-of select="."/>
				</td>
			</tr>
		</xsl:for-each>
	</xsl:template>
	
</xsl:stylesheet>
