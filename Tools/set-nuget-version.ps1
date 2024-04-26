param (
    [Parameter(Mandatory=$true)]
    [string]$version
)

function UpdateToolVersion($filePath, $version) {
    # Load the existing JSON file
    $json = Get-Content $filePath | ConvertFrom-Json

    # Update the version
    $json.tools."unofficial.Urho3DNet.Editor".version = $version

    # Convert the updated object back to JSON
    $json | ConvertTo-Json -Depth 100 | Set-Content $filePath
}

function UpdateVersionInPropsFile($filePath, $version) {
    # Load the XML file
    [xml]$xml = Get-Content $filePath

    # Find the Version element and update its value
    $xml.Project.PropertyGroup.Urho3DNetVersion = $version

   # Create a UTF8 encoding object without BOM
   $utf8NoBom = New-Object System.Text.UTF8Encoding $false

   # Create an XmlTextWriter with the file path and encoding
   $writer = New-Object System.Xml.XmlTextWriter($filePath, $utf8NoBom)

   # Set the writer's formatting to be indented
   $writer.Formatting = [System.Xml.Formatting]::Indented

   # Save the updated XML back to the file
   $xml.WriteTo($writer)
   $writer.Flush()
   $writer.Close()
}

UpdateToolVersion -filePath "$PSScriptRoot/../.config/dotnet-tools.json" -version $version
UpdateVersionInPropsFile -filePath "$PSScriptRoot/../Directory.Build.props" -version $version
