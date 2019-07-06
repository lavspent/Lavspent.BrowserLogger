# Lavspent.BrowserLogger [![Build Status](https://travis-ci.org/semack/Lavspent.BrowserLogger.svg?branch=master)](https://travis-ci.org/semack/Lavspent.BrowserLogger)

### What?!

Let you use your browser as a log target from any Asp.Net Core application.

### How?!

##### Install:

```
install-package Lavspent.BrowserLogger`
```


##### In your code:

```C#
public void ConfigureServices(IServiceCollection services)
{
    // ...
    services.AddBrowserLogger();
}
```


```C#
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    // ...
    app.UseWebSockets();
    app.UseBrowserLogger();
}
```


##### In your browser:
```
http://localhost:5000/console
```
