# Simple http server socket system

This tool makes it simple to add simple http and websocket upgrading to any application without the use of httpsys or other heavy frameworks. No dependencies outside net6 are used.

## Objects
**HttpConnectionListener**:  
Wrapper around a tcplistener, let's you handle requests and connects through events
- OnRequest gives you the ability to resolve requests through the given response object, any unresolved or upgraded request is responded with a 500
- OnWebsocket hands you the upgraded socket ready for communication, pingpong is set to 10 seconds

**WebsocketConnection**:  
Wraps the websocket object, bubbles up messages through events, provides simple interface for sending
- OnText & OnBinary are triggered when messages are recieved
- OnDisconnected is obvious
- Send is obvious
- Close is obvious

Example Usage:
```csharp
var listener = new HttpConnectionListener();
listener.OnRequest += (sender, request, response) =>
{
    if(request.IsWebsocketUpgrade)
        response.WebsocketUpgrade();
    else if(request.IsWebsiteRequest)
        respponse.Resolve(200, HttpConsts.WEBSITE_BODY, HttpConsts.WEBSITE_HEADERS);
    else
        response.Resolve(404);
};
listener.OnWebsocket += (sender, request, socket) =>
{
    //here's where you do whatever you want with the newly connected socket
    socket.OnText += (sender, message) => 
    {
        Console.WriteLine(message);
    };
    socket.OnDisconnected += (sender, e) =>
    {
        Console.WriteLine("Disconnected");
    }
    socket.OnBinary += (sender, bytes) =>
    {
        //do whatever ya want with this
    }
};
listener.Start(new IPEndpoint(IPAddress.Any, HOSTING_PORT));
```