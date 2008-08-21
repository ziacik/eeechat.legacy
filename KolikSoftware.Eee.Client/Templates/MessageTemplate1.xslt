<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet	version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:eee="http://tempuri.org/EeeDataSet.xsd">
	<xsl:output method="xml" />

	<xsl:template match="/eee:EeeDataSet">
		<root>
			<xsl:for-each select="eee:Message">
				<xsl:variable name="bgColor">
					<xsl:if test="eee:MessageNo mod 2 = 0">#FFFFFF</xsl:if>
					<xsl:if test="eee:MessageNo mod 2 = 1">#EFEFEF</xsl:if>
				</xsl:variable>

                <message messageId="{eee:MessageID}">
                    <div style="background-color:{$bgColor}; padding-bottom: 5px; padding-top: 5px; font-family: Arial;">
                        <div>
                            <b>
                                <font color="gray">
                                    &lt;
                                    <font color="#{eee:ColorHex}">
                                        <xsl:value-of select="eee:Login" />
                                    </font>
                                    <xsl:value-of select="concat(' ', substring(eee:Time,12,5))" />
                                    <xsl:value-of select="concat(' ', eee:Room)" />
                                    &gt;
                                </font>
                            </b>
                        </div>
                        <div>
                            <font color="#{eee:ColorHex}">
                                <xsl:choose>
                                    <xsl:when test="eee:ToUserID != 0">
                                        <i>
                                            <xsl:copy-of select="eee:Message" />
                                        </i>
                                    </xsl:when>
                                    <xsl:otherwise>
                                        <xsl:copy-of select="eee:Message" />
                                    </xsl:otherwise>
                                </xsl:choose>
                            </font>
                        </div>
                    </div>
                    
                </message>
			</xsl:for-each>
		</root>
	</xsl:template>

</xsl:stylesheet>