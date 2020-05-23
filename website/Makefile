# Copyright 2020, G. Christopher Warrington <code@cw.codes>

# This software is released under the GNU AFFERO GENERAL PUBLIC LICENSE
# Version 3. A copy of this license is included in the file LICENSE.
#
# SPDX-License-Identifier: AGPL-3.0-only

.POSIX:
.PHONY: all clean
PANDOC = pandoc
PANDOC_SYNTAX_DEFNS = --syntax-definition=syntax/jsonc.xml --syntax-definition=syntax/elisp.xml
PANDOC_OPTS = --highlight-style=kate --standalone --template=template/html.html

all: index.html

clean:
	rm -f index.html

index.html: ../README.md index.yaml syntax/jsonc.xml syntax/elisp.xml template/html.html
	$(PANDOC) $(PANDOC_SYNTAX_DEFNS) $(PANDOC_OPTS) --from=gfm --to=html --metadata-file=index.yaml --output "$@" "$<"
	minify "$@" --output "$@"
