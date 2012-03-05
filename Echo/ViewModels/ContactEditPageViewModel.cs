using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Caliburn.Micro;
using Echo.Model;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Microsoft.Phone.Tasks;
using Echo.Helpers;
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;
using System.Windows.Media;


namespace Echo.ViewModels
{
    public class ContactEditPageViewModel : Screen
    {
        private Echo.Views.ContactEditPage MyView;
        private readonly INavigationService navService;
        private UDCListModel dc;
        private UserModel um;
        private PhotoChooserTask pc;
        private Uri newPic;
        private Uri oldPic;

        private bool changedPicture;
        private bool changingPicture;

        private string _UserID;
        public bool SavingAllowed
        {
            get
            {
                return UserID.Length > 0;
            }
        }
        public bool CreateUser { get; set; }
        public string FirstName
        {
            get
            {
                return um.FirstName;
            }
            set
            {
                if (value != um.FirstName)
                {
                    um.FirstName = value;
                    NotifyOfPropertyChange("FirstName");
                }
            }
        }
        public string LastName
        {
            get
            {
                return um.LastName;
            }
            set
            {
                if (value != um.LastName)
                {
                    um.LastName = value;
                    NotifyOfPropertyChange("LastName");
                }
            }
        }
        public string UserID
        {
            get
            {
                return _UserID;
            }
            set
            {
                if (value != _UserID)
                {
                    _UserID = value;
                    if (_UserID.Length == 0)
                    {
                        (MyView.ApplicationBar.Buttons[0] as Caliburn.Micro.AppBarButton).IsEnabled = false;
                    }
                    else if ((MyView.ApplicationBar.Buttons[0] as Caliburn.Micro.AppBarButton).IsEnabled == false)
                    {
                        (MyView.ApplicationBar.Buttons[0] as Caliburn.Micro.AppBarButton).IsEnabled = true;
                    }
                    NotifyOfPropertyChange("UserID");
                }
            }
        }
        private string _UserImagePath;
        public string UserImagePath
        {
            get
            {
                if (_UserImagePath != null)
                {
                    return _UserImagePath;
                }
                else
                {
                    return "";
                }
            }
            set
            {
                if (value != _UserImagePath)
                {
                    _UserImagePath = value;
                    NotifyOfPropertyChange("UserImagePath");
                    NotifyOfPropertyChange("UserImageSource");
                }
            }
        }

        public ImageSource UserImageSource
        {
            get
            {
                if (UserImagePath.Length > 0)
                {
                    BitmapImage img = new BitmapImage();
                    using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        using (IsolatedStorageFileStream stream = isoStore.OpenFile(UserImagePath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                        {
                            img.SetSource(stream);
                        }
                    }
                    return img;
                }
                else
                {
                    return new BitmapImage();
                }
            }
        }

        public Boolean hasImage { get { return UserImagePath.Length > 1; } }

        public int TargetUserID { get; set; }

        private ObservableCollection<GroupMembership> _GroupList;
        public ObservableCollection<GroupMembership> GroupList
        {
            get
            {
                return _GroupList;
            }
            set
            {
                if (value != _GroupList)
                {
                    _GroupList = value;
                    NotifyOfPropertyChange("GroupList");
                }
            }
        }

        readonly IWindowManager windowMan;

        public ContactEditPageViewModel(INavigationService NavigationService, IWindowManager windowMan, UDCListModel dc)
        {
            this.dc = dc;
            this.navService = NavigationService;
            this.windowMan = windowMan;
            this.changedPicture = false;
            this.changingPicture = false;

            pc = new PhotoChooserTask();
            pc.ShowCamera = true;
            pc.PixelHeight = 150;
            pc.PixelWidth = 150;
            pc.Completed += new EventHandler<PhotoResult>(pc_Completed);
        }

        void pc_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult.Equals(TaskResult.OK) && um != null)
            {
                // Create a filename for JPEG file in isolated storage.
                String tempJPEG = "UserIcons/" + System.Guid.NewGuid().ToString() + ".jpg";

                // Create virtual store and file stream. Check for duplicate tempJPEG files.
                using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (!isoStore.DirectoryExists("UserIcons"))
                    {
                        isoStore.CreateDirectory("UserIcons");
                    }
                    while (isoStore.FileExists(tempJPEG)) // If a file exists, we cannot overwrite it, so we change the filename
                    {
                        tempJPEG = "UserIcons/" + System.Guid.NewGuid().ToString() + ".jpg";
                    }

                    IsolatedStorageFileStream fileStream = isoStore.CreateFile(tempJPEG);

                    if (um.HasImage)
                        oldPic = new Uri(um.UserImagePath, UriKind.Relative);
                    newPic = new Uri(tempJPEG, UriKind.Relative);

                    WriteableBitmap wb = new WriteableBitmap(150, 150);
                    wb.SetSource(e.ChosenPhoto);


                    // Encode WriteableBitmap object to a JPEG stream.
                    Extensions.SaveJpeg(wb, fileStream, wb.PixelWidth, wb.PixelHeight, 0, 85);

                    //wb.SaveJpeg(fileStream, wb.PixelWidth, wb.PixelHeight, 0, 85);
                    fileStream.Close();
                }
                UserImagePath = newPic.ToString();
                changedPicture = true;
            }
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            if (!changedPicture)
            {
                if (!CreateUser)
                {
                    um = dc.GetUser(TargetUserID);
                    if (um == null)
                    {
                        MessageBox.Show("Error loading user!!!!");
                        navService.GoBack();
                        return;
                    }
                    _UserID = um.UserID;
                }
                else
                {
                    um = new UserModel();
                    TargetUserID = um.ID;
                }
                NotifyOfPropertyChange("FirstName");
                NotifyOfPropertyChange("LastName");
                NotifyOfPropertyChange("UserID");
            
                UserImagePath = um.UserImagePath;
            }
            LoadGroupsFromDatabase();
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            MyView = view as Echo.Views.ContactEditPage;
            if (CreateUser)
            {
                (MyView.ApplicationBar.Buttons[0] as Caliburn.Micro.AppBarButton).IsEnabled = false;
            }
        }

        public void PictureTapped()
        {
            changingPicture = true;
            pc.Show();
        }

        public void Cancel()
        {
            if (changedPicture)
            {
                if (oldPic != null)
                {
                    um.UserImagePath = oldPic.ToString();
                }
                if (newPic != null)
                {
                    using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        if (myIsolatedStorage.FileExists(newPic.ToString()))
                        {
                            myIsolatedStorage.DeleteFile(newPic.ToString());
                        }
                    }
                }
                changedPicture = false;
            }
            navService.GoBack();
        }

        public override void CanClose(Action<bool> callback)
        {
            if (!changingPicture && changedPicture)
            {
                if (oldPic != null)
                {
                    um.UserImagePath = oldPic.ToString();
                }
                if (newPic != null)
                {
                    using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        if (myIsolatedStorage.FileExists(newPic.ToString()))
                        {
                            myIsolatedStorage.DeleteFile(newPic.ToString());
                        }
                    }
                }
                changedPicture = false;
            }
            changingPicture = false;
            callback(true);
        }

        public void Save()
        {
            // Update the value of any active element (i.e. a TextBox that is still "being edited")
            object focusObj = FocusManager.GetFocusedElement();
            if (focusObj != null && focusObj is TextBox)
            {
                var binding = (focusObj as TextBox).GetBindingExpression(TextBox.TextProperty);
                binding.UpdateSource();
            }
            if (CreateUser)
            {
                um.UserID = UserID;
                if (!dc.addUser(um))
                {
                    MessageBox.Show("The user could not be saved. This is probably because a user with the ID " + '"' + UserID + '"' + " already exists.");
                    return;
                }
            }

            if (!UserID.Equals(um.UserID))
            {
                if (!dc.changeUserID(um, UserID))
                {
                    MessageBox.Show("Error saving user. Perhaps the User ID is already taken?");
                    return;
                }
            }

            foreach (GroupMembership gm in GroupList)
            {
                if (gm.Dirty)
                {
                    var group = from GroupModel gr in dc.GroupList where gr.GroupName.Equals(gm.GroupName) select gr;
                    GroupModel g;
                    if (group.Any())
                    {
                        g = group.First();

                        if (gm.UserIsMember)
                        {
                            g.addUser(um);
                            //dc.addToGroup(um, g);
                        }
                        else
                        {
                            g.remUser(um);
                            //dc.removeFromGroup(um, g);
                        }
                        dc.SubmitChanges();
                    }
                }
            }

            if (changedPicture)
            {
                if (newPic != null)
                {
                    um.UserImagePath = newPic.ToString();
                }
                if (oldPic != null)
                {
                    using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        if (myIsolatedStorage.FileExists(oldPic.ToString()))
                        {
                            myIsolatedStorage.DeleteFile(oldPic.ToString());
                        }
                    }
                }
                changedPicture = false;
            }

            /*
            navService.RemoveBackEntry();
            navService.UriFor<MainPageViewModel>()
                .WithParam(x => x.Reload, changedAnything || CreateUser)
                .WithParam(x => x.ClearBackStack, true)
                .Navigate();
             */
            dc.LoadListsFromDatabase();
            navService.GoBack();
        }

        public void DeleteGroup(GroupMembership gm)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete " + '"' + gm.GroupName + '"' + "?", "Delete group", MessageBoxButton.OKCancel);
            if (result.Equals(MessageBoxResult.OK))
            {
                string name = gm.GroupName;
                GroupList.Remove(gm);
                dc.removeGroup(name);
            }
        }

        public IEnumerable<IResult> NewGroupTapped()
        {
            var groupDialog = new ShowDialog<GroupDialogViewModel>();
            yield return groupDialog;

            string result = groupDialog.Dialog.Result;
            if (result != null)
            {
                if (!dc.addGroup(result))
                {
                    MessageBox.Show("Group could not be added. Maybe a group with this name already exists.");
                    yield break;
                }
                GroupList.Add(new GroupMembership(result, true, true));
            }
        }

        private void LoadGroupsFromDatabase()
        {
            GroupList = new ObservableCollection<GroupMembership>();
            foreach (GroupModel m in dc.GroupList)
            {
                _GroupList.Add(new GroupMembership(m.GroupName, CreateUser ? false : m.UserIsMember(TargetUserID), false)); // if we're creating a user, he's not a member of any group
            }
            NotifyOfPropertyChange("GroupList");
        }
    }

    public class GroupMembership : ViewModelBase
    {
        private string _GroupName;
        public string GroupName
        {
            get
            {
                return _GroupName;
            }
            set
            {
                if (value != _GroupName)
                {
                    _GroupName = value;
                    RaisePropertyChangedEvent("GroupName");
                }
            }
        }
        private bool _UserIsMember;
        public bool UserIsMember
        {
            get
            {
                return _UserIsMember;
            }
            set
            {
                if (value != _UserIsMember)
                {
                    _UserIsMember = value;
                    Dirty ^= true;
                    RaisePropertyChangedEvent("UserIsMember");
                }
            }
        }

        private bool _Dirty;
        public bool Dirty
        {
            get
            {
                return _Dirty;
            }
            private set
            {
                _Dirty = value;
            }
        }

        public GroupMembership(string GroupName, bool UserIsMember, bool Dirty)
        {
            this._Dirty = Dirty;
            this.GroupName = GroupName;
            this._UserIsMember = UserIsMember;
        }
    }

    /// <summary>
    /// Tombstoning handler - saves the First name, last name and UserID since the app can be tombstoned during image selection
    /// </summary>
    public class ContactEditPageViewModelStorage : StorageHandler<ContactEditPageViewModel>
    {
        public override void Configure()
        {
            Property(x => x.UserID)
                .InPhoneState()
                .RestoreAfterActivation();
            Property(x => x.FirstName)
                .InPhoneState()
                .RestoreAfterActivation();
            Property(x => x.LastName)
                .InPhoneState()
                .RestoreAfterActivation();
        }
    }
}
