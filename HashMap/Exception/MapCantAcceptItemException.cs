namespace HashMap.Exception
{
    public class MapCantFitItemException : System.Exception
    {
        public MapCantFitItemException() { }
        public MapCantFitItemException(string message) : base(message) { }
        public MapCantFitItemException(string message, System.Exception inner) : base(message, inner) { }
    }
}
