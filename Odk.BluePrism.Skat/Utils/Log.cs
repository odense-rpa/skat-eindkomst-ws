namespace Odk.BluePrism.Skat.Utils
{
    public class Log : ILog
    {
        public void Error(string message)
        {
            var msg = $"Error: {message}";
            System.Diagnostics.Debug.Fail(msg);
            System.Diagnostics.Trace.Fail(msg);
        }

        public void Info(string message)
        {
            var msg = $"Info: {message}";
            System.Diagnostics.Debug.WriteLine(msg);
            System.Diagnostics.Trace.WriteLine(msg);
        }
    }
}
