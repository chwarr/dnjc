;;; flycheck-dnjc.el --- Flycheck checker for .NET Json Check (dnjc) -*- lexical-binding: t; -*-

;; Copyright 2020, G. Christopher Warrington <code@cw.codes>
;;
;; dnjc is free software: you can redistribute it and/or modify it under the
;; terms of the GNU Affero General Public License Version 3 as published by the
;; Free Software Foundation.
;;
;; dnjc is distributed in the hope that it will be useful, but WITHOUT ANY
;; WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
;; FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more
;; details.
;;
;; A copy of the GNU Affero General Public License Version 3 is included in
;; the file LICENSE in the root of the repository.

;; This file is not part of GNU Emacs.

;; Author: Christopher Warrington <code@cw.codes>
;; Package-Version: 1.0
;; Package-Requires: ((emacs "24") (flycheck "32"))
;; Keywords: convenience, languages, tools
;; URL: https://www.thebluepolicebox.com/dnjc/

;;; Commentary:

;; Flycheck checker for JSON that uses the .NET Json Check (dnjc)
;;
;; You can install `dnjc' using the `dotnet' command line tool:
;;   dotnet tool install --global DotNetJsonCheck.Tool
;;
;; Add this checker to Flycheck's list of known checkers by calling
;; `flycheck-setup-dnjc'.
;;
;; If you use `use-package', something like this will work:
;;
;; (use-package flycheck-dnjc
;;   :config (flycheck-dnjc-setup))

;;; Code:

(require 'flycheck)

(flycheck-def-option-var
    flycheck-dnjc-strict
    nil
    json-dnjc
  "Whether to use dnjc's `--strict' mode.

When in strict mode, comments and trailing commas are not
permitted. `dnjc' defaults to allowing more relaxed syntax."
  :type 'boolean
  :safe #'booleanp
  :package-version '(flycheck-dnjc . "1.0"))

(flycheck-def-args-var
    flycheck-dnjc-args
    json-dnjc
  :package-version '(flycheck-dnjc . "1.0"))

(flycheck-define-checker json-dnjc
  "Check JSON using the .NET Json Check (`dnjc') tool.

`dnjc' uses .NET's `System.Text.Json.JsonDocument' class to
validate JSON."

  :command ("dnjc"
            (option-flag "--strict" flycheck-dnjc-strict)
            (eval flycheck-dnjc-args))
  :standard-input t
  :modes json-mode
  :error-patterns
  ((error line-start
          "Error" "\t"
          line "\t"
          column "\t"
          (message)
          line-end)))

;;;###autoload
(defun flycheck-dnjc-setup ()
  "Set up flycheck to use the .NET JSON Check (`dnjc') checker."
  (add-to-list 'flycheck-checkers 'json-dnjc))

(provide 'flycheck-dnjc)

;;; flycheck-dnjc.el ends here
