### What?!

Let you use your browser as a log target from any Asp.Net Core application.

### How?!

##### In your code:

```C#
import Lavspent.BrowserLogger;
```
```C#
public void ConfigureServices(IServiceCollection services)
{
    // ...
    services.AddBrowserLoggerService();
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
