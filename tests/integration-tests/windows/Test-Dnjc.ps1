<#
.SYNOPSIS

Runs integration tests against the given dnjc executable.

.OUTPUTS

The result of each test is written to either the output stream or the error
stream, depending on whether it passed or not.

The shape of the result object is:

* ActualExitCode
* Args
* ExpectedExitCode
* InputFile

#>
[CmdletBinding(SupportsShouldProcess=$True)]
param
(
    # Path to the dnjc executable to test
    [Parameter(Mandatory)]
    [string]
    $DnjcPath,

    # Path to test-data folder. The file $TestDataPath\test-list.csv must
    # exist.
    #
    # The input files in test-list.csv are assumed to be relative to
    # $TestDataPath.
    #
    # If not set, defaults to ..\test-data
    [string]
    $TestDataPath = $null
)

Set-StrictMode -Version Latest


if (-not $TestDataPath) {
    $TestDataPath = "$PSScriptRoot\..\test-data\"
}

$testListFile = Join-Path $TestDataPath 'test-list.csv'


if (-not (Test-Path -LiteralPath $DnjcPath -PathType Leaf)) {
    throw "Executable `"$DnjcPath`" does not exist."
}

if (-not (Test-Path -LiteralPath $testListFile -PathType Leaf)) {
    throw "Test list `"$testListFile`" does not exist."
}

$importCsvArgs = @{
    '-Delimiter' = ',';
    '-Encoding' = 'ASCII';
    '-Header' = ('ExitCode','Args','InputFile');
    '-LiteralPath' = $testListFile;
}

$testCases = Import-Csv @importCsvArgs

$anyErrors = $False

foreach ($testCase in $testCases) {
    $dnjcArgs = $testCase.Args -split ' '
    $inputFilePath = Join-Path $TestDataPath $testCase.InputFile
    Get-Content -Encoding Ascii $inputFilePath | & $DnjcPath $dnjcArgs | Out-Null

    $result = [PSCustomObject]@{
        'ActualExitCode' = $LASTEXITCODE;
        'Args' = $dnjcArgs -join ' ';
        'ExpectedExitCode' = $testCase.ExitCode;
        'InputFile' = $testCase.InputFile;
    }

    if ($LASTEXITCODE -eq $testCase.ExitCode) {
        Write-Output $result
    } else {
        $anyErrors = $True
        Write-Error $result
    }
}

if ($anyErrors) {
    exit 1
} else {
    exit 0
}
