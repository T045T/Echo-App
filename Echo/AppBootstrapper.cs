﻿namespace Echo
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Windows.Controls;
    using System.Windows.Media;
    using Caliburn.Micro;
    using Echo.Logic;
    using Echo.Model;
    using Echo.ViewModels;
    using Microsoft.Phone.Controls;

    public class AppBootstrapper : PhoneBootstrapper
    {
        PhoneContainer container;
        //static AppBootstrapper()
        //{
        //    LogManager.GetLog = type => new DebugLogger(type);
        //}


        protected override PhoneApplicationFrame CreatePhoneApplicationFrame()
        {
            return new TransitionFrame() { Background = new SolidColorBrush(Colors.Transparent) };
        }

        protected override void Configure()
        {
            container = new PhoneContainer(RootFrame);
            container.RegisterPhoneServices();
            container.PerRequest<MainPageViewModel>();
            container.PerRequest<RecentsViewModel>();
            container.PerRequest<ContactsViewModel>();
            container.PerRequest<TrainerFrontViewModel>();
            container.PerRequest<SettingsPageViewModel>();
            container.PerRequest<ContactEditPageViewModel>();
            container.PerRequest<ContactDetailsPageViewModel>();
            container.PerRequest<GroupPageViewModel>();
            container.PerRequest<ActiveCallPageViewModel>();
            container.PerRequest<CallLogPageViewModel>();
            container.PerRequest<NetworkTestPageViewModel>();
            container.PerRequest<WelcomePageViewModel>();

            container.PerRequest<GroupDialogViewModel, GroupDialogViewModel>();
            container.PerRequest<IncomingCallDialogViewModel, IncomingCallDialogViewModel>();
            container.Singleton<SettingsModel>();
            container.Singleton<UDCListModel>();
            container.Singleton<Connection>();
            container.Singleton<EchoClientLogic>();


            AddCustomConventions();
        }

        protected override object GetInstance(Type service, string key)
        {
            return container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            container.BuildUp(instance);
        }

        static void AddCustomConventions()
        {
            ConventionManager.AddElementConvention<Pivot>(Pivot.ItemsSourceProperty, "SelectedItem", "SelectionChanged").ApplyBinding =
                (viewModelType, path, property, element, convention) =>
                {
                    if (ConventionManager
                        .GetElementConvention(typeof(ItemsControl))
                        .ApplyBinding(viewModelType, path, property, element, convention))
                    {
                        ConventionManager
                            .ConfigureSelectedItem(element, Pivot.SelectedItemProperty, viewModelType, path);
                        ConventionManager
                            .ApplyHeaderTemplate(element, Pivot.HeaderTemplateProperty, null, viewModelType);
                        return true;
                    }
                    return false;
                };
            /*
            ConventionManager.AddElementConvention<Panorama>(Panorama.ItemsSourceProperty, "SelectedItem", "SelectionChanged").ApplyBinding =
                (viewModelType, path, property, element, convention) => {
                    if (ConventionManager
                        .GetElementConvention(typeof(ItemsControl))
                        .ApplyBinding(viewModelType, path, property, element, convention))
                    {
                        ConventionManager
                            .ConfigureSelectedItem(element, Panorama.SelectedItemProperty, viewModelType, path);
                        ConventionManager
                            .ApplyHeaderTemplate(element, Panorama.HeaderTemplateProperty, null, viewModelType);
                        return true;
                    }

                    return false;
                };
             */
            ConventionManager.AddElementConvention<MenuItem>(ItemsControl.ItemsSourceProperty, "DataContext", "Click");
        }
    }

    class DebugLogger : ILog
    {
        #region Fields
        private readonly Type _type;
        #endregion

        #region Constructors
        public DebugLogger(Type type)
        {
            _type = type;
        }
        #endregion

        #region Helper Methods
        private string CreateLogMessage(string format, params object[] args)
        {
            return string.Format("[{0}] {1}",
                                 DateTime.Now.ToString("o"),
                                 string.Format(format, args));
        }
        #endregion

        #region ILog Members
        public void Error(Exception exception)
        {
            Debug.WriteLine(CreateLogMessage(exception.ToString()), "ERROR");
        }
        public void Info(string format, params object[] args)
        {
            Debug.WriteLine(CreateLogMessage(format, args), "INFO");
        }
        public void Warn(string format, params object[] args)
        {
            Debug.WriteLine(CreateLogMessage(format, args), "WARN");
        }
        #endregion
    }
}
