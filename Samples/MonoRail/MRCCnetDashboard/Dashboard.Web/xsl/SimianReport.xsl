<?xml version="1.0"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns="http://www.w3.org/TR/xhtml1/strict">
	<xsl:output method="html"/>

	<xsl:variable name="simian.root" select="//simian"/>
	<xsl:variable name="simian.version" select="$simian.root/@version" />

	<xsl:template match="/">

			<div id="simian-report">
				<table class="section-table" cellSpacing="0" cellPadding="2" width="98%" border="0">
					<tr>
						<td class="sectionheader" colSpan="4">Simian <xsl:value-of select="$simian.version" /> Report</td>
					</tr>
					<xsl:apply-templates select="$simian.root//check"/>					
				</table>
			</div>
	</xsl:template>
	
	<xsl:template match="check">
		<tr>
			<td><b><xsl:text>Configuration</xsl:text></b></td>
		</tr>
		<xsl:for-each select="./@*" >
			<tr>
				<td class="section-data">
					<xsl:value-of select="name()"/>
				</td>
				<td class="section-data">
					<xsl:value-of select="."/>
				</td>
				<td/>
				<td/>
			</tr>
		</xsl:for-each>
		<tr>
			<td><b><xsl:text>Results</xsl:text></b></td>
		</tr>
		<xsl:apply-templates select="./set"/>					
		<tr>
			<td><b><xsl:text>Summary</xsl:text></b></td>
		</tr>
		<xsl:apply-templates select="./summary"/>					
	</xsl:template>
	
	<xsl:template match="set">
		<xsl:for-each select="./@*" >
			<tr>
				<td class="section-data">
					<xsl:value-of select="name()"/>
				</td>
				<td class="section-data">
					<xsl:value-of select="."/>
				</td>
				<td/>
				<td/>
			</tr>
		</xsl:for-each>
		<xsl:for-each select="./block" >
			<tr>
				<td class="section-data">
					<xsl:value-of select="./@sourceFile"/>
				</td>
				<td class="section-data">
					<xsl:value-of select="./@startLineNumber"/>
				</td>
				<td class="section-data">
					<xsl:value-of select="./@endLineNumber"/>
				</td>
				<td/>
			</tr>
		</xsl:for-each>
		<tr>
			<td colspan="4">
				<hr size="1" width="100%" color="#888888"/>
			</td>
		</tr>
	</xsl:template>
	
	<xsl:template match="summary">
		<xsl:for-each select="./@*" >
			<tr>
				<td class="section-data">
					<xsl:value-of select="name()"/>
				</td>
				<td class="section-data">
					<xsl:value-of select="."/>
				</td>
				<td/>
				<td/>
			</tr>
		</xsl:for-each>
	</xsl:template>
	
</xsl:stylesheet>
