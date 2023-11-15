## Setting Up the FabricUserApiDemo Application

This document is provided to assist you with getting the
**FabricUserApiDemo** project up and running so you can begin your
testing with the Fabric User APIs.

- You will start by creating a new Azure AD application to authenticate
  users and acquire access tokens.

- After that, you will then configure the **FabricUserApiDemo** project
  with the **ApplicationId** of your application.

In case you have not heard, Microsoft recently renamed ***Azure Active
Directory*** to ***Microsoft Entra ID***. In the past, you would uses
the Azure AD portal to create an Azure application which can be used to
call Microsoft APIs such as the Microsoft Graph API and the Fabric User
API. Now, you will use the **Microsoft Entra admin center** to create a
new application for the C# console application named
**FabricUserApiDemo**.

Start by navigating to **Microsoft Entra admin center** at the following
URL.

- [**https://entra.microsoft.com/**](https://entra.microsoft.com/)

On the home page of the **Microsoft Entra admin center**, drop down
the **Applications** section in the left navigation and click the **App
registrations** link.

<img src="./images/Setup/media/image1.png"
style="width:7.5in;height:3.08472in"
alt="A screenshot of a computer Description automatically generated" />

On the **App registrations** page, click **New registration**.

<img src="./images/Setup/media/image2.png"
style="width:7.5in;height:1.54375in"
alt="A screenshot of a computer Description automatically generated" />

Give the new application a name of **Fabric User API Demo** and leave
the Supported account types setting with the default selection
of **Accounts in this organizational directory only**.

<img src="./images/Setup/media/image3.png"
style="width:7.26806in;height:3.07639in" />

Move down to the **Redirect URI** section. Select **Public
client/native** application in the drop down menu and enter a redirect
URI of [**http://localhost**](http://localhost/). Make sure to create
the URL with **http** and not **https**.

<img src="./images/Setup/media/image4.png"
style="width:7.5in;height:1.43472in"
alt="A screenshot of a computer Description automatically generated" />

Click **Register** to create the new application.

<img src="./images/Setup/media/image5.png"
style="width:5.53125in;height:1.3875in"
alt="A white rectangular object with blue text Description automatically generated" />

Now that you have created the application, you need to record
Application ID for use later in the C# console application. Copy
the **Application ID** from the application summary page in the
Microsoft Entra admin center.

<img src="./images/Setup/media/image6.png"
style="width:6.10526in;height:1.96115in" />
