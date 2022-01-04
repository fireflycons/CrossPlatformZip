## 0.6.1

* No functional change. Documentation site update only.

## 0.6.0

* Fix - File system permissions not correctly extracted on Linux. [Issue link](https://github.com/fireflycons/CrossPlatformZip/issues/7)

## 0.5.2

* Enhancement - Add SourceLink support. [Issue link](https://github.com/fireflycons/CrossPlatformZip/issues/5)

## 0.5.1

* Added [DragonFruit Command Line Parser](https://github.com/dotnet/command-line-api) to the console application and [instructions to build it](https://fireflycons.github.io/Firefly-CrossPlatformZip/articles/command-line.html)

## 0.5.0

* Refactor `ExternalAttributes` to be `Traits` since this is really what these classes represent.
* Add a trait to detect file name/directory name duplication when creating a zip for Windows from a case sensitive file system. Throw an exception if detected.

## 0.4.1

When adding files from a unix filesystem to a windows zip, store the MS-DOS attributes as Archive.
Was previously trying to read attributes with `File.GetAttributes`, which does not return valid attributes as of netcore2.0

## 0.4.0

Reworked public interface to take a settings class as there were too many arguments to the Zip methods. Old interface marked as obsolete.

## 0.3.0

Basic implementation of Unix Extra Fields in central directory entries of zips targeting unix-like operating systems.

* Unix Extra type 3 (tag Id 0x7875) - Stores uid/gid. Presently this is set to 0/0 until netcore3.0 which suppots POSIX properly
* Extended Timestamp (tag ID 0x5455) - Timestamps are stored but not yet restored by this unzip implementation.