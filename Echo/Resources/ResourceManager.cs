﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Echo.Resources
{
    public class ResourceManager
    {
        private static Text myLocalizedText = new Text();
        public Text MyLocalizedText
        {
            get { return myLocalizedText; }
        }

        public ResourceManager() { }

    }
}