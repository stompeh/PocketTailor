using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

namespace PocketTailor
{

    public partial class SafeFindHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeFindHandle() : base(true) { }
        protected override bool ReleaseHandle()
        { return FindClose(this.handle); }

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FindClose(IntPtr handle);
    }

    public static class PocketHandler
    {
        private static ObservableCollection<string>? ADSstreamsList = new();

        private const int ERROR_HANDLE_EOF = 38;
        private enum StreamInfoLevels { FindStreamInfoStandard = 0 }


        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern SafeFindHandle FindFirstStreamW(
            [MarshalAs(UnmanagedType.LPWStr)] string lpFileName,
            StreamInfoLevels InfoLevel,
            [In, Out] WIN32_FIND_STREAM_DATA lpFindStreamData,
            uint dwFlags);


        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FindNextStreamW(
            SafeFindHandle hndFindFile, 
            [In, Out, MarshalAs(UnmanagedType.LPStruct)] WIN32_FIND_STREAM_DATA lpFindStreamData);


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private class WIN32_FIND_STREAM_DATA
        {
            public long StreamSize;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 296)]
            public string cStreamName;
        }

        public static ObservableCollection<string> EnumerateStreams(FileInfo file)
        {
            ADSstreamsList.Clear();

            if (file == null)
                throw new ArgumentNullException("file");

            WIN32_FIND_STREAM_DATA findStreamData = new WIN32_FIND_STREAM_DATA();
            SafeFindHandle handle = FindFirstStreamW(file.FullName, StreamInfoLevels.FindStreamInfoStandard, findStreamData, 0);

            if (handle.IsInvalid)
                throw new Win32Exception();

            try
            {
                do
                {
                    ADSstreamsList.Add(findStreamData.cStreamName);
                }
                while (FindNextStreamW(handle, findStreamData));

                int lastError = Marshal.GetLastWin32Error();

                if (lastError != ERROR_HANDLE_EOF) throw new Win32Exception(lastError);
            }
            finally
            {
                handle.Dispose();
            }

            return ADSstreamsList;
        }


    }
}
