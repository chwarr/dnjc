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
<#
Copyright 2020, G. Christopher Warrington <code@cw.codes>

dnjc is free software: you can redistribute it and/or modify it under the
terms of the GNU Affero General Public License Version 3 as published by the
Free Software Foundation.

dnjc is distributed in the hope that it will be useful, but WITHOUT ANY
WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more
details.

A copy of the GNU Affero General Public License Version 3 is included in the
file LICENSE in the root of the repository.

SPDX-License-Identifier: AGPL-3.0-only
#>
[CmdletBinding(SupportsShouldProcess=$True)]
param
(
    # Path to the dnjc executable to test
    [Parameter(Mandatory)]
    [string]
    $DnjcPath,

    # Directory with test data. The file $TestDataDir\test-list.csv must
    # exist.
    #
    # The input files in test-list.csv are assumed to be relative to
    # $TestDataDir.
    #
    # If not set, defaults to $PSScriptRoot\..\test-data
    [string]
    $TestDataDir = $null
)

Set-StrictMode -Version Latest


if (-not $TestDataDir) {
    $TestDataDir = "$PSScriptRoot\..\test-data\"
}

$testListFile = Join-Path $TestDataDir 'test-list.csv'


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
    $inputFilePath = Join-Path $TestDataDir $testCase.InputFile

    if ($PSCmdlet.ShouldProcess("$($testCase.ExitCode),$dnjcArgs", 'Test')) {

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
}

if ($anyErrors) {
    exit 1
} else {
    exit 0
}
