---
layout: post
title: Running PowerShell scripts during NuGet package installation and removal
date: 2015-02-21 21:46:54.000000000 +00:00
type: post
parent_id: '0'
published: true
password: ''
status: publish
categories:
- ".NET"
tags:
- nuget
- powershell
meta:
  _edit_last: '1'
  _publicize_facebook_user: https://www.facebook.com/lazarconstantincosmin
  _publicize_twitter_user: "@CosminLazar"
  _yoast_wpseo_focuskw: PowerShell in NuGet
  _yoast_wpseo_metadesc: How to execute PowerShell scripts when installing/removing
    NuGet packages
  _yoast_wpseo_linkdex: '52'
  _yoast_wpseo_title: Running PowerShell scripts during NuGet package installation/removal
  _wpas_done_all: '1'
  _wpas_skip_5526398: '1'
  _wpas_skip_5526404: '1'
  _wpas_skip_6755022: '1'
  _wpas_skip_7497947: '1'
  _yoast_wpseo_focuskw_text_input: PowerShell in NuGet
  _yoast_wpseo_content_score: '30'
  _yoast_wpseo_primary_category: ''
author:
  login: admin
  email: cosminconstantinlazar@gmail.com
  display_name: Cosmin Lazar
  first_name: Cosmin
  last_name: Lazar
permalink: "/netframework/running-powershell-scripts-during-nuget-package-installation-and-removal.html"
excerpt: How to execute PowerShell scripts when installing/removing NuGet packages
---
Your NuGet package can contain PowerShell scripts which, based on a convention, will be called during the package installation/removal process.

- **_Init.ps1_** runs the first time a package is installed in a solution
- **_Install.ps1_** runs when a package is installed in a project <- no longer supported in Visual Studio 2017 ([read more](http://blog.nuget.org/20170316/NuGet-now-fully-integrated-into-MSBuild.html){: target="_blank"})
- **_Uninstall.ps1_** runs every time a package is uninstalled <- no longer supported in Visual Studio 2017 ([read more](http://blog.nuget.org/20170316/NuGet-now-fully-integrated-into-MSBuild.html){: target="_blank"})
- _you can read more about it [here](https://docs.nuget.org/create/creating-and-publishing-a-package){: target="_blank"}_

Your scripts should begin with the following line

```powershell
param($installPath, $toolsPath, $package, $project)
```

Where

- $installPath path to where the project is installed
- $toolsPath path to the extracted tools directory
- $package information about the currently installing package
- $project reference to the EnvDTE project the package is being installed into


In order to be executed, the scripts need to be included in the tools directory of the nuget package. You can achieve this by editing your *.nuspec file and adding a file reference.

```xml
<?xml version="1.0"?>
<package>
  <metadata>...</metadata>
  <files>
	<file src="scripts\NugetScripts\Install.ps1" target="tools\Install.ps1" />
  </files>
</package>
```

You will quickly find debugging to be fairly painful if you need to operate with the $project reference. In this case, you are highly encouraged to test all your $project actions in the Visual Studio Package Manager Console by writing

```powershell
$project = Get-Project
```
Now you can interactively test your operations in the console, and you don't have to constantly pack and install/uninstall your package to check if the scripts work as intended.