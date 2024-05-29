using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okolo
{
    class Position
    {
        public static List<Position> List { get; } = new List<Position>();

        public int id_position { get; set; }
        public string name { get; set; }
        public int salary { get; set; }
        public string decription { get; set; }
    }
}
