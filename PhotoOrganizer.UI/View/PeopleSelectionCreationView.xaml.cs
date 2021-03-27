using MahApps.Metro.Controls;
using System.Windows.Input;

namespace PhotoOrganizer.UI.View
{
    /// <summary>
    /// Interaction logic for PeopleSelectionCreationView.xaml
    /// </summary>
    public partial class PeopleSelectionCreationView : MetroWindow
    {
        public PeopleSelectionCreationView()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            FocusManager.SetFocusedElement(this, NameTextBox);
            Keyboard.Focus(NameTextBox);
        }
    }
}
