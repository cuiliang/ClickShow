# Detecting Key Combinations and Seuqnces

## Quickstart Combinations
Assuming your goal is to detect whenever some key combination, shortcut is pressed and perform some action as a response. Use class `Combination` to describe key combinations.
### 1. Define key combinations
```CSharp
var undo = Combination.FromString("Control+Z");
var fullScreen = Combination.FromString("Shift+Alt+Enter");
```

The string is just names of keys concatenated by '+' character without spaces. For key names you can use any of `Keys` enum member names. See.: [Keys Enumeration on MSDN](https://msdn.microsoft.com/en-us/library/microsoft.xna.framework.input.keys.aspx) 

Btw. constructing a `Combination` form a strings is probably the most convenient way, but you can also use bolder methods:

```CSharp
var undo2 = Combination.TriggeredBy(Keys.Z).With(Keys.Control);
```

The last key in a string "Shift+Alt+Enter" has a special meaning. It is a trigger key. The combination will be detected whenever the trigger key is pressed and all other keys are already down at that moment.
The order of keys inside combination except the last one is arbitrary. The combination `"Shift+Alt+Enter"` is equivalent to `"Alt+Shift+Enter"`. This corresponds to the default behaviour of most applications.

Now we need to tell our code what to do whenever a particular key combination is detected. Here we use actions instead of events. An action is either a method with `void` return and no arguments or a trivial lambda.

### 2. Define actions
```CSharp
Action actionUndo = DoSomething;
Action actionFullScreen = () => { Console.WriteLine("You Pressed FULL SCREEN"); };

void DoSomething()
{
	Console.WriteLine("You pressed UNDO");
}
```  

### 3,4. Assign actions to key combinations and start
Finally assign an action to every key combination and start listening to keyboard events.
```CSharp
var assignment = new Dictionary<Combination, Action>
{
    {undo, actionUndo},
    {fullScreen, actionFullScreen}
};

Hook.GlobalEvents().OnCombination(assignment);
```

Here we use dictionary, but in general the method `OnCombination` requires `IEnumerable` of `KeyValuePair<Combination, Action>`-s, which allows you to have more then one action per key combination.

`Hook.GlobalEvents()` will listen to all keyword events from all applications. If you want it to be limited only to keyboard events from your applications use `Hook.AppEvents()` instead.

**Full Listing**

```CSharp
//1. Define key combinations
var undo = Combination.FromString("Control+Z");
var fullScreen = Combination.FromString("Shift+Alt+Enter");

//2. Define actions
Action actionUndo = DoSomething;
Action actionFullScreen = () => { Console.WriteLine("You Pressed FULL SCREEN"); };

void DoSomething()
{
    Console.WriteLine("You pressed UNDO");
}

//3. Assign actions to key combinations
var assignment = new Dictionary<Combination, Action>
{
    {undo, actionUndo},
    {fullScreen, actionFullScreen}
};

//4. Install listener
Hook.GlobalEvents().OnCombination(assignment);
```

**The same in a short form**

```CSharp
void DoSomething()
{
    Console.WriteLine("You pressed UNDO");
}

Hook.GlobalEvents().OnCombination(new Dictionary<Combination, Action>
{
    {Combination.FromString("Control+Z"), DoSomething},
    {Combination.FromString("Shift+Alt+Enter"), () => { Console.WriteLine("You Pressed FULL SCREEN"); }}
});
```

## Quickstart Sequences
Since applications run out of simple key combinations for shortcuts many of them use sequences.

![](/keysequence.png)

A sequence can be described using `Sequnce` class.

```CSharp
var exitVim = Sequnce.FromString("Shift+Z,Z");
var rename = Sequnce.FromString("Control+R,R");
var exitReally = Sequnce.FromString("Escape,Escape,Escape");
```

We treat sequences as series of combinations. The string is basically a comma separated list of key combination strings. Note that a single key can also be used.

The rest is very similar to combinations; you map sequences to actions and pass it to the `OnSequnce` method.

```CSharp
var assignment = new Dictionary<Sequence, Action>
{
    {exitVim, ()=>Console.WriteLine("No!")},
    {rename, ()=>Console.WriteLine("rename2")},
    {exitReally, ()=>Console.WriteLine("Ok.")},
};

Hook.GlobalEvents().OnSequence(assignment);
```

### The longest sequence matches
If you are listening to overlapping sequences like `"A,B,C"` and `"B,C"` both sequences might match if you type A,B,C. In this case only the longest one will fire. The action corresponding to the shorter sequences will not be invoked. My observation was that that's the way how most applications behave. 