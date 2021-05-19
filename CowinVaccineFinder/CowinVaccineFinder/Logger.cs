using log4net;

namespace CowinVaccineFinder
{
    class Logger
    {
        static public ILog GetLogger<T>()
        {
            ILog log = LogManager.GetLogger(typeof(T));
            return log;
        }
    }
}
