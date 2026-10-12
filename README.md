# Grimm Tales Dialogue Viewer

This is a simple Tauri / Blazor offline app for browsing exported json data (`.gtdialogue`) data from Grimm Tales' Dialogue System.

**Please check the Releases tab for the latest release.**

## Important Usage Notes and Tips

- Opening multiple `.gtdialogue` files will create multiple tabs. These tabs can be rearranged by dragging, just like a browser. Additionally, you can use the mouse wheel to scroll through tabs if they go underneath the top-right editor buttons.
- In the opened graph, use left-click to explore the graph. Use the mouse wheel to zoom. 
- If you press Ctrl + Mouse Wheel, you can adjust the spacing of all nodes, in case some nodes overlap.
- Left-click a node to select it and inspect its data in the right details pane. 
- Nodes that are transparent are not relevant to localization efforts.
- Text that is **localizable** will appear green in the details window, along with buttons to copy its internal data. 

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