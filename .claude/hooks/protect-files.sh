#!/usr/bin/env bash
set -uo pipefail
path="$(jq -r '.tool_input.file_path // empty')"
[[ -z "$path" ]] && exit 0

deny() {
  echo "BLOCKED by .claude/hooks/protect-files.sh: $1" >&2
  exit 2
}

case "$path" in
  */.git/*) deny "never edit .git internals" ;;
  */Migrations/*) deny "EF migrations are generated; change the model and regenerate with dotnet ef" ;;
  */bin/*|*/obj/*) deny "build output is generated" ;;
  *.env|*/.env.*|*secrets.json|*.pfx|*.key) deny "secret files are off limits" ;;
esac
exit 0
