# Weblogger
![GitHub](https://img.shields.io/github/license/ewilliams0305/WebLogger) 
![GitHub all releases](https://img.shields.io/github/downloads/ewilliams0305/WebLogger/total) 
![Nuget](https://img.shields.io/nuget/dt/WebLogger)
![GitHub issues](https://img.shields.io/github/issues/ewilliams0305/WebLogger)

WebLogger is a websocket server designed to provide an accessible console application served to an html user interface.
The WebLogger library tagets .netstandard 2.0 and can be used in any .net framework 4.7 and .net Core application.
WebLogger will manage the server and provide an easy way to create a custom CLI using commands and prompts.  
This library also includes an HTML front end using vanilla JS to handle the socket connection.  
The webpage is embedded into the DLL and will be extracted when executed to a destination of your choosing.

![WebLogger Console](console.PNG)

## Table of Contents
1. [VS Solution](#Visual-Studio-Solution)
2. [Example Program](#WebLogger-Example-Program)
3. [WebLogger](#Create-a-WebLogger)
4. [Commands](#Console-Commands)
5. [Embedded HTML](#Embedded-HTML)
6. [Command Discovery](#Discovery-Commands)
7. [Serilog Sink](#Serilog-Sink)
8. [Release Notes](#Release-Notes)

## Visual Studio Solution

The included solution includes five projects including two example projects and 3 libraries. 

- /source/`WebLogger.csproj`
- /source/`WebLogger.Serilog.csproj`
- /source/`WebLogger.Crestron.csproj`
- /example/`WebLogger.ConsoleApp.csproj`
- /example/`WebLogger.CrestronApp.csproj`

A unit test project is also included and located in the tests directory

- /tests/`WebLogger_UnitTests`

### WebLogger.csproj

This is lowest level library including all WebLogger types and logic.  This library has one dependancy on WebSocketSharp

[WebSocketSharp](https://github.com/PingmanTools/websocket-sharp/)

### WebLogger.Serilog.csproj

The WebLogger.Serilog project provides a serilog sink used to write structured logging outputs to the WebLogger console.
Included in this project are the WebLogger Sink and Extension methods to streamline the configuration and implmentation.

[Serilog](https://github.com/serilog/serilog)


### WebLogger.Crestron.csproj
Adds a reference to the Crestron SDK.  This project provides some helpful Crestron commands to use with the console.
Since browsers will block ws when the html page is served via https this server solves (albeit unsecured) this issue by providing 
an unsecured http server to distrubute the HTML files.

### WebLogger.ConsoleApp Example Program
The Weblogger example is a simple console application showing SerilogSink usage.

### WebLogger.CrestronApp Example Program
The Weblogger example is a Crestron SDK SimpleSharp program that demonstrates how to instantiate the `WebLogger` class and add console commands with callbacks.

## Create a WebLogger

To Create a new instance of the `WebLoger` class included in the `using WebLogger` namespace.  
Creating a new instace will:

1. Create a Websocket Server at the specified port
2. Copy all embedded resource HTML files to your local file system at the specified directory
3. Create a few default console commands

```csharp
using WebLogger;
```

Create a new instance and start the server using the ```WebLoggerFactory```.  The default factory method will 
return an ```IWebLogger``` interface using the default ```WebLogger``` concrete implmentation.

```csharp
var logger = WebLoggerFactory.CreateWebLogger();

```

Optionally use the Lambda ```Action<WebLoggerOptions``` to override the default parameters.
Note: Currently secured web sockets are not fully supported, options to provide a valid certificate will be provided in the next release.
https://github.com/ewilliams0305/WebLogger/issues/7

```csharp
var logger = WebLoggerFactory.CreateWebLogger(options =>
{
    options.Secured = false;  
    options.WebSocketTcpPort = 54321;                  //allows you to provide a TCP port used by the web socket server
    options.DestinationWebpageDirectory = "C:/Temp/";  //allows you to provide a file directory to extract the embedded html files.
});

```

Call the start method to extract the embedded resources and start the web socket server at the specified port.

```csharp
logger.Start();
```



## Console Commands

### Register Console Commands

Custom console commands be created at any point in the life cycle of the `WebLogger`.  
To add a custom command create a new instance of the `ConsoleCommand` class.  Each console command will be added to a dictionary using the `Command` property as the key.  Each console command should have a pointer to callback method.  When a string matching the `Command` name is received from the `WebLogger` server, the callback will be invoked. 

###### Default Constructor

Using the default constuctor and setting all properties with the object initialization syntax

```csharp

ConsoleCommands.RegisterCommand(new ConsoleCommand(
    "EXAMPLE",
    "Simple example of console command",
    "Parameter: NA",
    (cmd, args) =>
    {
        Log.Logger.Information("{command} Received", cmd);
    }));

ConsoleCommands.RegisterCommand(new ConsoleCommand(
    "TEST",
    "Simple example of console command",
    "Parameter: NA",
    (cmd, args) =>
    {
        Log.Logger.Information("{command} Received", cmd);
    }));

```


Console command callback signature
```csharp

/// <summary>
/// Console command callback
/// </summary>
/// <param name="command">Command that was issued</param>
/// <param name="arguments">Argument passed into the command seperated by spaces</param>
public void ConsoleCommandCallback(string command, List<string> arguments);

```

## Embedded HTML

Located in the `WebLogger` project is a folder titled `HTML`.  All HTML source files have been added to the project and configured as an embedded resource.  
These files will be automatically extracted and written to the provided `applicationDirectory` in the weeblogger's default constructor.

```csharp 
public WebloggerSink(IFormatProvider formatProvider, int port, bool secured, string applicationDirectory)
    {
        _logger = new WebLogger(port,  secured, applicationDirectory);
        _logger.Start();

        _formatProvider = formatProvider;
    }
```
**Be aware, the program will check if the files are already created and ONLY write them if they are not found.  This means the HTML files will need to be deleted off the server if changees to the HTML are made**
After loading the code to your VC4, create a new room and associate it with the program.  Once the room has been started browse to `/opt/crestron/virtualcontrol/RunningPrograms/[ROOM ID]/html/logger` to review the files on the server.  
Files will be served up using the index/html file found at http://server/VirtualControl/Rooms/EXAMPLE/Html/logger/index.html

When using the WebLogger.Crestron library you can create a custom http file server and distibute the HTML page via an unsecured webserver

```csharp

 Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .WriteTo.WebloggerSink(54321, false, Directory.GetApplicationRootDirectory())
    .CreateLogger();

var webPageServer = new WebLoggerHttpServer(
    port: 8081,
    directory: Path.Combine(Directory.GetApplicationRootDirectory(), "html/logger/"));

ConsoleCommands.RegisterCommand(new ConsoleCommand(
    "EXAMPLE",
    "Simple example of console command",
    "Parameter: NA",
    (cmd, args) =>
    {
        Log.Logger.Information("{command} Received", cmd);
    }));

ConsoleCommands.RegisterCommand(new ConsoleCommand(
    "TEST",
    "Simple example of console command",
    "Parameter: NA",
    (cmd, args) =>
    {
        Log.Logger.Information("{command} Received", cmd);
    }));

```

The above code will result in a url http://ip:8081/index.html 
While the files will be stored in the application directory /html/logger/


## Discovery Commands

Console commands can now be discovered adn instnatiated via System.Reflection
To begin create a custom class implmennting the ```IWebLoggerCommand```

```casharp

public sealed class DoWorkCommand : IWebLoggerCommand
{
    public string Command => "DO";
    public string Description => "Does work";
    public string Help => "Does lots of stuff";
    public Func<string, List<string>, string> CommandHandler => DoTheWork;

    public DoWorkCommand()
    {
            
    }

    public string DoTheWork(string command, List<string> args)
    {
        // HANDLE THE COMMAND HERE
        return "The work was done";
    }
}

```

Using the IWebLogger extension method ```DiscoverCommands(Assembly)``` discovery all commands with a default constructor.
The dscovery methods will create an instance of your commands located in the provided assemblies.

```csharp

logger.DiscoverCommands(Assembly.GetAssembly(typeof(Program)))
    .DiscoverCommands(Assembly.GetExecutingAssembly())
    .DiscoverCommands(Assembly.GetAssembly(tyeof(SomeOtherAssemblyMarker)));

logger.Start();

```

## Serilog Sink

```csharp
// Option 1: Let the sink extension Create the instance.  
// When logger is closed and flushed the web logger will be disposed and stopped.

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .WriteTo.WebloggerSink(54321, false, "C:/Temp/")
    .WriteTo.Console()
    .CreateLogger();

```
Option 2: Create the logger and pass it into the sink.

```csharp

// Option 2: Create a logger and pass it into the Sink Extension
// When logger is closed and flushed the web logger will be disposed and stopped.

var logger = WebLoggerFactory.CreateWebLogger(options =>
{
    options.Secured = false;
    options.WebSocketTcpPort = 54321;
    options.DestinationWebpageDirectory = "C:/Temp/";
});

logger.Start();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .WriteTo.WebloggerSink(logger)
    .WriteTo.Console()
    .CreateLogger();

```

## Release Notes

#### Version 1.0.1
- Initial Release

#### Version 1.1.0
- Changed command handler from ```Action<string, List<string>``` to ```Func<string, List<string>, CommandResponse``` 
to provide a command response.  Returned string will now be Writen to the webLogger output.

- Created ```WebLogger.Serilog``` Project and Nuget Package.  This allowed the web logger to remove the dependancy on Serilog.
Weblogger.Serilog now provides logger configuration extensions and SerilogSink for the weblogger server.

- Created extension methods to discovery all defined commands in a provided assembly.  