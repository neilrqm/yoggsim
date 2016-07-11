using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoggSim
{
    class Program
    {
        static void Main(string[] args)
        {
            CardFactory.GetRandomMinion();  // trigger static constructor
        }
    }
}
