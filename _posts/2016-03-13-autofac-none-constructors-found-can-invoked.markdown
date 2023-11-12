---
layout: post
title: Autofac - None of the constructors found can be invoked
date: 2016-03-13 17:27:32.000000000 +00:00
type: post
parent_id: '0'
published: true
password: ''
status: publish
categories:
- ".NET"
tags:
- autofac
- constructor
- DependencyResolutionException
- invoke
- register
- resolve
meta:
  _edit_last: '1'
  _yoast_wpseo_primary_category: '19'
  _publicize_facebook_user: https://www.facebook.com/lazarconstantincosmin
  _publicize_twitter_user: "@CosminLazar"
  _yoast_wpseo_focuskw_text_input: Autofac constructor
  _yoast_wpseo_focuskw: Autofac constructor
  _yoast_wpseo_metadesc: Fixing Autofac 'None of the constructors found can be invoked'
    due to automatically registered compiler generated state machines.
  _yoast_wpseo_linkdex: '56'
  _wpas_done_all: '1'
  _wpas_mess: Autofac - None of the constructors found can be invoked
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
permalink: "/netframework/autofac-none-constructors-found-can-invoked.html"
---
# The bug

A common pattern used when working with [Autofac](http://autofac.org/) (or any other IoC containers) is to register multiple implementations of an interface, then resolve them all and call a method on each implementation (e.g.: an `IEventHandler` interface that you use to notify all registered handlers that something happened).

Take the following example:

{% gist cacf3d0558496b8a6857 AutofacConstructorBug.cs %}

Trying to resolve all implementors of `IHaveDeferredEnumerable` via `.Resolve<IEnumerable<IHaveDeferredEnumerable>>()` will throw

> None of the constructors found with 'Autofac.Core.Activators.Reflection.DefaultConstructorFinder' on type 'Autofac.Test.Scenarios.ScannedAssembly.HasDeferredEnumerable+<Get>d__0' can be invoked with the available services and parameters:
Cannot resolve parameter 'Int32 <>1__state' of constructor 'Void .ctor(Int32)'.

However, trying to resolve a single implementation of the interface via `.Resolve<IHaveDeferredEnumerable>()` will work as expected. This means that `HasDeferredEnumerable` can be built independently, however for some reasons it cannot be constructed when trying to get all implementations of `IHaveDeferredEnumerable`.

# The journey

Studying the error message further reveals some strange behavior, Autofac is complaining that the type `'Autofac.Test.Scenarios.ScannedAssembly.HasDeferredEnumerable+<Get>d__0'` cannot be resolved because of some `'Int32 <>1__state'` parameter required in a `'Void .ctor(Int32)'` constructor. Totally weird, as I am indirectly asking for the type `'Autofac.Test.Scenarios.ScannedAssembly.HasDeferredEnumerable'` which only has the default parameter-less constructor, weird.

To be studied and understood, a problem must be first isolated, therefore, I stripped all other code doing container work and was only left with `IHaveDeferredEnumerable`, `HasDeferredEnumerable`, and the container registration `cb.RegisterAssemblyTypes(ScenarioAssembly).AsImplementedInterfaces();`

To get even better isolation, I changed the container registration from registering all the types found in my test assembly (`RegisterAssemblyTypes`) with `cb.RegisterType<HasDeferredEnumerable>().As<IHaveDeferredEnumerable>()`. Now everything worked as expected, I was able to get the single implementation via `.Resolve<IHaveDeferredEnumerable>()` and all implementations via `.Resolve<IEnumerable<IHaveDeferredEnumerable>>()`. It was now obvious that the problem occurred sometime during type registration and only when bulk registering types via the `RegisterAssemblyTypes` method.

Since Autofac is an [open source project](https://github.com/autofac/Autofac) I checked out the repository, wrote a failing test and step-by-step debugged my way through the type registration process. This is how I found out that a concrete implementation was discovered for `IEnumerable<IHaveDeferredEnumerable>`, even though there is none in my scanned assembly.

Since this voodoo magic needs to be cleared out, I fired up [dotPeek](https://www.jetbrains.com/decompiler/) and decompiled my assembly, to find the following:

`[gist id="cacf3d0558496b8a6857" file="Decompiled_HasDeferredEnumerable.cs"]`

My `HasDeferredEnumerable` class has been enriched with a private class, which implements `IEnumerable<IHaveDeferredEnumerable>` and not surprisingly requires an integer to be constructed. How did that class ended-up in there without me typing it you ask? Well, the .NET compiler does in fact changes your code and generates new types as needed. In this particular case, it was because of the deferred execution in the `Get()` method - the compiler generates a helper state machine class to handle your [deferred execution](https://blogs.msdn.microsoft.com/oldnewthing/20080812-00/?p=21273).

# The fix

Fixing the issue was fairly trivial as the helper class is decorated with the `[CompilerGenerated]` attribute and it was just a matter of filtering out these compiler generated classes when running bulk type discovery. You can find the [accepted pull request here](https://github.com/autofac/Autofac/pull/719) and it will probably only end-up in version 4.0.0.

If you cannot wait until version 4.0.0 you can either remove the deferred execution (remove yield return/break) from the classes affected by the bug, or just make sure to manually register them into the container.
