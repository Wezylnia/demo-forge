# DemoForge

DemoForge turns CLI recipes into README-ready terminal demos.

Define your demo once in `.demoforge/demo.yml`, then generate clean Markdown, HTML, transcript, and session artifacts without opening a screen recorder.

## Status

- CLI demos only
- Recipe-driven and deterministic
- Local-first
- No cloud services
- No AI features
- No screen recording in the MVP
- GIF/MP4 planned through `DemoForge.Media`, not implemented yet

## Quick start

```bash
dotnet run --project src/DemoForge.Cli -- init
dotnet run --project src/DemoForge.Cli -- validate
dotnet run --project src/DemoForge.Cli -- run
```

Or run the included sample:

```bash
dotnet run --project src/DemoForge.Cli -- run samples/cli-basic/.demoforge/demo.yml
```

## Commands

- `demoforge init`
- `demoforge validate`
- `demoforge run`
- `demoforge render`

## Outputs

Each run can generate:

- `session.json`
- `transcript.txt`
- `demo.md`
- `demo.html`
- `README-snippet.md`
- `manifest.json`

## Structure

```text
src/
  DemoForge.Cli/
  DemoForge.Core/
  DemoForge.Media/
tests/
  DemoForge.Core.Tests/
samples/
  cli-basic/
  fake-cli/
docs/
```

## Notes

The core pipeline is:

```text
recipe -> command execution -> terminal session -> exported artifacts
```

That keeps the MVP repeatable, testable, and independent from screen capture tooling.
