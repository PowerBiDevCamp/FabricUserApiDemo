## Setting Up the FabricUserApiDemo Application

This document is provided to assist you with getting the
**FabricUserApiDemo** project up and running so you can begin your
testing with the Fabric User APIs. To set up this project for testing,
you will need a local installation of either Visual Studio 2022 or
Visual Studio Code.

This document will demonstrate setting up and running the application
using Visual Studio 2022 Community Edition which can be downloaded for
free from
[**here**](https://visualstudio.microsoft.com/thank-you-downloading-visual-studio/?sku=Community&channel=Release&version=VS2022&source=VSLandingPage&passive=false&cid=2030).

To set up the **FabricUserApiDemo** project for testing, you must
complete the following steps.

- Create a new Azure AD application to authenticate users and acquire
  access tokens

- Update the **AppSettings.cs** file with the **ApplicationId** of the
  new application

- Update the **AppSettings.cs** file with the Id of a capacity with
  enabled Fabric functionality.

In case you have not heard, Microsoft recently renamed ***Azure Active
Directory*** to ***Microsoft Entra ID***. In the past, you would
typically go to the Azure AD portal to create an new Azure AD
application which is used to call Microsoft secured APIs such as the
Microsoft Graph API and the Fabric User API. Now, it’s recommended to
use the **Microsoft Entra admin center** to create a new application for
the C# console application named **FabricUserApiDemo**.

Start by navigating to **Microsoft Entra admin center** at the following
URL.

- [**https://entra.microsoft.com/**](https://entra.microsoft.com/)

On the home page of the **Microsoft Entra admin center**, drop down
the **Applications** section in the left navigation and click the **App
registrations** link.

<img src="./images/Setup/media/image1.png"
style="width:6.35172in;height:2.61244in"
alt="A screenshot of a computer Description automatically generated" />

On the **App registrations** page, click **New registration**.

<img src="./images/Setup/media/image2.png"
style="width:4.97453in;height:1.02392in"
alt="A screenshot of a computer Description automatically generated" />

Give the new application a name of **Fabric User API Demo** and leave
the **Supported account types** setting with the default selection
of **Accounts in this organizational directory only**.

<img src="./images/Setup/media/image3.png"
style="width:4.01291in;height:1.69856in" />

Move down to the **Redirect URI** section. Select **Public
client/native** application in the drop down menu and enter a redirect
URI of [**http://localhost**](http://localhost/).

<img src="./images/Setup/media/image4.png"
style="width:7.5in;height:1.43472in"
alt="A screenshot of a computer Description automatically generated" />

The redirect URL of <http://localhost> was added for developers when
Microsoft introduced .NET Core. This simple redirect URL provides a safe
and easy way to authenticate .NET applications on your local desktop for
debugging and testing purposes. Resist the temptation to create the URL
with the **https** protocol instead of with **http** because it will not
work. The http protocol is the only one that will work.

Click **Register** to create the new application.

<img src="./images/Setup/media/image5.png"
style="width:5.53125in;height:1.3875in"
alt="A white rectangular object with blue text Description automatically generated" />

Now that you have created the application, you need to record
Application ID for use later in the C# console application. Copy
the **Application ID** from the application summary page.

<img src="./images/Setup/media/image6.png"
style="width:6.10526in;height:1.96115in" />

## Run the FabricUserApiDemo Application in Visual Studio

Download all the files in the **FabricUserApiDemo** repository as a ZIP
archive using this [download
link](https://github.com/PowerBiDevCamp/FabricUserApiDemo/archive/refs/heads/main.zip).
Once you have downloaded the ZIP archive expand it’s contents into a
local folder named **FabricUserApiDemo** as shown in the following
screenshot. Once you have copied all the files from the ZIP archive into
the local folder, double click on FabricUserApi.sln to open the project
in Visual Studio 2022.

<img src="./images/Setup/media/image7.png"
style="width:4.28708in;height:2.12309in" />

Once the **FabricUserApiDemo** project opens in Visual Studio, you
should be able to see the top-level files and folders in the Solution
Explorer as shown in the following screenshot. Next, open the source
file named **AppSettings.cs** so you can update configuration values
inside.

<img src="./images/Setup/media/image8.png"
style="width:1.78374in;height:2.49761in" />

Xxxxx

<img src="./images/Setup/media/image9.png"
style="width:4.13573in;height:1.63636in" />

Xx

<img src="./images/Setup/media/image10.png"
style="width:4.19139in;height:0.69067in" />

Xxx

<https://app.powerbi.com/admin-portal/capacities/capacitiesList/>

ssss

<img src="./images/Setup/media/image11.png"
style="width:4.80861in;height:1.86806in" />

ssss

<img src="./images/Setup/media/image12.png"
style="width:4.82775in;height:1.08118in" />

<img src="./images/Setup/media/image13.png"
style="width:5.58852in;height:1.91874in" />

<img src="./images/Setup/media/image14.png"
style="width:5.66986in;height:0.66587in" />

<img src="./images/Setup/media/image15.png"
style="width:4.0727in;height:1.98092in"
alt="A screenshot of a computer Description automatically generated" />

<img src="./images/Setup/media/image16.png"
style="width:2.56133in;height:4.80384in"
alt="A screenshot of a computer application Description automatically generated" />

<img src="./images/Setup/media/image17.png"
style="width:3.36364in;height:0.73179in" />

<img src="./images/Setup/media/image18.png"
style="width:4.76555in;height:0.99881in" />

Xxx

<img src="./images/Setup/media/image19.png"
style="width:4.47199in;height:1.99542in"
alt="A screenshot of a computer Description automatically generated" />

<img src="./images/Setup/media/image20.png"
style="width:5.04785in;height:0.88268in" />

<img src="./images/Setup/media/image21.png"
style="width:5.08612in;height:0.9704in" />

<img src="./images/Setup/media/image22.png"
style="width:7.49792in;height:4.20556in" />
