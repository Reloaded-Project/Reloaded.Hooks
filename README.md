<div align="center">
	<h1>Project Reloaded: Hooking</h1>
	<img src="./docs/Images/ReloadedLogo.png" width="150" align="center" />
	<br/> <br/>
	<strong><i>WTF You can unit test function hooks!?</i></strong>
	<br/> <br/>
	<!-- Coverage -->
	<a href="https://codecov.io/gh/Reloaded-Project/Reloaded.Hooks">
		<img src="https://codecov.io/gh/Reloaded-Project/Reloaded.Hooks/branch/master/graph/badge.svg" alt="Coverage" />
	</a>
	<!-- NuGet -->
	<a href="https://www.nuget.org/packages/Reloaded.Hooks">
		<img src="https://img.shields.io/nuget/v/Reloaded.Hooks.svg" alt="NuGet" />
	</a>
</div>

# Introduction
Reloaded.Hooks is a library for intercepting and modifying existing binary functions on `x86` and `x64` machines. It is most often used to either intercept Win32 API calls (e.g. `NtCreateFile` to find out what files the current process is loading) or to patch existing functions within a program; e.g. patching software at runtime.

If this concept is unfamiliar to you; I would suggest to research the term `Hooking` while reading the documentation.

Reloaded.Hooks is a managed alternative to native libraries such as `MinHook` and `Detours`, targeted at tackling more advanced/difficult use cases; such as when functions do not use standard *[calling conventions](https://en.wikipedia.org/wiki/Calling_convention)*.

## Feature Highlights
+ Support for x86 and x64 architectures.
+ Call & hook unmanaged functions with *custom calling conventions*.
+ Stack function hooks. You can hook already hooked functions as many times as you like.
+ Mid function x86/x64 assembly hooks; similar to the likes of `Cheat Engine`.
+ Highly compatible. Detects & patches common variations of existing function hooks when hooking. Hook functions hooked by other libraries; this feature is unique to Reloaded.Hooks. 
+ Generate native wrapper functions for converting between custom calling conventions. e.g. Stdcall to Fastcall converter.
+ Many lower level utility functions allowing you to deal with things like *Virtual Function Tables*. 

## Documentation

As advanced as the library may sound, in reality using the library is super simple.
Please visit the [dedicated documentation site for getting started](https://reloaded-project.github.io/Reloaded.Hooks/).

In addition, feel free to look through `Reloaded.Hooks.Tests` for some ideas 😉.

## Contributions
As with the standard for all of the `Reloaded-Project`, repositories; contributions are very welcome and encouraged.

Feel free to implement new features, make bug fixes or suggestions so long as they are accompanied by an issue with a clear description of the pull request.

If you are implementing new features, please do provide the appropriate unit tests to cover the new features you have implemented; try to keep the coverage high 😊.

## Authors & Contributions

- Reloaded.Hooks uses the `Flat Assembler` (FASM) by Tomasz Grysztar.

- Reloaded.Hooks uses the `Iced` library by 0xd4d.

### Legacy

- Older versions of `Reloaded.Hooks` used the `SharpDisasm` library by *Justin Stenning (spazzarama)*, a partial port of Udis86 by *Vivek Thampi*. Both of these libraries are originally distributed under the under the 2-clause "Simplified BSD License". 
