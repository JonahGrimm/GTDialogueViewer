# Grimm Tales Dialogue Viewer

This is a simple Tauri / Blazor offline app for browsing exported json data (`.gtdialogue`) data from Grimm Tales' Dialogue System.

**Please check the Releases tab for the latest release.**

## Compatible File Types

GTDialogueViewer accepts `.gtdialogue` and `.gtspeakericons`. Both of which are just json files.

`.gtdialogue` contains all the data for a UDialogueAsset.

`.gtspeakericons` contains a map for routing icon path references to actual images on the file system. This file is shipped alongside a neighboring folder called `/Icons/` which contains all the image assets.


## Commands
```
// Compiles and runs a build for debugging
cargo tauri dev

// Compiles a standalone build
// Exported installers will become available in \src-tauri\target\release\bundle
cargo tauri build
```