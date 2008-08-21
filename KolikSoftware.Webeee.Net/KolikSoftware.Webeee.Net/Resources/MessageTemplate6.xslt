<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet	version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:eee="http://tempuri.org/EeeDataSet.xsd">
	<xsl:output method="xml" />

	<xsl:template match="/eee:EeeDataSet">
		<root>
			<xsl:for-each select="eee:Message">
                <message messageId="{eee:MessageID}">
                    <div style="font-family:Lucida Sans; font-size:10px; padding-top: 5px; padding-bottom: 15px;">
                        <div style="color:#F0F0F0; width: 1%; float:left; border: 1px #{eee:ColorHex} solid; background-color: #{eee:ColorHex}; filter: progid:DXImageTransform.Microsoft.Gradient(gradientType=0,startColorStr=Gainsboro,endColorStr=#{eee:ColorHex}); padding: 1px 4px 1px 4px;">
                            <xsl:value-of select="eee:Login" />
                        </div>
                        <div style="color:#23819C; padding: 1px 4px 1px 4px;">
                            <xsl:value-of select="concat(' ', substring(eee:Time,12,5))" />
                            <font color="White">
                                <b>
                                    <xsl:value-of select="concat(' ', eee:Room)" />
                                </b>
                            </font>
                        </div>
                        <xsl:choose>
                            <xsl:when test="eee:ToUserID != 0">
                                <div style="border: 1px #{eee:ColorHex} solid; padding: 4px 4px 4px 4px; background-color: Gainsboro">
                                    <xsl:copy-of select="eee:Message" />
                                </div>
                            </xsl:when>
                            <xsl:otherwise>
                                <div style="border: 1px #{eee:ColorHex} solid; padding: 4px 4px 4px 4px;">
                                    <xsl:copy-of select="eee:Message" />
                                </div>
                            </xsl:otherwise>
                        </xsl:choose>
                    </div>
                </message>
            </xsl:for-each>
		</root>
	</xsl:template>
</xsl:stylesheet>