using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DerOptimist
{
    public class InputFileGroup
    {
        public string BaseName;
        public int FirstDigit;
        public List<string> Files;
        public MediaType MediaT;

        public InputFileGroup()
        {
            FirstDigit = 0;
            BaseName = "";
            Files = new List<string>();
        }

        public InputFileGroup(string f)
        {
            Files = new List<string>();
            Files.Add(f);
        }
    }
}
