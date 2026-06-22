window.BENCHMARK_DATA = {
  "lastUpdate": 1782142503537,
  "repoUrl": "https://github.com/ernestohs/Munchausen",
  "entries": {
    "Assembly size": [
      {
        "commit": {
          "author": {
            "email": "ernesto@talos-lab.ai",
            "name": "/dev/10x",
            "username": "ernestohs"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "9a87786033c7f76541e76e3baf5c1ca39863facf",
          "message": "ci: quality metrics + cross-OS test matrix (#13)\n\n* ci: add quality metrics tracking and cross-OS test matrix\n\nSet up trend tracking for coverage, package size, and benchmarks so each\niteration of the package can be compared over time, and exercise the\ndeterministic seeded output on every supported platform.\n\nNew workflow (.github/workflows/quality.yml):\n- Collects line, branch, and method coverage via coverlet.collector and\n  ReportGenerator, measures the release assembly size, and runs the\n  BenchmarkDotNet suite.\n- Publishes all three as trend series to gh-pages using\n  github-action-benchmark, and writes a snapshot table to the job summary\n  on every run.\n- Benchmarks are skipped on pull requests (expensive and noisy on shared\n  runners); they run on pushes to master and on manual dispatch.\n- Trend history is written only on pushes to master, so a manual\n  workflow_dispatch run validates the whole pipeline without polluting the\n  recorded data.\n\nCross-platform CI (.github/workflows/ci.yml):\n- Build and test now run on ubuntu-latest, windows-latest, and\n  macos-latest (arm64) with fail-fast disabled, so a platform-specific\n  golden or PRNG break is surfaced instead of hidden behind a single\n  Linux/x64 run.\n\nSupporting changes:\n- Add coverlet.collector to the three test projects for coverage\n  collection.\n- Ignore TestResults and BenchmarkDotNet.Artifacts output directories.\n\n* ci: Skip gh-pages re-fetching on second benchmark step",
          "timestamp": "2026-06-22T02:13:00-06:00",
          "tree_id": "4e0bb8dee49c9457fc4b741123657a05bee6db5f",
          "url": "https://github.com/ernestohs/Munchausen/commit/9a87786033c7f76541e76e3baf5c1ca39863facf"
        },
        "date": 1782116061835,
        "tool": "customSmallerIsBetter",
        "benches": [
          {
            "name": "Assembly size",
            "value": 129.5,
            "unit": "KB"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "ernesto@talos-lab.ai",
            "name": "/dev/10x",
            "username": "ernestohs"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "45fcd067510678ad96212c8473575aac6e3c9a33",
          "message": "Update release (#14)",
          "timestamp": "2026-06-22T02:56:45-06:00",
          "tree_id": "6c78efda580ab357f5c8530720c13948304398f9",
          "url": "https://github.com/ernestohs/Munchausen/commit/45fcd067510678ad96212c8473575aac6e3c9a33"
        },
        "date": 1782118651478,
        "tool": "customSmallerIsBetter",
        "benches": [
          {
            "name": "Assembly size",
            "value": 129.5,
            "unit": "KB"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "49699333+dependabot[bot]@users.noreply.github.com",
            "name": "dependabot[bot]",
            "username": "dependabot[bot]"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "06cddab93d73748ed823fb2e260b2381995afdd5",
          "message": "Bump actions/checkout from 4 to 7 (#15)\n\nBumps [actions/checkout](https://github.com/actions/checkout) from 4 to 7.\n- [Release notes](https://github.com/actions/checkout/releases)\n- [Changelog](https://github.com/actions/checkout/blob/main/CHANGELOG.md)\n- [Commits](https://github.com/actions/checkout/compare/v4...v7)\n\n---\nupdated-dependencies:\n- dependency-name: actions/checkout\n  dependency-version: '7'\n  dependency-type: direct:production\n  update-type: version-update:semver-major\n...\n\nSigned-off-by: dependabot[bot] <support@github.com>\nCo-authored-by: dependabot[bot] <49699333+dependabot[bot]@users.noreply.github.com>",
          "timestamp": "2026-06-22T09:02:05-06:00",
          "tree_id": "19d974778f0905d11becdb7f978f683718135fd7",
          "url": "https://github.com/ernestohs/Munchausen/commit/06cddab93d73748ed823fb2e260b2381995afdd5"
        },
        "date": 1782140602126,
        "tool": "customSmallerIsBetter",
        "benches": [
          {
            "name": "Assembly size",
            "value": 129.5,
            "unit": "KB"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "49699333+dependabot[bot]@users.noreply.github.com",
            "name": "dependabot[bot]",
            "username": "dependabot[bot]"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "8bc1bf6d67b9fe355c4dc6b155b9bf0921e42884",
          "message": "Bump actions/setup-dotnet from 4 to 5 (#16)\n\nBumps [actions/setup-dotnet](https://github.com/actions/setup-dotnet) from 4 to 5.\n- [Release notes](https://github.com/actions/setup-dotnet/releases)\n- [Commits](https://github.com/actions/setup-dotnet/compare/v4...v5)\n\n---\nupdated-dependencies:\n- dependency-name: actions/setup-dotnet\n  dependency-version: '5'\n  dependency-type: direct:production\n  update-type: version-update:semver-major\n...\n\nSigned-off-by: dependabot[bot] <support@github.com>\nCo-authored-by: dependabot[bot] <49699333+dependabot[bot]@users.noreply.github.com>",
          "timestamp": "2026-06-22T09:24:05-06:00",
          "tree_id": "bd02dde2132c0c0a9dcfd478e2d26382ee182791",
          "url": "https://github.com/ernestohs/Munchausen/commit/8bc1bf6d67b9fe355c4dc6b155b9bf0921e42884"
        },
        "date": 1782141890685,
        "tool": "customSmallerIsBetter",
        "benches": [
          {
            "name": "Assembly size",
            "value": 129.5,
            "unit": "KB"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "49699333+dependabot[bot]@users.noreply.github.com",
            "name": "dependabot[bot]",
            "username": "dependabot[bot]"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "e6c08a4689a3a6a2e4f2df312243969c02ac1808",
          "message": "Bump actions/upload-artifact from 4 to 7 (#17)\n\nBumps [actions/upload-artifact](https://github.com/actions/upload-artifact) from 4 to 7.\n- [Release notes](https://github.com/actions/upload-artifact/releases)\n- [Commits](https://github.com/actions/upload-artifact/compare/v4...v7)\n\n---\nupdated-dependencies:\n- dependency-name: actions/upload-artifact\n  dependency-version: '7'\n  dependency-type: direct:production\n  update-type: version-update:semver-major\n...\n\nSigned-off-by: dependabot[bot] <support@github.com>\nCo-authored-by: dependabot[bot] <49699333+dependabot[bot]@users.noreply.github.com>",
          "timestamp": "2026-06-22T09:34:10-06:00",
          "tree_id": "c89b33602b206a496a7cce2f869372b79ead0249",
          "url": "https://github.com/ernestohs/Munchausen/commit/e6c08a4689a3a6a2e4f2df312243969c02ac1808"
        },
        "date": 1782142503515,
        "tool": "customSmallerIsBetter",
        "benches": [
          {
            "name": "Assembly size",
            "value": 129.5,
            "unit": "KB"
          }
        ]
      }
    ]
  }
}