﻿using System;
using System.Data.Linq;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.ComponentModel;
using Echo.Helpers;

namespace Echo.Model
{
    public class UserDataContext : DataContext
    {
        public UserDataContext(string connectionString)
            : base(connectionString)
        {
            //LoadListsFromDatabase();
        }
        public UserDataContext()
            : base(DefaultPath)
        {
            if (!this.DatabaseExists())
                this.CreateDatabase();
            //LoadListsFromDatabase();
        }
        public const string DefaultPath = "Data Source=isostore:/Users.sdf";

        public Table<UserModel> UserTable;
        public Table<GroupMapModel> JunctionTable;
        public Table<GroupModel> GroupTable;
        public Table<CallLogModel> CallLogTable;
        public Table<CallLogEntry> EntryTable;



        
    }
}
