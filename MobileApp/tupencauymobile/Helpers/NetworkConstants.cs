
namespace tupencauymobile.Helpers
{
    public static class NetworkConstants
    {
        public static string BaseAddress =
        DeviceInfo.Platform == DevicePlatform.Android ? "http://10.41.10.172:5234" : "http://localhost:5234";
        public static string localUrl = $"{BaseAddress}/api";
    }
}
