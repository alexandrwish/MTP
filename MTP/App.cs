using System;

namespace MTP
{
    public class App
    {
        public static bool UseMockDataStore = false;
        public static string BackendUrl = "http://megatestproject.herokuapp.com/rest/";

        public static void Initialize()
        {
            if (UseMockDataStore)
                ServiceLocator.Instance.Register<IDataStore<Item>, MockDataStore>();
            else
                ServiceLocator.Instance.Register<IDataStore<Item>, CloudDataStore>();
        }
    }
}