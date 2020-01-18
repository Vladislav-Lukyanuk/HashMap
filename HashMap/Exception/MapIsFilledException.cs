namespace HashMap.Exception
{
    public class MapIsFilledException : System.Exception
    {
        public MapIsFilledException() { }
        public MapIsFilledException(string message): base(message) { }
        public MapIsFilledException(string message, System.Exception inner) : base(message, inner) { }
    }
}
