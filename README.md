# ClipboardManager

Very simple clipboard manager for copying/cutting and pasting files on windows from the commandline.
The code is very simple, only 150 lines long.

This is integrated with windows ctrl+c ctrl+v, meaning that you can copy/cut a file with ctrl+c/ctrl+x and then paste/move it from the commandline. You can also copy/cut it from the commandline and paste/move it with ctrl+v.

## Features
- Copy files
- Cut files
- Paste files
- Show clipboard content
- Integrated with windows ctrl+c ctrl+v

## Installation
Just build the project with dotnet
```pwsh
dotnet build --configuration Release
```

Then but the binary some directory that you have in `PATH`.

## Examples

Copy multiple files
```pwsh
ClipboardManager --copy ./test1.txt ../test/test2.txt
```
<br>

Cut multiple files
```pwsh
ClipboardManager --cut ./test1.txt ../test/test2.txt
```
<br>

Paste/Move the copied/cut files in the current directory
```pwsh
ClipboardManager --paste
```
<br>

Paste/Move the copied/cut files in the current directory and overwrite the existing file with the same name if exists
```pwsh
ClipboardManager --paste -f
```
<br>

Paste/Move the copied/cut files in a specified directory
```pwsh
ClipboardManager --paste ./some_dir
```
<br>

Show copied files from clipboard
```pwsh
ClipboardManager --show
```