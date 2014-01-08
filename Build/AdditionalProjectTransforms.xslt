<xsl:if test="/Input/Properties/PostProcessWithDx = 'True'">
  <Target Name="AfterBuild">
    <xsl:choose>
      <xsl:when test="/Input/Generation/Platform = 'Windows'">
        <Exec>
          <xsl:attribute name="Command">
            <xsl:value-of select="concat(
  /Input/Generation/RootPath,
  'ThirdParty/Dx/Dx.Process/bin/Debug/Dx.Process.exe')" />
            <xsl:text> "$(TargetPath)"</xsl:text>
          </xsl:attribute>
        </Exec>
      </xsl:when>
      <xsl:otherwise>
        <Exec>
          <xsl:attribute name="Command">
            <xsl:text>mono </xsl:text>
            <xsl:value-of select="concat(
  /Input/Generation/RootPath,
  'ThirdParty/Dx/Dx.Process/bin/Debug/Dx.Process.exe')" />
            <xsl:text> "$(TargetPath)"</xsl:text>
          </xsl:attribute>
        </Exec>
      </xsl:otherwise>
    </xsl:choose>
  </Target>
</xsl:if>

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