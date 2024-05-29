using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okolo
{
    public class markers
    {
        public static List<markers> List { get; } = new List<markers>();

        public int id_marker { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public string name { get; set; }

        public markers(int id, double Lat, double Lng, string Name)
        {
            id_marker = id;
            lat = Lat;
            lng = Lng;
            name = Name;
        }
    }
}
