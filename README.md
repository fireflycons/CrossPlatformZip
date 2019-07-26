# CrossPlatformZip

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

## .NET Framework Support

This library is built on netstadard2.0.

