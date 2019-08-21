---
title: Facebook, Google, and external provider authentication in ASP.NET Core
author: rick-anderson
description: This tutorial demonstrates how to build an ASP.NET Core 2.x app using OAuth 2.0 with external authentication providers.
ms.author: riande
ms.custom: mvc
ms.date: 05/10/2019
uid: security/authentication/social/index
---
# Facebook, Google, and external provider authentication in ASP.NET Core

By [Valeriy Novytskyy](https://github.com/01binary) and [Rick Anderson](https://twitter.com/RickAndMSFT)

This tutorial demonstrates how to build an ASP.NET Core 2.2 app that enables users to sign in using OAuth 2.0 with credentials from external authentication providers.

[Facebook](xref:security/authentication/facebook-logins), [Twitter](xref:security/authentication/twitter-logins), [Google](xref:security/authentication/google-logins), and [Microsoft](xref:security/authentication/microsoft-logins) providers are covered in the following sections. Other providers are available in third-party packages such as [AspNet.Security.OAuth.Providers](https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers) and [AspNet.Security.OpenId.Providers](https://github.com/aspnet-contrib/AspNet.Security.OpenId.Providers).

![Social media icons for Facebook, Twitter, Google plus, and Windows](index/_static/social.png)

Enabling users to sign in with their existing credentials:
* Is convenient for the users.
* Shifts many of the complexities of managing the sign-in process onto a third party. 

For examples of how social logins can drive traffic and customer conversions, see case studies by [Facebook](https://www.facebook.com/unsupportedbrowser) and [Twitter](https://dev.twitter.com/resources/case-studies).

## Create a New ASP.NET Core Project

# [Visual Studio](#tab/visual-studio)

* Create a new project.
* Select **ASP.NET Core Web Application** and **Next**.
* Provide a **Project name** and confirm or change the **Location**. Select **Create**.
* Select **ASP.NET Core 2.2** in the drop down. Select **Web Application** in the template list.
* Under **Authentication**, select **Change** and set the authentication to **Individual User Accounts**. Select **OK**.
* In the **Create a new ASP.NET Core Web Application** window, select **Create**.

# [Visual Studio Code](#tab/visual-studio-code)

* Open the [integrated terminal](https://code.visualstudio.com/docs/editor/integrated-terminal).

* Change directories (`cd`) to a folder which will contain the project.

* Run the following commands:

  ```console
  dotnet new webapp -o WebApp1 -au Individual -uld
  code -r WebApp1
  ```

  * The `dotnet new` command creates a new Razor Pages project in the *WebApp1* folder.
  * `-uld` uses LocalDB instead of SQLite. Omit `-uld` to use SQLite.
  * `-au Individual` creates the code for Individual authentication.
  * The `code` command opens the *WebApp1* folder in a new instance of Visual Studio Code.

* A dialog box appears with **Required assets to build and debug are missing from 'WebApp1'. Add them?** Select **Yes**.

# [Visual Studio for Mac](#tab/visual-studio-mac)

* Select **File** > **New Solution**.
* Select **.NET Core** > **App** in the sidebar. Select the **Web Application** template. Select **Next**.
* Set the **Target Framework** drop down to **.NET Core 2.2**. Select **Next**.
* Provide a **Project Name**. Confirm or change the **Location**. Select **Create**.

---

## Apply migrations

* Run the app and select the **Register** link.
* Enter the email and password for the new account, and then select **Register**.
* Follow the instructions to apply migrations.

[!INCLUDE[Forward request information when behind a proxy or load balancer section](includes/forwarded-headers-middleware.md)]

## Use SecretManager to store tokens assigned by login providers

Social login providers assign **Application Id** and **Application Secret** tokens during the registration process. The exact token names vary by provider. These tokens represent the credentials your app uses to access their API. The tokens constitute the "secrets" that can be linked to your app configuration with the help of [Secret Manager](xref:security/app-secrets#secret-manager). Secret Manager is a more secure alternative to storing the tokens in a configuration file, such as *appsettings.json*.

> [!IMPORTANT]
> Secret Manager is for development purposes only. You can store and protect Azure test and production secrets with the [Azure Key Vault configuration provider](xref:security/key-vault-configuration).

Follow the steps in [Safe storage of app secrets in development in ASP.NET Core](xref:security/app-secrets) topic to store tokens assigned by each login provider below.

## Setup login providers required by your application

Use the following topics to configure your application to use the respective providers:

* [Facebook](xref:security/authentication/facebook-logins) instructions
* [Twitter](xref:security/authentication/twitter-logins) instructions
* [Google](xref:security/authentication/google-logins) instructions
* [Microsoft](xref:security/authentication/microsoft-logins) instructions
* [Other provider](xref:security/authentication/otherlogins) instructions

[!INCLUDE[](includes/chain-auth-providers.md)]

## Optionally set password

When you register with an external login provider, you don't have a password registered with the app. This alleviates you from creating and remembering a password for the site, but it also makes you dependent on the external login provider. If the external login provider is unavailable, you won't be able to sign in to the web site.

To create a password and sign in using your email that you set during the sign in process with external providers:

* Select the **Hello &lt;email alias&gt;** link at the top-right corner to navigate to the **Manage** view.

![Web application Manage view](index/_static/pass1a.png)

* Select **Create**

![Set your password page](index/_static/pass2a.png)

* Set a valid password and you can use this to sign in with your email.

## Next steps

* This article introduced external authentication and explained the prerequisites required to add external logins to your ASP.NET Core app.

* Reference provider-specific pages to configure logins for the providers required by your app.

* You may want to persist additional data about the user and their access and refresh tokens. For more information, see <xref:security/authentication/social/additional-claims>.
