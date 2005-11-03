<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns="http://schemas.microsoft.com/intellisense/ie5">

	<xsl:output method="html"/>

	<xsl:variable name="fxcop.root" select="//FxCopReport"/>
	<xsl:variable name="fxcop.version" select="$fxcop.root/@Version" />
	<xsl:variable name="fxcop.lastAnalysis" select="$fxcop.root/@LastAnalysis"/>
	<xsl:variable name="message.list" select="$fxcop.root//Messages"/>

	<xsl:template match="/">
		<xsl:variable name="modifications.list" select="/cruisecontrol/modifications" />
		<xsl:variable name="modifications.list.count" select="count($modifications.list)" />
		<xsl:variable name="message.list.count" select="count($message.list)"/>
		<xsl:variable name="namespaces.list" select="$fxcop.root/Namespaces"/>
		<xsl:variable name="targets.list" select="$fxcop.root/Targets"/>

		<xsl:if test="($message.list.count > 0) and ($modifications.list.count > 0)">
			<div id="fxcop-summary">
				<script>
				function toggleRuleVisiblity(blockId)
				{
					var block = document.getElementById(blockId);
					var plus = document.getElementById(blockId + '.plus');
					if (block.style.display=='none') {
						block.style.display='block';
						plus.innerText='- ';
					} else {
						block.style.display='none';
						plus.innerText='+ ';
					}
				}
				</script>

				<table class="section-table" cellSpacing="0" cellPadding="2" width="98%" border="0">
					<tr>
						<td class="sectionheader" colSpan="4">FxCop <xsl:value-of select="$fxcop.version" /> Summary</td>
					</tr>
					<tr>
						<td><b>Target rules</b></td>
					</tr>
					<xsl:apply-templates select="$targets.list"/>					
					<tr>
						<td><b>Namespace rules</b></td>
					</tr>
					<xsl:apply-templates select="$namespaces.list" />
				</table>
			</div>
		</xsl:if>
	</xsl:template>
	
	<!-- Reports rules relating specifically to namespaces -->
	<xsl:template match="Namespace">
		<xsl:if test="@Name">
			<tr style="cursor:pointer">
				<xsl:attribute name="onClick">toggleRuleVisiblity('<xsl:value-of select="@Name"/>');</xsl:attribute>
				<td colspan="2" class="section-data">
					<span><xsl:attribute name="id"><xsl:value-of select="@Name"/>.plus</xsl:attribute>+ </span>
					<xsl:value-of select="@Name" />
					<div style="display: none; border: 1px solid gray">
						<xsl:attribute name="id"><xsl:value-of select="@Name"/></xsl:attribute>
						<xsl:apply-templates select="Messages">
							<xsl:sort select="Message//@Level" />
						</xsl:apply-templates>
					</div>
				</td>
			</tr>
		</xsl:if>
	</xsl:template>
	
	<xsl:template match="Target">
		<xsl:if test="@Name">
			<xsl:variable name="filename" select="translate(@Name, '\', '/')"/>
			<tr style="cursor:pointer">
				<xsl:attribute name="onClick">toggleRuleVisiblity('<xsl:value-of select="$filename"/>');</xsl:attribute>
				<td colspan="2" class="section-data">
					<span><xsl:attribute name="id"><xsl:value-of select="$filename"/>.plus</xsl:attribute>+ </span>
					<xsl:value-of select="@Name" />
					<div style="display: none; border: 1px solid gray">
						<xsl:attribute name="id"><xsl:value-of select="$filename"/></xsl:attribute>
						<xsl:apply-templates select="Messages">
							<xsl:sort select="Message//@Level" />
						</xsl:apply-templates>
						<xsl:apply-templates select="Modules"/>
					</div>
				</td>
			</tr>
		</xsl:if>	
	</xsl:template>
	
	<xsl:template match="Module">
		<xsl:apply-templates select="Namespaces//Classes">
			<xsl:sort select="Class"/>
		</xsl:apply-templates>
	</xsl:template>	
	
	<!-- Starts of the rules relating to class defined in the application -->
	<xsl:template match="Class">
		<div class="fxcop-class">
			<table style="WIDTH: 30pc">
				<tr>
					<td colspan="2" class="fxcop-class-header"><img src="images/class.gif"/><xsl:value-of select="@Name"/></td>
				</tr>

				<xsl:if test="Messages">
				<tr>
					<td>
					<xsl:apply-templates select="Messages">
						<xsl:sort select="Message//@Level"/>
					</xsl:apply-templates>
					</td>					
				</tr>
				</xsl:if>

				<xsl:if test="Fields">
					<tr>
						<xsl:apply-templates select="Fields">
							<xsl:sort select="Field" />
						</xsl:apply-templates>
					</tr>
				</xsl:if>		
				<xsl:if test="Properties">
					<tr>
						<xsl:apply-templates select="Properties">
							<xsl:sort select="Property"/>
						</xsl:apply-templates>
					</tr>
				</xsl:if>
				<xsl:apply-templates select="Methods" mode="fromClass">
					<xsl:sort select="Method"/>
				</xsl:apply-templates>
			</table>
		</div>		
	</xsl:template>
	
	<!-- Displays the fields.  These refer to variables at class level -->
	<xsl:template match="Field">
		<tr>
			<td width="20%"><img src="images/field.gif"/><xsl:value-of select="@Name"/></td>
			<td>
				<xsl:apply-templates select="Messages">
					<xsl:sort select="Message//@Level" />
				</xsl:apply-templates>
			</td>
		</tr>
	</xsl:template>
	
	<!-- Displays methods on the class -->
	<xsl:template match="Method" mode="fromClass">
		<tr>
			<td colspan="2"><img src="images/method.gif"/><xsl:value-of select="@Name"/></td>
		</tr>
		<tr>
			<td width="20%"></td>
			<td>
				<xsl:apply-templates select="Messages">
					<xsl:sort select="Message//@Level" />
				</xsl:apply-templates>
			</td>
		</tr>	
	</xsl:template>		
	
	<!-- Displays the summary of the property -->
	<xsl:template match="Property">
		<tr>
			<td colspan="3"><img src="images/property.gif"/><xsl:value-of select="@Name"/></td>
		</tr>
			<xsl:apply-templates select="Methods" mode="fromProperty">
				<xsl:sort select="@Name"/>
			</xsl:apply-templates>
	</xsl:template>
	
	<!-- Displays accessor methods for properties -->
	<xsl:template match="Method" mode="fromProperty">
		<tr>
			<td width="10%"></td>
			<td colspan="2"><xsl:value-of select="@Name"/></td>
		</tr>
		<tr>
			<td width="10%"></td>
			<td width="10%"></td>
			<td>
				<xsl:apply-templates select="Messages">
					<xsl:sort select="Message//@Level" />
				</xsl:apply-templates>
			</td>
		</tr>	
	</xsl:template>
	

		
	<xsl:template match="Message">
			<xsl:variable name="level" select=".//@Level" />
			<xsl:variable name="certainty" select=".//@Certainty" />
			<xsl:variable name="ruleName" select="Rule/@TypeName" />
			<xsl:variable name="sourceFile" select=".//SourceCode/@File" />
			<xsl:variable name="line" select=".//SourceCode/@Line" />
			<xsl:variable name="resolution" select=".//Resolution/Text" />

			<div class="section-data" style="margin-left:10px">
				<img>
					<xsl:if test="$level='CriticalError'">
						<xsl:attribute name="src">images/fxcop-critical-error.gif</xsl:attribute>
					</xsl:if>
					<xsl:if test="$level='Error'">
						<xsl:attribute name="src">images/fxcop-error.gif</xsl:attribute>
					</xsl:if>
					<xsl:if test="$level='Warning'">
						<xsl:attribute name="src">images/fxcop-warning.gif</xsl:attribute>
					</xsl:if>
					<xsl:if test="$level='CriticalWarning'">
						<xsl:attribute name="src">images/fxcop-critical-warning.gif</xsl:attribute>
					</xsl:if>
					<xsl:attribute name="alt"><xsl:value-of select="$level"/> (<xsl:value-of select="$certainty"/>% certainty)</xsl:attribute>
				</img>
				<a>
					<xsl:attribute name="href"><xsl:value-of select="$fxcop.root/Rules/Rule[@TypeName=$ruleName]/Url"/></xsl:attribute>
					<xsl:value-of select="$ruleName"/>
				</a>
				<xsl:if test=".//SourceCode/@Line">
					in <xsl:value-of select="$sourceFile"/>
				</xsl:if>
				<xsl:if test="$line > 0">
					(line: <xsl:value-of select="$line"/>)
				</xsl:if>
				<div style="margin-left:18px; margin-bottom:4px">
					<xsl:value-of select="$resolution"/>
				</div>
			</div>
	</xsl:template>

</xsl:stylesheet>
