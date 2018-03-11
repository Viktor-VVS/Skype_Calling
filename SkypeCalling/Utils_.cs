using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SkypeCalling
{
    public class Utils_
    {
        public static void ActionWithGuiThreadInvoke(Control obj, Action _action)
        {
            //obj.Dispatcher.Invoke(new Action(delegate
            obj.Dispatcher.Invoke(new Action(delegate
            {
                _action.Invoke();
            }
            ));
        }

        public static void ActionInThread(Action _action)
        {
            Task.Factory.StartNew(() => _action.Invoke());
        }
        /*
         using 
            Utils.ActionInThread( () =>
            {

            });
         */
      
      
    }

    public class StringsParser
    {
        public string Content { get; private set; }
        public string extractedString { get; private set; }
        public int movePosition { get; set; }
        public int extractPosition { get; set; }
        public int startPosition { get; set; }
        public StringsParser(string content)
        {
            Content = content;
            extractPosition = 0;
            movePosition = 0;
            extractedString = ""; // позиция которая нужна для Extract/вырезки
            startPosition = 0; //позиция с которой начнается каждый следующ поиск 

        }

        public bool backSearchTo(string findOccur, bool caseSensitive = true)
        {
            //if ((movePosition = Content.LastIndexOf(findOccur, 0, startPosition, caseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase)) != -1)
            if ((movePosition = Content.LastIndexOf(findOccur, 0, startPosition, caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase)) != -1)
            {
                //startPosition = movePosition + findOccur.Length;
                startPosition = movePosition;
                return true;
            }
            else
                return false;
        }

        public bool backSearchTo_NotIncluding(string findOccur, bool caseSensitive = true)
        {
            //if ((movePosition = Content.LastIndexOf(findOccur, 0, startPosition, caseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase)) != -1)
            if ((movePosition = Content.LastIndexOf(findOccur, 0, startPosition, caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase)) != -1)

            {
                //startPosition = movePosition + findOccur.Length;
                startPosition = movePosition;
                movePosition = startPosition + findOccur.Length;
                return true;
            }
            else
                return false;
        }

        // set cursor at the begin found findOccur
        public bool searchTo(string findOccur, bool caseSensitive = true)
        {
            //if ((movePosition = Content.IndexOf(findOccur, startPosition, caseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase)) != -1)
            if ((movePosition = Content.IndexOf(findOccur, startPosition, caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase)) != -1)
            {
                startPosition = movePosition + findOccur.Length;
                return true;
            }
            else
                return false;
        }
        // set cursor after found findOccur
        public bool searchTo_NotIncluding(string findOccur, bool caseSensitive = true)
        {
            //if ((movePosition = Content.IndexOf(findOccur, startPosition, caseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase)) != -1)
            if ((movePosition = Content.IndexOf(findOccur, startPosition, caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase)) != -1)
            {
                startPosition = movePosition + findOccur.Length;
                movePosition = startPosition;
                return true;
            }
            else
                return false;
        }
        public bool exctractTo(string findOccur, bool caseSensitive = true)
        {
            //if ((extractPosition = Content.IndexOf(findOccur, startPosition, caseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase)) != -1)
            if ((extractPosition = Content.IndexOf(findOccur, startPosition, caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase)) != -1)
            {
                extractPosition = extractPosition + findOccur.Length;
                startPosition = extractPosition;
                extractedString = Content.Substring(movePosition, extractPosition - movePosition);
                return true;
            }
            else
                return false;
        }
        public bool exctractTo_NotIncluding(string findOccur, bool caseSensitive = true)
        {
            //if ((extractPosition = Content.IndexOf(findOccur, startPosition, caseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase)) != -1)
            if ((extractPosition = Content.IndexOf(findOccur, startPosition, caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase)) != -1)
            {
                startPosition = extractPosition + findOccur.Length;
                extractedString = Content.Substring(movePosition, extractPosition - movePosition);


                return true;
            }
            else
                return false;
        }

        public bool exctractToEnd()
        {
            //if ((extractPosition = Content.IndexOf(findOccur, startPosition, caseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase)) != -1)

            extractPosition = Content.Length;
            startPosition = extractPosition;
            extractedString = Content.Substring(movePosition, extractPosition - movePosition);

            return true;
        }

        public bool exctractToEnd2()
        {
            //if ((extractPosition = Content.IndexOf(findOccur, startPosition, caseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase)) != -1)

            extractPosition = Content.Length;
            extractedString = Content.Substring(startPosition, extractPosition - startPosition);

            return true;
        }

        public void replaceExtractedWith(string newContent)
        {
            int extractLen = extractPosition - movePosition;
            Content = Content.Remove(movePosition, extractLen);
            Content = Content.Insert(movePosition, newContent);
            startPosition += newContent.Length - extractLen;
        }
        public void Reset()
        {
            extractPosition = 0;
            movePosition = 0;
            extractedString = "";
            startPosition = 0;
        }

    }

    public static class Extract
    {
        public static string Between(string Src, string start1, string start2)
        {
            StringsParser sp = new StringsParser(Src);
            sp.searchTo_NotIncluding(start1);
            sp.exctractTo_NotIncluding(start2);
            return sp.extractedString;
        }

        public static string BetweenEnd(string Src, string start1)
        {
            StringsParser sp = new StringsParser(Src);
            sp.searchTo_NotIncluding(start1);
            sp.exctractToEnd2();
            return sp.extractedString;
        }

        public static string BetweenStart(string Src, string start2)
        {
            StringsParser sp = new StringsParser(Src);
            sp.exctractTo_NotIncluding(start2);
            return sp.extractedString;
        }


        public static string BetweenInclude(string Src, string start1, string start2)
        {
            StringsParser sp = new StringsParser(Src);
            sp.searchTo(start1);
            sp.exctractTo(start2);
            return sp.extractedString;
        }
    }

    public enum WINDOW_MESSAGE : uint
    {
        WM_ACTIVATE = 0x0006,
        WM_ACTIVATEAPP = 0x001C,
        WM_AFXFIRST = 0x0360,
        WM_AFXLAST = 0x037F,
        WM_APP = 0x8000,
        WM_ASKCBFORMATNAME = 0x030C,
        WM_CANCELJOURNAL = 0x004B,
        WM_CANCELMODE = 0x001F,
        WM_CAPTURECHANGED = 0x0215,
        WM_CHANGECBCHAIN = 0x030D,
        WM_CHANGEUISTATE = 0x0127,
        WM_CHAR = 0x0102,
        WM_CHARTOITEM = 0x002F,
        WM_CHILDACTIVATE = 0x0022,
        WM_CLEAR = 0x0303,
        WM_CLOSE = 0x0010,
        WM_COMMAND = 0x0111,
        WM_COMPACTING = 0x0041,
        WM_COMPAREITEM = 0x0039,
        WM_CONTEXTMENU = 0x007B,
        WM_COPY = 0x0301,
        WM_COPYDATA = 0x004A,
        WM_CREATE = 0x0001,
        WM_CTLCOLORBTN = 0x0135,
        WM_CTLCOLORDLG = 0x0136,
        WM_CTLCOLOREDIT = 0x0133,
        WM_CTLCOLORLISTBOX = 0x0134,
        WM_CTLCOLORMSGBOX = 0x0132,
        WM_CTLCOLORSCROLLBAR = 0x0137,
        WM_CTLCOLORSTATIC = 0x0138,
        WM_CUT = 0x0300,
        WM_DEADCHAR = 0x0103,
        WM_DELETEITEM = 0x002D,
        WM_DESTROY = 0x0002,
        WM_DESTROYCLIPBOARD = 0x0307,
        WM_DEVICECHANGE = 0x0219,
        WM_DEVMODECHANGE = 0x001B,
        WM_DISPLAYCHANGE = 0x007E,
        WM_DRAWCLIPBOARD = 0x0308,
        WM_DRAWITEM = 0x002B,
        WM_DROPFILES = 0x0233,
        WM_ENABLE = 0x000A,
        WM_ENDSESSION = 0x0016,
        WM_ENTERIDLE = 0x0121,
        WM_ENTERMENULOOP = 0x0211,
        WM_ENTERSIZEMOVE = 0x0231,
        WM_ERASEBKGND = 0x0014,
        WM_EXITMENULOOP = 0x0212,
        WM_EXITSIZEMOVE = 0x0232,
        WM_FONTCHANGE = 0x001D,
        WM_GETDLGCODE = 0x0087,
        WM_GETFONT = 0x0031,
        WM_GETHOTKEY = 0x0033,
        WM_GETICON = 0x007F,
        WM_GETMINMAXINFO = 0x0024,
        WM_GETOBJECT = 0x003D,
        WM_GETTEXT = 0x000D,
        WM_GETTEXTLENGTH = 0x000E,
        WM_HANDHELDFIRST = 0x0358,
        WM_HANDHELDLAST = 0x035F,
        WM_HELP = 0x0053,
        WM_HOTKEY = 0x0312,
        WM_HSCROLL = 0x0114,
        WM_HSCROLLCLIPBOARD = 0x030E,
        WM_ICONERASEBKGND = 0x0027,
        WM_IME_CHAR = 0x0286,
        WM_IME_COMPOSITION = 0x010F,
        WM_IME_COMPOSITIONFULL = 0x0284,
        WM_IME_CONTROL = 0x0283,
        WM_IME_ENDCOMPOSITION = 0x010E,
        WM_IME_KEYDOWN = 0x0290,
        WM_IME_KEYLAST = 0x010F,
        WM_IME_KEYUP = 0x0291,
        WM_IME_NOTIFY = 0x0282,
        WM_IME_REQUEST = 0x0288,
        WM_IME_SELECT = 0x0285,
        WM_IME_SETCONTEXT = 0x0281,
        WM_IME_STARTCOMPOSITION = 0x010D,
        WM_INITDIALOG = 0x0110,
        WM_INITMENU = 0x0116,
        WM_INITMENUPOPUP = 0x0117,
        WM_INPUTLANGCHANGE = 0x0051,
        WM_INPUTLANGCHANGEREQUEST = 0x0050,
        WM_KEYDOWN = 0x0100,
        WM_KEYFIRST = 0x0100,
        WM_KEYLAST = 0x0108,
        WM_KEYUP = 0x0101,
        WM_KILLFOCUS = 0x0008,
        WM_LBUTTONDBLCLK = 0x0203,
        WM_LBUTTONDOWN = 0x0201,
        WM_LBUTTONUP = 0x0202,
        WM_MBUTTONDBLCLK = 0x0209,
        WM_MBUTTONDOWN = 0x0207,
        WM_MBUTTONUP = 0x0208,
        WM_MDIACTIVATE = 0x0222,
        WM_MDICASCADE = 0x0227,
        WM_MDICREATE = 0x0220,
        WM_MDIDESTROY = 0x0221,
        WM_MDIGETACTIVE = 0x0229,
        WM_MDIICONARRANGE = 0x0228,
        WM_MDIMAXIMIZE = 0x0225,
        WM_MDINEXT = 0x0224,
        WM_MDIREFRESHMENU = 0x0234,
        WM_MDIRESTORE = 0x0223,
        WM_MDISETMENU = 0x0230,
        WM_MDITILE = 0x0226,
        WM_MEASUREITEM = 0x002C,
        WM_MENUCHAR = 0x0120,
        WM_MENUCOMMAND = 0x0126,
        WM_MENUDRAG = 0x0123,
        WM_MENUGETOBJECT = 0x0124,
        WM_MENURBUTTONUP = 0x0122,
        WM_MENUSELECT = 0x011F,
        WM_MOUSEACTIVATE = 0x0021,
        WM_MOUSEFIRST = 0x0200,
        WM_MOUSEHOVER = 0x02A1,
        WM_MOUSELAST = 0x020D,
        WM_MOUSELEAVE = 0x02A3,
        WM_MOUSEMOVE = 0x0200,
        WM_MOUSEWHEEL = 0x020A,
        WM_MOUSEHWHEEL = 0x020E,
        WM_MOVE = 0x0003,
        WM_MOVING = 0x0216,
        WM_NCACTIVATE = 0x0086,
        WM_NCCALCSIZE = 0x0083,
        WM_NCCREATE = 0x0081,
        WM_NCDESTROY = 0x0082,
        WM_NCHITTEST = 0x0084,
        WM_NCLBUTTONDBLCLK = 0x00A3,
        WM_NCLBUTTONDOWN = 0x00A1,
        WM_NCLBUTTONUP = 0x00A2,
        WM_NCMBUTTONDBLCLK = 0x00A9,
        WM_NCMBUTTONDOWN = 0x00A7,
        WM_NCMBUTTONUP = 0x00A8,
        WM_NCMOUSEHOVER = 0x02A0,
        WM_NCMOUSELEAVE = 0x02A2,
        WM_NCMOUSEMOVE = 0x00A0,
        WM_NCPAINT = 0x0085,
        WM_NCRBUTTONDBLCLK = 0x00A6,
        WM_NCRBUTTONDOWN = 0x00A4,
        WM_NCRBUTTONUP = 0x00A5,
        WM_NCXBUTTONDBLCLK = 0x00AD,
        WM_NCXBUTTONDOWN = 0x00AB,
        WM_NCXBUTTONUP = 0x00AC,
        WM_NCUAHDRAWCAPTION = 0x00AE,
        WM_NCUAHDRAWFRAME = 0x00AF,
        WM_NEXTDLGCTL = 0x0028,
        WM_NEXTMENU = 0x0213,
        WM_NOTIFY = 0x004E,
        WM_NOTIFYFORMAT = 0x0055,
        WM_NULL = 0x0000,
        WM_PAINT = 0x000F,
        WM_PAINTCLIPBOARD = 0x0309,
        WM_PAINTICON = 0x0026,
        WM_PALETTECHANGED = 0x0311,
        WM_PALETTEISCHANGING = 0x0310,
        WM_PARENTNOTIFY = 0x0210,
        WM_PASTE = 0x0302,
        WM_PENWINFIRST = 0x0380,
        WM_PENWINLAST = 0x038F,
        WM_POWER = 0x0048,
        WM_POWERBROADCAST = 0x0218,
        WM_PRINT = 0x0317,
        WM_PRINTCLIENT = 0x0318,
        WM_QUERYDRAGICON = 0x0037,
        WM_QUERYENDSESSION = 0x0011,
        WM_QUERYNEWPALETTE = 0x030F,
        WM_QUERYOPEN = 0x0013,
        WM_QUEUESYNC = 0x0023,
        WM_QUIT = 0x0012,
        WM_RBUTTONDBLCLK = 0x0206,
        WM_RBUTTONDOWN = 0x0204,
        WM_RBUTTONUP = 0x0205,
        WM_RENDERALLFORMATS = 0x0306,
        WM_RENDERFORMAT = 0x0305,
        WM_SETCURSOR = 0x0020,
        WM_SETFOCUS = 0x0007,
        WM_SETFONT = 0x0030,
        WM_SETHOTKEY = 0x0032,
        WM_SETICON = 0x0080,
        WM_SETREDRAW = 0x000B,
        WM_SETTEXT = 0x000C,
        WM_SETTINGCHANGE = 0x001A,
        WM_SHOWWINDOW = 0x0018,
        WM_SIZE = 0x0005,
        WM_SIZECLIPBOARD = 0x030B,
        WM_SIZING = 0x0214,
        WM_SPOOLERSTATUS = 0x002A,
        WM_STYLECHANGED = 0x007D,
        WM_STYLECHANGING = 0x007C,
        WM_SYNCPAINT = 0x0088,
        WM_SYSCHAR = 0x0106,
        WM_SYSCOLORCHANGE = 0x0015,
        WM_SYSCOMMAND = 0x0112,
        WM_SYSDEADCHAR = 0x0107,
        WM_SYSKEYDOWN = 0x0104,
        WM_SYSKEYUP = 0x0105,
        WM_TCARD = 0x0052,
        WM_TIMECHANGE = 0x001E,
        WM_TIMER = 0x0113,
        WM_UNDO = 0x0304,
        WM_UNINITMENUPOPUP = 0x0125,
        WM_USER = 0x0400,
        WM_USERCHANGED = 0x0054,
        WM_VKEYTOITEM = 0x002E,
        WM_VSCROLL = 0x0115,
        WM_VSCROLLCLIPBOARD = 0x030A,
        WM_WINDOWPOSCHANGED = 0x0047,
        WM_WINDOWPOSCHANGING = 0x0046,
        WM_WININICHANGE = 0x001A,
        WM_XBUTTONDBLCLK = 0x020D,
        WM_XBUTTONDOWN = 0x020B,
        WM_XBUTTONUP = 0x020C
    }

    public class Gui
    {
        public class WindowInfoEx
        {
            public string windowName { get; set; }
            public string className { get; set; }
            public int hwnd { get; set; }
            public int threadId { get; set; }
            public int pid { get; set; }
            public user32.WINDOWINFO winfo;
        }
        public static List<int> FindAllWindows_TopWindows()
        {
            List<int> hwnds = new List<int>();
            user32.EnumWindows((hwnd, param) =>
            {
                hwnds.Add((int)hwnd);
                return true;
            }, IntPtr.Zero);

            return hwnds;
        }

        public static string GetTextOfWindow(int hwnd)
        {
            int length = (int)user32.SendMessage((IntPtr)hwnd, (uint)WINDOW_MESSAGE.WM_GETTEXTLENGTH, IntPtr.Zero, null);
            StringBuilder sb = new StringBuilder(length + 1);
            user32.SendMessage((IntPtr)hwnd, (uint)WINDOW_MESSAGE.WM_GETTEXT, (IntPtr)sb.Capacity, sb);
            return sb.ToString();
        }

        public static string GetClassOfWindow(int hwnd)
        {
            StringBuilder ClassName = new StringBuilder(256);
            user32.GetClassName((IntPtr)hwnd, ClassName, ClassName.Capacity);
            return ClassName.ToString();
        }

        public static WindowInfoEx GetWindowInformation(int hwnd)
        {
            WindowInfoEx findowInfo = new WindowInfoEx();
            findowInfo.hwnd = hwnd;
            user32.WINDOWINFO info = new user32.WINDOWINFO();
            info.cbSize = (uint)Marshal.SizeOf(info);
            user32.GetWindowInfo((IntPtr)hwnd, ref info);

            int processID = 0;
            int threadID = user32.GetWindowThreadProcessId((IntPtr)hwnd, out processID);
            findowInfo.pid = processID;
            findowInfo.threadId = threadID;
            findowInfo.windowName = GetTextOfWindow(hwnd);
            findowInfo.className = GetClassOfWindow(hwnd);

            return findowInfo;
        }
    }

    public static class user32
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWINFO
        {
            public uint cbSize;
            public RECT rcWindow;
            public RECT rcClient;
            public uint dwStyle;
            public uint dwExStyle;
            public uint dwWindowStatus;
            public uint cxWindowBorders;
            public uint cyWindowBorders;
            public ushort atomWindowType;
            public ushort wCreatorVersion;

            public WINDOWINFO(Boolean? filler)
                : this()   // Allows automatic initialization of "cbSize" with "new WINDOWINFO(null/true/false)".
            {
                cbSize = (UInt32)(Marshal.SizeOf(typeof(WINDOWINFO)));
            }

        }

        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, [Out] StringBuilder lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowInfo(IntPtr hwnd, ref WINDOWINFO pwi);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowThreadProcessId([In] IntPtr hWnd, [Out, Optional] IntPtr lpdwProcessId);

        [DllImport("user32.dll")]
        public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hWnd);
    }
}
