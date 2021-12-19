# CrossPlatformZip

[![Build status](https://ci.appveyor.com/api/projects/status/d2cpscmh141cy3wq?svg=true)](https://ci.appveyor.com/project/fireflycons/crossplatformzip)

![Nuget](https://img.shields.io/nuget/v/Firefly.CrossPlatformZip)

[Nuget Package](https://www.nuget.org/packages/Firefly.CrossPlatformZip/)

## What it is

I've found, especially with AWS that it can be fussy about the format of the central directory of a zip file when unzipping on a platform other than that where the zip was created. For instance, a zip file created by Windows with Windows paths (backslash) in the central directory, when unzipped on Linux with certain unzip programs which truly honour the paths with the separator character so you end up with individual _files_ in the target directory rather than the expected directory structure, e.g

```
# ls -la

-rwxrwxrwx 1 root    root       4096 Dec 18 16:41 'dir1\dir2\file.txt'
```

As we can see, a file is created in the target directory, rather than the expected directory structure.

What this module does is allow you to specify when creating the zip file the _target_ operating system where you expect the archive to be unzipped, and thus the zip central directory is formatted with the correct path separator.

This module is mostly about _creating_ zip files, though an unzip method is also provided. This will create the correct pathnames for the operating system it is running on, irrespective of the path style within the zip central directory.

Essentially I am providing a very lightweight wrapper around [SharpZipLib](https://github.com/icsharpcode/SharpZipLib) - thanks guys!

## Uses

AWS lambda function code is executed on Amazon Linux instances, thus the uploaded zip file needs to have a unix format central directory which includes unix file permission attributes, or the lambda executor user cannot read the files. If you build your lambda functions on a Windows workstation, all windows zip utilities create zips with Windows central directories meaning that unzipping on unix-like systems can result in files with no permissions (to anyone but root).

Include this as part of a deployment script for lambda and other cases where you're deploying from Windows to a Unix/Linux host.

## POSIX Attribute support

When creating a ZIP with `TargetPlatform.Unix` then the following logic applies.

* If running on Windows, files are examined for being executable. Binary files are checked for an ELF header, and text files are checked for existence of a shebang. If either is true, the permissions `rwxrwxrwx` are stored in the zip, else `rw-rw-rw-`
* If running on Linux, files are stored with the attributes as read from the file system.

## .NET Framework Support

This library is built on netstandard2.0.

## API Documention

API documention can be found [here](https://fireflycons.github.io/Firefly-CrossPlatformZip/api/index.html)

## Projects using this library

* [PSCloudFormation](https://github.com/fireflycons/PSCloudFormation) - uses it to prepare AWS CloudFormation packages.

## Still to do

* Ensure central directory content matches exactly those produced by Linux zip and Windows built-in zip in terms of local header fields
* Honour extended field data
    * On extraction, map to target OS, i.e. Unix file times to NTFS
