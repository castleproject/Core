<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" 
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
	xmlns:context="urn:castle">
	
	<xsl:output method="xml" omit-xml-declaration="yes" indent="no"/>

	<xsl:template match="define">
		<xsl:value-of select=" context:Add( string(@flag) ) " />
	</xsl:template>

	<xsl:template match="undef">
		<xsl:value-of select=" context:Remove( string(@flag) ) "/>
	</xsl:template>

	<xsl:template match="if">
		<xsl:choose>
			<xsl:when test=" @defined and @not-defined ">
				<xsl:message terminate="yes">'if' element cannot have both defined, and not-defined attributes</xsl:message>
			</xsl:when>
			<xsl:when test=" @defined ">
				<xsl:if test=" boolean( context:Contains( string(@defined) ) ) " >
					<xsl:apply-templates />
				</xsl:if>
			</xsl:when>
			<xsl:when test=" @not-defined ">
				<xsl:if test=" not( boolean( context:Contains( string(@not-defined) ) ) ) " >
					<xsl:apply-templates />
				</xsl:if>
			</xsl:when>
			<xsl:otherwise>
				<xsl:message terminate="yes">'if' element expects either defined or not-defined attribute</xsl:message>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template match="choose">
		<xsl:choose>
			<xsl:when test="when">
				<xsl:variable name="node" select=" context:ProcessChoose( * ) "/>
				
				<xsl:if test=" $node ">
					<xsl:apply-templates select=" $node/node() " />
				</xsl:if>
			</xsl:when>
			<xsl:otherwise>
				<xsl:message terminate="yes">'choose' element expects at least one 'when' child element</xsl:message>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template match="*">
		<xsl:element name="{ name() }">
			<xsl:copy-of select="@*" />
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>

<!--	<xsl:template match="text()" priority="-1">
		<xsl:value-of select="."/>
	</xsl:template>-->
</xsl:stylesheet>

  
