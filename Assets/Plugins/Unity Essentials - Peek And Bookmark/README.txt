Peek and Bookmark - Editor Toolkit

Peek, bookmark and browse previously selected objects. Greatly speed up your unity workflow!
Asset store link: https://assetstore.unity.com/packages/tools/utilities/peek-and-bookmark-editor-toolkit-180119
Bookmark gameObjects in play or edit mode. They are saved even when you change scene or after reloading project.

Get started:
- Find PeekAndBookmark preference:
	- on Unity 2017 and bellow, via Edit>Preference>Peek And BookMark
	- on Unity 2018 and higher, direclty via Unity Essentials>Peek And BookMark (or by right clic on peek toolbar)

- On the Peek toolbar (browse selection), you can scroll up and down to go thought previous selections
- You can defined global shortCut for previous/next arrow
- Bookmark anything, you can either:
	- right clic on an object, then go to UnityEssentials > BookMark
	- just add from the previously selected list the item you want with the bookmark button
- A GameObject not present in the current scene can be access with the little "unity" button next to the item name
- Keep track of deleted gameObjects (if an undo is applied, link is kept).

- Intelligent Outline: hold SHIFT and move your mouse to select gameObject on the sceneView based on visual: what you see is what you get
- Ultra Fast Selection of gameObjects Hidden behind others: point your mouse under a gameObject, hold SHIFT and scroll with mouse

Useful methods (see their summary for more info):
PeekLogic.GetLastObjectOfThisTypeOrFindItInAsset<YourScript>(ref UnityEngine.Object found, "Assets/")
PeekLogic.AddToBookMark(UnityEngine.Object toSave);
PeekLogic.OpenPeekWindow();
