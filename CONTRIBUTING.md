# Contributing to Munchausen

Thanks for contributing. This document describes the branching model and how
releases are produced. For the design rules that govern the library itself,
see `CLAUDE.md` and the documents under `docs/`.

## Branching model

Munchausen uses a GitFlow-style model with three long-lived branch roles:

| Branch         | Purpose                          | CI            | Publishes              |
| -------------- | -------------------------------- | ------------- | ---------------------- |
| `develop`      | Integration of finished features | build + test  | nothing                |
| `release/x.y`  | Stabilizing a release candidate  | build + test  | `x.y.0-rc.N` prerelease |
| `master`       | Stable, released history         | build + test  | stable `x.y.z`         |

Feature work happens on short-lived branches off `develop`. Release candidates
are cut to `release/x.y`. Stable releases land on `master`.

## Versioning

Package versions are derived from git tags by [MinVer](https://github.com/adamralph/minver);
there is no version number to edit in the project file. The tag prefix is `v`.

- A stable tag `v1.2.3` produces version `1.2.3`.
- A prerelease tag `v1.2.0-rc.1` produces `1.2.0-rc.1`.

The stable version bump on `master` is computed automatically from
[Conventional Commit](https://www.conventionalcommits.org/) messages since the
last tag:

- `feat:` -> minor bump
- `fix:` -> patch bump
- a `BREAKING CHANGE:` footer (or `!` after the type) -> major bump

Please write commit messages in that form so the bump is correct.

## Working on a feature

```bash
git checkout develop && git pull
git checkout -b feat/short-description
# ... commit using conventional messages ...
git push -u origin feat/short-description
gh pr create --base develop --fill
```

Open the PR against `develop`. Merge once CI is green.

## Cutting a release candidate

When `develop` holds enough for the next release (say `1.1.0`):

```bash
# 1. Branch from develop
git checkout develop && git pull
git checkout -b release/1.1
git push -u origin release/1.1

# 2. Tag the first candidate to publish it
git tag v1.1.0-rc.1
git push origin v1.1.0-rc.1   # publishes 1.1.0-rc.1 to nuget.org
```

The version number (`1.1.0`) is chosen by hand based on what landed in
`develop`: new features mean a minor bump, fixes only mean a patch bump.

During stabilization, commit fixes to `release/1.1` and publish updated
candidates by tagging again:

```bash
git tag v1.1.0-rc.2
git push origin v1.1.0-rc.2   # publishes 1.1.0-rc.2
```

Port those fixes back so they are not lost when the branch is retired:

```bash
git checkout develop && git pull
git merge release/1.1
git push
```

Prereleases are only installed by consumers who opt into prerelease packages,
so the `release/x.y` branch is a safe staging channel.

## Shipping a stable release

```bash
gh pr create --base master --head release/1.1 --fill
```

Merge that PR. On merge to `master`, the `Draft release` workflow computes the
next version from conventional commits and creates a **draft** GitHub release
with generated notes. Review it, then publish:

```bash
gh release list
gh release edit v1.1.0 --draft=false   # or click "Publish" in the GitHub UI
```

Publishing the draft creates the `v1.1.0` tag, which triggers the `Release`
workflow to build, pack, and push the stable package to nuget.org and GitHub
Packages. Publishing is the only manual gate on the irreversible nuget.org
push; a published version cannot be unpublished, only deprecated.

## Workflow reference

| Workflow             | Trigger                     | Effect                                   |
| -------------------- | --------------------------- | ---------------------------------------- |
| `ci.yml`             | push / PR to any main branch | cross-OS build and test                  |
| `release-rc.yml`     | push to `release/**`        | publish prerelease to nuget.org          |
| `version.yml`        | push to `master`            | upsert the stable draft release          |
| `release.yml`        | a GitHub release is published | build, pack, push stable package         |

## Before opening a PR

- Run `dotnet build` and `dotnet test`; CI treats warnings as errors.
- Keep diffs scoped; do not edit the public API surface files unless a
  milestone's done-criteria call for it (see `CLAUDE.md`).
