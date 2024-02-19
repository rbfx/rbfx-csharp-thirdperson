# Rebel Framework Thrid Person Project Template

[![IMAGE ALT TEXT HERE](https://img.youtube.com/vi/tzlmrk5sSJY/0.jpg)](https://www.youtube.com/watch?v=tzlmrk5sSJY)

## Install the Third-Person Project Template:

Open your terminal or command prompt.

Use the following command to install the rbfx.template.thirdperson template package from NuGet:

```
dotnet new install rbfx.template.thirdperson
```

This command will download and install the template package. If you want to install a specific version, you can specify it using the format rbfx.template.thirdperson::<package-version>.

## Create a New Solution Using the Installed Template:

Navigate to the directory where you want to create your solution.

Run the following command to generate a new solution based on the installed template:

```
dotnet new rbfx-thirdperson -n MyAwesomeGame
```

Replace MyAwesomeGame with your desired solution name.

This command will create a new solution with the necessary project structure and files based on the Third-Person template.

## Explore and Customize:

Inside the newly created solution folder (MyAwesomeGame in this example), you’ll find project files for the game, character, and other related components.

Customize the project files according to your requirements. You can add more game features, modify existing code, and create additional projects within the solution.

## Build and Run:

Build the solution using the following command:

```
dotnet build
```

Run your game using:

```
dotnet run --project MyAwesomeGame.Desktop/MyAwesomeGame.Desktop.csproj
```

This will compile your solution and execute the game.
Remember to adjust the commands and folder names based on your specific setup. Happy coding! 🚀🎮

For more information, you can refer to the [official documentation on installing and managing SDK templates](https://learn.microsoft.com/en-us/dotnet/core/install/templates).

# GitHub Actions Workflow Execution:

Whenever you push changes to the master branch, GitHub Actions will automatically trigger the workflow.

It will build your game, create a GitHub release, and upload the precompiled game artifact.

## Verify the Release:

Visit the [Releases](https://github.com/rbfx/rbfx-csharp-thirdperson/releases) section of your repository to verify that the release was created successfully.

## Optional: itch.io Publishing:

If you want to publish to itch.io setup itch.io token. Go to your GitHub repository.
Navigate to ```Settings > Secrets and variables```.
Add a new secret named **BUTLER_API_KEY** and set its value to your **itch.io** personal access token.
Add new variable **ITCH_PROJECT**  to the itch.io project id like [rebelfork/rbfx-csharp-thirdperson](https://rebelfork.itch.io/rbfx-csharp-thirdperson)

That’s it! Your GitHub Actions workflow will now build your game and make it available for download via GitHub Releases. Optionally, it can also publish to itch.io if configured. 🚀🎮
