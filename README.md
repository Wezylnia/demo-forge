# DemoForge

DemoForge turns CLI recipes into README-ready terminal demos.

Define your demo once in `.demoforge/demo.yml`, then generate clean Markdown, HTML, transcript, and session artifacts without opening a screen recorder.

```text
recipe -> command execution -> terminal session -> README-ready artifacts
```

DemoForge is built for CLI projects that need repeatable demos in documentation, release notes, blog posts, or product pages without manually recording the terminal every time.

## Why

Recording a polished CLI demo is usually manual:

- open a screen recorder
- run commands carefully
- crop the recording
- rewrite the same README snippet
- repeat the process after every product change

DemoForge replaces that with a deterministic recipe and a reproducible export pipeline.

## Status

- CLI demos only
- Recipe-driven and deterministic
- Local-first
- No cloud services
- No AI features
- No screen recording in the MVP
- GIF/MP4 planned through `DemoForge.Media`, not implemented yet

## What it does

- Parses a YAML recipe from `.demoforge/demo.yml`
- Validates commands, output settings, and export formats
- Runs setup steps and demo steps locally
- Captures stdout/stderr with timing metadata
- Applies built-in and custom redaction rules
- Exports transcript, session JSON, Markdown, HTML, and README snippet files

## Project layout

```text
src/
  DemoForge.Cli/      Command entrypoints and user-facing CLI
  DemoForge.Core/     Recipes, execution, capture, safety, rendering, export
  DemoForge.Media/    Reserved extension point for future GIF/MP4 support
tests/
  DemoForge.Core.Tests/
samples/
  cli-basic/
  fake-cli/
docs/
```

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

## Example recipe

```yaml
version: 1

demo:
  name: "My CLI Demo"
  outputDir: "assets/demo"

steps:
  - title: "Show help"
    command: "mytool --help"

export:
  formats:
    - transcript
    - session-json
    - markdown
    - html
    - readme-snippet
```

## Commands

- `demoforge init`
  Creates a starter `.demoforge/demo.yml`.
- `demoforge validate [recipe]`
  Validates a recipe without running commands.
- `demoforge run [recipe]`
  Executes the recipe and writes artifacts.
- `demoforge run --dry-run [recipe]`
  Shows which commands would run.
- `demoforge run --allow-dangerous [recipe]`
  Allows commands that match dangerous-command heuristics.
- `demoforge render <session.json> --format html,markdown`
  Re-renders artifacts from a captured session.

## Outputs

Each run can generate:

- `session.json`
- `transcript.txt`
- `demo.md`
- `demo.html`
- `README-snippet.md`
- `manifest.json`

Typical output:

```text
assets/demo/
  demo.html
  demo.md
  manifest.json
  README-snippet.md
  session.json
  transcript.txt
```

## Safety

- Suspicious commands such as `rm -rf` and `git clean -fdx` are blocked unless `--allow-dangerous` is used.
- Redaction runs before artifacts are written.
- Validation rejects known but unimplemented media formats such as `gif` and `mp4`.

## Development

```bash
dotnet build DemoForge.slnx
dotnet test DemoForge.slnx
dotnet run --project src/DemoForge.Cli -- run samples/cli-basic/.demoforge/demo.yml
```

## Limitations

- No browser or desktop automation
- No real screen recording
- No cloud publishing
- No GitHub Action integration yet
- No GIF/MP4 export in the current build

## Documentation

- [Recipe schema](docs/recipe-schema.md)
- [Examples](docs/examples.md)
