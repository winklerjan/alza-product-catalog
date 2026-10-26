#!/usr/bin/env bash
set -uo pipefail
input="$(cat)"
[[ "$(jq -r '.stop_hook_active // false' <<<"$input")" == "true" ]] && exit 0
cd "$CLAUDE_PROJECT_DIR" || exit 0
compgen -G '*.sln' >/dev/null || compgen -G '*.slnx' >/dev/null || exit 0
git status --porcelain | grep -Eq '\.(cs|csproj|props|json)"?$' || exit 0

if ! out="$(dotnet build --nologo -v q 2>&1)"; then
  echo "Build fails with the uncommitted changes. Fix it before handing back:" >&2
  tail -n 40 <<<"$out" >&2
  exit 2
fi
exit 0
