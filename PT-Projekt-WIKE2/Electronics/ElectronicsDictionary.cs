using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PT_Projekt_WIKE2.Electronics
{
    public sealed class ElectronicsDictionary
    {
        private List<ElectronicElement> Elements { get; }

        public Tuple<ElectronicElement, int> FindClosest(string name)
        {
            int cost = 1000;
            ElectronicElement electronicElement = new ElectronicElement("", "", "");

            foreach (var e in Elements)
            {
                int temp = LevensteinDistance(name, e.Name.ToLower());

                if (temp < cost)
                {
                    cost = temp;
                    electronicElement = e;
                }
            }

            return new Tuple<ElectronicElement, int>(electronicElement, cost);
        }

        private int LevensteinDistance(string name, string name2)
        {
            int i, j, m, n, cost;
            int[,] d;

            m = name.Length;
            n = name2.Length;

            d = new int[m + 1, n + 1];

            for (i = 0; i <= m; i++)
                d[i, 0] = i;
            for (j = 1; j <= n; j++)
                d[0, j] = j;

            for (i = 1; i <= m; i++)
            {
                for (j = 1; j <= n; j++)
                {
                    if (name[i - 1] == name2[j - 1])
                        cost = 0;
                    else
                        cost = 1;

                    d[i, j] = Math.Min(d[i - 1, j] + 1,   /* remove */
                    Math.Min(d[i, j - 1] + 1,         /* insert */
                    d[i - 1, j - 1] + cost));        /* change */
                }
            }

            return d[m, n];
        }
        public ElectronicsDictionary()
        {
            Elements = new List<ElectronicElement> { new ElectronicElement(@"Ryzen 5 1600", @"https://en.wikichip.org/wiki/amd/ryzen_5/1600", @"Ryzen 5 1600 is a 64-bit hexa-core mid-range performance x86 desktop microprocessor set to be introduced by AMD in early 2017. This processor is based on AMD's Zen microarchitecture and is fabricated on a 14 nm process. The 1600 operates at a base frequency of 3.2 GHz with a TDP of 65 W and a Boost frequency of up to 3.6 GHz. This model is a less overclockable friendly version of the 1600X. This MPU supports up to 64 GiB of dual-channel DDR4-2666 ECC memory.") };
        }

        public ElectronicsDictionary(string json)
        {
            Elements = JsonConvert.DeserializeObject<List<ElectronicElement>>(json);
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(Elements);
        }
    }
}