window.BENCHMARK_DATA = {
  "lastUpdate": 1782116061853,
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
      }
    ]
  }
}