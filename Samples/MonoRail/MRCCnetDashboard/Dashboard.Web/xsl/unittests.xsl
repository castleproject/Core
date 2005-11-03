<?xml version="1.0"?>
<xsl:stylesheet
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">

    <xsl:output method="html"/>

    <xsl:variable name="nunit2.result.list" select="//test-results"/>
    <xsl:variable name="nunit2.testcount" select="sum($nunit2.result.list/@total)"/>
    <xsl:variable name="nunit2.failures" select="sum($nunit2.result.list/@failures)"/>
    <xsl:variable name="nunit2.notrun" select="sum($nunit2.result.list/@not-run)"/>
    <xsl:variable name="nunit2.time" select="sum($nunit2.result.list/test-suite[position()=1]/@time)"/>
    <xsl:variable name="nunit2.case.list" select="$nunit2.result.list//test-case"/>
    <xsl:variable name="nunit2.suite.list" select="$nunit2.result.list//test-suite"/>
    <xsl:variable name="nunit2.failure.list" select="$nunit2.case.list//failure"/>
    <xsl:variable name="nunit2.notrun.list" select="$nunit2.case.list//reason"/>

    <xsl:variable name="testsuite.list" select="/cruisecontrol/build/buildresults//testsuite"/>
    <xsl:variable name="testcase.list" select="$testsuite.list/testcase"/>
    <xsl:variable name="testcase.error.list" select="$testcase.list/error"/>
    <xsl:variable name="testsuite.error.count" select="count($testcase.error.list)"/>
    <xsl:variable name="testcase.failure.list" select="$testcase.list/failure"/>
    <xsl:variable name="totalErrorsAndFailures" select="count($testcase.error.list) + count($testcase.failure.list) + count($nunit2.failure.list)"/>

    <xsl:variable name="nunit.tests.present" select="count(//test-results) > 0 or count(/cruisecontrol/build/buildresults//testsuite) > 0" />
    <xsl:variable name="junit.tests.present" select="count(//cruisecontrol//testsuite) > 0" />

    <xsl:template match="/">

    <xsl:choose>
      <xsl:when test="$nunit.tests.present">  
        <table class="section-table" cellpadding="2" cellspacing="0" border="0" width="98%">

            <!-- Unit Tests -->
            <tr>
                <td class="sectionheader" colspan="2">
                   Tests run: <xsl:value-of select="$nunit2.testcount"/>, Failures: <xsl:value-of select="$nunit2.failures"/>, Not run: <xsl:value-of select="$nunit2.notrun"/>, Time: <xsl:value-of select="$nunit2.time"/> seconds
                </td>
            </tr>

            <xsl:choose>
                <xsl:when test="count($testsuite.list) + count($nunit2.suite.list) = 0">
                    <tr><td colspan="2" class="section-data">No Tests Run</td></tr>
                    <tr><td colspan="2" class="section-error">This project doesn't have any tests</td></tr>
                </xsl:when>

                <xsl:when test="$totalErrorsAndFailures = 0">
                    <tr><td colspan="2" class="section-data">All Tests Passed</td></tr>
                </xsl:when>
            </xsl:choose>

            <xsl:apply-templates select="$testcase.error.list"/>
            <xsl:apply-templates select="$testcase.failure.list | $nunit2.failure.list"/>
            <xsl:apply-templates select="$nunit2.notrun.list"/>

            <tr><td colspan="2"> </td></tr>

            <xsl:if test="$totalErrorsAndFailures > 0">
                <tr>
                    <td class="sectionheader" colspan="2">
                        Unit Test Failure and Error Details (<xsl:value-of select="$totalErrorsAndFailures"/>)
                    </td>
                </tr>

                <!-- (PENDING) Why doesn't this work if set up as variables up top? -->
                <xsl:call-template name="testdetail">
                    <xsl:with-param name="detailnodes" select="//testsuite/testcase[.//error]"/>
                </xsl:call-template>

                <xsl:call-template name="testdetail">
                    <xsl:with-param name="detailnodes" select="//testsuite/testcase[.//failure]"/>
                </xsl:call-template>
                
                <xsl:call-template name="nunit2testdetail">
                    <xsl:with-param name="detailnodes" select="//test-suite/results/test-case[.//failure]"/>
                </xsl:call-template>

                <tr><td colspan="2"> </td></tr>
            </xsl:if>
            
            <xsl:if test="$nunit2.notrun > 0">
                <tr>
                    <td class="sectionheader" colspan="2">
                        Warning Details (<xsl:value-of select="$nunit2.notrun"/>)
                    </td>
                </tr>
                <!-- (PENDING) Why doesn't this work if set up as variables up top? -->
                <xsl:call-template name="nunit2testdetail">
                    <xsl:with-param name="detailnodes" select="//test-suite/results/test-case[.//reason]"/>
                </xsl:call-template>
                <tr><td colspan="2"> </td></tr>
            </xsl:if>
        </table>
      </xsl:when>
    
      <xsl:when test="$junit.tests.present">  
          <xsl:variable name="testsuite.list" select="cruisecontrol//testsuite"/>
          <xsl:variable name="testsuite.error.count" select="count($testsuite.list/error)"/>
          <xsl:variable name="testcase.list" select="$testsuite.list/testcase"/>
          <xsl:variable name="testcase.error.list" select="$testcase.list/error"/>
          <xsl:variable name="testcase.failure.list" select="$testcase.list/failure"/>
          <xsl:variable name="totalErrorsAndFailures" select="count($testcase.error.list) + count($testcase.failure.list)"/>

      
      	<table align="center" cellpadding="2" cellspacing="0" border="0" width="100%">
      
      	    <!-- Unit Tests -->
      	    <tr>
      		<td class="sectionheader" colspan="2">
      		   Tests run: <xsl:value-of select="count($testcase.list)"/>,
      		   &#160;Failures: <xsl:value-of select="$totalErrorsAndFailures"/>,
      		   &#160;Time: <xsl:value-of select="sum($testcase.list/@time)"/> seconds
      		</td>
      	    </tr>
            
            <xsl:choose>
		<xsl:when test="count($testsuite.list) = 0">
		    <tr>
			<td colspan="2" class="section-data">
			    No Tests Run
			</td>
		    </tr>
		    <tr>
			<td colspan="2" class="section-error">
			    This project doesn't have any tests
			</td>
		    </tr>
		</xsl:when>

		<xsl:when test="$totalErrorsAndFailures = 0">
		    <tr>
			<td colspan="2" class="section-data">
			    All Tests Passed
			</td>
		    </tr>
		</xsl:when>
	    </xsl:choose>
      
      	    <xsl:apply-templates select="$testcase.error.list"/>
      	    <xsl:apply-templates select="$testcase.failure.list"/>
      	    <tr/>
      	    <tr><td colspan="2">&#160;</td></tr>
      
      	    <xsl:if test="$totalErrorsAndFailures > 0">
      
      	      <tr>
      		<td class="sectionheader" colspan="2">
      		    &#160;Unit Test Failure and Error Details:&#160;(<xsl:value-of select="$totalErrorsAndFailures"/>)
      		</td>
      	      </tr>
      
      	      <!-- (PENDING) Why doesn't this work if set up as variables up top? -->
      	      <xsl:call-template name="testdetail">
      		<xsl:with-param name="detailnodes" select="//testsuite/testcase[.//error]"/>
      	      </xsl:call-template>
      
      	      <xsl:call-template name="testdetail">
      		<xsl:with-param name="detailnodes" select="//testsuite/testcase[.//failure]"/>
      	      </xsl:call-template>
      
      
      	      <tr><td colspan="2">&#160;</td></tr>
      	    </xsl:if>
      	</table>

      </xsl:when>
      
    </xsl:choose>    
    </xsl:template>

    <!-- UnitTest Errors -->
    <xsl:template match="error">
        <tr>
            <xsl:if test="position() mod 2 = 0">
                <xsl:attribute name="class">section-oddrow</xsl:attribute>
            </xsl:if>
            <td class="section-data">Error</td>
            <td class="section-data"><xsl:value-of select="../@name"/></td>
        </tr>
    </xsl:template>

    <!-- UnitTest Failures -->
    <xsl:template match="failure">
        <tr>
            <xsl:if test="($testsuite.error.count + position()) mod 2 = 0">
                <xsl:attribute name="class">section-oddrow</xsl:attribute>
            </xsl:if>
            <td class="section-data">Failure</td>
            <td class="section-data"><xsl:value-of select="../@name"/></td>
        </tr>
    </xsl:template>

    <!-- UnitTest Warnings -->
    <xsl:template match="reason">
        <tr>
            <xsl:if test="($totalErrorsAndFailures + position()) mod 2 = 0">
                <xsl:attribute name="class">section-oddrow</xsl:attribute>
            </xsl:if>
            <td class="section-data">Warning</td>
            <td class="section-data"><xsl:value-of select="../@name"/></td>
        </tr>
    </xsl:template>

    <!-- UnitTest Errors And Failures Detail Template -->
    <xsl:template name="testdetail">
      <xsl:param name="detailnodes"/>

      <xsl:for-each select="$detailnodes">

        <xsl:if test="position() > 1">
        <tr><td colspan="2"><hr size="1" width="100%" color="#888888"/></td></tr>
        </xsl:if>

        <tr><td class="section-data">Test:</td><td class="section-data"><xsl:value-of select="@name"/></td></tr>
       
        <xsl:if test="error">
        <tr><td class="section-data">Type:</td><td class="section-data"><xsl:value-of select="error/@type"/></td></tr>
        <tr><td class="section-data">Message:</td><td class="section-data"><xsl:value-of select="error/@message"/></td></tr>
        <tr>
            <td></td>
            <td class="section-error">
                <pre><xsl:call-template name="br-replace">
                        <xsl:with-param name="word" select="error"/>
                    </xsl:call-template></pre>
            </td>
        </tr>
        </xsl:if>

        <xsl:if test="failure">
        <tr><td class="section-data">Type:</td><td class="section-data"><xsl:value-of select="failure/@type"/></td></tr>
        <tr><td class="section-data">Message:</td><td class="section-data"><xsl:value-of select="failure/@message"/></td></tr>
        <tr>
            <td></td>
            <td class="section-error">
                <pre><xsl:call-template name="br-replace">
                        <xsl:with-param name="word" select="failure"/>
                    </xsl:call-template></pre>
            </td>
        </tr>
        </xsl:if>
        
      </xsl:for-each>
    </xsl:template>

    <!-- UnitTest Errors And Failures Detail Template -->
    <xsl:template name="nunit2testdetail">
        <xsl:param name="detailnodes"/>

        <xsl:for-each select="$detailnodes">
        
            <xsl:if test="position() > 1">
            <tr><td colspan="2"><hr size="1" width="100%" color="#888888"/></td></tr>
            </xsl:if>

            <xsl:if test="failure">
            <tr><td class="section-data">Test:</td><td class="section-data"><xsl:value-of select="@name"/></td></tr>
            <tr><td class="section-data">Type:</td><td class="section-data">Failure</td></tr>
            <tr><td class="section-data">Message:</td><td class="section-data"><xsl:value-of select="failure//message"/></td></tr>
            <tr>
                <td></td>
                <td class="section-error">
                    <pre><xsl:value-of select="failure//stack-trace"/></pre>
                </td>
            </tr>
            </xsl:if>

            <xsl:if test="reason">
            <tr><td class="section-data">Test:</td><td class="section-data"><xsl:value-of select="@name"/></td></tr>
            <tr><td class="section-data">Type:</td><td class="section-data">Warning</td></tr>
            <tr><td class="section-data">Message:</td><td class="section-data"><xsl:value-of select="reason//message"/></td></tr>
            </xsl:if>

        </xsl:for-each>
    </xsl:template>

    <xsl:template name="br-replace">
        <xsl:param name="word"/>
        <xsl:variable name="cr"><xsl:text>
        <!-- </xsl:text> on next line on purpose to get newline -->
        </xsl:text></xsl:variable>
        <xsl:choose>
            <xsl:when test="contains($word,$cr)">
                <xsl:value-of select="substring-before($word,$cr)"/>
                <br/>
                <xsl:call-template name="br-replace">
                    <xsl:with-param name="word" select="substring-after($word,$cr)"/>
                </xsl:call-template>
            </xsl:when>
            <xsl:otherwise>
                <xsl:value-of select="$word"/>
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>

</xsl:stylesheet>
