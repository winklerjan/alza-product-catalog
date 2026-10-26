#!/usr/bin/env bash
set -uo pipefail
path="$(jq -r '.tool_input.file_path // empty')"
[[ "$path" == *.cs && -f "$path" ]] || exit 0
cd "$CLAUDE_PROJECT_DIR" || exit 0
rel="${path#"$CLAUDE_PROJECT_DIR"/}"
dotnet format whitespace --folder --include "$rel" --verbosity quiet >/dev/null 2>&1 || true
exit 0
