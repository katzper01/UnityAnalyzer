# UnityAnalyzer

A simple .NET console app for unity project analysis, capable of:

- printing out game objects hierarchy for each scene,
- finding unused scripts.

## Build 

The build has been tested only on ArchLinux. For other systems the process might look a bit different.

```
pacman -S dotnet-runtime dotnet-sdk
chmod +x install.sh
sudo install.sh
```

## Usage 

```
UnityAnalyzer unity_project_path output_folder_path
```
