
# C#BasicsFileManager

This is a simple file manager written on C# with Windows Forms.

# Interface

This file manager have 1 window with 3 sections:
![Image of File Manager](https://i.imgur.com/4jYxh2S.png)

### 1 section
This is a tree view of catalogues.
### 2 section
This is output window.
### 3 section
This is command line.

# List of commands

Here's list of commands:

1. ```help command_name``` - Output list of commands.
    - command_name - output info about a command.

2. ```cp source destination``` - Copy file or directory from source to destination.
    - source - current file or directory path. Can be relative.
    - destrination - path for a new copy of a file or directory. Can be relative.

3. ```del path``` - Delete file or directory.
    - path - file or directory path. Can be relative.

4. ```info path``` - Show info of file or directory.
    - path - file or directory path. Can be relative.

5. ```cd path``` - Move to new catalogue.
    - path - directory path. Can be relative.

6. ```clr``` - Clear output window.
