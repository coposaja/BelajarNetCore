---
title: Microsoft Account external login setup with ASP.NET Core
author: rick-anderson
description: This sample demonstrates the integration of Microsoft account user authentication into an existing ASP.NET Core app.
ms.author: riande
ms.custom: mvc
ms.date: 05/11/2019
uid: security/authentication/microsoft-logins
---
# Microsoft Account external login setup with ASP.NET Core

By [Valeriy Novytskyy](https://github.com/01binary) and [Rick Anderson](https://twitter.com/RickAndMSFT)

This sample shows you how to enable users to sign in with their Microsoft account using the ASP.NET Core 2.2 project created on the [previous page](xref:security/authentication/social/index).

## Create the app in Microsoft Developer Portal

* Navigate to the [Azure portal - App registrations](https://go.microsoft.com/fwlink/?linkid=2083908) page and create or sign into a Microsoft account:

If you don't have a Microsoft account, select **Create one**. After signing in you are redirected to the **App registrations** page:

* Select **New registration**
* Enter a **Name**.
* Select an option for **Supported account types**.  <!-- Accounts for any org work with MS domain accounts. Most folks probably want the last option, personal MS accounts -->
* Under **Redirect URI**, enter your development URL with `/signin-microsoft` appended. For example, `https://localhost:44389/signin-microsoft`. The Microsoft authentication scheme configured later in this sample will automatically handle requests at `/signin-microsoft` route to implement the OAuth flow.
* Select **Register**

### Create client secret

* In the left pane, select **Certificates & secrets**.
* Under **Client secrets**, select **New client secret**

  * Add a description for the client secret.
  * Select the **Add** button.

* Under **Client secrets**, copy the value of the client secret.

> [!NOTE]
> The URI segment `/signin-microsoft` is set as the default callback of the Microsoft authentication provider. You can change the default callback URI while configuring the Microsoft authentication middleware via the inherited [RemoteAuthenticationOptions.CallbackPath](/dotnet/api/microsoft.aspnetcore.authentication.remoteauthenticationoptions.callbackpath) property of the [MicrosoftAccountOptions](/dotnet/api/microsoft.aspnetcore.authentication.microsoftaccount.microsoftaccountoptions) class.

## Store the Microsoft client ID and client secret

Run the following commands to securely store `ClientId` and `ClientSecret` using [Secret Manager](xref:security/app-secrets):

```console
dotnet user-secrets set Authentication:Microsoft:ClientId <Client-Id>
dotnet user-secrets set Authentication:Microsoft:ClientSecret <Client-Secret>
```

Link sensitive settings like Microsoft `ClientId` and `ClientSecret` to your application configuration using the [Secret Manager](xref:security/app-secrets). For the purposes of this sample, name the tokens `Authentication:Microsoft:ClientId` and `Authentication:Microsoft:ClientSecret`.

[!INCLUDE[](~/includes/environmentVarableColon.md)]

## Configure Microsoft Account Authentication

Add the Microsoft Account service to `Startup.ConfigureServices`:

[!code-csharp[](~/security/authentication/social/social-code/StartupMS.cs?name=snippet&highlight=10-14)]

[!INCLUDE [default settings configuration](includes/default-settings.md)]

[!INCLUDE[](includes/chain-auth-providers.md)]

See the [MicrosoftAccountOptions](/dotnet/api/microsoft.aspnetcore.builder.microsoftaccountoptions) API reference for more information on configuration options supported by Microsoft Account authentication. This can be used to request different information about the user.

## Sign in with Microsoft Account

Run the and click **Log in**. An option to sign in with Microsoft appears. When you click on Microsoft, you are redirected to Microsoft for authentication. After signing in with your Microsoft Account (if not already signed in) you will be prompted to let the app access your info:

Tap **Yes** and you will be redirected back to the web site where you can set your email.

You are now logged in using your Microsoft credentials:

[!INCLUDE[Forward request information when behind a proxy or load balancer section](includes/forwarded-headers-middleware.md)]

## Troubleshooting

* If the Microsoft Account provider redirects you to a sign in error page, note the error title and description query string parameters directly following the `#` (hashtag) in the Uri.

  Although the error message seems to indicate a problem with Microsoft authentication, the most common cause is your application Uri not matching any of the **Redirect URIs** specified for the **Web** platform.
* If Identity isn't configured by calling `services.AddIdentity` in `ConfigureServices`, attempting to authenticate will result in *ArgumentException: The 'SignInScheme' option must be provided*. The project template used in this sample ensures that this is done.
* If the site database has not been created by applying the initial migration, you will get *A database operation failed while processing the request* error. Tap **Apply Migrations** to create the database and refresh to continue past the error.

## Next steps

* This article showed how you can authenticate with Microsoft. You can follow a similar approach to authenticate with other providers listed on the [previous page](xref:security/authentication/social/index).

* Once you publish your web site to Azure web app, create a new client secrets in the Microsoft Developer Portal.

* Set the `Authentication:Microsoft:ClientId` and `Authentication:Microsoft:ClientSecret` as application settings in the Azure portal. The configuration system is set up to read keys from environment variables.