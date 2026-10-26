#!/usr/bin/env bash
set -uo pipefail
cmd="$(jq -r '.tool_input.command // empty')"

deny() {
  echo "BLOCKED by .claude/hooks/guard-bash.sh: $1. Do not work around this; stop and ask the developer." >&2
  exit 2
}

grep -Eq '(^|[;&|[:space:]])git[[:space:]]+push' <<<"$cmd" && deny "pushing is done manually"
grep -Eq -- '--no-verify' <<<"$cmd" && deny "git hooks must not be skipped"
grep -Eq 'git[[:space:]]+(reset[[:space:]]+--hard|clean[[:space:]]+-[a-zA-Z]*f|rebase|config)' <<<"$cmd" && deny "history- or config-changing git command"
grep -Eq 'git[[:space:]]+commit[^;&|]*--amend' <<<"$cmd" && deny "amending commits rewrites history"
grep -Eq 'rm[[:space:]]+-[a-zA-Z]*r[a-zA-Z]*[[:space:]]+(/|~|\.|\*|\$HOME)([[:space:]/]|$)' <<<"$cmd" && deny "recursive delete of a root, home, or project path"
grep -Eq 'dotnet[[:space:]]+ef[[:space:]]+database[[:space:]]+drop' <<<"$cmd" && deny "dropping the database"
grep -Eq '(curl|wget)[^|]*\|[[:space:]]*(sudo[[:space:]]+)?(ba|z)?sh' <<<"$cmd" && deny "piping downloads into a shell"
grep -Eq '(^|[;&|[:space:]])sudo[[:space:]]' <<<"$cmd" && deny "sudo is not allowed"

exit 0
