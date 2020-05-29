# Copyright 2020, G. Christopher Warrington <code@cw.codes>
#
# dnjc is free software: you can redistribute it and/or modify it under the
# terms of the GNU Affero General Public License Version 3 as published by the
# Free Software Foundation.
#
# dnjc is distributed in the hope that it will be useful, but WITHOUT ANY
# WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
# FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more
# details.
#
# A copy of the GNU Affero General Public License Version 3 is included in
# the file LICENSE in the root of the repository.
#
# SPDX-License-Identifier: AGPL-3.0-only

.POSIX:
.PHONY: all clean
PANDOC = pandoc
PANDOC_SYNTAX_DEFNS = --syntax-definition=syntax/jsonc.xml --syntax-definition=syntax/elisp.xml
PANDOC_OPTS = --highlight-style=syntax/highlight.theme --standalone --template=template/html.html

all: index.html

clean:
	rm -f index.html

index.html: ../README.md index.yaml syntax/jsonc.xml syntax/elisp.xml syntax/highlight.theme template/html.html
	$(PANDOC) $(PANDOC_SYNTAX_DEFNS) $(PANDOC_OPTS) --from=gfm --to=html --metadata-file=index.yaml --output "$@" "$<"
	minify "$@" --output "$@"
