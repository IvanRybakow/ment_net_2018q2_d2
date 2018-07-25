<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet
  version="1.0"
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:cd="http://library.by/catalog"
  extension-element-prefixes="cd">

  <xsl:output method="html" indent="yes"/>
  <xsl:key name="groups" match="cd:book" use="cd:genre" />
  <xsl:param name="Date" select="''"/>

  <xsl:template match="/">
    <html>
      <body>
        <table border="1">
          <caption><b>Current collecction by genre on <xsl:value-of select="$Date"/></b></caption>
          <tr>
            <th>Author</th>
            <th>Name</th>
            <th>Publish Date</th>
            <th>Register Date</th>
          </tr>
          <xsl:for-each select="//cd:book[count(. | key('groups', cd:genre)[1]) = 1]">
            <xsl:variable name="current-genre-key" select="cd:genre"/>
            <xsl:variable name="current-genre" select="key('groups', $current-genre-key)"/>
            <tr>
              <td colspan="4">
                <b>Genre: <xsl:value-of select="cd:genre"/></b>
              </td>
            </tr>
            <xsl:for-each select="$current-genre">
              <tr>
                <td>
                  <xsl:value-of select="cd:author"/>
                </td>
                <td>
                  <xsl:value-of select="cd:title"/>
                </td>
                <td>
                  <xsl:value-of select="cd:publish_date"/>
                </td>
                <td>
                  <xsl:value-of select="cd:registration_date"/>
                </td>
              </tr>
            </xsl:for-each>
            <tr>
              <td colspan="3">
                <b><xsl:value-of select="cd:genre"/> total</b>
              </td>
              <td colspan="1">
                <b><xsl:value-of select="count(/cd:catalog/cd:book[cd:genre = $current-genre-key])"/> total</b>
              </td>
            </tr>
          </xsl:for-each>
          <tr>
            <td colspan="3">
              <b>Grand Total: </b>
              <td colspan="1">
                <b><xsl:value-of select="count(/cd:catalog/cd:book)"/> total</b>
              </td>
            </td>
          </tr>
        </table>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>
