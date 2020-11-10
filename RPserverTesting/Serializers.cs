using Newtonsoft.Json.Linq;

namespace RPserverTesting
{
    public class IdObject
    {
        public string id { get; set; }
    }
    
    public class IdVObject:IdObject
    {
        public string version { get; set; }
    }

    public class PingObject
    {
        public int ping { get; set; }
    }

    public class RConnectM:IdVObject
    {
        public int exercise_id { get; set;}
    }

    public class Error
    {
        public int code { get; set; }
        public int param { get; set; }
        public string message { get; set; }
    }

    public class RConnectMError:IdObject
    {
        public Error error { get; set; }
    }
        
}