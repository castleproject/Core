<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:fn="http://www.w3.org/2004/07/xpath-functions" xmlns:xdt="http://www.w3.org/2004/07/xpath-datatypes">

        <xsl:output method="html"/>
        <xsl:variable name="VIL.root" select="//MetricReport"/>
<xsl:template match="/">
    <html>
    <head><title>Metrics Report</title></head>
    <style>
        .Title {font-family: Verdana; font-size: 14pt; color: black; font-weight: bold}
        .ListItem {font-family: Verdana; font-size: 12pt; color: black; font-weight: bold}
        .ColumnHeader {font-family: Verdana; font-size: 7pt; background-color:white; color: black}
        .Information {font-family: Verdana; font-size: 10pt; color: black; font-weight: bold; text-align: left}
    </style>
 
    <body bgcolor="white" alink="Black" vlink="Black" link="Black">
        <xsl:apply-templates select="$VIL.root"/>
        </body>
        </html>
</xsl:template>

  <xsl:template match="MetricReport">
    <div class="Title">Metric report</div>
    <xsl:apply-templates select="Specification"/>
    <xsl:apply-templates select="Results"/>
</xsl:template> 

  <xsl:template match="Specification">
                <br/>
      <xsl:apply-templates select="Assemblies"/>
</xsl:template> 

  <xsl:template match="Results">
  <br/>
   <xsl:apply-templates select="CodeElement"/>
  </xsl:template> 
 
  <xsl:template match="CodeElement">
  <div class="ListItem"><xsl:value-of select="@name"/></div>
  <table border="1" cellspacing="0" width="100%">
      <tr>
   <xsl:for-each select="Metric">
        <th class="ColumnHeader"><xsl:value-of select="@label"/></th>
      </xsl:for-each>
     </tr>
     <tr>
        <xsl:for-each select="Metric">
        <td><xsl:value-of select="."/></td>
      </xsl:for-each>
        </tr>
        </table>
  </xsl:template> 

 
  <xsl:template match="Assemblies">
   <table border="0">
    <tr >
      <th class="ColumnHeader" align="left">Assemblies:</th>
    </tr>
     <xsl:apply-templates select="Assembly"/>
    </table>
</xsl:template> 

 <xsl:template match="Assembly">
    <tr>
      <td><xsl:value-of select="."/></td>
    </tr>
</xsl:template>   
         
</xsl:stylesheet>