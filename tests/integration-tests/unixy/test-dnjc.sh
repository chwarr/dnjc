#!/bin/sh

# -e: exit on error (ish)
# -f: disable globbing
# -u: fail if expanding an unset variable
set -efu

usage() {
    cat<<EOF
${0} -d dnjc-path [-t test_data_dir]

    Runs integration tests against the given dnjc executable.

    The result of each test is written to either standard out. Passing tests
    are prefixed with "OK:" and failing tests with "FAIL:".

    If any tests fail, exits with 2.

    -d dnjc_path: path the the dnjc executable to test

    -t test_data_dir: directory with test cases. Must have a test-list.csv
    file. If not specified, uses ../test-data relative to the script.

EOF
}

# Invokes $1 with standard input connected to $2 with the rest of the
# arguments as arguments.
invoke_on_with() {
    cmd="${1}"
    input="${2}"
    shift 2

    "${cmd}" "$@" <"${input}" >/dev/null
}

if [ $# -eq 0 ]; then
    usage
    exit 1
fi

while getopts 'd:t:' name; do
    case "${name}" in
        d) dnjc_path="${OPTARG}" ;;
        t) test_data_dir="${OPTARG}" ;;

        :) echo "${0}: -${name} needs a value"; usage; exit 1 ;;
        ?) usage; exit 1 ;;
    esac
done

shift $((OPTIND - 1))

test_data_dir="${test_data_dir:-$(dirname "${0}")/../test-data}"
test_list_path="${test_data_dir}/test-list.csv"

if [ ! -x "${dnjc_path}" ]; then
    echo "'${dnjc_path}' does not exist or is not executable"
    exit 1
elif [ ! -f "${test_list_path}" ]; then
    echo "'${test_list_path}' does not exist"
    exit 1
fi

error_count=0

# test-list.csv is comma delimited
test_list_fs=','
IFS="${test_list_fs}"
while read -r expected_exit_code args input
do
    # Skip blank lines
    if [ -z "${expected_exit_code}" ]; then
        continue
    fi

    if [ -z "${input}" ]; then
        echo "Malformed test case. Missing input file: ${expected_exit_code},${args}"
        exit 1
    fi

    test_case_input="${test_data_dir}/${input}"

    # Set IFS to space so that multiple ${args} get passed as multiple args.
    #
    # dnjc_exit_code is used to capture the exit code of dnjc while still
    # using set -e for this script
    IFS=' '
    # shellcheck disable=SC2086
    #
    # SC2086: Double quote to prevent globbing and word splitting.
    #
    # We want splitting here on ${args}. Globbing is disabled with set -f
    invoke_on_with "${dnjc_path}" "${test_case_input}" ${args} \
        && dnjc_exit_code=$? \
        || dnjc_exit_code=$?

    if [ "${expected_exit_code}" -eq "${dnjc_exit_code}" ]; then
        test_result='OK'
    else
        error_count=$((error_count + 1))
        test_result='FAIL'
    fi

    echo "${test_result}:${expected_exit_code},${args},${input}"

    # Restore to parse the next line of input
    IFS="${test_list_fs}"
done <"${test_list_path}"

if [ "${error_count}" -gt 0 ]; then
    exit 2
else
    exit 0
fi
