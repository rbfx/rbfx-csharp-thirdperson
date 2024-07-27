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
dotnet new rbfx.thirdperson -n MyAwesomeGame
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

That's it! Your GitHub Actions workflow will now build your game and make it available for download via GitHub Releases. Optionally, it can also publish to itch.io if configured.

## Optional: Steam Publishing

This action assumes you are registered as a [partner](https://partner.steamgames.com/) with Steam.

### Set STEAM_APPID and STEAM_USERNAME action secret

Set STEAM_USERNAME to the builder's user name.

Set STEAM_APPID to the application or demo id.

### Depots

The Github Action deploys into 3 depots:

- Depot 1: Operating System : Windows, Architecture : 64-bit OS Only
- Depot 2: Operating System : Linux + SteamOS, Architecture : 64-bit OS Only
- Depot 3: Operating System : macOS, Architecture : 64-bit OS Only

If either of these depots missing the publish_to_steam job will fail.

Once you are done defining your depots, **publish the changes** that you have made from the [Publish](https://partner.steamgames.com/apps/publishing) page. If you try to run Github Action before publishing the depots the action will fail to publish binaries.

### Create a Steam Build Account

Create a specialised builder account that only has access to `Edit App Metadata` and `Publish App Changes To Steam`,
and permissions to edit your specific app.

https://partner.steamgames.com/doc/sdk/uploading#Build_Account

### Set STEAM_CONFIG_VDF action secret

Deploying to Steam requires using Multi-Factor Authentication (MFA) through Steam Guard unless `totp` is passed.
This means that simply using username and password isn't enough to authenticate with Steam. 
However, it is possible to go through the MFA process only once by setting up GitHub Secrets for `configVdf` with these steps:
1. Install [Valve's offical steamcmd](https://partner.steamgames.com/doc/sdk/uploading#1) on your local machine. All following steps will also be done on your local machine. [Downloading_SteamCMD](https://developer.valvesoftware.com/wiki/SteamCMD#Downloading_SteamCMD)
1. Try to login with `steamcmd +login <username> <password> +quit`, which may prompt for the MFA code. If so, type in the MFA code that was emailed to your builder account's email address.
1. Validate that the MFA process is complete by running `steamcmd +login <username> +quit` again. It should not ask for the MFA code again.
1. The folder from which you run `steamcmd` will now contain an updated `config/config.vdf` file.
    1. Windows: Use certutil to convert config.vdf content to base64 encoded string ```certutil -encodehex -f config/config.vdf config_base64.txt 0x40000001 1>nul```
    1. Linux/MacOS:  Use ```cat config/config.vdf | base64 > config_base64.txt``` to encode the file.
1. Copy the contents of `config_base64.txt` to a GitHub Secret `STEAM_CONFIG_VDF`.
1. `If:` when running the action you recieve another MFA code via email, run `steamcmd +set_steam_guard_code <code>` on your local machine and repeat the `config.vdf` encoding and replace secret `STEAM_CONFIG_VDF` with its contents.

More documentation on steam publishing could be found at https://github.com/game-ci/steam-deploy

## Optional: Google Play Publishing

To publish app to Google Play directly from the GitHub Action you need to define several secrets in the pipeline.

### PLAY_KEYSTORE, PLAY_KEYSTORE_ALIAS and PLAY_KEYSTORE_PASS

First you need to generate Java Key Store file by running the following command:

```shell
keytool -v -genkey -v -keystore googleplay.jks -alias someKindOfName -keyalg RSA -validity 10000
```

**Don't user quotes " as part of the password, it may mess up the GitHub action scripts!**

Replace alias with a name related to you. Store the alias into PLAY_KEYSTORE_ALIAS secret of the GitHub pipeline. The password you set to the keystore should go into PLAY_KEYSTORE_PASS secret.

Also you need to store the whole content of the googleplay.jks file into the PLAY_KEYSTORE secret. The easy way of doing that is to encode the file content into base64 string and store the string value into the secret by running the following command on windows:

```shell
certutil -encodehex -f googleplay.jks googleplay.txt 0x40000001 1>nul
```

or using openssl elsewhere:

```shell
openssl base64 < googleplay.jks | tr -d '\n' | tee googleplay.txt
```

Upload the content of googleplay.txt to PLAY_KEYSTORE variable. 

**Dont' forget to delete googleplay.txt and keep the googleplay.jks in a safe place locally!**

More on this: https://thewissen.io/making-maui-cd-pipeline/

### PLAYSTORE_SERVICE_ACC

## Configure access via service account

This step use https://github.com/r0adkll/upload-google-play for the publishing. Here is what you need to do:

1. Enable the Google Play Android Developer API.
   1. Go to https://console.cloud.google.com/apis/library/androidpublisher.googleapis.com.
   1. Click on Enable.
1. Create a new service account in Google Cloud Platform ([docs](https://developers.google.com/android-publisher/getting_started#service-account)).
   1. Navigate to https://cloud.google.com/gcp.
   1. Open `Console` > `IAM & Admin` > `Credentials` > `Manage service accounts` > `Create service account`.
   1. Pick a name for the new account. Do not grant the account any permissions.
   1. To use it from the GitHub Action use either:
      - Account key in GitHub secrets (simpler):
        1. Open the newly created service account, click on `keys` tab and add a new key, JSON type.
        1. When successful, a JSON file will be automatically downloaded on your machine.
        1. Store the content of this file to your GitHub secrets, e.g. `PLAYSTORE_SERVICE_ACC`.
1. Add the service account to Google Play Console.
   1. Open https://play.google.com/console and pick your developer account.
   1. Open Users and permissions.
   1. Click invite new user and add the email of the service account created in the previous step.
   1. Grant permissions to the app that you want the service account to deploy in `app permissions`.
1. Create new application via Google Play Console
   1. Open https://play.google.com/console and pick your developer account.
   1. Press `Create App` and create new application using the same ApplicationId as in your c# project
   1. Make sure you upload an apk or aab manually first by creating a release through the play console.
