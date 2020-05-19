<#
.SYNOPSIS

Installs the dnjc tool from a local package directory

.OUTPUTS

A PathInfo object for the installed executable

#>
[CmdletBinding(SupportsShouldProcess=$True)]
param
(
    # Package directory.
    #
    # If not set, uses the pack\ directory in the repo root.
    [string]
    $SourcePath = $null,

    # The version to install.
    #
    # If not set, will skip passing a version to `dotnet tool install`.
    [string]
    $Version = $null,

    # Path to install dnjc to.
    #
    # If not set, uses a temporary directory.
    [string]
    $DestPath = $null
)

Set-StrictMode -Version Latest

if (-not $SourcePath) {
    $SourcePath = "$PSScriptRoot\..\..\..\pack\Debug\netcoreapp3.1\"
}

$SourcePath = Resolve-Path $SourcePath

if (-not (Test-Path -LiteralPath $SourcePath -PathType Container)) {
    throw "Source `"$SourcePath`" does not exist."
}

if (-not (Test-Path $DestPath -PathType Container)) {
    $DestPath = [System.IO.Path]::Combine(
        [System.IO.Path]::GetTempPath(),
        [System.IO.Path]::GetRandomFileName())
}

if ($PSCmdlet.ShouldProcess('dnjc', "Install from `"$SourcePath`"")) {
    $dotnetArgs = @(
        'tool', 'install',
        '--add-source', $SourcePath,
        '--tool-path', $DestPath
    )

    if ($Version) {
        $dotnetArgs += ('--version', $Version)
    }

    $dotnetArgs += ('DotNetJsonCheck.Tool')

    & dotnet $dotnetArgs | Out-Null
    if (-not $?) {
        throw "dotnet tool install failed with $LASTEXITCODE"
    }

    $finalExecutable = Resolve-Path "$DestPath\dnjc.exe"

    if (-not (Test-Path -LiteralPath $finalExecutable -PathType Leaf)) {
        throw "Cannot find expected output `"$finalExecutable`""
    }

    Write-Output $finalExecutable
}
