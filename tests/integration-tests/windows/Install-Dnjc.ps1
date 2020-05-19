<#
.SYNOPSIS

Installs the dnjc tool from a local package directory

.OUTPUTS

A PathInfo object for the installed executable

#>
[CmdletBinding(SupportsShouldProcess=$True)]
param
(
    # Source package directory.
    #
    # If not set, uses the debug pack\ directory in the repo root.
    [string]
    $SourceDir = $null,

    # The version to install.
    #
    # If not set, will skip passing a version to `dotnet tool install`.
    [string]
    $Version = $null,

    # Directory to install dnjc into.
    #
    # If not set, uses a temporary directory.
    [string]
    $DestDir = $null
)

Set-StrictMode -Version Latest

if (-not $SourceDir) {
    $SourceDir = "$PSScriptRoot\..\..\..\pack\Debug\netcoreapp3.1\"
}

$SourceDir = Resolve-Path $SourceDir

if (-not (Test-Path -LiteralPath $SourceDir -PathType Container)) {
    throw "Source `"$SourceDir`" does not exist."
}

if (-not (Test-Path $DestDir -PathType Container)) {
    $DestDir = [System.IO.Path]::Combine(
        [System.IO.Path]::GetTempPath(),
        [System.IO.Path]::GetRandomFileName())
}

if ($PSCmdlet.ShouldProcess('dnjc', "Install from `"$SourceDir`"")) {
    $dotnetArgs = @(
        'tool', 'install',
        '--add-source', $SourceDir,
        '--tool-path', $DestDir
    )

    if ($Version) {
        $dotnetArgs += ('--version', $Version)
    }

    $dotnetArgs += ('DotNetJsonCheck.Tool')

    & dotnet $dotnetArgs | Out-Null
    if (-not $?) {
        throw "dotnet tool install failed with $LASTEXITCODE"
    }

    $finalExecutable = Resolve-Path "$DestDir\dnjc.exe"

    if (-not (Test-Path -LiteralPath $finalExecutable -PathType Leaf)) {
        throw "Cannot find expected output `"$finalExecutable`""
    }

    Write-Output $finalExecutable
}
