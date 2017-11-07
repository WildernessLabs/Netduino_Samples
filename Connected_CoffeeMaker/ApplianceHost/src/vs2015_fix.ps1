[CmdletBinding()]
param([switch]$Elevated,[switch]$Disable)

$netmfVersions = "v4.1","v4.2","v4.3"
$basedir = "C:\Program Files (x86)\MSBuild\Microsoft\.NET Micro Framework\"

#
# Functions
#

function Test-Admin {
  $currentUser = New-Object Security.Principal.WindowsPrincipal $([Security.Principal.WindowsIdentity]::GetCurrent())
  $currentUser.IsInRole([Security.Principal.WindowsBuiltinRole]::Administrator)
}

function FixUp([string]$path) {
   FixUpCSharpTargets $path"\"
   FixUpDeviceTargets $path"\"
   FixUpVisualBasicTargets $path"\"
}

function FixUpCSharpTargets ([string]$dir) {
   $path = $dir + "CSharp.targets"
   Write-Host "Fixing up " $path

   $doc = [xml]([System.IO.File]::ReadAllText($path))
#  $doc.PreserveWhitespace = 1
   $node = $doc.Project.PropertyGroup
   if (!$node) {
      Write-Host "ERROR: PropertyGroup node not found"
      return
   }
   
   if ($node.CscToolExe) {
      $cscToolExe = $node.CscToolExe
      $cscToolExe = "Csc.exe"
   } else {
      $cscToolExe = $doc.CreateElement("CscToolExe", $doc.DocumentElement.NamespaceURI)
      $cscToolExe = $node.AppendChild($cscToolExe)
      $cscToolExe.InnerText = "Csc.exe"
   }

   if ($node.CscToolPath) {
      $cscToolPath = $node.CscToolPath
      $cscToolPath = "`$([MSBuild]::GetRegistryValueFromView('HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\.NETFramework', 'InstallRoot', null, RegistryView.Registry32))v4.0.30319"
   } else {
      $cscToolPath = $doc.CreateElement("CscToolPath", $doc.DocumentElement.NamespaceURI)
      $cscToolPath = $node.AppendChild($cscToolPath)
      $cscToolPath.InnerText = "`$([MSBuild]::GetRegistryValueFromView('HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\.NETFramework', 'InstallRoot', null, RegistryView.Registry32))v4.0.30319"
   }

   if (!$disable) {
      $doc.Save($path);
   }
}

function FixUpDeviceTargets ([string]$dir)
{
   $path = $dir + "Device.targets"
   Write-Host "Fixing up " $path

   $doc = [xml]([System.IO.File]::ReadAllText($path))
   #  $doc.PreserveWhitespace = 1
   $ns = New-Object Xml.XmlNamespaceManager $doc.NameTable
   $ns.AddNamespace("ns", $doc.DocumentElement.NamespaceURI)
   $node = $doc.SelectSingleNode("//ns:Project/ns:Target[@Name='ResolveCodeAnalysisRuleSet']", $ns)
   $import = $doc.Project.Import
   if ($node) {
      $node.InnerText = ""
   } else {
      $node = $doc.CreateElement("Target", $doc.DocumentElement.NamespaceURI)
      $attrib = $doc.CreateAttribute("Name")
      $attrib.Value = "ResolveCodeAnalysisRuleSet"
      $attrib = $node.Attributes.Append($attrib)
      if ($import) {
         $node = $doc.Project.InsertAfter($node, $import)
      } else {
         $node = $doc.AppendChild($node)
      }
   }

   if (!$disable) {
      $doc.Save($path);
   }
}

function FixUpVisualBasicTargets ([string]$dir) {
   $path = $dir + "VisualBasic.targets"
   Write-Host "Fixing up " $path

   $doc = [xml]([System.IO.File]::ReadAllText($path))
#  $doc.PreserveWhitespace = 1
   $node = $doc.Project.PropertyGroup
   if (!$node) {
      Write-Host "ERROR: PropertyGroup node not found"
      return
   }
   
   if ($node.VbcToolExe) {
      $vbcToolExe = $node.VbcToolExe
      $vbcToolExe = "vbc.exe"
   } else {
      $vbcToolExe = $doc.CreateElement("VbcToolExe", $doc.DocumentElement.NamespaceURI)
      $vbcToolExe = $node.AppendChild($vbcToolExe)
      $vbcToolExe.InnerText = "Vbc.exe"
   }

   if ($node.VbcToolPath) {
      $vbcToolPath = $node.VbcToolPath
      $vbcToolPath = "`$([MSBuild]::GetRegistryValueFromView('HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\.NETFramework', 'InstallRoot', null, RegistryView.Registry32))v4.0.30319"
   } else {
      $vbcToolPath = $doc.CreateElement("VbcToolPath", $doc.DocumentElement.NamespaceURI)
      $vbcToolPath = $node.AppendChild($vbcToolPath)
      $vbcToolPath.InnerText = "`$([MSBuild]::GetRegistryValueFromView('HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\.NETFramework', 'InstallRoot', null, RegistryView.Registry32))v4.0.30319"
   }

   if (!$disable) {
      $doc.Save($path);
   }
}

#
# Mainline
#

# Make sure that we are running elevated
if ((Test-Admin) -eq $false)  {
    if ($elevated) 
    {
        # tried to elevate, did not work, aborting
    } 
    else {
        Start-Process powershell.exe -Verb RunAs -ArgumentList ('-noprofile -noexit -file "{0}" -elevated' -f ($myinvocation.MyCommand.Definition))
   }
   exit
}

# Fix each version of netmf
foreach ($version in $netmfVersions)
{
   FixUp "$basedir$version"
}