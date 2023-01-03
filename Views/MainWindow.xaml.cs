using System.Windows;

namespace AutoFlash
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            var trig = new Microsoft.Xaml.Behaviors.EventTrigger();  //to initialize behaviors
            InitializeComponent();
        }
    }
}
