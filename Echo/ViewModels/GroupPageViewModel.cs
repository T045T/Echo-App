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
using Caliburn.Micro;
using Echo.Model;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace Echo.ViewModels
{
    public class GroupPageViewModel : Screen
    {
        public string GroupName { get; set; }
        private INavigationService navService;
        private UDCListModel udc;
        private SettingsModel setModel;
        private GroupModel Group;

        public IEnumerable<UserModel> Items
        {
            get
            {
                if (Group == null)
                {
                    return new ObservableCollection<UserModel>();
                }
                if (setModel.NameOrder)
                {
                    return Group.Users.OrderBy((x) => x.FirstName);
                }
                else
                {
                    return Group.Users.OrderBy((x) => x.LastName);
                }
            }
        }
        
        public Visibility NameOrder
        {
            get { return setModel.NameOrder ? Visibility.Visible : Visibility.Collapsed; }
        }
        public Visibility NotNameOrder
        {
            get { return !setModel.NameOrder ? Visibility.Visible : Visibility.Collapsed; }
        }

        public GroupPageViewModel(INavigationService navService, UDCListModel udc, SettingsModel setModel)
        {
            this.navService = navService;
            this.udc = udc;
            this.setModel = setModel;
        }

        protected override void OnViewReady(object view)
        {
            base.OnViewReady(view);

            if (GroupName != null)
            {
                var tmpGroup = from g in udc.GroupList where g.GroupName.Equals(GroupName) select g;
                if (tmpGroup.Any())
                {
                    Group = tmpGroup.First();
                    // User information (in Property Items) is now accessible
                    NotifyOfPropertyChange("Items");
                }
            }
        }

        public void ContactTapped(UserModel um)
        {
            //UserModel tmp = udc.changeUserID(um, "foobert");

            navService.UriFor<ContactDetailsPageViewModel>().
                //WithParam(x => x.CreateUser, false).
                    WithParam(x => x.TargetUserID, um.ID).
                    Navigate();

        }
    }
}
