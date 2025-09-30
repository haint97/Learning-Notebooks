# GitHub Copilot Repository Instructions

## Response Splitting for Long Outputs

If the response you're generating is too long and risks being cut off or exceeding length limits, **automatically split the response into multiple clearly numbered parts** (e.g., "Part 1 of 3", "Part 2 of 3", etc.).

**After each part:**
- Stop and wait for explicit acknowledgment from the user (e.g., the user replies with "next") before continuing.
- Do **not** continue to the next part unless the user confirms.

**This rule applies to:**
- Code generation (especially long functions or multi-file outputs)
- Documentation or code comments
- Explanations or technical breakdowns

## General Guidelines

- Keep responses clear and concise where possible.
- Prefer readable, maintainable code over overly clever implementations.
- When showing examples, use the project's existing style and structure where applicable.
