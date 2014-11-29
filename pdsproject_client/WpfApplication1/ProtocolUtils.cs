using System;
using System.Collections.Concurrent;

namespace Protocol
{
    public class ProtocolUtils
    {
        public static ConcurrentDictionary<string, string> protocolDictionary = new ConcurrentDictionary<string, string>();

        public const string REQUEST = "request";
        public const string CONTENT = "content";
        public const string TYPE = "type";
        public const string FILE = "file";
        public const string NAME = "name";
        public const string SIZE = "size";

        public const string SET_CLIPBOARD_TEXT = "CLIPBOARD_TEXT";
        public const string SET_CLIPBOARD_FILES = "CLIPBOARD_FILES";
        public const string SET_CLIPBOARD_IMAGE = "CLIPBOARD_IMAGE";
        public const string SET_CLIPBOARD_AUDIO = "CLIPBOARD_AUDIO";
        public const string TRANSFER_FILES = "BEGIN_TRANSFER_FILES";
        public const string TRANSFER_IMAGE = "BEGIN_TRANSFER_IMAGE";
        public const string GET_CLIPBOARD_DIMENSION = "GET_CLIPBOARD_DIMENSION";
        public const string GET_CLIPBOARD_CONTENT = "GET_CLIPBOARD_CONTENT";
        public const string GET_CLIPBOARD_FILES = "GET_CLIPBOARD_FILES";
        public const string GET_CLIPBOARD_IMG = "GET_CLIPBOARD_IMG";
        public const string GET_CLIPBOARD_AUDIO = "GET_CLIPBOARD_AUDIO";
        public const string GET_CLIPBOARD_DATA = "GET_CLIPBOARD_DATA";
        public const string SET_RESET_FOCUS = "SET_RESET_FOCUS";
        public const string FOCUS_ON = "FOCUS_ON";
        public const string FOCUS_OFF = "FOCUS_OFF";
        public const string TRY_AUTHENTICATE = "TRY_AUTHENTICATE";
        public const string REMOTE_PASTE = "REMOTE_PASTE";

        public const string TMP_IMAGE_FILE = ".\\tmp.jpg";
        public const string TMP_DIR = ".\\tmp\\";

        public const ushort TOKEN_DIM = 16;

        public const long CLIBPOARD_DIM_THRESHOLD = 5 * 1024 * 1024;

        public struct FileStruct
        {
            public string name;
            public long size;
            public string dir;
        }      
    }
}
