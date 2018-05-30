using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPClient
{
    public class FileData
    {
        public Metadata Metadata { get; set; }
        public byte[] Data { get; set; }
    }
}
