using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dango_viewer {
    class Dango {
        public string Kanji { get; set; }
        public string Kana { get; set; }
        public string Meanning { get; set; }

        public bool GetIsEmpty() {
            return Kanji == "" && Kana == "" && Meanning == "";
        }
    }
}
