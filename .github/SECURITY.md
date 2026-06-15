# Security Policy

## Supported versions

Security fixes are provided for the latest released `1.x` version of
Munchausen. Please make sure you are on the current release before
reporting an issue.

## Reporting a vulnerability

Please do not report security vulnerabilities through public GitHub
issues, discussions, or pull requests.

Instead, use GitHub's private vulnerability reporting:

1. Go to the repository's **Security** tab.
2. Click **Report a vulnerability**.
3. Provide a description, the affected version, and steps to reproduce.

This opens a private channel visible only to the maintainers. We will
acknowledge your report, keep you informed of progress, and credit you in
the release notes unless you prefer to remain anonymous.

Munchausen is a mock-data generation library intended for tests and
development. It is not designed to produce cryptographically secure
randomness; do not use its output for secrets, tokens, or security
sensitive values.
