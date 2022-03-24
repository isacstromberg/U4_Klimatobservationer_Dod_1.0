using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace U4_Klimatobservationer_Dod_1._0.Models
{
    public class Observation
    {
        public int? id { get; set; }
        public DateTime? date { get; set; }
        public int? observer_id { get; set; }

        public int? geolocation_id { get; set; }



        public override string ToString()
        {
            return $" ID: {id} DATE: {date} Observer_ID: {observer_id} Geolocation_id: {geolocation_id}";
        }

    }
}
