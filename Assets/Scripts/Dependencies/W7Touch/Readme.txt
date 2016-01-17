Instructions
------------
- Add the W7TouchManager to the scene.
- The W7TouchManager has static methods GetTouch and GetTouchCount to retrieve the touches. You should use this methods to process the touches.
- Each W7Touch has a unique id, absolute position, delta position and delta time.

NGUI instructions
-----------------
- Add the W7TouchManager to the scene.
- Add the UICameraW7Touch next to your UICamera.
- Single touches over UIWidgets are converted to NGUI standard events.
- Activate "Use gestures" to receive two finger gestures (OnScroll, OnDrag, OnTwist) in your UIWidgets.
- Activate "Use raw" to receive all the touches in your UIWidgets (OnTouch).


Considerations
--------------
- More than one touch per finger can be received, depending on your update rate and the windows message queue.
- In the editor, touches are detected in the whole window, not just in the game window, thus the coordinates are calculated relative to the whole editor window. For example, you must touch the bottom right corner of the editor window to receive an event in your bottom right corner of the game window. If anyone knows how to calculate the game window rect, please tell me.

FAQ
---
- I get some errors in UICameraW7Touch like "UICamera.GetTouch(int) is inaccessible due to its protection level":
Update to lastest NGUI version.

- I get duplicated NGUI events:
Disable "use mouse" in UICamera.
