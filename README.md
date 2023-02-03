# Dlubal Software GmbH
<!-- ![image](/source_code/img/logo.png {width=180px height=180px}) -->

<!-- <img src="/source_code/img/logo.png" width="180" height="180"  alt="Dlubal Software" /> -->

[![image](https://img.shields.io/twitter/follow/dlubal_en?style=social)](https://twitter.com/dlubal_en "Twitter Follow")
[![image](https://img.shields.io/badge/GitHub-Dlubal_Software-darkblue?logo=github&amp;)](https://github.com/Dlubal-Software "Github Follow")
[![image](https://img.shields.io/badge/http://-dlubal.com-darkblue)](https://www.dlubal.com/en-US "RFEM Latest")
[![image](https://img.shields.io/badge/docs-API-darkblue?logo=read-the-docs&amp;logoColor=white)](https://dlubal-software.github.io/.github/ "RFEM Latest")

# Welcome to Dlubal CSharp library

[![image](https://img.shields.io/badge/COMPATIBILITY-RFEM%206.02.025-yellow)](https://www.dlubal.com/en-US/products/rfem-fea-software/what-is-rfem)
[![image](https://img.shields.io/badge/COMPATIBILITY-RSTAB%209.02.025-yellow)](https://www.dlubal.com/en-US/products/rstab-beam-structures/what-is-rstab)
[![image](https://img.shields.io/badge/COMPATIBILITY-RSECTION%201.02.025-yellow)](https://www.dlubal.com/en-US/products/cross-section-properties-software/rsection)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![CodeFactor](https://www.codefactor.io/repository/github/dlubal-software/dlubal_csharp_client/badge)](https://www.codefactor.io/repository/github/dlubal-software/dlubal_csharp_client)
<!-- [![codecov](https://codecov.io/gh/jarabroz/Dlubal_CSharp_Client/branch/main/graph/badge.svg?token=wQ4PBPY8XY)](https://codecov.io/gh/jarabroz/Dlubal_CSharp_Client) -->
[![Nuget](https://img.shields.io/nuget/v/Dlubal.RFEMWebServiceLibrary)](https://www.nuget.org/packages/Dlubal.RFEMWebServiceLibrary)
![Nuget](https://img.shields.io/nuget/dt/Dlubal.RFEMWebServiceLibrary)
[![Nuget](https://img.shields.io/nuget/v/Dlubal.RSTABWebServiceLibrary)](https://www.nuget.org/packages/Dlubal.RSTABWebServiceLibrary)
![Nuget](https://img.shields.io/nuget/dt/Dlubal.RSTABWebServiceLibrary)
[![Nuget](https://img.shields.io/nuget/v/Dlubal.RSECTIONWebServiceLibrary)]((https://www.nuget.org/packages/Dlubal.RSECTIONWebServiceLibrary))
![Nuget](https://img.shields.io/nuget/dt/Dlubal.RSECTIONWebServiceLibrary)
[![Build Status](https://dev.azure.com/Dlubal-Software/Dlubal%20CSharp%20WebService%20library/_apis/build/status/Dlubal-Software.Dlubal_CSharp_Client?branchName=main)](https://dev.azure.com/Dlubal-Software/Dlubal%20CSharp%20WebService%20library/_build/latest?definitionId=7&branchName=main)
[![Deployment Status](https://vsrm.dev.azure.com/Dlubal-Software/_apis/public/Release/badge/679c7705-1a5c-446b-940c-2791dc987d88/2/2)](https://vsrm.dev.azure.com/Dlubal-Software/_apis/public/Release/badge/679c7705-1a5c-446b-940c-2791dc987d88/2/2)


<!-- ### Table of Contents
- [RfemCSharpWsClient](#rfemcsharpwsclient)
  * [Description](#description)
  * [Architecture](#architecture)
    + [Data Structure](#data-structure)
  * [Getting started](#getting-started)
    + [Dependencies](#dependencies)
    + [Step by step](#step-by-step)
    + [Examples](#examples)
  * [License](#license)
  * [Contribute](#contribute) -->

## Description

This C# project is focused on opening [**RFEM 6**](https://www.dlubal.com/en/products/rfem-fea-software/what-is-rfem), [**RSTAB 9**](https://www.dlubal.com/en/products/rstab-beam-structures/what-is-rstab) and [**RSECTION 1**](https://www.dlubal.com/en/products/cross-section-properties-software/rsection) to all our customers, enabling them to interact with our applications on much higher level. If you are looking for tool to help you solve parametric models or optimization tasks, you are on the right place. This project and community will create support for all your projects. The goal is to create easily expandable C# library communicating instructions to RFEM / RSTAB through [Web Services](https://en.wikipedia.org/wiki/Web_service) (WS), [SOAP](https://cs.wikipedia.org/wiki/SOAP) and [WSDL](https://en.wikipedia.org/wiki/Web_Services_Description_Language) technology.
![image](https://user-images.githubusercontent.com/37547309/118759006-6711cd80-b870-11eb-8019-da3312a75e64.png)
WS enable you to access your local version of RFEM / RSTAB / RSECTION or remote via internet connection.

## Getting started

<!-- You can download [actual release](https://github.com/Dlubal-Software/Dlubal_CSharp_Client/releases/latest) of our C# library and the use it for your project or you can clone / fork our repository and compile our library by yourself following steps below. -->

### Geting started with Visual Studio Code


### Getting started with Visual Studio

* Download [dotnet 6.0 sdk](https://dotnet.microsoft.com/download/dotnet/6.0) and install it
* Download [Visual Studio 2022 Community](https://visualstudio.microsoft.com/cs/thank-you-downloading-visual-studio/?sku=Community&channel=Release&version=VS2022&source=VSLandingPage&cid=2030&passive=false) and install it
* Create new project
* Go to Nuget Package manager and search for Dlubal. Install one of the selected packages

### Steps for downloaded release
* Go to [release location](https://github.com/Dlubal-Software/Dlubal_CSharp_Client/releases/latest)
* Download zip file called DlubalC.Library.zip
* Unzip it
* Put dll into your project
* Reference them
* Compile your project
* Make application which uses our Web Services

### Steps for Visual Studio

    * Install [GitHub Extension](https://marketplace.visualstudio.com/items?itemName=GitHub.GitHubExtensionforVisualStudio)
* Download [GitHub Desktop](https://desktop.github.com/)
* Clone / fork this repository
* Open Visual Studio and open Solution from [source_code\DlubalWebService.sln](source_code\DlubalWebService.sln) and compile it
* Made your own project or use one from [examples](/examples)

### Steps for Visual Studio Code
* Download [dotnet 6.0 sdk](https://dotnet.microsoft.com/download/dotnet/6.0) and install it
* Download [Visual Studio Code](https://code.visualstudio.com/) and install it
* Open Visual Studio Code and install following extensions
    * [C#](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp) - extension for working with C# in VS Code
    * [Test Explorer UI](https://marketplace.visualstudio.com/items?itemName=hbenl.vscode-test-explorer) - extension for Unit Testing general
    * [.NET Core Test Explorer](https://marketplace.visualstudio.com/items?itemName=hbenl.vscode-test-explorer) - extension for Unit Testing of .Net projects
    * [Code Spell Checker](https://marketplace.visualstudio.com/items?itemName=streetsidesoftware.code-spell-checker) - useful extension for spell checking
    * [GitHub Pull Requests and Issues](https://marketplace.visualstudio.com/items?itemName=GitHub.vscode-pull-request-github) - extension for easy access to the GitHub pull request
    * [GitHub Issues](https://marketplace.visualstudio.com/items?itemName=ms-vscode.github-issues-prs) - extension for easy access to the GitHub issues
    * [Git Lens](https://marketplace.visualstudio.com/items?itemName=eamodio.gitlens) - extension for better work with Source management
* Download [Git](https://git-scm.com/downloads) and install it (needed for better functionality of Git Lens)
* Download [GitHub Desktop](https://desktop.github.com/)
* Clone / fork this repository
* Open Visual Studio Code
* Build library project
* Made your own project or use one from [examples](/examples)




<!-- ## Architecture
![image](https://user-images.githubusercontent.com/37547309/118119185-44a22f00-b3ee-11eb-9d60-3d74a4a96f81.png) -->
<!-- ### Data Structure -->
## Dependencies
* .NET 5.0 Core or .NET 6.0 Core (depends on your choice, libraries are multi-target)
* RFEM 6 or RSTAB9 application

### Examples
Examples can be found under [Examples](/examples) folder.
## License
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

## Contribute
Contributions are always welcome! Please ensure your pull request adheres to the following guidelines [Contributing](/CONTRIBUTING.md)


