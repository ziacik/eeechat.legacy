<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet	version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:eee="http://tempuri.org/EeeDataSet.xsd">
	<xsl:output method="xml" />

	<xsl:template match="/eee:EeeDataSet">
		<root>
			<xsl:for-each select="eee:Message">
				<message messageId="{eee:MessageID}">
                    <div style="font-family:Verdana; font-size:11px; padding-top: 2px; padding-bottom: 2px;">
                        <div style="background-color:#E7F3F1;">
                            <b>
                                <font color="gray">
                                    <font color="#{eee:ColorHex}">
                                        <xsl:value-of select="eee:Login" />
                                    </font>
                                    <xsl:value-of select="concat(' ', substring(eee:Time,12,5))" />
                                    <font color="blue">
                                        <xsl:value-of select="concat(' ', eee:Room)" />
                                    </font>
                                </font>
                            </b>
                        </div>
                        <div style="font-family:Verdana; font-size:11px; padding-bottom: 10px;">
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