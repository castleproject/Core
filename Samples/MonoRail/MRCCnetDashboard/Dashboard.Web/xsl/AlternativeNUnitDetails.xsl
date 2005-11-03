<?xml version="1.0" standalone="yes"?>
<xsl:stylesheet
	version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns="http://www.w3.org/TR/html4/strict.dtd"
	exclude-result-prefixes=""
    >

 	<xsl:output method="html"
    encoding="ISO-8859-1"
    standalone="yes"
    version="1.0"
    doctype-public="-//W3C//DTD HTML 4.01//EN"
    doctype-system="http://www.w3.org/TR/html4/strict.dtd"
  	indent="yes"/>


	<xsl:template match="/">
        <div id="report">
			<script type="text/javascript">
				function toggleDiv(imgId, divId)
				{
					eDiv = document.getElementById(divId);
					eImg = document.getElementById(imgId);

					if ( eDiv.style.display == "none" )
					{
						eDiv.style.display="block";
						eImg.src="images/arrow_minus_small.gif";
				 	}
					else
					{
						eDiv.style.display = "none";
						eImg.src="images/arrow_plus_small.gif";
					}
				}
				
				function toggleTr(imgId, trId)
				{
					eTr = document.getElementById(trId);
					eImg = document.getElementById(imgId);

					if ( eTr.style.display == "none" )
					{
						/* Setting a TR to display:block doesn't work in proper browsers
						but IE6's dodgy CSS implementation doesn't know table-row so
						we need to try...catch it */
						try
						{
							eTr.style.display="table-row";
						}
						catch(e)
						{
							eTr.style.display="block";
						}
						eImg.src="images/arrow_minus_small.gif";
				 	}
					else
					{
						eTr.style.display = "none";
						eImg.src="images/arrow_plus_small.gif";
					}
				}
			</script>

	        <style type="text/css">
				#master { font: small Verdana }
				#master table { border-collapse:collapse; table-layout: fixed;}
				#master tr { height: 25px; vertical-align: middle; }
				#master th { text-align: center; border-bottom: 1px solid black; border-top: 1px solid black;}
				#master div { box-sizing: content-box; -moz-box-sizing: content-box; }
				#master img { display: inline-block; vertical-align: middle; }

				.col1 { width: 100%; overflow: hidden; min-width: 190px;}
				.col2 { width: 350px; min-width: 170px; }
				.col3 { width: 60px; text-align: center; min-width: 40px; max-width: 40px; }

				th.col2 { border-right: 1px solid black; border-left: 1px solid black; -moz-box-sizing: content-box;}
				td.col2 div { margin: 0px; padding: 0px; float: left; height: 15px; }

				.testAssembly { border: 2px solid black; padding: 5px 2px 5px 2px; margin-bottom: 10px; min-width: 400px}
				.testAssembly table { width: 100%; min-width: 400px; }

				.testAssembly .testTable { margin: 0 auto; left: 25px; width: 95%; border: 1px solid gray;}
				.testTable td { border: 1px solid gray; overflow: hidden; padding: 2px 5px 2px 5px; background-color: inherit;}
				.testName { width: 40%; white-space: nowrap;}
				.testMsg { width: 60%; }
				.testMsg div { overflow: auto; width: 100%;}

				tr.succeeded { background-color: rgb(145,222,121);}
				tr.failed { background-color: rgb(245,147,147);}
				tr.ignored { background-color: rgb(252,242,80);}
				tr.unknown { background-color: gray;}

				div.succeeded { border: 1px solid green; background-color: rgb(59,216,23);}
				div.failed { border: 1px solid rgb(177,26,26); background-color: rgb(228,95,95);}
				div.ignored { border: 1px solid rgb(233,221,26); background-color: yellow;}
				div.unknown { border: 1px solid black; background-color: gray;}

				.category { padding-left: 21px; font-style: oblique; font-size: smaller;}
				.clickable { cursor: pointer; }
			</style>
			<xsl:for-each select="//test-results[test-suite]">
				<xsl:variable name="divId">
						<xsl:value-of select="generate-id(test-suite/@name)" />
				</xsl:variable>
				<div style="margin-bottom: 5px;">
					<xsl:attribute name="onclick">
						<xsl:text>toggleDiv('img-</xsl:text>
						<xsl:value-of select="$divId" />
						<xsl:text>','</xsl:text>
						<xsl:value-of select="$divId" />
						<xsl:text>')</xsl:text>
					</xsl:attribute>
					<xsl:attribute name="class">
						<xsl:text>clickable</xsl:text>
					</xsl:attribute>
					
					<img src="images/arrow_minus_small.gif" alt="Toggle display of Tests contained within this assembly">
						<xsl:attribute name="id">
							<xsl:text>img-</xsl:text>
							<xsl:value-of select="$divId" />
						</xsl:attribute>
					</img>
					<xsl:text>&#0160;</xsl:text>
					<strong>
						<xsl:call-template name="getSuiteName">
							<xsl:with-param name="name" select="test-suite/@name" />
						</xsl:call-template>
					</strong>
					<xsl:text>&#0160;-&#0160;</xsl:text>
					<xsl:value-of select="@total + @not-run"/>
					<xsl:text>&#0160;tests&#0160;(</xsl:text>
					<xsl:value-of select="@total - @failures" />
					<xsl:text>&#0160;passed,&#0160;</xsl:text>
					<xsl:value-of select="@failures" />
					<xsl:text>&#0160;failed,&#0160;</xsl:text>
					<xsl:value-of select="@not-run" />
					<xsl:text>&#0160;didn't run)</xsl:text>
				</div>
				<div class="testAssembly">
					<xsl:attribute name="id">
						<xsl:value-of select="$divId" />
					</xsl:attribute>
					<table>
						<tr>
							<th class="col1">Test Fixture</th>
							<th class="col2">Results</th>
							<th class="col3">Passes</th>
						</tr>
					<xsl:for-each select=".//test-suite[results/test-case]">
						<xsl:sort select="@name" order="ascending" data-type="text" />
						<xsl:variable name="testsId">
							<xsl:value-of select="generate-id(results/test-case/@name)" />
						</xsl:variable>
						<tr>
							<td class="col1">
								<span>
									<xsl:attribute name="onclick">
										<xsl:text>toggleTr('img-</xsl:text>
										<xsl:value-of select="$testsId" />
										<xsl:text>','</xsl:text>
										<xsl:value-of select="$testsId" />
										<xsl:text>')</xsl:text>
									</xsl:attribute>
									<xsl:attribute name="class">
										<xsl:text>clickable</xsl:text>
									</xsl:attribute>
									
									<img src="images/arrow_plus_small.gif" alt="Toggle display of the tests within this text fixture">
										<xsl:attribute name="id">
											<xsl:text>img-</xsl:text>
								 			<xsl:value-of select="$testsId" />
										</xsl:attribute>
									</img>
									<xsl:value-of select="@name"/>
								</span>
							</td>
							<td class="col2">
								<xsl:apply-templates select="." mode="graph"/>
							</td>
							<td class="col3">
								<xsl:call-template name="GetTests">
									<xsl:with-param name="CurrentSuite" select="." />
								</xsl:call-template>
							</td>
						</tr>
						<tr>
							<xsl:attribute name="id">
								<xsl:value-of select="$testsId" />
							</xsl:attribute>
							<xsl:attribute name="style">
								<xsl:text>display:none;</xsl:text>
							</xsl:attribute>							
							<td colspan="3">
								<xsl:apply-templates select="." mode="tests" />
							</td>
						</tr>
					</xsl:for-each>
					</table>
				</div>
            </xsl:for-each>
		</div>
	</xsl:template>

	<!-- Template for a particular TestFixture -->
	<xsl:template match="test-suite[results/test-case]" mode="tests">
		<table class="testTable">
			<xsl:for-each select="results/test-case[@success='False']">
				<xsl:sort select="@name" order="ascending" data-type="text"/>
				<xsl:apply-templates select="." mode="test" />
			</xsl:for-each>
			<xsl:for-each select="results/test-case[@executed='False']">
				<xsl:sort select="@name" order="ascending" data-type="text"/>
				<xsl:apply-templates select="." mode="test" />
			</xsl:for-each>
			<xsl:for-each select="results/test-case[@success='True']">
				<xsl:sort select="@name" order="ascending" data-type="text"/>
				<xsl:apply-templates select="." mode="test" />
			</xsl:for-each>
		</table>
	</xsl:template>

	<!-- Writes a line in the table for a particular test -->
	<xsl:template match="results/test-case" mode="test">
		<tr>
			<xsl:attribute name="class">
				<xsl:call-template name="GetClass">
					<xsl:with-param name="Executed" select="@executed" />
					<xsl:with-param name="Succeeded" select="@success" />
				</xsl:call-template>
			</xsl:attribute>
			
			<td class="testName">
				<xsl:call-template name="GetImage">
					<xsl:with-param name="Executed" select="@executed" />
					<xsl:with-param name="Succeeded" select="@success" />
				</xsl:call-template>
				<xsl:text>&#0160;</xsl:text>
				<xsl:call-template name="GetTestName">
					<xsl:with-param name="Name" select="@name"/>
				</xsl:call-template>
				<xsl:if test="categories">
					<xsl:for-each select="categories/category">
						<div class="category">
							<xsl:text>[</xsl:text>
							<xsl:value-of select="@name"/>
							<xsl:text>]</xsl:text>
						</div>
					</xsl:for-each>
				</xsl:if>
			</td>
			<td class="testMsg">
				<xsl:value-of select=".//message" />
				<xsl:if test=".//stack-trace">
					<br/>
					<div style="margin-top: 5px;">
						<xsl:value-of select=".//stack-trace" />
					</div>
				</xsl:if>
			</td>
		</tr>
	</xsl:template>
	
	<!-- Gets the class to use depending upon whether the test passed or failed -->
	<xsl:template name="GetClass">
		<xsl:param name="Executed" />
		<xsl:param name="Succeeded" />
			<xsl:choose>
				<xsl:when test="$Executed='False'">
					<xsl:text>ignored</xsl:text>
				</xsl:when>
				<xsl:when test="$Executed='True' and $Succeeded='False'">
					<xsl:text>failed</xsl:text>
				</xsl:when>
				<xsl:when test="$Executed='True' and $Succeeded='True'">
					<xsl:text>succeeded</xsl:text>
				</xsl:when>
				<xsl:otherwise>
					<xsl:text>unknown</xsl:text>
				</xsl:otherwise>
			</xsl:choose>
	</xsl:template>

	<!-- Gets the image tag to show success or failure -->	
	<xsl:template name="GetImage">
		<xsl:param name="Executed" />
		<xsl:param name="Succeeded" />
		
		<xsl:choose>
			<xsl:when test="$Executed='False'">
				<img src="images/fxcop-error.gif" alt="The test wasn't run" width="16" height="16" />
			</xsl:when>
			<xsl:when test="$Executed='True' and $Succeeded='False'">
				<img src="images/fxcop-critical-error.gif" alt="The test failed" width="16" height="16"/>
			</xsl:when>
			<xsl:when test="$Executed='True' and $Succeeded='True'">
				<img src="images/check.jpg" alt="The test suceeded" width="16" height="16"/>
			</xsl:when>
		</xsl:choose>
	</xsl:template>
	
	<!-- Template to draw a graph for a particular TestFixture -->
	<xsl:template match="test-suite[results/test-case]" mode="graph">
		<xsl:variable name="numTests">
			<xsl:call-template name="GetNumberOfTests">
				<xsl:with-param name="CurrentSuite" select="." />
			</xsl:call-template>
		</xsl:variable>
		<xsl:for-each select="results/test-case">
			<xsl:sort select="@executed" order="descending" data-type="text"/>
			<xsl:sort select="@success" order="descending" data-type="text" />
			<div>
				<xsl:attribute name="style">
					<xsl:text>width:</xsl:text>
					<xsl:value-of select="(350 - ($numTests * 2)) div $numTests"/>
					<xsl:text>px;</xsl:text>
				</xsl:attribute>
				<xsl:attribute name="class">
					<xsl:call-template name="GetClass">
						<xsl:with-param name="Executed" select="@executed"/>
						<xsl:with-param name="Succeeded" select="@success"/>
					</xsl:call-template>
				</xsl:attribute>
				<xsl:text>&#0160;</xsl:text>
			</div>
		</xsl:for-each>
	</xsl:template>
	
	<!-- Calculates the total number of tests in a particular testfixture -->
	<xsl:template name="GetNumberOfTests">
		<xsl:param name="CurrentSuite" />
		<xsl:value-of select="count($CurrentSuite/results/test-case)"/>
	</xsl:template>

	<!-- Gets the text for the third column -->
	<xsl:template name="GetTests">
		<xsl:param name="CurrentSuite" />

		<xsl:text>(</xsl:text>
		<xsl:value-of select="count($CurrentSuite/results/test-case[@success='True'])"/>
		<xsl:text>/</xsl:text>
		<xsl:value-of select="count($CurrentSuite/results/test-case)"/>
		<xsl:text>)</xsl:text>
	</xsl:template>

	<!-- Takes a full path and returns the filename -->
	<xsl:template name="getSuiteName">
		<xsl:param name="name"/>
		<xsl:choose>
			<xsl:when test="contains($name, '\')">
				<xsl:call-template name="getSuiteName">
					<xsl:with-param name="name" select="substring-after($name, '\')"/>
				</xsl:call-template>
			</xsl:when>
			<xsl:when test="contains($name, '/')">
				<xsl:call-template name="getSuiteName">
					<xsl:with-param name="name" select="substring-after($name, '/')"/>
				</xsl:call-template>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="$name"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	
	<!-- Takes a fully qualified test name and returns just the name of the test -->
	<xsl:template name="GetTestName">
		<xsl:param name="Name"/>
		<xsl:choose>
			<xsl:when test="contains($Name, '.')">
				<xsl:call-template name="GetTestName">
					<xsl:with-param name="Name" select="substring-after($Name, '.')"/>
				</xsl:call-template>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="$Name"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
</xsl:stylesheet>
