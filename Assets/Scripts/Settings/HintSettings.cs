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
                @""
            },
            {
                HintAbout.ANIMATION_OPTIONS,
                @"In the animation type selection scene, you can select:

- <b>realtime animation</b> with video stream broadcast through the Spout system.
- <b>animation with recording</b>. For this option, you must also specify the animation time and FPS."
            },
            {
                HintAbout.ANIMATION_RUNTIME,
                ""
            },
            {
                HintAbout.ANIMATION_RECORD,
                ""
            },
            {
                HintAbout.RECORD_MANAGEMENT,
                ""
            },
        };
    }
}
