# Recipe Schema

DemoForge recipes are YAML files stored by default at `.demoforge/demo.yml`.

## Top-level fields

- `version`: must be `1`
- `demo`: metadata and output settings
- `terminal`: terminal rendering settings for exporters
- `workingDirectory`: default working directory for commands
- `env`: extra environment variables
- `setup`: optional commands executed before demo steps
- `steps`: required demo commands
- `redact`: extra regex or literal redaction rules
- `export`: output formats and README snippet options

## Supported formats

- `transcript`
- `session-json`
- `markdown`
- `html`
- `readme-snippet`

`gif` and `mp4` are recognized by validation but intentionally rejected in this build because media export is not implemented yet.
