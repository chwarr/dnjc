# Copyright 2020, G. Christopher Warrington <code@cw.codes>

# This file is not part of GNU Emacs.
#
# This software is released under the GNU AFFERO GENERAL PUBLIC LICENSE
# Version 3. A copy of this license is included in the file LICENSE.
#
# SPDX-License-Identifier: AGPL-3.0-only


# Hat-tip to Christopher Wellons <wellons@nullprogram.com> for the initial
# Makefile
# <https://github.com/skeeto/x86-lookup/blob/609b2ba70dc5a246ac9b4b5f89eb5ef4331519bf/Makefile>
# that this started from. That Makefile was placed into the public
# domain/licensed under the Unlicense <http://unlicense.org/>.

# Requirements:
#
#    emacs 25+
#    realpath

.POSIX:
.PHONY: compile clean
EMACS = emacs
BATCH = $(EMACS) --batch --quick --directory=.

# Don't want to affect the existing Emacs install, so we set HOME to
# TEMP_HOME when invoking emacs.
TEMP_HOME = build

compile: flycheck-dnjc.elc

clean:
	rm -rf "$(TEMP_HOME)"
	rm -f flycheck-dnjc.elc

.SUFFIXES: .el .elc
.el.elc:
# Test installing the package. This also handles any dependencies.
	HOME="$$(realpath "$(TEMP_HOME)")" $(BATCH) --load=batch-compiler-helper.el --funcall batch-compile-helper-package-install-file "$*.el"
# Byte-compile alone. This also creates the expected .elc
	HOME="$$(realpath "$(TEMP_HOME)")" $(BATCH) --load=batch-compiler-helper.el --funcall batch-byte-compile "$*.el"
