## .NET Server Stack (Prototype)

Inspiried by software like netty, finagle, wcf 

### Benefits

- Shared cross cutting concerns and bootup pattern
  - Logging
  - Dependency injection
  - Configuration
  - Startup class
  - Hosting API
  - Middleware pipeline

### Gaps

- Missing unified client stack
- Context object is dependent on the server implementation. Should we expose a feature collection directly as the default?


## Architecture

```
                  [incoming]                                                 [outgoing]
[transport]  [bytes] --> [IFrameDecoder<T>]  [Dispatcher<T>]  [IFrameEncoder<U>] --> [bytes] [transport]
                                            [IFrameHandler<T>]   

```
