window.BENCHMARK_DATA = {
  "lastUpdate": 1782162929863,
  "repoUrl": "https://github.com/ernestohs/Munchausen",
  "entries": {
    "Munchausen benchmarks": [
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
        "date": 1782116205722,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Munchausen.Benchmarks.GenerationBenchmarks.BuildWarm",
            "value": 2353148.271223958,
            "unit": "ns",
            "range": "± 12767.393434681582"
          },
          {
            "name": "Munchausen.Benchmarks.GenerationBenchmarks.GenerateCar",
            "value": 1780.8051535742622,
            "unit": "ns",
            "range": "± 10.497056475489106"
          },
          {
            "name": "Munchausen.Benchmarks.GenerationBenchmarks.GenerateCars",
            "value": 1118044.1453125,
            "unit": "ns",
            "range": "± 8494.414296750909"
          },
          {
            "name": "Munchausen.Benchmarks.GenerationBenchmarks.GenerateCustomerWithCollection",
            "value": 5904.511262003581,
            "unit": "ns",
            "range": "± 58.20616096914185"
          },
          {
            "name": "Munchausen.Benchmarks.GenerationBenchmarks.PrngThroughput",
            "value": 3384.705641233004,
            "unit": "ns",
            "range": "± 2.4333053231602486"
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
        "date": 1782118797392,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Munchausen.Benchmarks.GenerationBenchmarks.BuildWarm",
            "value": 2478992.7434895835,
            "unit": "ns",
            "range": "± 10902.165917299235"
          },
          {
            "name": "Munchausen.Benchmarks.GenerationBenchmarks.GenerateCar",
            "value": 1873.0454896291096,
            "unit": "ns",
            "range": "± 14.881236733674871"
          },
          {
            "name": "Munchausen.Benchmarks.GenerationBenchmarks.GenerateCars",
            "value": 1079765.7821614584,
            "unit": "ns",
            "range": "± 5986.890329302844"
          },
          {
            "name": "Munchausen.Benchmarks.GenerationBenchmarks.GenerateCustomerWithCollection",
            "value": 5874.588126627604,
            "unit": "ns",
            "range": "± 59.57340374056493"
          },
          {
            "name": "Munchausen.Benchmarks.GenerationBenchmarks.PrngThroughput",
            "value": 3377.438555908203,
            "unit": "ns",
            "range": "± 4.7903265962163095"
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
        "date": 1782140770527,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Munchausen.Benchmarks.GenerationBenchmarks.BuildWarm",
            "value": 2438775.728645833,
            "unit": "ns",
            "range": "± 14564.085426227075"
          },
          {
            "name": "Munchausen.Benchmarks.GenerationBenchmarks.GenerateCar",
            "value": 1798.6534299850464,
            "unit": "ns",
            "range": "± 5.000845534678046"
          },
          {
            "name": "Munchausen.Benchmarks.GenerationBenchmarks.GenerateCars",
            "value": 1141274.4522235577,
            "unit": "ns",
            "range": "± 1580.9380614611275"
          },
          {
            "name": "Munchausen.Benchmarks.GenerationBenchmarks.GenerateCustomerWithCollection",
            "value": 5933.072186396672,
            "unit": "ns",
            "range": "± 10.950296124575557"
          },
          {
            "name": "Munchausen.Benchmarks.GenerationBenchmarks.PrngThroughput",
            "value": 3388.2081838754507,
            "unit": "ns",
            "range": "± 8.57349247450997"
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
        "date": 1782142020293,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Munchausen.Benchmarks.GenerationBenchmarks.BuildWarm",
            "value": 2513481.881119792,
            "unit": "ns",
            "range": "± 17462.17783144408"
          },
          {
            "name": "Munchausen.Benchmarks.GenerationBenchmarks.GenerateCar",
            "value": 1906.3602414449056,
            "unit": "ns",
            "range": "± 25.409891779498295"
          },
          {
            "name": "Munchausen.Benchmarks.GenerationBenchmarks.GenerateCars",
            "value": 1106075.6791666667,
            "unit": "ns",
            "range": "± 10521.371566597803"
          },
          {
            "name": "Munchausen.Benchmarks.GenerationBenchmarks.GenerateCustomerWithCollection",
            "value": 5876.441549682617,
            "unit": "ns",
            "range": "± 35.49993268711347"
          },
          {
            "name": "Munchausen.Benchmarks.GenerationBenchmarks.PrngThroughput",
            "value": 3122.5273933410645,
            "unit": "ns",
            "range": "± 4.10619537942974"
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
        "date": 1782142652885,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Munchausen.Benchmarks.GenerationBenchmarks.BuildWarm",
            "value": 2423839.0643028845,
            "unit": "ns",
            "range": "± 17783.970294973245"
          },
          {
            "name": "Munchausen.Benchmarks.GenerationBenchmarks.GenerateCar",
            "value": 1736.7710287911552,
            "unit": "ns",
            "range": "± 9.219920690677453"
          },
          {
            "name": "Munchausen.Benchmarks.GenerationBenchmarks.GenerateCars",
            "value": 1115740.2865885417,
            "unit": "ns",
            "range": "± 2211.7504147161108"
          },
          {
            "name": "Munchausen.Benchmarks.GenerationBenchmarks.GenerateCustomerWithCollection",
            "value": 5897.711029639611,
            "unit": "ns",
            "range": "± 17.40790105551649"
          },
          {
            "name": "Munchausen.Benchmarks.GenerationBenchmarks.PrngThroughput",
            "value": 3386.8287130991616,
            "unit": "ns",
            "range": "± 6.481791266357326"
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
          "id": "23b6a42cc446189ab20cb7b5a0bd027142c4efe1",
          "message": "Bump coverlet.collector from 6.0.2 to 10.0.1 (#18)\n\n---\nupdated-dependencies:\n- dependency-name: coverlet.collector\n  dependency-version: 10.0.1\n  dependency-type: direct:production\n  update-type: version-update:semver-major\n- dependency-name: coverlet.collector\n  dependency-version: 10.0.1\n  dependency-type: direct:production\n  update-type: version-update:semver-major\n- dependency-name: coverlet.collector\n  dependency-version: 10.0.1\n  dependency-type: direct:production\n  update-type: version-update:semver-major\n...\n\nSigned-off-by: dependabot[bot] <support@github.com>\nCo-authored-by: dependabot[bot] <49699333+dependabot[bot]@users.noreply.github.com>",
          "timestamp": "2026-06-22T10:12:30-06:00",
          "tree_id": "c3ef6995efed2817415df585b177fc010b882f9a",
          "url": "https://github.com/ernestohs/Munchausen/commit/23b6a42cc446189ab20cb7b5a0bd027142c4efe1"
        },
        "date": 1782144931782,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Munchausen.Benchmarks.GenerationBenchmarks.BuildWarm",
            "value": 2397326.8255208335,
            "unit": "ns",
            "range": "± 5661.156765342607"
          },
          {
            "name": "Munchausen.Benchmarks.GenerationBenchmarks.GenerateCar",
            "value": 1731.5680054982504,
            "unit": "ns",
            "range": "± 12.436347075678395"
          },
          {
            "name": "Munchausen.Benchmarks.GenerationBenchmarks.GenerateCars",
            "value": 1110370.6260516827,
            "unit": "ns",
            "range": "± 4087.0543296847545"
          },
          {
            "name": "Munchausen.Benchmarks.GenerationBenchmarks.GenerateCustomerWithCollection",
            "value": 5882.178173700969,
            "unit": "ns",
            "range": "± 12.087829673828798"
          },
          {
            "name": "Munchausen.Benchmarks.GenerationBenchmarks.PrngThroughput",
            "value": 3382.825035367693,
            "unit": "ns",
            "range": "± 1.1876385104115843"
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
          "id": "82cb700ca6fe5de929b46fcb6e8a1de550f0a8ca",
          "message": "Bump Microsoft.NET.Test.Sdk from 17.11.1 to 18.6.0 (#19)\n\n---\nupdated-dependencies:\n- dependency-name: Microsoft.NET.Test.Sdk\n  dependency-version: 18.6.0\n  dependency-type: direct:production\n  update-type: version-update:semver-major\n- dependency-name: Microsoft.NET.Test.Sdk\n  dependency-version: 18.6.0\n  dependency-type: direct:production\n  update-type: version-update:semver-major\n- dependency-name: Microsoft.NET.Test.Sdk\n  dependency-version: 18.6.0\n  dependency-type: direct:production\n  update-type: version-update:semver-major\n...\n\nSigned-off-by: dependabot[bot] <support@github.com>\nCo-authored-by: dependabot[bot] <49699333+dependabot[bot]@users.noreply.github.com>\nCo-authored-by: /dev/10x <ernesto@talos-lab.ai>",
          "timestamp": "2026-06-22T10:38:50-06:00",
          "tree_id": "50e236e01e79abe9c589811db619d8ed08070881",
          "url": "https://github.com/ernestohs/Munchausen/commit/82cb700ca6fe5de929b46fcb6e8a1de550f0a8ca"
        },
        "date": 1782146523905,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Munchausen.Benchmarks.GenerationBenchmarks.BuildWarm",
            "value": 2371560.116736779,
            "unit": "ns",
            "range": "± 7540.65170522077"
          },
          {
            "name": "Munchausen.Benchmarks.GenerationBenchmarks.GenerateCar",
            "value": 1737.4725476582846,
            "unit": "ns",
            "range": "± 15.531808982501564"
          },
          {
            "name": "Munchausen.Benchmarks.GenerationBenchmarks.GenerateCars",
            "value": 1111148.642299107,
            "unit": "ns",
            "range": "± 2481.014269103323"
          },
          {
            "name": "Munchausen.Benchmarks.GenerationBenchmarks.GenerateCustomerWithCollection",
            "value": 5814.078096117292,
            "unit": "ns",
            "range": "± 11.742625180256617"
          },
          {
            "name": "Munchausen.Benchmarks.GenerationBenchmarks.PrngThroughput",
            "value": 3385.661572969877,
            "unit": "ns",
            "range": "± 3.2971475769057803"
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
          "id": "5449d26872a62f33852e1af1cc5c97bae6fc3b4b",
          "message": "ci: GitFlow release pipeline with MinVer and conventional-commit drafts (#20)",
          "timestamp": "2026-06-22T15:12:14-06:00",
          "tree_id": "90352a69ffc953287e7a3393382f86a62d6d6cd5",
          "url": "https://github.com/ernestohs/Munchausen/commit/5449d26872a62f33852e1af1cc5c97bae6fc3b4b"
        },
        "date": 1782162929022,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Munchausen.Benchmarks.GenerationBenchmarks.BuildWarm",
            "value": 2391761.3557477677,
            "unit": "ns",
            "range": "± 13515.725436894898"
          },
          {
            "name": "Munchausen.Benchmarks.GenerationBenchmarks.GenerateCar",
            "value": 1754.3338550840106,
            "unit": "ns",
            "range": "± 3.1684619484846346"
          },
          {
            "name": "Munchausen.Benchmarks.GenerationBenchmarks.GenerateCars",
            "value": 1110768.7315204327,
            "unit": "ns",
            "range": "± 1686.3022345311058"
          },
          {
            "name": "Munchausen.Benchmarks.GenerationBenchmarks.GenerateCustomerWithCollection",
            "value": 5953.250371042887,
            "unit": "ns",
            "range": "± 46.2480000074"
          },
          {
            "name": "Munchausen.Benchmarks.GenerationBenchmarks.PrngThroughput",
            "value": 3385.5826105390274,
            "unit": "ns",
            "range": "± 2.3802075351237413"
          }
        ]
      }
    ]
  }
}