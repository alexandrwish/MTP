using System;

namespace MTP
{
    public class App
    {
        private const bool UseMockDataStore = false;
        public const string BackendUrl = "https://megatestproject.herokuapp.com/";

        public static void Initialize()
        {
            if (UseMockDataStore)
                ServiceLocator.Instance.Register<IDataStore<Item>, MockDataStore>();
            else
                ServiceLocator.Instance.Register<IDataStore<Item>, CloudDataStore>();
        }
    }
}