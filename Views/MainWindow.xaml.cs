using System.Windows;
using System.Windows.Interactivity;

namespace AutoFlashIMRT
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Interaction.GetBehaviors(this);
            InitializeComponent();
        }
    }
}
