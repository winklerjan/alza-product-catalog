---
description: Code review via the code-reviewer subagent
argument-hint: [scope, default: whole repository]
---
Use the code-reviewer subagent to review: ${ARGUMENTS:-the whole repository}.
Then summarize its findings, say which you agree with and why, and propose which to fix.
Do not change any code until I choose.
