#!/bin/sh

# -e: exit on error (ish)
# -f: disable globbing
# -u: fail if expanding an unset variable
set -efu

usage() {
    cat<<EOF
${0} [-d dest_dir] [-s source_dir] [-v version]

    Install dnjc from NuGet packages. Writes the installed location to
    standard out.

    -d dest_dir: the destination directory to install into. If not set, a
    directory under \$TMPDIR will be created.

    -s source_dir: directory containing the NuGet packages to install from.
    If not set, uses the pack directory from the root of the repo.

    -v version: The version of dnjc to install. If set a --version argument
    is passed to \`dotnet tool install\`; otherwise no --version is passed.

EOF
}

if [ $# -eq 0 ]; then
    usage
    exit 1
fi

while getopts 'd:s:v:' name; do
    case "${name}" in
        d) dest_dir="${OPTARG}" ;;
        s) source_dir="${OPTARG}" ;;
        v) version="${OPTARG}" ;;

        :) echo "${0}: -${name} needs a value"; usage; exit 1 ;;
        ?) usage; exit 1 ;;
    esac
done

# Not all systems have $TMPDIR set, but /tmp should almost surely exist.
tmp_dir="${TMPDIR:-/tmp}"

dest_dir="${dest_dir:-$tmp_dir/dnjc-install-$(od -An -N4 -tx /dev/urandom | tr -d ' ')}"
source_dir="${source_dir:-../../../pack/Debug/netcoreapp3.1}"

if [ ! -d "${source_dir}" ]; then
    echo "'${source_dir}' does not exist"
    exit 1
fi

mkdir -p "${dest_dir}"

# The ${version:+} syntax is to only pass those values if version is set.

>/dev/null dotnet tool install \
    --add-source "${source_dir}" \
    --tool-path "${dest_dir}" \
    ${version:+"--version"} ${version:+"${version}"} \
    DotNetJsonCheck.Tool

dnjc_path="${dest_dir}/dnjc"

if [ ! -x "${dest_dir}" ]; then
    echo "dnjc was not installed as expected to '${dest_dir}'"
    exit 1
fi

echo "${dnjc_path}"
