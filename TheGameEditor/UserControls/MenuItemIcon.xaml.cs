using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TheGameEditor.UserControls
{
    /// <summary>
    /// Interaction logic for MenuItemIcon.xaml
    /// </summary>
    public partial class MenuItemIcon : UserControl
    {
        public MenuItemIcon()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the Value which is being displayed
        /// </summary>
        public Canvas Icon
        {
            get { return (Canvas)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        /// <summary>
        /// Identified the Label dependency property
        /// </summary>
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(Canvas),
              typeof(MenuItemIcon), new PropertyMetadata(default(Canvas)));
    }
}
