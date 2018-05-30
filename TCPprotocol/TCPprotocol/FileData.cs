using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPprotocol
{
    public class FileData
    {
        public Metadata Metadata { get; set; }
        public byte[] Data { get; set; }
    }
}
