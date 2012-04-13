using System.Data.Linq;

namespace Echo.Model
{
    public class UserDataContext : DataContext
    {
        public Table<UserModel> UserTable;
        public Table<GroupMapModel> JunctionTable;
        public Table<GroupModel> GroupTable;
        public Table<CallLogModel> CallLogTable;
        public Table<CallLogEntry> EntryTable;

        public UserDataContext(string connectionString, bool overWrite)
            : base(connectionString)
        {
            if (!this.DatabaseExists())
            {
                this.CreateDatabase();
            }
            else if (overWrite)
            {
                this.DeleteDatabase();
                this.CreateDatabase();
            }
            this.SubmitChanges();
            //LoadListsFromDatabase();
        }
        public UserDataContext()
            : base(DefaultPath)
        {
            if (!this.DatabaseExists())
                this.CreateDatabase();
            //LoadListsFromDatabase();
            this.SubmitChanges();
        }
        public const string DefaultPath = "Data Source=isostore:/Users.sdf";
    }
}
