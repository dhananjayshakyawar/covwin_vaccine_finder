using System;
using System.Collections.Generic;

namespace CowinVaccineFinder
{
    internal interface ICowinService
    {
        IEnumerable<State> GetAllStates();
        IEnumerable<District> GetAllDistrictsByState(State state);
        IEnumerable<CovidCenter> GetDistrictSchedule(District district, DateTime startDate);
    }
}