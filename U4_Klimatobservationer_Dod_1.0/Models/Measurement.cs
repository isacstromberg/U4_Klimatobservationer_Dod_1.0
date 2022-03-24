using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace U4_Klimatobservationer_Dod_1._0.Models
{
    public class Measurement
    {
        public int? id { get; set; }
        public double? value { get; set; }
        public int? observation_id { get; set; }

        public int? category_id { get; set; }


        public override string ToString()
        {
             return $"ID: {id} Value: {value} Observation_ID: {observation_id} Category_id: {category_id}";
        }

    }
}
