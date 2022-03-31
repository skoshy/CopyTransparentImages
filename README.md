# Copy Transparent Images (from Chrome)

> NOTE: This repo will be archived as of 2022-03-31, as copying transparent images seems to work by default on Windows 11 with both Chrome and Firefox without need for this script. Feel free to fork it if you need it for any reason.

I made this script to easily copy transparent images from Chrome while still retaining transparency information from the image.

Normally, copying a transparent image from Chrome and pasting it to GIMP/Photoshop results in a black background where transparency should be. Running this script fixes that.

## How to Use

Simply run the script after copying an image to the clipboard.

## Example

This GIF shows how copying a transparent image results in a black image, but after running this script, it pastes normally.

![Example](http://i.imgur.com/Y68iJpD.gif)

## Download

You can download a built version from [the release page](https://github.com/skoshy/CopyTransparentImages/releases).

## Exit Codes

- 0 for success
- 1 for failure
