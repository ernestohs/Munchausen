# Contributing to Munchausen

Thanks for your interest in Munchausen, an inference-first mock-data
generator for .NET. This guide covers how to build the project, the
conventions the codebase enforces, and how to get a change merged.

## Prerequisites

- The .NET 8 SDK (`net8.0`).
- Git.

## Build and test

```bash
dotnet restore
dotnet build --configuration Release
dotnet test  --configuration Release
```

Warnings are treated as errors (`TreatWarningsAsErrors`), and CI enforces
the same, so a build that is "green except for warnings" will fail. Build
and test locally before opening a pull request.

## Documentation and design authority

The canonical, public documentation for Munchausen is the
[Munchausen Contributor Handbook](https://github.com/ernestohs/Munchausen/blob/master/.docs/MUNCHAUSEN_HANDBOOK.md).
It is the single source of truth for the v1.0 public API, behavioral
contracts, the inference catalog, dataset reference, internal architecture,
and the testing and release gates. Read the relevant sections before making
a change, and if a change would conflict with a documented contract, open an
issue to discuss it first rather than working around it in code.

Start with the handbook's **Contributor Guide** and **Testing and Release
Gates** sections.

## Conventions the codebase enforces

These are not style preferences; they are checked by analyzers and tests:

- **The public API surface is locked.** New or changed public members are
  flagged by the Public API analyzer. Keep changes internal unless a
  public addition has been agreed in an issue first.
- **Determinism is contractual.** Seeded output and PRNG vectors are
  reproducible by design. Do not regenerate golden outputs to make a test
  pass; a failing golden means the code changed behavior.
- **No `System.Random` in `src/`.** All randomness flows through the
  project's deterministic random source.
- **XML docs on every public member.** Missing docs (CS1591) is an error.
- `Nullable` is enabled, namespaces are file-scoped, and types are
  `sealed`/`internal` by default.

## Pull request workflow

1. Fork the repository and create a topic branch.
2. Make your change with tests alongside it.
3. Ensure `dotnet build` and `dotnet test` pass with no warnings.
4. Open a pull request against `master` and fill in the template.
5. CI (build, test, and CodeQL) must pass. Pull requests are merged with
   squash, so a clean, descriptive PR title becomes the commit message.

## License

By contributing, you agree that your contributions are licensed under the
MIT License that covers this project.
