# dnjc, .NET JSON Check

[.NET JSON Check][dnjc], `dnjc`, is a command line tool to check for JSON
syntax errors using the [`System.Text.Json`][stj] parsers.

```powershell
PS> Get-Content "good.json" | dnjc
# Nothing wrong: no output
PS> Get-Content "bad.json" | dnjc
6	2		'}' is an invalid start of a value.
```

The source code [is hosted on GitHub](https://github.com/chwarr/dnjc).

## Install

`dnjc` can be installed as a [.NET global tool][dotnet-global-tools]:

```powershell
dotnet tool install --global DotNetJsonCheck.Tool
```

If this is the first .NET global tool you've installed, you may need to
restart your shell/console for it to pick up the changes to your PATH.

## Versions

The current versions of the tool and the library that powers it are shown
below.

| Package | Version |
|---------|---------|
| DotNetJsonCheck.Tool | [![DotNetJsonCheck.Tool NuGet version](https://img.shields.io/nuget/v/DotNetJsonCheck.Tool?logo=nuget&style=flat-square)][nuget-dnjc-tool] |
| DotNetJsonCheck | [![DotNetJsonCheck NuGet version](https://img.shields.io/nuget/v/DotNetJsonCheck?logo=nuget&style=flat-square)][nuget-dnjc] |

## Invocation

After installing, invoke it, giving it the JSON you want to check on
standard input. Using PowerShell, this is something like:

```powershell
Get-Content "input.json" | dnjc
```

Other shells, like Bash, CMD, and Zsh can use redirection:

```bash
dnjc <input.json
```

Any errors will be written to standard out.

## Options

With no switches, `dnjc` will allow `/* */` and `//` comments as well
as trailing commas. This JSON document will be successfully parsed:

```jsonc
{
    // System.Text.Json can parse this file.
    "hello": "world",
}
```

These defaults were chosen because this is how the
[`Microsoft.Extensions.Configuration.Json`][mecj] library invokes
`JsonDocument.Parse()`, and those are the JSON files I'm most often editing.

This behavior can be controlled:

* `--allow-comments`: allows `/* */` and `//` style comments (defaults to
  enabled)
* `--allow-trailing-commas`: allows trailing commas in arrays and objects
  (defaults to enabled)
* `--strict`: parses JSON strictly (no comments, no trailing commas)
* `--help`: prints help and exits
* `--version`: prints version information and exits

Any number of options may be passed. They are processed in order. For
example, to allow comments but not trailing commas, pass `--strict
--allow-comments` in that order.

## Emacs integration

The [Emacs][emacs] package `flycheck-dnjc.el` can be used to configure a
[Flycheck][flycheck] checker that uses `dnjc`.

To install it, [download it][flycheck-dnjc.el] and then install it with
<kbd>M-x</kbd> `package-install-file` <kbd>RET</kbd>
`/path/to/where/you/downloaded/flycheck-dnjc.el`

If you use [`use-package`][use-package] to manage your packages, add this to
your `.emacs` file:

```elisp
(use-package flycheck-dnjc
  :config (setup-flycheck-dnjc))
```

If you aren't using any package managers, make sure that
`flycheck-dnjc` in on your load-path and add:

```elisp
(require 'flycheck-dnjc)
(setup-flycheck-dnjc)
```

## Error reporting

Any errors encountered will be written to standard out, one per line. Each
line has the format

```
LEVEL<TAB>LINE<TAB>COLUMN<TAB>MESSAGE
```

* LEVEL: the severity of the issue. Currently only "Error" is used.
* LINE: the 1-based line number where the error occurred or started.
* COLUMN: the 0-based byte offset into the line where the error occurred.
* MESSAGE: the detailed error message.

If LINE or COLUMN cannot be determined, they will be empty.

Additional columns may be added in the future at the end. Ensure your
parsing can handle this.

The exit code of `dnjc` will be

* 0: JSON parsed successfully
* 1: invalid arguments
* 2: errors processing the JSON
* 126: catastrophic failure

## To Do

Things that I want to do, in rough priority order

1. Add option to require that the top-level value be an object, an array,
   &amp;c.
1. Use `package-upload-file` to create a hostable package archive?

## License

Copyright 2020, G. Christopher Warrington

dnjc is free software: you can redistribute it and/or modify it under the
terms of the GNU Affero General Public License Version 3 as published by the
Free Software Foundation.

dnjc is distributed in the hope that it will be useful, but WITHOUT ANY
WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more
details.

A copy of the GNU Affero General Public License Version 3 is included in the
file [LICENSE] at the root of the repository.

[dnjc]: https://www.thebluepolicebox.com/dnjc/
[dotnet-global-tools]: https://aka.ms/global-tools
[emacs]: https://www.gnu.org/software/emacs/
[flycheck-dnjc.el]: https://raw.githubusercontent.com/chwarr/dnjc/master/emacs/flycheck-dnjc.el
[flycheck]: https://www.flycheck.org/
[LICENSE]: https://github.com/chwarr/dnjc/blob/master/LICENSE
[mecj]: https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.configuration.json?view=dotnet-plat-ext-3.1
[nuget-dnjc-tool]: https://www.nuget.org/packages/DotNetJsonCheck.Tool/
[nuget-dnjc]: https://www.nuget.org/packages/DotNetJsonCheck/
[stj]: https://docs.microsoft.com/en-us/dotnet/api/system.text.json?view=netcore-3.1
[use-package]: https://github.com/jwiegley/use-package
