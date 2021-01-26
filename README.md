# AspNetCore.EasyTesting

> Test library for integration test make easy and this library is fully tested!

![banner](docs/images/easy-testing-logo.png)

![Build And Test](https://github.com/WDWWW/aspnetcore-easy-testing/workflows/Build%20And%20Test/badge.svg)
![Nuget](https://img.shields.io/nuget/v/AspNetCore.EasyTesting)
![Nuget](https://img.shields.io/nuget/dt/AspNetCore.EasyTesting)
![GitHub commit activity](https://img.shields.io/github/commit-activity/y/wdwww/aspnetcore-easy-testing)

## Features
- Replace to memory cache for `IDistributedCache`
- Replace to InMemoryDb Easily for your DbContext
- Replace internal service to your own object.
- Change ASP.NET Core host environment value(like `Development`, `Production`) 
- Mock/Fake internal service using by Moq, FakeItEasy, NSubstitute
- Provide build api for writing testing code declarative.
- Getting/Using service after init the test service.
- Provide hooks like `SetupFixture`, `Configure*` Methods.
- Fake Authentication!!
- Verify your service registration for your conditional registration logic. :satisfied:

## Getting Start!

First of all, you have to install this package like below in your project.

```bash
dotnet add packge AspNetCore.EasyTesting
```

And you can create SUT(Short word of System Under Test) object of your Startup class.

```csharp
using var sut = new SystemUnderTest<Startup>(); // That's all!
```

If you want to replace your own services to fake service and check response data.
Write code like below. 

```csharp
sut.Replace<IYourOwnService>(new FakeService());

var client = sut.CreateHttpClient();

var response = await client.GetAsync("path/to/your/api");

resopnse.ShouldBeOk<YourApiResponse>(apiResponse => {
    apiResponse.Field.Should().NotBeEmpty();
});
```

And this framework is not depends on ANY test framework. What you want, you can use this library for writing integration test easily! :')

### Replace DbContext to In-Memory DbContext.

If you want to change your DbContext connection with in-memory db-context? You don't need to separate environment in your api project!
Just add package and call `ReplaceInMemoryDbContext<TDbContext>` or `ReplaceSqliteInMemoryDbContext<TDbContext>`! 

```sh
dotnet add package AspNetCore.EasyTesting.EntityFrameworkCore 
```

```cs
sut.ReplaceInMemoryDbContext<SampleDb>() // Replace your registered both `DbContextOptions<TDbContext>` and `DbContextOptions`
    .SetupFixture<SampleDb>(async db =>
    {
        db.SampleDataEntities.Add(new SampleDataEntity());
        await db.SaveChangesAsync();
    }); 
```

Note that if there are no any specific database name about `ReplaceInMemoryDbContext`, the database name is assigned `Guid.NewGuid().ToString()`
to prevent fail test when parallel test run environment.   

### Replace to use MemoryCache for IDistributed type.

Use `ReplaceDistributedInMemoryCache` If you want to replace distributed cache with memory cache

```cs
sut.ReplaceDistributedInMemoryCache()
    .CreateClient();
```



### Mock/Fake you owned services!

If you want to replace your own service with mock/fake object. 

```sh
dotnet add package AspNetCore.EasyTesting.[Moq/FakeItEasy/NSubstitute]
```

Note that, how many you call replace fake/mock service method, this create library only once and re-use fake/mock object. that mean, you can write mocking chaining code elegantly. ;) 

#### Moq
```csharp
// All cases is same!
var mockService = sut.MockService<ISampleService>();
sut.MockService<ISampleService>(out var mock);
sut.MockService<ISampleService>(mock => mock
   .Setup(service => service.GetSampleDate())
   .Returns("Action mocked!"))

// Verify helper
sut.VerifyCallOnce<ISampleService>(service => service.GetSampleDate());
sut.VerifyCall<ISampleService>(service => service.GetSampleDate(), Times.Once());
```

#### FakeItEasy
```csharp
// Dummy service
sut.ReplaceDummyService<ISampleService>();

// Replace fake service
var fakeService = sut.FakeService<ISampleService>();
sut.FakeService<ISampleService>(out var fakeService);
sut.FakeService<ISampleService>(service => A
   .CallTo(() => service.GetSampleDate())
   .Returns("MockedData"))

// Take registered fake service
var fakeService = sut.GetFakeService<ISampleService>();
sut.UseFakeService<ISampleService>(service => A.CallTo(() => service.GetSampleDate()).MustHaveHappend());
```

#### NSubstitute
```csharp
// Replace service with substitute
sut.ReplaceWithSubstitute<ISampleService>(out var mockedService);
sut.ReplaceWithSubstitute<ISampleService>(substitute => /* setup */);
var substitute = sut.ReplaceWithSubstitute<ISampleService>();

// Verify
sut.GetSubstitute<ISampleService>().Received().GetSampleDate();
sut.UseSubstitute<ISampleService>(substitute => substitute.Received().GetSampleDate());
```

### Replace Pre-Registered application service with new service.

```csharp
sut.ReplaceService<ISampleService, FakeSampleServic>();
sut.ReplaceService<ISampleService>(new FakeSampleService()); // It will be always registered as singleton.
```

### Use internal service object for verifying test is successfully working.

Use `SystemUnderTest.UsingServiceAsync` or `SystemUnderTest.UsingService` methods for use internal service.

```csharp
await sut.UsingServiceAsync<ISampleService>(async service => {
    var updated = await service.GetSameplAsync(OriginalFixtureId);
    updated.Data.Should().Be("Newer Sample Data");
});
```


### Verify lifetime of service registration.

Use `SystemUnderTest.VerifyRegisteredLifeTimeOfService` to verify to register your application service lifetime rightly.

```csharp
var sut = new SystemUnderTest<Startup>();

sut.VerifyRegisteredLifeTimeOfService<ISampleService>(ServiceLifetime.Scoped);
```

### Verify implementation type registration about service type

Use `VerifyRegisteredImplementationTypeOfService` to verify implementation type registration for service type
when you have conditional service registration logic on startup class. 

```csharp 
// Given
var sut = new SystemUnderTest<Startup>();
sut.UseProductionEnvironment();

// When
sut.CreateClient();

// Then
sut.VerifyRegisteredImplementationTypeOfService<ISampleService, ProductionSampleService>();
```


### Change host environment value

``` cs
var client = new SystemUnderTest<Startup>()
    .UseProductionEnvironment()
    .CreateClient();
```

- `UseEnvironment`
- `UseDevelopmentEnvironment`
- `UseStagingEnvironment`
- `UseProductionEnvironment`

### Use delegate methods of IWebHostBuilder for customizing by your hand! :wave:

SystemUnderTest provide delegate methods of IWebHostBuilder to allow fully customization of your way. Check out below methods. 

- `SystemUnderTest.ConfigureServices`
- `SystemUnderTest.ConfigureAppConfiguration`
- `SystemUnderTest.UseSetting`
- `SystemUnderTest.SetupWebHostBuilder`


### Fake authentication layer of ASP.NET Core

We can do fake authentication handler easily through call few method. 
But It's not recommended for custom complex implementation of authentication process.

```cs
usnig var sut = new SystemUnderTest<Startup>();

// this replace disable defualt original authentication handler with fake authentication handler  
var httpClient = sut.NoUserAuthentication()
    // .AllowAuthentication("Bearer", new ClaimsPrincipal(/* configure test user principal */)) for faking specific authentication scheme.
    // .DenyAuthentication() for test unauthorized call
    .CreateClient();

// Now you can call any authorized actions
```

This library provide you four type methods.
- `NoUserAuthentication`  : When server couldn't find authentication related parts(Cookie, Token, JWT ... etc) from http call.
- `DenyAuthentication`    : When server found authentication related parts But it's invalid
- `AllowAuthentication`   : When server found valid authentication related parts and can make `IPrincipal`
- `FakeAuthentication`    : What you want, just call this method and provide valid ticket or principal instance.

If you don't provide `sheme` parameter, it will use `DefaultScheme` to fake that is already configured in `Startup.cs` by your hand. :wave:

```cs
services.AddAuthentication("Bearer") // "Bearer" scheme is default authentication scheme.
```

Note that if you want to use your own `IIdentity` when fake authentication handler for some scheme.
`IIdentity.IsAuthenticated` should return `true`. if it is not, every call is denied by ASP.NET Core authentication middleware. 

### Support GRPC Client

This is just simple grpc support for creating grpc client easily.

```sh
dotnet add package AspNetCore.EasyTesting.Grpc 
```


- `CreateGrpcChannel`   : create grpc channel for creating grpc client.
- `CreateGrpcClient<T>` : `T` is generated grpc client by protoc.

```csharp
usign var client = new SystemUnderTest<Startup>()
    .UseProductionEnvironment()
    .CreateGrpcClient<SomeServiceClient>();

await client.DoSomethingAsync(new { Property = 1 });
```

### More methods.
- `ReplaceConfigureOptions` : Replace registered configure options. if you used custom configure some option class in startup class.
- `ReplaceNamedConfigureOptions` : Like above but just for named options.
- `DisableOptionValidations` : Remove all options validators about `TOptions`
- `DisableOptionDataAnnotationValidation` : Disable only data annotation validation.
- `VerifyRegistrationByCondition` : Verify service registration by condition expression. 
- `OverrideAppConfiguration` overloads
- `ReplaceLoggerFactory` : Replace logger factory to generating custom logger.
  - for example : [checkout XUnit test sample](src/Wd3w.AspNetCore.EasyTesting.Test/SystemUnderTest/ReplaceLoggerFactoryTest.cs)
- `DisableStartupFilters`, `DisableStartupFilter<TImplementationFilter>` : Remove service registrations of `IStartFilter`
- `RemoveSingleBy`, `RemoveAllBy`, `Remove`, `RemoveAll` : Remove service or implementation types.
- `VerifyNoRegistrationByCondition`, `VerifyNoRegistration` : Verify there are no registration of condition or service type.

### And more use cases..

Visit this library [test project](src/Wd3w.AspNetCore.EasyTesting.Test). You can see every cases to use this library. 

## LICENSE

MIT License

Copyright (c) 2021 WDWWW

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.