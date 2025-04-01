using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HintAbout
{
    UNKNOWN,
    MAIN_MENU,
    SETTINGS,
    ASSETS,
    ADD_ASSET_SETTINGS,
    SCENES,
    ADD_SCENE_SETTINGS,
    ANIMATION_OPTIONS,
    SCENE_EDITING,
    ANIMATION_RUNTIME,
    ANIMATION_RECORD,
    RECORD_MANAGEMENT
}

namespace Settings
{
    public static class Hints
    {
        public static HintAbout currentHintAbout = HintAbout.MAIN_MENU;
        public static string GetCurrentHintMessage()
        {
            return GetHintMessage(currentHintAbout);
        }
        public static string GetHintMessage(HintAbout hintAbout)
        {
            return hintsDictionary[hintAbout];
        }
        private static readonly Dictionary<HintAbout, string> hintsDictionary = new()
        {
            {
                HintAbout.MAIN_MENU,
                @"The <color=green><b>laser</b></color> coming out of your right controller is used to interact with the UI. Point and ""click"" with <color=blue><b>Trigger(R)</b></color>.

There are 4 buttons in the main menu:
- <b>Scenes</b> - to go to the menu with scenes adding/editing/adjusting scenes for animation;</li>
- <b>Assets</b> - to go to the menu with adding/editing/customizing assets (characters, locations, props);</li>
- <b>Settings</b> - to go to the menu of settings and calibration of trackers;</li>
- <b>Exit</b> - to exit the application.</li>

Sometimes you might need to:
- <b>enter text</b> - in these situations the virtual keyboard will be created, to use it click on keys with <color=green><b>laser</b></color>;
- <b>scroll list of options</b> - point at it with <color=green><b>laser</b></color>, and use <color=blue><b>Thumbstick(R)</b></color> to scroll.
- <b>select filepath</b> - in these cases the <color=green><b>file browser</b></color> will open, allowing you to browse through the files on your system

At any moment of using the application you can open situation-specific hints by holding <color=red><b>X button</b></color>."
            },
            {
                HintAbout.SETTINGS,
                @"The settings menu is used to calibrate the trackers. In particular, the following actions are possible:

- display the character's object for visualization;
- enable/disable the use of similar trackers from SteamVR in the application;
- adjust the general calibration (offset, smoothness of movements)
- calibrate each tracker individually (offset, rotation);
- save and download the calibration settings."
            },
            {
                HintAbout.ASSETS,
                @"The Assets menu is divided into two parts:

- <b>Assets List</b> - list of assets imported and ready for usage in scenes;
- <b>Asset Preview</b> - is displayed when you select any of the imported assets. It contains detailed information about a particular asset. At the bottom of the preview there is a button for deleting.

Also in the upper part there is a menubar for selecting the type of displayed assets (locations, characters, props respectively)"
            },
            {
                HintAbout.ADD_ASSET_SETTINGS,
                @"In the asset importing menu, you need to:
                
- enter the asset <b>name</b>;
- specify the <b>filepath</b> to a <b>.gltf, .glb, .vrm</b> file, representing this asset.

After specifying the above requirements, the ""Save"" button will become available."
            },
            {
                HintAbout.SCENES,
                @"The scene menu is divided into two parts:

- <b>Scenes List</b> - list of scenes created and ready for animation;
- <b>Scene Preview</b> - is displayed when you select any of the created scenes. It contains detailed information about a particular scene. At the bottom of the preview there are three buttons for deleting, editing and animation respectively.

To create a new scene, you should click on the ""Add new"" button."
            },
            {
                HintAbout.ADD_SCENE_SETTINGS,
                @"In the scene creation menu, you need to:

- enter the scene <b>name</b>
- specify the <b>location</b> where the animation will be performed
- specify the <b>characters</b> to be used in the scene

After specifying the above requirements, the ""Manage"" button will become available. When you click on it, you will be transferred to location for setting props in the scene"
            },
            {
                HintAbout.SCENE_EDITING,
                @"Possible actions:

- Use <color=blue><b>Thumbstick(R)</b></color> to select a prop to create (either camera or any object prop), or edit tool;
- Use <color=red><b>Thumbstick(L)</b></color> to move around(teleport) in location;
- Hold <color=blue><b>Grip(R)</b></color> to create a selected prop. Its transform will ""stick"" to a <color=blue><b>Controller(R)</b></color>. By moving <color=blue><b>Thumbstick(R)</b></color> you can change the distance between controller and a prop;
- Click <color=blue><b>Trigger(R)</b></color> while pointing on camera to change its settings (Spout sender name, FOV);
- Press <color=blue><b>A button/B button</b></color> to return to main menu with/without saving the scene respectively;
- While using edit tool you can hold <color=blue><b>Grip(R)</b></color> pointing on a prop to edit its position/distance."
            },
            {
                HintAbout.ANIMATION_OPTIONS,
                @"In the animation type selection scene, you can select:

- <b>realtime animation</b> with video stream broadcast through the Spout system.
- <b>animation with recording</b>. For this option, you must also specify the animation time and FPS."
            },
            {
                HintAbout.ANIMATION_RUNTIME,
                @"In Runtime Animation all the cameras have relative spout names, so they can be used in external applications. Possible actions:

- Use <color=blue><b>Thumbstick(R)</b></color> to select a character;
- Use <color=red><b>Thumbstick(L)</b></color> to move around(teleport) in location;
- Hold <color=red><b>Grip(L)</b></color> or <color=blue><b>Grip(R)</b></color> near object props to manipulate them. Their transform will ""stick"" to a controller;
- Click <color=blue><b>Trigger(R)</b></color> while pointing on camera to open its UI (with ""Take Screenshot"" and ""Set as main"" buttons);
  - <b>Take Screenshot</b> will save the image it ""sees"";
  - <b>Set as Main</b> will set this camera to be main camera(have ""Main"" spout name);
- Press <color=blue><b>B button</b></color> to return to main menu."
            },
            {
                HintAbout.ANIMATION_RECORD,
                @"In Record Animation you can record characters' movements separatly, so that in the final animation all of them will be involved. Possible actions:

- Use <color=blue><b>Thumbstick(R)</b></color> to select a character;
- Use <color=red><b>Thumbstick(L)</b></color> to move around(teleport) in location;
- Hold <color=red><b>Grip(L)</b></color> or <color=blue><b>Grip(R)</b></color> near object props to manipulate them. Their transform will ""stick"" to a controller;
- Press <color=blue><b>A button</b></color> to start the recording (timer near <color=blue><b>Controller(R)</b></color> will show seconds left);
- Press <color=red><b>Y button</b></color> to make a transition to an exporting scene (animations should be already recorded);
- Press <color=blue><b>B button</b></color> to return to main menu."
            },
            {
                HintAbout.RECORD_MANAGEMENT,
                @"In Record Management you can edit the exporting settings of the recorded animation and manipulate cameras.

Use <color=blue><b>Trigger(R)</b></color> to interact with UI:
- <b>Settings</b> button will open export settings window, where you can choose path to a folder, filename, preset and resolution;
- <b>Export</b> button will start exporting the animation frame by frame;
- <b>Tools</b>:
  - <b>Cut</b>. When this tool is selected you can click anywhere on timeline to make a cut, so that segments are separated;
  - <b>Switch</b>. When this tool is selected you can click on any segment to select the camera used;
  - <b>Join</b>. When this tool is selected you can hold any segment separator and move <color=green><b>laser</b></color> to the near left or the near right segment to join;
  - <b>Resize</b>. When this tool is selected you can hold any segment separator and move <color=green><b>laser</b></color> left or right to resize the segment.

Press <color=blue><b>B button</b></color> to return to main menu."
            },
        };
    }
}
