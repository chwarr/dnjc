;;; batch-compile-helper.el --- Batch byte-compile helper -*- lexical-binding: t; -*-

;; Copyright 2020, G. Christopher Warrington
;;
;; Author: Christopher Warrington <code@cw.codes>

;; This file is not part of GNU Emacs.

;; This software is released under the GNU AFFERO GENERAL PUBLIC LICENSE
;; Version 3. A copy of this license is included in the file LICENSE.
;;
;; SPDX-License-Identifier: AGPL-3.0-only

;;; Commentary:

;; Bootstap helper script to byte-compile packages in --batch mode by
;; installing them. This also makes sure that their dependencies are
;; specified correctly.
;;
;; When this library is loaded, it sets up the package library for a batch
;; build.
;;
;; To use in a Makefile, add a command like:
;;
;;     emacs --batch --quick --directory=. --load=batch-compile-helper.el --funcall=batch-compile-helper-package-install-file "$*.el"

(require 'cl-macs)
(require 'package)

;;; Code:

(defun batch-compiler-helper--init-package ()
  "Initialize a vanilla package subsystem."
  (message "HOME: %s" (getenv "HOME"))
  (setq package-archives '(("gnu" . "https://elpa.gnu.org/packages/")
                           ("melpa" . "https://melpa.org/packages/")))
  (package-initialize)
  (message "package-user-dir: %s" package-user-dir))

(defun batch-compile-helper-package-install-file ()
  "Install package files specified on the command line."
  (if (not noninteractive)
      (error "`batch-compile-helper-package-install-file' is to be used only with --batch"))
  (batch-compiler-helper--init-package)
  (package-refresh-contents)
  ;; This setq is taken from bytecomp.el circa Emacs 26.3.
  ;;
  ;; "Better crash loudly than attempting to recover from undefined
  ;; behavior."
  (setq attempt-stack-overflow-recovery nil
        attempt-orderly-shutdown-on-fatal-signal nil)
  (let ((any-errors nil))
    (cl-loop for package-file in command-line-args-left do
             (message "Installing file %s (and dependencies)" package-file)
             (condition-case err
                 (package-install-file package-file)
               ;; TODO: this error could be unpacked better
               (error (error "Error installing package %s: %s" package-file err)
                      (setq any-errors t))))
    (kill-emacs (if any-errors 1 0))))

(batch-compiler-helper--init-package)

(provide 'batch-compile-helper)

;;; batch-compiler-helper.el ends here
