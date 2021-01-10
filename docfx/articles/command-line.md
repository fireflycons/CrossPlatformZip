# Command Line Interface

A super-simple command line interface is provided with the repo, but at present you need to build and deploy it yourself.

It is most useful for creating zips for AWS Lambda code from a Windows environment, where the content of the zip file requires Unix file attributes and permissions. The created zip will have all file permissions as `rwxrwxrwx` and all directories as `drwxrwxrwx`

Only a single path argument is supported and may point to a file to zip or if the path is a directory, the content will be added recursively.

## Building the command line tool

Open a command prompt in the root of the repo, then run the following

```
dotnet build
dotnet pack
dotnet tool install --global --add-source ./src/Firefly.CrossPlatformZip.Console/nupkg/ Firefly.CrossPlatformZip.Console
```

The zip tool is now globally installed with an alias of `xpzip`

## Using the tool

Enter `xpzip -h` for list of command line arguments


