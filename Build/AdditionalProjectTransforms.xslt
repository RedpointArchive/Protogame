<xsl:if test="/Input/Properties/DocumentationFile = 'True'">
  <PropertyGroup>
    <DocumentationFile>
      <xsl:choose>
        <xsl:when test="/Input/Generation/Platform = 'Windows'">
          <xsl:text>bin\Debug\</xsl:text>
        </xsl:when>
        <xsl:otherwise>
          <xsl:text>bin/Debug/</xsl:text>
        </xsl:otherwise>
      </xsl:choose>
      <xsl:value-of select="$project/@Name" />
      <xsl:text>.xml</xsl:text>
    </DocumentationFile>
  </PropertyGroup>
</xsl:if>