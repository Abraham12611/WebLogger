# VirtualControlWeblogger
 Websocket server designed to provide an accessible console application to a Crestron VC4 program instance

 ## Table of Contents
1. [VS Solution](#Visual-Studio-Solution)
2. [Example Program](#WebLogger-Example-Program)
3. [Web Logger](#Create-a-WebLogger)
4. [Console Commands](#Register-Console-Commands)

## Visual Studio Solution

The included solution includes two projects.  `WebLoggerExample` and `VirtualControl.Debugging`. 

### WebLogger Example Program

 The Weblogger example is a Crestron SDK SimpleSharp program that demonstrates how to instantiate the `WebLogger` class and add console commands with callbacks.

#### Create a WebLogger

Create a new instance of the `WebLoger` class included in the `VirtualControl.Debugging.WebLogger` namespace

```csharp
using VirtualControl.Debugging.WebLogger;
```

Create a new instance and start the server

```csharp
WebLogger logger = new WebLogger(54321, false);
logger.Start();
```

#### Register Console Commands

Custom console commands be created at any point in the life cycle of the `WebLogger`.  To add a custom command create a new instance of the `ConsoleCommand` class.

###### Default Constructor

Using the default constuctor and setting all properties with the object initialization syntax

```csharp
new ConsoleCommand()
{
    Command = "EXAMPLE",
    Description = "Simple example of console command",
    Help = "Parameter: NA",
    CommandAction = (cmd, args) =>
    {
        logger.WriteLine($"{cmd} Haha");
    }
};
```

Using the optional constuctor to set all properties at instantiation
```csharp
new ConsoleCommand("EXAMPLE", "Simple example of console command", "Parameter: NA", Handler);
```
