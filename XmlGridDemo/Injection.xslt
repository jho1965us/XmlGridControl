<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
    <xsl:output method="xml" indent="yes"/>

    <xsl:template match="MeedioConfiguration">
      <Menu>
      </Menu>
    </xsl:template>
  <xsl:template match="menu_items">
    <Caption>
      <xsl:value-of select="description/Entry[Key='caption']"/>
    </Caption>
  </xsl:template>
</xsl:stylesheet>
