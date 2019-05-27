using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PT_Projekt_WIKE2.Electronics
{
    public sealed class ElectronicElement
    {
        public string Name { get; }
        public string URL { get; }
        public string Info { get; }

        public ElectronicElement(string name, string url, string info)
        {
            Name = name;
            URL = url;
            Info = info;
        }
    }
}
