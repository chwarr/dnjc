;;; flycheck-dnjc.el --- Flycheck checker for .NET Json Check (dnjc) -*- lexical-binding: t; -*-

;; Copyright 2020, G. Christopher Warrington
;;
;; Author: Christopher Warrington <c45207@mygcw.net>
;; Package-Version: 0.1
;; Package-Requires: ((flycheck "32"))
;; Keywords: convenience, languages, tools
;; URL: https://github.com/chwarr/dnjc

;; This file is not part of GNU Emacs.

;; This software is released under the GNU AFFERO GENERAL PUBLIC LICENSE
;; Version 3. A copy of this license is included in the file LICENSE.
;;
;; SPDX-License-Identifier: AGPL-3.0-only

;;; Commentary:

;; Flycheck checker for JSON that uses the .NET Json Check (dnjc)
;;
;; You can install `dnjc' using the `dotnet' command line tool:
;;   dotnet tool install --global DotNetJsonCheck.Tool
;;
;; Add this checker to Flycheck's list of known checkers by calling
;; `setup-flycheck-dnjc'.
;;
;; If you use `use-package', something like this will work:
;;
;; (use-package flycheck-dnjc
;;   :config (setup-flycheck-dnjc))

;;; Code:

(require 'flycheck)

(flycheck-def-option-var
    flycheck-dnjc-strict
    nil
    json-dnjc
  "Whether to use dnjc's `--strict' mode.

When in strict mode, comments and trailing commas are not
permitted."
  :type 'boolean
  :safe #'booleanp
  :package-version '(flycheck-dnjc . "0.1"))

(flycheck-define-checker json-dnjc
  "Check JSON using the .NET Json Check (`dnjc') tool.

`dnjc' uses .NET's `System.Text.Json.JsonDocument' class to
validate JSON."

  :command ("dnjc"
            (option-flag "--strict"
                         flycheck-dnjc-strict))
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
(defun setup-flycheck-dnjc ()
  "Set up flycheck to use the .NET JSON Check (`dnjc') checker."
  (add-to-list 'flycheck-checkers 'json-dnjc))

(provide 'flycheck-dnjc)

;;; flycheck-dnjc.el ends here
