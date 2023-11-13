---
layout: post
title: How to debug NuGet package scripts
date: 2015-03-10 17:21:17.000000000 +00:00
type: post
parent_id: '0'
published: true
password: ''
status: publish
categories:
- ".NET"
tags:
- debug
- nuget
- Visual Studio
meta:
  _edit_last: '1'
  _publicize_facebook_user: https://www.facebook.com/lazarconstantincosmin
  _publicize_twitter_user: "@CosminLazar"
  _yoast_wpseo_focuskw: debug NuGet script
  _yoast_wpseo_metadesc: How to debug NuGet package scripts (Init.ps1, Install.ps1,
    Uninstall.ps1)
  _wpas_mess: How to debug NuGet package scripts http://wp.me/p49ir3-5I
  _yoast_wpseo_linkdex: '65'
  _wpas_done_all: '1'
author:
  login: admin
  email: cosminconstantinlazar@gmail.com
  display_name: Cosmin Lazar
  first_name: Cosmin
  last_name: Lazar
permalink: "/netframework/how-to-debug-nuget-package-scripts.html"
---
You can step by step debug your **_Init.ps1_**, **_Install.ps1_**, or **_Uninstall.ps1_** using [**NuGetDebugTools**](https://www.nuget.org/packages/NuGetDebugTools/). It won't be the best debugging experience you'll ever have in your life, but it will surely get the job done!

At first you need to open the Package Manager Console in Visual Studio and:

1. install the **_NuGetDebugTools_** package
2. add the debugger
3. set breakpoints for the scripts you want to debug

{% gist af008b4da93da643d4ae InstallNuGetDebugTools.ps1 %}
Now you're all set and you can begin installing your own package. Depending on which breakpoints you've set with the Set-PSBreakpoint command, when a break point gets hit you will be prompted with a dialog. This is where you will be entering your commands debugging commands (e.g. **_s_** for step into, **_v_** for step over)

{% figure caption:"Dialog for entering debugging commands" %}
![NuGetDebugTools_cmdprompt]({{ site.baseurl }}/assets/2015/03/NuGetDebugTools_cmdprompt-300x131.png "Dialog for entering debugging commands")
{% endfigure %}

The basic commands that you can enter are:
- **_s_** - StepInto (_Step to the next statement into functions, scripts, etc_)
- **_v_** - StepOver (_Step to the next statement over functions, scripts, etc_)
- **_o_** - StepOut (_Step out of the current function, script, etc_)
- **_c_** - Continue (_Continue operation, also on Cancel or empty_)
- **_q_** - Quit (_Stop operation and exit the debugger_)
- **_?, h_** - Help (_Write this help message_)
- **_k_** - CallStack (_Write call stack, aka Get-PSCallStack_)
- **_K_** - CallStackDetailed (_Write detailed call stack using Format-List_)

And you will be receiving feedback and additional information in the Package Manager Console

![NuGetDebugTools_output]({{ site.baseurl }}/assets/2015/03/NuGetDebugTools_output.png)
*You can find an exhaustive list of commands and more detailed information on the [NuGetDebugTools github page](https://github.com/nightroman/PowerShelf/tree/master/Pack/NuGetDebugTools)*
