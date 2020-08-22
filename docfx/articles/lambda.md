# Zipping a Lambda

Here we assume a javascript lambda that has had all its dependences bundled in with WebPack. We want to upload the package as `index.js`. Messages will be logged directly to console.

```csharp
var settings = new CrossPlatformZipSettings{

    AlternateFileName = "index.js",
    Artifacts = "webpacked-lambda.js",
    CompressionLevel = 9,
    TargetPlatform = ZipPlatform.Unix,
    ZipFile = "mylambda.zip"
};

Zipper.Zip(settings);
```

Or, a lambda with multiple files

```csharp
var settings = new CrossPlatformZipSettings{

    Artifacts = "directory-containing-lambda-code",
    CompressionLevel = 9,
    TargetPlatform = ZipPlatform.Unix,
    ZipFile = "mylambda.zip"
};

Zipper.Zip(settings);
```

