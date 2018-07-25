<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet
  version="1.0"
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:cd="http://library.by/catalog"
  extension-element-prefixes="cd">

  <xsl:output method="xml" indent="yes"/>


  <xsl:template match="/">
    <rss xmlns:blogChannel="http://library.by/catalog" version="2.0">
      <Channel>
        <title>Books Catalog</title>
        <link>http://library.by/catalog</link>
        <description>Local Library Books Catalog</description>
        <language>en-us</language>
        <xsl:for-each select="/cd:catalog/cd:book">
          <item>
            <title>
              <xsl:value-of select="cd:title"/>
            </title>
            <description>
              <xsl:value-of select="cd:description"/>
            </description>
            <pubDate>
              <xsl:value-of select="cd:registration_date"/>
            </pubDate>
            <xsl:if test="cd:isbn">
              <link>
                <xsl:value-of select="concat('http://my.safaribooksonline.com/', cd:isbn)"/>
              </link>
            </xsl:if>
          </item>
        </xsl:for-each>
      </Channel>
    </rss>
  </xsl:template>
</xsl:stylesheet>
