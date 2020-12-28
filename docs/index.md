<div align="center">
	<h1>Project Reloaded: Hooking</h1>
	<img src="https://i.imgur.com/BjPn7rU.png" width="150" align="center" />
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
	<!-- Build Status -->
	<a href="https://ci.appveyor.com/project/sewer56lol/reloaded-hooks">
		<img src="https://ci.appveyor.com/api/projects/status/hfeonbkitheaclo3?svg=true" alt="Build Status" />
	</a>
</div>

# Introduction
The purpose of this project is to support extending (hooking) u̷nma͘na̕g̡ed͡ ͞cod҉e (͟A̸P͡Is)́ w͟͡i͠t̴͠h̨͢͡ ̸́͟p̢҉u̢͟r̷̀è͢ ̡mąń̨͜a͏̨g͢e̴d... wait... *you've heard this before many times, haven't you?*

Why stop at APIs? What if we could work with *"something more"*?

Well, that's about the goal of this library.


## Feature Highlights
+ Support for x86 and x64 architectures.
+ Call & Hook unmanaged functions with *custom calling conventions*.
+ Stack function hooks. Double, Triple, Quadruple, n-tuple hook functions.
+ Generate native functions that convert CDECL/Microsoft x64 function calls to *custom calling convention* calls, and vice versa.
+ Detects & Patches common variations of existing function hooks when hooking. Hook functions hooked by other libraries; this feature is unique to Reloaded.Hooks.  
+ Utility functions allowing you to deal with aspects such as *function pointers* and *Virtual Function Tables*. 

## Documentation

As advanced as the library may sound, in reality using the library is super simple.
The following links below should help you get started with the library:

+ [Explain Hooking: What do you use this library for](https://github.com/Reloaded-Project/Reloaded.Hooks/issues/1)
+ [Getting Started](Docs/Getting-Started.md)

In addition, feel free to look through `Reloaded.Hooks.Tests` for some ideas 😉.

## Contributions
As with the standard for all of the `Reloaded-Project`, repositories; contributions are very welcome and encouraged.

Feel free to implement new features, make bug fixes or suggestions so long as they are accompanied by an issue with a clear description of the pull request.

If you are implementing new features, please do provide the appropriate unit tests to cover the new features you have implemented; try to keep the coverage high 😊.

## Authors & Contributions

Reloaded.Hooks uses the `SharpDisasm` library by *Justin Stenning (spazzarama)*, a partial port of Udis86 by *Vivek Thampi*. Both of these libraries are originally distributed under the under the 2-clause "Simplified BSD License". 
