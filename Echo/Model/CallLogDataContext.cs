using System.Data.Linq;

namespace Echo.Model
{
    public class CallLogDataContext : DataContext
    {
        public CallLogDataContext(string connectionString)
            : base(connectionString)
        { }

        public Table<CallLogModel> CallLogTable;
        public Table<CallLogEntry> EntryTable;

    }
}
