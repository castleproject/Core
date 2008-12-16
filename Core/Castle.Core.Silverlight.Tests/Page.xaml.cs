using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Castle.Core.Silverlight.Tests
{
    public partial class Page : UserControl
    {
        public Page()
        {
            InitializeComponent();
            runner.Run(this.GetType().Assembly);
        }
    }
}
