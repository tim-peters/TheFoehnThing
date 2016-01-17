using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.ComponentModel;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

public class W7TouchManager : MonoBehaviour {
    private int hHook = 0;
	public bool isWinTouchDevice = false;

    #region WIN_API
    private delegate int HookProc(int code, IntPtr wParam, IntPtr lParam);
    private const int WH_GETMESSAGE = 3;
    private const int HC_ACTION = 0;
    private const int TWF_WANTPALM = 0x00000002;
    private const int WM_TOUCH = 0x0240;
    private const int WM_NCPOINTERDOWN = 0x0242;
    private const int WM_NCPOINTERUP = 0x0243;
    private const int WM_NCPOINTERUPDATE = 0x0241;
    private const int WM_POINTERACTIVATE = 0x024B;
    private const int WM_POINTERCAPTURECHANGED = 0x024C;
    private const int WM_POINTERDOWN = 0x0246;
    private const int WM_POINTERENTER = 0x0249;
    private const int WM_POINTERLEAVE = 0x024A;
    private const int WM_POINTERUP = 0x0247;
    private const int WM_POINTERUPDATE = 0x0245;
    private const int WM_TIMER = 0x0113;

    public const int TOUCHEVENTF_MOVE = 0x0001; //	Movement has occurred. Cannot be combined with TOUCHEVENTF_DOWN.
    public const int TOUCHEVENTF_DOWN = 0x0002; //	The corresponding touch point was established through a new contact. Cannot be combined with TOUCHEVENTF_MOVE or TOUCHEVENTF_UP.
    public const int TOUCHEVENTF_UP = 0x0004; // A touch point was removed.
    public const int TOUCHEVENTF_INRANGE = 0x0008; // A touch point is in range. This flag is used to enable touch hover support on compatible hardware. Applications that do not want support for hover can ignore this flag.
    public const int TOUCHEVENTF_PRIMARY = 0x0010; // Indicates that this TOUCHINPUT structure corresponds to a primary contact point. See the following text for more information on primary touch points.
    public const int TOUCHEVENTF_NOCOALESCE = 0x0020; // When received using GetTouchInputInfo, this input was not coalesced.
    public const int TOUCHEVENTF_PALM = 0x0080; // The touch event came from the user's palm.

    public const int POINTER_FLAG_NONE = 0x00000000;   //Default
    public const int POINTER_FLAG_NEW = 0x00000001;   //Indicates the arrival of a new pointer
    public const int POINTER_FLAG_INRANGE = 0x00000002;   //Indicates that this pointer continues to exist. When this flag is not set, it indicates the pointer has left detection range. 
    public const int POINTER_FLAG_INCONTACT = 0x00000004;   //Indicates that this pointer is in contact with the digitizer surface. When this flag is not set, it indicates a hovering pointer.
    public const int POINTER_FLAG_FIRSTBUTTON = 0x00000010;   //Indicates a primary action, analogous to a left mouse button down.
    public const int POINTER_FLAG_SECONDBUTTON = 0x00000020;   //Indicates a secondary action, analogous to a right mouse button down.
    public const int POINTER_FLAG_THIRDBUTTON = 0x00000040;   //Analogous to a mouse wheel button down.
    public const int POINTER_FLAG_FOURTHBUTTON = 0x00000080;   //
    public const int POINTER_FLAG_FIFTHBUTTON = 0x00000100;   //
    public const int POINTER_FLAG_PRIMARY = 0x00002000;   //Indicates that this pointer has been designated as the primary pointer.
    public const int POINTER_FLAG_CONFIDENCE = 0x00004000;   //The presence of this flag indicates that the source device has high confidence that this input is part of an intended interaction.
    public const int POINTER_FLAG_CANCELED = 0x00008000;
    public const int POINTER_FLAG_DOWN = 0x00010000;   //Indicates that this pointer transitioned to a down state; that is, it made contact with the digitizer surface.
    public const int POINTER_FLAG_UPDATE = 0x00020000;   //Indicates that this is a simple update that does not include pointer state changes.
    public const int POINTER_FLAG_UP = 0x00040000;   //Indicates that this pointer transitioned to an up state; that is, it broke contact with the digitizer surface.
    public const int POINTER_FLAG_WHEEL = 0x00080000;   //Indicates input associated with a pointer wheel. For mouse pointers, this is equivalent to the action of the mouse scroll wheel (WM_MOUSEWHEEL).
    public const int POINTER_FLAG_HWHEEL = 0x00100000;   //Indicates input associated with a pointer h-wheel. For mouse pointers, this is equivalent to the action of the mouse horizontal scroll wheel (WM_MOUSEHWHEEL).
    public const int POINTER_FLAG_CAPTURECHANGED = 0x00200000;   //Indicates that this pointer was captured by (associated with) another element and the original element has lost capture

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    private static extern int GetCurrentThreadId();

    [DllImport("kernel32.dll")]
    private static extern int GetLastError();

    [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    public static extern IntPtr GetDesktopWindow();

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetWindowRect(HandleRef hwnd, out RECT lpRect);

    [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    private static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    private static extern bool UnhookWindowsHookEx(int idHook);

    [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    private static extern int CallNextHookEx(int idHook, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool RegisterTouchWindow(IntPtr hwnd, uint flags);

    /*[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool RegisterPointerInputTarget(IntPtr hwnd, uint flags);*/

    [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CloseTouchInputHandle(IntPtr hTouchInput);

    [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    private static extern bool GetTouchInputInfo(IntPtr hTouchInput, int cInputs, [In, Out] TOUCHINPUT[] pInputs, int cbSize);

    [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    private static extern bool GetPointerInfo(uint pointerId, [In, Out] ref POINTER_INFO pointerInfo);

    [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    private static extern uint SetTimer(IntPtr hWnd, Int32 nIDEvent, Int32 uElapse, HookProc lpfn);

    [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    private static extern bool KillTimer(IntPtr hWnd, Int32 nIDEvent);

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT {
        public int Left;        // x position of upper-left corner
        public int Top;         // y position of upper-left corner
        public int Right;       // x position of lower-right corner
        public int Bottom;      // y position of lower-right corner
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT {
        public int X;
        public int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MSG {
        public IntPtr hwnd;
        public UInt32 message;
        public IntPtr wParam;
        public IntPtr lParam;
        public UInt32 time;
        public POINT pt;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TOUCHINPUT {
        public UInt32 x;
        public UInt32 y;
        public IntPtr hSource;
        public UInt32 dwID;
        public UInt32 dwFlags;
        public UInt32 dwMask;
        public UInt32 dwTime;
        public Int32 dwExtraInfo;
        public UInt32 cxContact;
        public UInt32 cyContact;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINTER_INFO {
        public POINTER_INPUT_TYPE pointerType;
        public UInt32 pointerID;
        public UInt32 frameID;
        public UInt32 pointerFlags;//Pointer flags
        public IntPtr sourceDevice; //handle
        public IntPtr hwndTarget; //hwnd
        public POINT ptPixelLocation;       //The predicted screen coordinates of the pointer, in pixels. 
        public POINT ptHimetricLocation;    //The predicted screen coordinates of the pointer, in HIMETRIC units. 
        public POINT ptPixelLocationRaw;    //The screen coordinates of the pointer, in pixels. For adjusted screen coordinates, see ptPixelLocation.
        public POINT ptHimetricLocationRaw; //The screen coordinates of the pointer, in HIMETRIC units. For adjusted screen coordinates, see ptHimetricLocation.
        public UInt32 dwTime;               //A message time stamp assigned by the system when this input was received.
        public UInt32 historyCount;
        public Int32 inputData;
        public UInt32 dwKeyStates;
        public UInt64 PerformanceCount;
        public UInt32 ButtonChangeType;     //POINTER_BUTTON_CHANGE_TYPE
    }

    public enum POINTER_INPUT_TYPE {
        PT_POINTER = 0x00000001,   //generic pointer type, this never appears in pointer messages or pointer data
        PT_TOUCH = 0x00000002,   //Touch pointer type
        PT_PEN = 0x00000003,   //Pen pointer type
        PT_MOUSE = 0x00000004    //Mouse pointer type
    }
    #endregion

    private static int screenOffsetX;
    private static int screenOffsetY;
    private static int screenWidth;
    private static int screenHeight;

    private static TOUCHINPUT[] inputs;

    private static Queue<TOUCHINPUT[]> rawTouchesQueue = new Queue<TOUCHINPUT[]>();
    private static List<W7Touch> touches = new List<W7Touch>();
    private static Dictionary<uint, uint> touchIdMap = new Dictionary<uint, uint>();
    private static uint lastTouchId = 100;

    void Start() {
        try {
            if (W7TouchCapabilities.HasTouch || W7TouchCapabilities.HasMultiTouch) {
                hHook = SetWindowsHookEx(WH_GETMESSAGE, new HookProc(TouchHookProc), (IntPtr)0, GetCurrentThreadId());
                Debug.Log("Installed touch hook");

                screenOffsetX = (int)Camera.main.pixelRect.x;
                screenOffsetY = (int)Camera.main.pixelRect.y;
                screenWidth = (int)Camera.main.pixelRect.width;
                screenHeight = (int)Camera.main.pixelRect.height;

                Debug.Log(string.Format("X-Offset: {0} Y-Offset: {1} Width: {2} Height: {3}", screenOffsetX, screenOffsetY, screenWidth, screenHeight));

                Debug.Log("Touch Manager successfully started");
				isWinTouchDevice = true;
            } else {
				isWinTouchDevice = false;
                Debug.LogError("Host seems not to be a Windows 7+ touch device. Assimilate mouse input instead.'"); 
            }
        } catch (Win32Exception) {
			isWinTouchDevice = false;
            Debug.Log("Could not install touch hook");
        }
    }

    void Update() {
        if (isWinTouchDevice && hHook != 0) {
            // clean previous touches
            List<W7Touch> touchesToDelete = new List<W7Touch>();

            // remove duplicate touch ids (down + move, move + move, ...)
            Dictionary<uint, W7Touch> duplicateTouches = new Dictionary<uint, W7Touch>();
            foreach (W7Touch touch in touches) {
                if (duplicateTouches.ContainsKey(touch.Id)) {
                    touchesToDelete.Add(duplicateTouches[touch.Id]);
                    duplicateTouches[touch.Id] = touch;
                } else {
                    duplicateTouches.Add(touch.Id, touch);
                }
            }
            foreach (W7Touch touch in touchesToDelete) {
                touches.Remove(touch);
            }
            touchesToDelete.Clear();

            // remove ended touches
            foreach (W7Touch touch in touches) {
                if (touch.Phase == TouchPhase.Ended || touch.Phase == TouchPhase.Canceled) {
                    touchesToDelete.Add(touch);
                    touchIdMap.Remove(touch.Id);
                } else if (touch.Phase == TouchPhase.Began) {
                    touch.Phase = TouchPhase.Stationary;
                }
            }
            foreach (W7Touch touch in touchesToDelete) {
                touches.Remove(touch);
            }
            touchesToDelete.Clear();

            lock (rawTouchesQueue) {
                while (rawTouchesQueue.Count > 0) {
                    TOUCHINPUT[] rawTouches = rawTouchesQueue.Dequeue();
                    for (uint i = 0; i < rawTouches.Length; ++i) {
                        TOUCHINPUT rawTouch = rawTouches[i];
                        //Debug.Log(rawTouch.dwID + ": " + rawTouch.dwFlags + " at " + rawTouch.x + "," + rawTouch.y);

                        if (!touchIdMap.ContainsKey(rawTouch.dwID)) {
                            touchIdMap.Add(rawTouch.dwID, lastTouchId);
                            lastTouchId++;
                        }

                        W7Touch touch;
                        try {
                            touch = touches.Find(t => t.Id == touchIdMap[rawTouch.dwID]);
                        } catch {
                            touch = null;
                        }

                        Vector2 pos = new Vector2(rawTouch.x * 0.01f, rawTouch.y * 0.01f);
                        if (touch != null) {
                            if ((rawTouch.dwFlags & TOUCHEVENTF_DOWN) == TOUCHEVENTF_DOWN) {
                                // duplicate down event, ignore
                                continue;
                            } else if ((rawTouch.dwFlags & TOUCHEVENTF_MOVE) == TOUCHEVENTF_MOVE) {
                                if (touch.Phase == TouchPhase.Canceled || touch.Phase == TouchPhase.Ended) {
                                    // moving an ended touch? do nothing, which means resume movement
                                    touch.UpdateTouch(pos);
                                } else if (touch.Phase == TouchPhase.Began) {
                                    // preserve began phase but update position;
                                    touch.UpdateTouch(pos);
                                    touch.Phase = TouchPhase.Began;
                                } else {
                                    touch.UpdateTouch(pos);
                                }
                            } else if ((rawTouch.dwFlags & TOUCHEVENTF_UP) == TOUCHEVENTF_UP) {
                                touch.EndTouch();
                            }
                        } else {
                            touch = new W7Touch(touchIdMap[rawTouch.dwID], pos);
                            touches.Add(touch);
                            if ((rawTouch.dwFlags & TOUCHEVENTF_DOWN) == TOUCHEVENTF_DOWN) {
                            } else if ((rawTouch.dwFlags & TOUCHEVENTF_MOVE) == TOUCHEVENTF_MOVE) {
                                // move without down? add a new movement touch
                                touch = new W7Touch(touchIdMap[rawTouch.dwID], pos);
                                touch.UpdateTouch(pos);
                                touches.Add(touch);
                            } else if ((rawTouch.dwFlags & TOUCHEVENTF_UP) == TOUCHEVENTF_UP) {
                                // up without down? add a new end touch
                                touch = new W7Touch(touchIdMap[rawTouch.dwID], pos);
                                touch.EndTouch();
                                touches.Add(touch);
                            }
                        }
                    }
                }
            }
            foreach (W7Touch touch in touches) {
                touch.Update();
                if (touch.Phase == TouchPhase.Canceled) {
                    var k = (from t in touchIdMap where t.Value == touch.Id select t.Key);
                    if (k.Count() > 0) {
                        touchIdMap.Remove(k.First());
                    }
                }
            }
        }
    }

    public static int GetTouchCount() {
        return touches.Count;
    }

    public static W7Touch GetTouch(int index) {
        return touches[index];
    }

    public int TouchHookProc(int nCode, IntPtr wParam, IntPtr lParam) {
        if (nCode < 0) {
            return CallNextHookEx(0, nCode, wParam, lParam);
        }
        switch (nCode) {
            case HC_ACTION:
                MSG msg = (MSG)Marshal.PtrToStructure(lParam, typeof(MSG));

                // It's ok to call this several times
                RegisterTouchWindow(msg.hwnd, TWF_WANTPALM);

                //RegisterPointerInputTarget(msg.hwnd, (int)POINTER_INPUT_TYPE.PT_TOUCH);

                switch (msg.message) {
                    case WM_TOUCH:
                        int inputCount = LoWord(msg.wParam.ToInt32());
                        inputs = new TOUCHINPUT[inputCount];

                        if (GetTouchInputInfo(msg.lParam, inputCount, inputs, Marshal.SizeOf(typeof(TOUCHINPUT)))) {
                            lock (rawTouchesQueue) {
                                rawTouchesQueue.Enqueue(inputs);
                            }
                            CloseTouchInputHandle(msg.lParam);
                        }
                        break;
                    case WM_NCPOINTERDOWN:
                    case WM_NCPOINTERUP:
                    case WM_NCPOINTERUPDATE:
                    case WM_POINTERACTIVATE:
                    case WM_POINTERCAPTURECHANGED:
                    case WM_POINTERDOWN:
                    case WM_POINTERENTER:
                    case WM_POINTERLEAVE:
                    case WM_POINTERUP:
                    case WM_POINTERUPDATE:
                        uint pointerID = (uint)LoWord(msg.wParam.ToInt32());
                        POINTER_INFO pointerInfo = new POINTER_INFO();

                        if (GetPointerInfo(pointerID, ref pointerInfo)) {
                            inputs = new TOUCHINPUT[1];
                            inputs[0] = new TOUCHINPUT();
                            inputs[0].dwID = pointerInfo.pointerID;
                            inputs[0].x = (uint)pointerInfo.ptPixelLocation.X * 100;
                            inputs[0].y = (uint)pointerInfo.ptPixelLocation.Y * 100;
                            inputs[0].dwFlags = 0;
                            if ((pointerInfo.pointerFlags & POINTER_FLAG_DOWN) == POINTER_FLAG_DOWN) {
                                inputs[0].dwFlags |= TOUCHEVENTF_DOWN;
                            }
                            if ((pointerInfo.pointerFlags & POINTER_FLAG_UPDATE) == POINTER_FLAG_UPDATE) {
                                inputs[0].dwFlags |= TOUCHEVENTF_MOVE;
                            }
                            if ((pointerInfo.pointerFlags & POINTER_FLAG_UP) == POINTER_FLAG_UP) {
                                inputs[0].dwFlags |= TOUCHEVENTF_UP;
                            }
                            lock (rawTouchesQueue) {
                                rawTouchesQueue.Enqueue(inputs);
                            }
                        }
                        break;
                }
                break;
        }
        return CallNextHookEx(0, nCode, wParam, lParam);
    }

    void OnDestroy() {
        if (hHook != 0) {
            if (!UnhookWindowsHookEx(hHook))
                Debug.LogError("Couldn't Unhook Window");

            Debug.Log("Window successfully Unhooked");
        }
    }

    private static int LoWord(int number) {
        return number & 0xffff;
    }
}