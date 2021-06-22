[![nuget][nuget-badge]][nuget-url] [![master][master-appveyor-badge]][master-appveyor-url]

 [nuget-badge]: https://img.shields.io/badge/nuget-v5.4.0-blue.svg
 [nuget-url]: https://www.nuget.org/packages/MouseKeyHook

![Mouse and Keyboard Hooking Library in c#](/mouse-keyboard-hook-logo.png)

## NEWS!
Added support for detection of key combinations and sequences see: [Quickstart - Detecting Key Combinations and Seuqnces](keycomb.md)

## What it does?

This library allows you to tap keyboard and mouse, to detect and record their activity even when an application is inactive and runs in background.

## Prerequisites

 - **Windows:** .Net 4.0+

## Installation and sources

<pre>
  nuget install MouseKeyHook
</pre>

 - [NuGet package][nuget-url]
 - [Source code][source-url]

 [source-url]: https://github.com/gmamaladze/globalmousekeyhook

 ## Usage

 ```csharp
 private IKeyboardMouseEvents m_GlobalHook;

 public void Subscribe()
 {
     // Note: for the application hook, use the Hook.AppEvents() instead
     m_GlobalHook = Hook.GlobalEvents();

     m_GlobalHook.MouseDownExt += GlobalHookMouseDownExt;
     m_GlobalHook.KeyPress += GlobalHookKeyPress;
 }

 private void GlobalHookKeyPress(object sender, KeyPressEventArgs e)
 {
     Console.WriteLine("KeyPress: \t{0}", e.KeyChar);
 }

 private void GlobalHookMouseDownExt(object sender, MouseEventExtArgs e)
 {
     Console.WriteLine("MouseDown: \t{0}; \t System Timestamp: \t{1}", e.Button, e.Timestamp);

     // uncommenting the following line will suppress the middle mouse button click
     // if (e.Buttons == MouseButtons.Middle) { e.Handled = true; }
 }

 public void Unsubscribe()
 {
     m_GlobalHook.MouseDownExt -= GlobalHookMouseDownExt;
     m_GlobalHook.KeyPress -= GlobalHookKeyPress;

     //It is recommened to dispose it
     m_GlobalHook.Dispose();
 }
 ```
(also have a look at the Demo app included with the source)

## How it works?

This library attaches to windows global hooks, tracks keyboard and mouse clicks and movement and raises common .NET events with KeyEventArgs and MouseEventArgs, so you can easily retrieve any information you need:
 * Mouse coordinates
 * Mouse buttons clicked
 * Mouse drag actions
 * Mouse wheel scrolls
 * Key presses and releases
 * Special key states

 Additionally, there are `MouseEventExtArgs` and `KeyEventExtArgs` which provide further options:
 * Input suppression
 * Timestamp
 * IsMouseDown/Up
 * IsKeyDown/Up.

## Troubleshooting and support

 - Usage or programming related question? Post it on [StackOverflow][so] using the tag *mousekeyhook*
 - Found a bug or missing a feature? Feed the [issue tracker][tracker]

 [so]: http://stackoverflow.com/questions/tagged/mousekeyhook
 [tracker]: https://github.com/gmamaladze/globalmousekeyhook/issues

## Current project build status
 The CI builds are generously hosted and run on the [Travis][travis] and [AppVeyor][appveyor] infrastructures.

|            | Travis-CI                                           | AppVeyor                                                  |
| :--------- | :-------------------------------------------------: | :-------------------------------------------------------: |
| **master** | [![master][master-travis-badge]][master-travis-url] | [![master][master-appveyor-badge]][master-appveyor-url] |
| **vNext**  | [![vNext][vNext-travis-badge]][vNext-travis-url]    | [![vNext][vNext-appveyor-badge]][vNext-appveyor-url]   |

[master-travis-url]: https://travis-ci.org/gmamaladze/globalmousekeyhook/branches/
[master-travis-badge]: https://travis-ci.org/gmamaladze/globalmousekeyhook.svg?branch=master

[vNext-travis-url]: https://travis-ci.org/gmamaladze/globalmousekeyhook/branches/
[vNext-travis-badge]: https://travis-ci.org/gmamaladze/globalmousekeyhook.svg?branch=vNext

[master-appveyor-url]: https://ci.appveyor.com/project/gmamaladze/globalmousekeyhook/branch/master
[master-appveyor-badge]: https://ci.appveyor.com/api/projects/status/tnkt7xiurmpg0qh8/branch/master?svg=true

[vNext-appveyor-url]: https://ci.appveyor.com/project/gmamaladze/globalmousekeyhook/branch/vNext
[vNext-appveyor-badge]: https://ci.appveyor.com/api/projects/status/tnkt7xiurmpg0qh8/branch/vNext?svg=true


[travis]: http://travis-ci.org/
[appveyor]: http://appveyor.com/

## History

|  Year       |     URL
--------------|--------------------------------
| 2000 - 2008 | http://www.codeproject.com/KB/cs/globalhook.aspx
| 2008 - 2015 | https://globalmousekeyhook.codeplex.com/
| 2015 - now  | https://github.com/gmamaladze/globalmousekeyhook


## Quick contributing guide

 - Fork and clone locally
 - Create a topic specific branch. Add some nice feature.
 - Send a Pull Request to spread the fun!

## License

The MIT license see: [LICENSE.txt](/LICENSE.txt)
