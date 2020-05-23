#!/bin/sh

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
