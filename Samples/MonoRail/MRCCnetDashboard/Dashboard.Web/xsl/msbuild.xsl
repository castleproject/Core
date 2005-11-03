<?xml version="1.0"?>
<!DOCTYPE xsl:stylesheet [
	<!ENTITY nbsp "&#160;">
]>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="html"/>

	<xsl:template match="/">
		<xsl:variable name="buildresults" select="//msbuild" />
		<xsl:choose>
			<xsl:when test="count($buildresults) > 0">
				<xsl:apply-templates select="$buildresults" />
			</xsl:when>
			<xsl:otherwise>
				<h2>Log does not contain any XML output from MSBuild.</h2>
				<p>
					Please make sure that MSBuild is executed using the XmlLogger
					(use the argument: <b style="white-space:nowrap">/logger:Kobush.Build.Logging.XmlLogger,Kobush.Build.dll</b>).
				</p>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template match="msbuild">
		<table class="section-table" cellpadding="2" cellspacing="0" border="0" width="98%">
			<tr>
				<td class="sectionheader">
					Build started <xsl:value-of select="@startTime"/>
				</td>
			</tr>
			<tr>
				<td>
					<xsl:apply-templates/>
				</td>
			</tr>
			<tr>
				<td class="sectionheader">
					<xsl:choose>
						<xsl:when test="@success = 'false'">
							Build FAILED
						</xsl:when>
						<xsl:otherwise>
							Build succeeded
						</xsl:otherwise>
					</xsl:choose>
				</td>
			</tr>
			<tr>
				<td>
					<xsl:variable name="errors" select="//error" />
					<xsl:variable name="warnings" select="//warning" />
					<xsl:if test="count($errors)+count($warnings) > 0">
						<xsl:apply-templates select="$errors"/>
						<xsl:apply-templates select="$warnings"/>
						<br/>
						<div style="color:orangered">
							<xsl:value-of select="count($errors)"/> Error(s)
						</div>
						<div style="color:gold">
							<xsl:value-of select="count($warnings)"/> Warning(s)
						</div>
						<br/>
					</xsl:if>
					Time elapsed <xsl:value-of select="@elapsedTime"/>
				</td>
			</tr>
		</table>
	</xsl:template>

	<xsl:template match="project">
		<div style="color:dodgerblue;margin:4 0">
			Project "<xsl:value-of select="@file"/>"
			<xsl:choose></xsl:choose>
			<xsl:choose>
				<xsl:when test="@name">
					(<xsl:value-of select="@name"/> target(s)):
				</xsl:when>
				<xsl:otherwise>
					(default targets):
				</xsl:otherwise>
			</xsl:choose>
		</div>
		<xsl:apply-templates/>
		<xsl:if test="@success = 'false'">
			<div style="color:dodgerblue;margin:2 0">
				Done building project "<xsl:call-template name="projectName">
					<xsl:with-param name="string">
						<xsl:value-of select="@file" />
					</xsl:with-param>
				</xsl:call-template>" -- FAILED.
			</div>
		</xsl:if>
	</xsl:template>

	<xsl:template name="projectName">
		<xsl:param name="string" />
		<xsl:choose>
			<xsl:when test="contains($string, '\')">
				<xsl:call-template name="projectName">
					<xsl:with-param name="string">
						<xsl:value-of select="substring-after($string, '\')" />
					</xsl:with-param>
				</xsl:call-template>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="$string" />
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template match="target">
		<div style="color:dodgerblue;margin:2 0">
			Target "<xsl:value-of select="@name"/>":
		</div>
		<xsl:if test="*">
			<div style="margin-left:10px;padding-left:10;border-left: 1px solid silver">
				<xsl:apply-templates />
			</div>
		</xsl:if>
		<xsl:if test="@success = 'false' and count(*//target/@success[.='false']) = 0">
			<div style="color:dodgerblue;margin:2 0">
				Done building target "<xsl:value-of select="@name"/>" -- FAILED.
			</div>
		</xsl:if>
	</xsl:template>

	<xsl:template match="task">
		<div style="color:dodgerblue;margin:2 0">
			Task "<xsl:value-of select="@name"/>":
		</div>
		<xsl:if test="*">
			<div style="margin-left:10px;padding-left:10;border-left: 1px solid silver">
				<xsl:apply-templates />
			</div>
		</xsl:if>
		<div style="color:dodgerblue;margin:2 0">
			Done executing task "<xsl:value-of select="@name"/>"
			<xsl:if test="@success = 'false'">-- FAILED</xsl:if>.
		</div>
	</xsl:template>

	<xsl:template match="message">

		<xsl:choose>
			<xsl:when test="@level = 'low'">
				<div style="color:silver">
					<xsl:value-of select="text()"/>
				</div>
			</xsl:when>
			<xsl:when test="@level = 'normal'">
				<div style="color:darkgray">
					<xsl:value-of select="text()"/>
				</div>
			</xsl:when>
			<xsl:otherwise>
				<div style="color:dimgray">
					<xsl:value-of select="text()"/>
				</div>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template match="error">
		<div style="color:orangered">
			<xsl:if test="@file != ''" >
				<xsl:value-of select="@file"/>&nbsp;(<xsl:value-of select="@line"/>,<xsl:value-of select="@column"/>):&nbsp;
			</xsl:if>
			error&nbsp;<xsl:value-of select="@code"/>:&nbsp;<xsl:value-of select="text()" />
		</div>
	</xsl:template>

	<xsl:template match="warning">
		<div style="color:gold">
			<xsl:if test="@file != ''" >
				<xsl:value-of select="@file"/>&nbsp;(<xsl:value-of select="@line"/>,<xsl:value-of select="@column"/>):&nbsp;
			</xsl:if>
			warning&nbsp;<xsl:value-of select="@code"/>:&nbsp;<xsl:value-of select="text()" />
		</div>
	</xsl:template>
</xsl:stylesheet>
