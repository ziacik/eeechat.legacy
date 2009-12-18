<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet	version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:eee="http://tempuri.org/EeeDataSet.xsd">
	<xsl:output method="xml" />

	<xsl:template match="/eee:EeeDataSet">
        <root>
            <xsl:for-each select="eee:Message">
                <message messageId="{eee:MessageID}">
                    <div style="font-family:Arial; font-size:0.7em; padding-top: 5px; padding-bottom: 15px;">
                        <div style="color:#F0F0F0; height: 10px; float:left; border: 1px #{eee:ColorHex} solid; background-color: #{eee:ColorHex}; filter: progid:DXImageTransform.Microsoft.Gradient(gradientType=0,startColorStr=Gainsboro,endColorStr=#{eee:ColorHex}); padding: 1px 4px 1px 4px;">
                            <xsl:choose>
                                <xsl:when test="eee:ExternalFrom != ''">
                                    <xsl:value-of select="eee:ExternalFrom" />
                                </xsl:when>
                                <xsl:otherwise>
                                    <xsl:value-of select="eee:Login" />
                                </xsl:otherwise>
                            </xsl:choose>
                            <xsl:value-of select="concat(' ', substring(eee:Time,12,5))" />
                        </div>
                        <div style="float:left; height: 10px; color: #8C8CFF; font-weight: bold; padding: 1px 4px 1px 4px;">
                            <xsl:value-of select="concat(' ', eee:Room)" />
                        </div>
                        <xsl:choose>
                            <xsl:when test="eee:ExternalFrom != ''">
                                <div style="clear: left; border: 1px #{eee:ColorHex} solid; padding: 4px 4px 4px 4px; background-color: #FCFCE9;">
                                    <xsl:copy-of select="eee:Message" />
                                </div>
                            </xsl:when>
                            <xsl:when test="eee:ToUserID != 0 and eee:ExternalFrom = ''">
                                <div style="clear: left; border: 1px #{eee:ColorHex} solid; padding: 4px 4px 4px 4px; background-color: Gainsboro;">
                                    <xsl:copy-of select="eee:Message" />
                                </div>
                            </xsl:when>
                            <xsl:otherwise>
                                <div style="clear: left; border: 1px #{eee:ColorHex} solid; padding: 4px 4px 4px 4px; background-color: White;">
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