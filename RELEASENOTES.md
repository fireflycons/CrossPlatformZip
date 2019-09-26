## 0.4.1

When adding files from a unix filesystem to a windows zip, store the MS-DOS attributes as Archive. 
Was previously trying to read attributes with `File.GetAttributes`, which does not return valid attributes as of netcore2.0

## 0.4.0

Reworked public interface to take a settings class as there were too many arguments to the Zip methods. Old interface marked as obsolete.

## 0.3.0

Basic implementataion of Unix Extra Fields in central directory entries of zips targeting unix-like operating systems.

* Unix Extra type 3 (tag Id 0x7875) - Stores uid/gid 
* Extended Timestamp (tag ID 0x5455)
