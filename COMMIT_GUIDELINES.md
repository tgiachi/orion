# Commit Guidelines

This project uses [Conventional Commits](https://www.conventionalcommits.org/) to automate semantic versioning.

## Commit Message Format

```
<type>[optional scope]: <description>

[optional body]

[optional footer]
```

### Commit Types

- `feat`: A new feature (increments the MINOR version)
- `fix`: A bug fix (increments the PATCH version)
- `docs`: Documentation changes
- `style`: Changes that do not affect the meaning of the code (spaces, formatting, etc.)
- `refactor`: Code changes that neither fix a bug nor add a feature
- `perf`: Changes that improve performance
- `test`: Adding or correcting tests
- `build`: Changes to the build system or external dependencies
- `ci`: Changes to CI files and scripts
- `chore`: Other changes that don't modify source or test files

### Breaking Changes

To indicate a breaking change (increments the MAJOR version), add `BREAKING CHANGE:` in the footer or add `!` after the type:

```
feat!: new API that breaks compatibility
```

or

```
feat: new feature

BREAKING CHANGE: detailed description of the change
```

### Examples

```
feat: add new login functionality

fix(auth): solve issue with JWT authentication

docs: update API documentation

refactor(core): migrate from ASP.NET Core 7 to 9

feat!: complete redesign of the public API
```

When you push or merge to the main branch, GitHub Actions will automatically run Versionize to update the version based on commits made since the last release.
