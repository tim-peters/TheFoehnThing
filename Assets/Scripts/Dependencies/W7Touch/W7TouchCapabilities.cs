using System;
using System.Runtime.InteropServices;

static class W7TouchCapabilities {
    // Taken from the Windows 7 SDK, possible transcription errors.
    [Flags]
    private enum TouchMetrics {
        IntegratedTouch = 0x1,
        ExternalTouch = 0x2,
        IntegratedPen = 0x4,
        ExternalPen = 0x8,
        MultiInput = 0x40,
        StackReady = 0x80
    }

    public static bool HasMultiTouch {
        get {
            return ((((TouchMetrics)GetSystemMetrics(SM_DIGITIZER)) &
                TouchMetrics.MultiInput) != 0);
        }
    }

    public static bool HasTouch {
        get {
            return ((((TouchMetrics)GetSystemMetrics(SM_DIGITIZER)) &
                TouchMetrics.IntegratedTouch) != 0) || 
                ((((TouchMetrics)GetSystemMetrics(SM_DIGITIZER)) &
                TouchMetrics.ExternalTouch) != 0);
        }
    }

    const int SM_DIGITIZER = 94;

    // Windows 7 SDK says that a failure here ( 0 ) will not set LastError.
    [DllImport("user32.dll")]
    private static extern int GetSystemMetrics(int nIndex);
}