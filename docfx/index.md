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

For further documentation, navigate via top menu (burger menu on mobile).

## Uses

AWS lambda function code is executed on Amazon Linux instances, thus the uploaded zip file needs to have a unix format central directory which includes unix file permission attributes, or he lambda executor user cannot read the files. If you build your lambda functions on a Windows workstation, all windows zip utilities create zips with Windows central directories meaning that unzipping on unix-like systems can result in files with no permissions (to anyone but root).

Include this as part of a deployment script for lambda and other cases where you're deploying from Windows to a Unix/Linux host.

### Creating a zip targeting Linux from a Windows Host

This library performs some processing to ensure that meaningful POSIX attributes are included in the zip directory when targeting Linux from a Windows machine. Each file being zipped is analysed as follows

* The Host ID record is set to Linux in the zip directory entries.
* If the file is binary, the first few bytes are read and checked for being a Linux ELF executable header. If this is true, the attributes are set to `-rwxrwxrwx`.
* If the file is text, the first line is examined for the presence of a shebang. If this is true, the attributes are set to `-rwxrwxrwx`.
* For all other files, the attributes are set to `-rw-rw-rw-`.

### Creating a zip targeting Windows from a Linux Host

* The Host ID record is set to MSDOS (suitable for all versions of Windows) in the zip directory entries.
* No POSIX attributes or ownership are set.
* Windows directory attribute is set for directories.

### Creating a zip targeting Linux from a Linux Host

* The Host ID record is set to Linux in the zip directory entries.
* Attributes and ownership as read from the file system are copied into the zip directory.
* Currently does not store links as links.

### Creating a zip targeting Windows from a Windows Host

* The Host ID record is set to MSDOS in the zip directory entries.
* Windows directory attribute is set for directories.

## .NET Framework Support

This library is built on netstandard2.0 thus runs on Windows (.NET Framework 4.8+ or Core), and Linux/MacOS (.NET Core)