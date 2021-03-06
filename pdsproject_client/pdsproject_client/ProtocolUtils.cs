﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerTest
{
    static class ProtocolUtils
    {

        public const string REQUEST = "request";
        public const string CONTENT = "content";
        public const string TYPE = "type";
        public const string FILE = "file";
        public const string NAME = "name";
        public const string SIZE = "size";

        public struct FileStruct {
            public string name;
            public long size;
        }
    }
}
