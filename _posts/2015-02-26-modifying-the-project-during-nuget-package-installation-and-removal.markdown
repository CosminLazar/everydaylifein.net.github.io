---
layout: post
title: Modifying the project during NuGet package installation and removal
date: 2015-02-26 20:54:42.000000000 +00:00
type: post
parent_id: '0'
published: true
password: ''
status: publish
categories:
- ".NET"
tags:
- BuildAction
- CopyToOutputDirectory
- nuget
- script
- Visual Studio
meta:
  _edit_last: '1'
  _publicize_facebook_user: https://www.facebook.com/lazarconstantincosmin
  _publicize_twitter_user: "@CosminLazar"
  _oembed_6d9f0a100f805063936a2dc6ff9f4a32: "{{unknown}}"
  _yoast_wpseo_focuskw: NuGet install.ps1 CopyToOutputDirectory BuildAction
  _yoast_wpseo_title: Change CopyToOutputDirectory, BuildAction from Install.ps1
  _yoast_wpseo_metadesc: How to change CopyToOutputDirectory, BuildAction and other
    project properties from NuGet package scripts (Init.ps1, Install.ps1, Uninstall.ps1)
  _yoast_wpseo_linkdex: '69'
  _yoast_wpseo_opengraph-description: How to change CopyToOutputDirectory, BuildAction
    and other project properties from NuGet package scripts (Init.ps1, Install.ps1,
    Uninstall.ps1)
  _yoast_wpseo_twitter-description: How to change CopyToOutputDirectory, BuildAction
    and other project properties from NuGet package scripts (Init.ps1, Install.ps1,
    Uninstall.ps1)
  _wpas_done_all: '1'
  _wpas_mess: Modifying the project during NuGet package installation and removal
    http://wp.me/p49ir3-5f
  _yoast_wpseo_focuskw_text_input: NuGet install.ps1 CopyToOutputDirectory BuildAction
  _yoast_wpseo_content_score: '90'
  _yoast_wpseo_primary_category: ''
  _wpas_skip_5526398: '1'
  _wpas_skip_5526404: '1'
  _wpas_skip_6755022: '1'
  _wpas_skip_7497947: '1'
author:
  login: admin
  email: cosminconstantinlazar@gmail.com
  display_name: Cosmin Lazar
  first_name: Cosmin
  last_name: Lazar
permalink: "/netframework/modifying-the-project-during-nuget-package-installation-and-removal.html"
---
By default NuGet does a great job installing and referencing all the assemblies present in the '*lib*' directory of the NuGet package. Furthermore, if you have other files which need to be added to the solution, you can simply add them to the '*content*' directory of the package. The files added to the '*content*' directory will be added to the solution with the following properties:

- BuildAction - Content
- CopyToOutputDirectory - Do not copy

*This is extremely nice if you have images, unmanaged dlls, or other things that just need to be added to the solution, and not referenced.*

However, there are times when the default behavior is not enough, and you need control over how your items get added to the project (e.g. if you need to have CopyToOutputDirectory=CopyIfNewer).

## Running scripts when installing the package

You will need to add a *Install.ps1* script to your package. You can read more on how to do that [here](http://everydaylifein.net/netframework/running-powershell-scripts-during-nuget-package-installation-and-removal.html){: target="_blank"}.

Now that you added *Install.ps1* to your package, it's time to change some values here and there. Of course, there are several ways this behavior can be achieved, I will start with how **NOT** to do it, and continue with how to do it.

### How **NOT** to do it

While trying to figure out myself how to achieve this, I found [this](http://stackoverflow.com/a/21161498){: target="_blank"} answer on stackoverflow.com. Unfortunately, I was hasty enough to take that solution as "good enough", and didn't scrolled down a few more answers (spoiler alert: one of them contains the correct way of doing it).

I adjusted the PowerShell code with a more "mean" xquery and got this bad boy

{% gist 99f2b78057f06c8f9b18 WrongInstall.ps1 %}

The above script loads the project file from the disk, performs some changes to it, and then flushes it back to the disk. Mission accomplished, however, you are actually changing the project "behind the scenes", and big surprise, Visual Studio is not entirely happy about it:

{% figure caption:"Project modified outside Visual Studio" %}
![Project modified outside Visual Studio](http://everydaylifein.net/wp-content/uploads/2015/02/SaveAsDiscard.png)`
{% endfigure %}

*Q: Should I Save, Discard, Overwrite, Ignore?*

*A: Can I chose more than one option?*

If you saved all your changes before running the package installation, in most of the cases you would be able to click *Discard* and get away with it, but not all the times. What's left, is *Save As* and later on performing a merge of the two project files (highly uncool). Moreover, I wonder how this will work if there are errors while installing the package and a rollback is needed.

### How to do it

The parameters that your Install.ps1 script is receiving are:

{% gist 78d99c4d57f9de097fd2 NugetPowershellScriptHeader.ps1 %}

The *$project* is a reference to the [EnvDTE](https://msdn.microsoft.com/en-us/library/envdte.dte.aspx){: target="_blank"} project the package is being installed into. This means that you get complete control over the project file in a standard way.

Here are a few examples on how to use it:

#### CopyToOutputDirectory

Given the following project structure

![RoccatSolutionDLL](http://everydaylifein.net/wp-content/uploads/2015/02/RoccatSolutionDLL.png)

Changing the *CopyToOutputDirectory* property of the talkfx-c.dll can be done with the following construct

{% gist 78d99c4d57f9de097fd2 ChangingCopyToOutputDirectory.ps1 %}

*Note: You need to work your way down the project structure hierarchy until you reach the item you are after (in this case CroccatTalkWrapper/win32-x86/talkfx-c.dll)*

The possible values for *CopyToOutputDirectory* are

- **0** = Do not copy
- *1* = Copy always
- *2* = Copy if newer

#### BuildAction

Keeping the same project structure as above

{% gist 78d99c4d57f9de097fd2 ChangingBuildAction.ps1 %}

Standard *BuildAction* values are:

- *0* = None
- *1* = Compile
- *2* = Content
- *3* = EmbeddedResource

#### Testing

If you need to test your *$project* manipulations, you can do it in the Visual Studio Package Manager Console

{% gist 78d99c4d57f9de097fd2 file=TestingTheProjectInPackageManagerConsole.ps1 %}

Now you can test all your manipulations in the console.

#### Compatibility

Starting with Visual Studio 2017 install.ps1 and uninstall.ps1 are no longer supported ([read more](http://blog.nuget.org/20170316/NuGet-now-fully-integrated-into-MSBuild.html){: target="_blank"}).
