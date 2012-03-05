using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Data.Linq;

namespace Echo.Model
{
    public class TestDataContext : DataContext
    {
        public TestDataContext() : base("isostore:/foo.sdl") { }

        public Table<UserModel> userTable;
    }
}
