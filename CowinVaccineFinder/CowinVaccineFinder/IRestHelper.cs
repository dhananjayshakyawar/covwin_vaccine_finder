using RestSharp;

namespace CowinVaccineFinder
{
    internal interface IRestHelper
    {
        string URL { get; }
        RestClient GetRestClient { get; }

    }
}