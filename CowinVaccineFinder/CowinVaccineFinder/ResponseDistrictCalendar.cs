using Newtonsoft.Json;
using System.Collections.Generic;

namespace CowinVaccineFinder
{
    class ResponseDistrictCalendar
    {
        [JsonProperty("centers")]
        public IEnumerable<CovidCenter> CovidCenters { get; set; }
    }
}
