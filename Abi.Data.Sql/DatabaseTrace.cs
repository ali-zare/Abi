using System.Text;

namespace Abi.Data.Sql
{
    public class DatabaseTrace
    {
        static DatabaseTrace()
        {
            tracer = new DatabaseTrace();
        }
        private DatabaseTrace()
        {
            builder = new StringBuilder();
        }

        private static DatabaseTrace tracer;

        private int counter;
        private bool isStarted;
        private StringBuilder builder;



        public static void Start()
        {
            if (IsStarted)
                throw new EntityDatabaseException("Database Trace Already Had Started");

            IsStarted = true;
        }
        public static void Stop()
        {
            Builder.Clear();
            IsStarted = false;
            Counter = 0;
        }
        public static void Clean()
        {
            Builder.Clear();
        }

        public static void Append(string text)
        {
            if (!IsStarted)
                return;

            Builder.AppendLine($"-- ({++Counter,4}) {text}");

            Builder.AppendLine();
        }
        public static void Append(string text, string title)
        {
            if (!IsStarted)
                return;

            Builder.AppendLine($"-- ({++Counter,4}) {title}");

            Builder.Append(text)
                   .AppendLine();

            Builder.AppendLine();
        }



        public static string Text => Builder.ToString();
        public static bool IsStarted { get => tracer.isStarted; private set => tracer.isStarted = value; }

        private static int Counter { get => tracer.counter; set => tracer.counter = value; }
        private static StringBuilder Builder => tracer.builder;
    }
}
