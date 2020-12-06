# RandImg: The Random Image Gallery



## What is RandImg
RandImg is your personal digital picture frame, right on your computer. You just load in some images and RandImg will shuffle them and present them for you to enjoy. RandImg is great for remembering those special moments or picturing those places you’d rather be. You could put up pictures of the beach as motivation to get your work done sooner, maybe browse some pictures of your pets back home to remember them as if they were with you. The possibilities are endless with RandImg.

There are many apps to browse photos, and even desktop backgrounds and screensavers let you cycle through pictures. However, RandImg is unique in how flexibly you can choose your pictures. For one, you don’t need to have all the photos in one place. You can link dozens of folders with photos so you can keep the organization as it is. RandImg will even look through the subfolders inside of the folders, in case your collection is a bit more complex. RandImg also comes with robust searching functions to further filter your images. If you put tags in the image titles, you can search for or exclude them while viewing. This flexibility when choosing images lets you personalize RandImg to show you exactly what you want to see.

If you have loads of precious pictures collecting dust in your hard drive, set them free with RandImg and relive the memories one picture at a time.

## How to Use RandImg
### Starting out
To use RandImg, start by selecting a folder to search images from. This can be done by clicking “Add” at the bottom of the window or dragging and dropping folders in. RandImg will search for images in these folders as well as images in sub-folders inside of them.

You can then set some options for RandImg. Window type will determine whether it starts fullscreen or as a resizable window. Autoplay lets RandImg cycle through images automatically.

You can filter through the image files selected using the search and exclude pattern fields. Details on filtering images can be found in [Advanced Selection](#advanced-selection), but you can leave it blank to select all files.

These settings can be saved to a file using the “Save Presets” button at the bottom right and can be loaded by pressing the “Load Presets” button. Presets can also be loaded by dragging a presets file onto the main window.

After the settings are set and the files are selected, press start to start showing images. Controls for RandImg while running are shown below. If you set the window type to resizable, there will be an intermediate window that will let you resize the display before starting.

### During the Show
While the gallery is running, you can control the program with a few hotkeys:

Hotkey  | Function
------------- | -------------
Escape  | Quit
Left/Right Arrow  | Next/Previous Image
Space  | Toggle Autoplay
R  | Rename Image
B  | Toggle Border
M  | Minimize
F  | Toggle Fullscreen
C  | Show Controls

### Renaming Images
When you press the "R" hotkey, you will be able to rename images. It will bring up a new window with the image, the old name, and a field for the new name. If you want to rename the image and leave it in the same folder, type the new name in the corresponding box and hit "Conform". If you want to rename it and move it to a different folder, hit "Move" and save it to a new location (the old version will be deleted). If you don't want to rename this image, hit the "Cancel" button or close the window.
### <a name="advanced-selection">Advanced Selection</a>
You can filter through the image files displayed using the search and exclude pattern fields. These will search based on single character tags in the file name between two ‘~’ symbols. For example, using a search pattern of “3” would show an image named “testimage~123~.png” and using an exclude patten of “1” would prevent it from being shown. The difference between the “and” and “or” versions of search patten are relevant when multiple tags are used. Using a search pattern of “14” would show “testimage~123~.png” for the “or” field but not for the “and” field. You can mix and match these three fields to perform more specific searches. Leaving all of these fields blank will not filter the output images.

## Tech Details
RandImg was made entirely with Microsoft's .NET Framework 4.7.2. Uses the addon Microsoft.WindowsAPICodePack-Shell for a better file selection GUI. 


