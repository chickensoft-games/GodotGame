# Chickensoft.GodotGame

[![Chickensoft Badge][chickensoft-badge]][chickensoft-website] [![Discord][discord-badge]][discord] [![Read the docs][read-the-docs-badge]][docs] ![line coverage][line-coverage] ![branch coverage][branch-coverage]

C# game template for Godot 4 with debug launch configurations, testing (locally and on CI/CD), code coverage, dependency update checks, and spell check working out-of-the-box!

---

<p align="center">
<img alt="Cardboard Box with Chickensoft Logo" src="icon.png" width="200">
</p>

## 🥚 Getting Started

This template allows you to easily create a C# game for Godot 4. Microsoft's `dotnet` tool allows you to easily create, install, and use templates.

```sh
# Install this template
dotnet new --install Chickensoft.GodotGame

# Generate a new project based on this template
dotnet new chickengame --name "MyGameName" --param:author "My Name"

cd MyGameName
dotnet build
```

## 💁 Getting Help

*Is this template broken? Encountering obscure C# build problems?* We'll be happy to help you in the [Chickensoft Discord server][discord].

## 🏝 Environment Setup

For the provided debug configurations and test coverage to work correctly, you must setup your development environment correctly. The [Chickensoft Setup Docs][setup-docs] describe how to setup your Godot and C# development environment, using Chickensoft's best practice recommendations.

### VSCode Settings

This template includes some Visual Studio Code settings in `.vscode/settings.json`. The settings facilitate terminal environments on Windows (Git Bash, PowerShell, Command Prompt) and macOS (zsh), as well as fixing some syntax colorization issues that Omnisharp suffers from. You'll also find settings that enable editor config support in Omnisharp and the .NET Roslyn analyzers for a more enjoyable coding experience.

> Please double-check that the provided VSCode settings don't conflict with your existing settings.

## .NET Versioning

The included [`global.json`](./global.json) specifies the version of the .NET SDK and `Godot.NET.Sdk` that the game should use. Using a `global.json` file allows [Renovatebot] to provide your repository with automatic dependency update pull requests whenever a new version of [GodotSharp] is released.

## 👷 Testing

An example test is included in `test/src/GameTest.cs` that demonstrates how to write a test for your package using [GoDotTest] and [godot-test-driver].

> [GoDotTest] is an easy-to-use testing framework for Godot and C# that allows you to run tests from the command line, collect code coverage, and debug tests in VSCode.

Tests run directly inside the game. The `.csproj` file is already pre-configured to prevent test scripts and test-only package dependencies from being included in release builds of your game!

On CI/CD, software graphics drivers from [mesa] emulate a virtual graphics device for Godot to render to, allowing you to run visual tests in a headless environment.

## 🏁 Application Entry Point

The `Main.tscn` and `Main.cs` scene and script file are the entry point of your game. In general, you probably won't need to modify these unless you're doing something highly custom.

If the game is running a release build, the `Main.cs` file will just immediately change the scene to `src/Game.tscn`. If the game is running in debug mode *and* GoDotTest has received the correct command line arguments to begin testing, the game will switch to the testing scene and hand off control to GoDotTest to run the game's tests.

In general, prefer editing `src/Game.tscn` over `src/Main.tscn`.

The provided debug configurations in `.vscode/launch.json` allow you to easily debug tests (or just the currently open test, provided its filename matches its class name).

## 🚦 Test Coverage

Code coverage requires a few `dotnet` global tools to be installed first. You should install these tools from the root of the project directory.

The `nuget.config` file in the root of the project allows the correct version of `coverlet` to be installed from the coverlet nightly distributions. Overriding the coverlet version will be required [until coverlet releases a stable version with the fixes that allow it to work with Godot 4][coverlet-issues].

```sh
dotnet tool install --global coverlet.console
dotnet tool update --global coverlet.console
dotnet tool install --global dotnet-reportgenerator-globaltool
dotnet tool update --global dotnet-reportgenerator-globaltool
```

> Running `dotnet tool update` for the global tool is often necessary on Apple Silicon computers to ensure the tools are installed correctly.

You can collect code coverage and generate coverage badges by running the bash script in `coverage.sh` (on Windows, you can use the Git Bash shell that comes with git).

```sh
# Must give coverage script permission to run the first time it is used.
chmod +x ./coverage.sh

# Run code coverage:
./coverage.sh
```

You can also run test coverage through VSCode by opening the command palette and selecting `Tasks: Run Task` and then choosing `coverage`.

If you are having trouble with `coverlet` finding your .NET runtime on Windows, you can use the PowerShell Script `coverage.ps1` instead.

```ps
.\coverage.ps1
```

## ⏯ Running the Project

Several launch profiles are included for Visual Studio Code:

- 🕹 **Debug Game**
  
  Runs the game in debug mode, allowing you to set breakpoints and inspect variables.

- 🎭 **Debug Current Scene**

  Debugs the game and loads the scene with the **same name** and **in the same path** as the C# file that's actively selected in VSCode: e.g., a scene named `MyScene.tscn` must reside in the same directory as `MyScene.cs`, and you must have selected `MyScene.cs` as the active tab in VSCode before running the launch profile.
  
  If GoDotTest is able to find a `.tscn` file with the same name in the same location, it will run the game in debug mode and load the scene.

  > Naturally, Chickensoft recommends naming scenes after the C# script they use and keeping them in the same directory so that you can take advantage of this launch profile.
  >
  > ⚠️ It's very easy to rename a script class but forget to rename the scene file, or vice-versa. When that happens, this launch profile will pass in the *expected* name of the scene file based on the script's name, but Godot will fail to find a scene with that name since the script name and scene name are not the same.

- 🧪 **Debug Tests**

  Runs the game in debug mode, specifying the command line flags needed by GoDotTest to run the tests. Debugging works the same as usual, allowing you to set breakpoints within the game's C# test files.

- 🔬 **Debug Current Test**

  Debugs the game and loads the test class with the **same name** as the C# file that's actively selected in VSCode: e.g., a test file named `MyTest.cs` must contain a test class named `MyTest`, and you must have selected `MyTest.cs` as the active tab in VSCode before running the launch profile.

  > ⚠️ It's very easy to rename a test class but forget to rename the test file, or vice-versa. When that happens, this launch profile will pass in the name of the file but GoDotTest will fail to find a class with that name since the filename and class name are not the same.

Note that each launch profile will trigger a build (see `./.vscode/tasks.json`) before debugging the game.

> ⚠️ **Important:** You must setup a `GODOT4` environment variable for the launch configurations above. If you haven't done so already, please see the [Chickensoft Setup Docs][setup-docs].

## 🏭 CI/CD

This game includes various GitHub Actions workflows to help with development.

### 🚥 Tests

Tests run directly inside the GitHub runner machine (using [chickensoft-games/setup-godot]) on every push to the repository. If the tests fail to pass, the workflow will also fail to pass.

You can configure which simulated graphics environments (`vulkan` and/or `opengl3`) you want to run tests on in [`.github/workflows/visual_tests.yaml`](.github/workflows/visual_tests.yaml).

Currently, tests can only be run from the `ubuntu` runners. If you know how to make the workflow install mesa and a virtual window manager on macOS and Windows, we'd love to hear from you!

Tests are executed by running the Godot test project in `Chickensoft.GodotGame` from the command line and passing in the relevant arguments to Godot so that [GoDotTest] can discover and run tests.

### 🧑‍🏫 Spellcheck

A spell check runs on every push to the repository. The spellcheck workflow settings can be configured in [`.github/workflows/spellcheck.yaml`](.github/workflows/spellcheck.yaml).

The [Code Spell Checker][cspell] plugin for VSCode is recommended to help you catch typos before you commit them. If you need add a word to the dictionary or ignore a certain path, you can edit the project's `cspell.json` file.

You can also words to the local `cspell.json` file from VSCode by hovering over a misspelled word and selecting `Quick Fix...` and then `Add "{word}" to config: cspell.json`.

![Fix Spelling](docs/spelling_fix.png)

### 🗂 Version Change

The included workflow in [`.github/workflows/version_change.yaml`](.github/workflows/version_change.yaml) can be manually dispatched to open a pull request that replaces the version number in `Chickensoft.GodotGame.csproj` with the version you specify in the workflow's inputs.

![Version Change Workflow](docs/version_change.png)

### 📦 Publish to Nuget

The included workflow in [`.github/workflows/publish.yaml`](.github/workflows/publish.yaml) can be manually dispatched when you're ready to publish your package to Nuget.

> To publish to nuget, you need a repository or organization secret named `NUGET_API_KEY` that contains your Nuget API key. The `NUGET_API_KEY` must be a GitHub actions secret to keep it safe!

![Publish Workflow](docs/publish.png)

### 🏚 Renovatebot

This repository includes a [`renovate.json`](./renovate.json) configuration for use with [Renovatebot]. Renovatebot can automatically open pull requests to help you keep your dependencies up to date when it detects new dependency versions have been released. Because Godot has such a rapid release cycle, automating dependency updates can be a huge time saver if you're trying to stay on the latest version of Godot.

![Renovatebot Pull Request](docs/renovatebot_pr.png)

> Unlike Dependabot, Renovatebot is able to combine all dependency updates into a single pull request — a must-have for Godot C# repositories where each sub-project needs the same Godot.NET.Sdk versions. If dependency version bumps were split across multiple repositories, the builds would fail in CI.

The easiest way to add Renovatebot to your repository is to [install it from the GitHub Marketplace][get-renovatebot]. Note that you have to grant it access to each organization and repository you want it to monitor.

The included `renovate.json` includes a few configuration options to limit how often Renovatebot can open pull requests as well as regex's to filter out some poorly versioned dependencies to prevent invalid dependency version updates.

---

🐣 Package generated from a 🐤 Chickensoft Template — <https://chickensoft.games>

<!-- Links -->

<!-- Header -->
[chickensoft-badge]: https://raw.githubusercontent.com/chickensoft-games/chickensoft_site/main/static/img/badges/chickensoft_badge.svg
[chickensoft-website]: https://chickensoft.games
[discord-badge]: https://raw.githubusercontent.com/chickensoft-games/chickensoft_site/main/static/img/badges/discord_badge.svg
[discord]: https://discord.gg/gSjaPgMmYW
[read-the-docs-badge]: https://raw.githubusercontent.com/chickensoft-games/chickensoft_site/main/static/img/badges/read_the_docs_badge.svg
[docs]: https://chickensoft.games/docs
[line-coverage]: badges/line_coverage.svg
[branch-coverage]: badges/branch_coverage.svg

<!-- Article -->
[GoDotTest]: https://github.com/chickensoft-games/go_dot_test
[setup-docs]: https://chickensoft.games/docs/setup
[cspell]: https://marketplace.visualstudio.com/items?itemName=streetsidesoftware.code-spell-checker
[Renovatebot]: https://www.mend.io/free-developer-tools/renovate/
[get-renovatebot]: https://github.com/apps/renovate
[godot-test-driver]: https://github.com/derkork/godot-test-driver
[coverlet-issues]: https://github.com/coverlet-coverage/coverlet/issues/1422
[GodotSharp]: https://www.nuget.org/packages/GodotSharp/
[chickensoft-games/setup-godot]: https://github.com/chickensoft-games/setup-godot
