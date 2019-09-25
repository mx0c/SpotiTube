using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotiTube
{
    class Setting
    {
        public Setting() {
            this.downloadPath = "";
            this.volumePerc = 0;
        }

        public String downloadPath { get; set; }
        public int volumePerc { get; set; }
    }
}
