# Releasing godotweens

The NuGet package ID is **`godotweens`**. GitHub hosts the source at
[`outfox/godotweens`](https://github.com/outfox/godotweens); `thygrrr` is the GitHub
maintainer account and the NuGet publishing username.

## Continuous integration

`.github/workflows/ci.yml` runs on pull requests, pushes to `main`, and manual
dispatches. The release workflow reuses the same build on the tagged commit.
On a Windows runner with .NET 10, it:

1. Restores and builds the solution in Release configuration.
2. Runs the test suite, including the native Godot/2dog tests.
3. Runs the testbed headlessly for 12 frames.
4. Packs the library, checks its contents and MIT metadata, and builds an isolated
   consumer against the resulting package.
5. Uploads `godotweens-packages` (`.nupkg` and `.snupkg`) and TRX test results as
   Actions artifacts.

Godot and 2dog arrive through NuGet; CI needs no separately installed editor.
The existing IL2125 trimming warnings do not fail the build. CI currently verifies
Windows; other operating systems and trimmed/AOT exports are not certified.
Actions are pinned to commit SHAs; Dependabot checks their revisions monthly.

## Create a release

Merge the release changes into `main` and confirm CI passes. Update the default
`Version` in `godotweens/godotweens.csproj` as appropriate, then tag the intended
commit. For example:

```powershell
git switch main
git pull --ff-only
git tag -a v0.1.0 -m 'godotweens 0.1.0'
git push origin v0.1.0
```

The tag must be `vMAJOR.MINOR.PATCH`, optionally with a SemVer prerelease suffix
such as `v0.2.0-rc.1`. Leading zeroes in numeric identifiers and build metadata
are rejected. The tag determines the assembly/package version for the release;
it overrides the development version in the project file.

`.github/workflows/release.yml` builds and tests that commit, then creates a GitHub
release with generated notes and attaches the `.nupkg` and `.snupkg`. Prerelease
tags create prereleases. Both publishing jobs use the exact packages produced by
that build. A failure in validation, tests, or packaging prevents publication.

The GitHub release is available even while NuGet publishing is disabled. Once
enabled, the subsequent NuGet job also pushes the package and its symbol package.
Publishing to NuGet is permanent: use a new version to correct a released package.
Do not move release tags. Prefer **Re-run failed jobs** for transient publishing
failures; rerunning the entire workflow rebuilds the package and replaces GitHub
release assets, while NuGet skips an already published version.

## Enable NuGet trusted publishing

The workflow uses [NuGet trusted publishing](https://learn.microsoft.com/en-us/nuget/nuget-org/trusted-publishing)
through [`NuGet/login`](https://github.com/NuGet/login). No long-lived API key or
GitHub secret is needed. Only the NuGet job can request an OIDC token.

Sign in to nuget.org as `thygrrr`, open **Trusted Publishing** from the account
menu, and create a GitHub policy with these exact values:

| Field | Value |
| --- | --- |
| Policy owner | `thygrrr` |
| GitHub repository owner | `outfox` |
| GitHub repository | `godotweens` |
| Workflow filename | `release.yml` |
| GitHub environment | `nuget` |
| Package pattern | `godotweens` |
| Scopes | Push new packages and new versions |

The GitHub owner is `outfox`, even though the authenticating user is `thygrrr`.
Use only the workflow filename in the policy, without `.github/workflows/`.
The new-package scope allows the first publication; the account must be eligible
to publish the package ID. For later releases, ownership must remain with the
policy owner.

The GitHub environment `nuget` should allow deployments only from tags matching
`v*`. The repository variable `NUGET_PUBLISH_ENABLED` is initially `false`.
After saving the NuGet policy, enable publication using the `thygrrr` account:

```powershell
gh auth switch --user thygrrr
gh variable set NUGET_PUBLISH_ENABLED --repo outfox/godotweens --body true
```

Then push the next release tag. To publish a release whose NuGet job was previously
skipped, enable the variable and rerun its whole release workflow, with the rebuild
behavior described above. To pause future NuGet publishing, set the variable back
to `false`; GitHub release uploads continue.

Account policy setup is separate from repository configuration: a GitHub login
does not grant access to the NuGet account. If the NuGet username changes, update
the workflow's `NuGet/login` input and review policy ownership before enabling it.
