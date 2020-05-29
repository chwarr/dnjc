#!/bin/sh

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
# A copy of the GNU Affero General Public License Version 3 is included in the
# file LICENSE in the root of the repository.
#
# SPDX-License-Identifier: AGPL-3.0-only

# -e: exit on error (ish)
# -f: disable globbing
# -u: fail if expanding an unset variable
set -efu
pandoc_download=https://github.com/jgm/pandoc/releases/download/2.9.2.1/pandoc-2.9.2.1-linux-amd64.tar.gz
pandoc_archive=$(mktemp --tmpdir pandoc.tgz.XXXXX)
pandoc_output_dir=$(mktemp --directory --tmpdir pandoc.XXXXX)
pandoc_bin=${pandoc_output_dir}/bin/pandoc

curl --location --silent --show-error --output "${pandoc_archive}" "${pandoc_download}"

tar --extract --file "${pandoc_archive}" --gzip --directory "${pandoc_output_dir}"  --strip-components=1

if [ ! -x "${pandoc_bin}" ]; then
    echo "'${pandoc_bin}' does not exist or is not executable"
    exit 1
fi

echo "${pandoc_bin}"
