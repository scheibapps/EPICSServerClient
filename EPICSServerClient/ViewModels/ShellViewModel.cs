using System.Windows;
using Microsoft.Practices.Prism.Commands;

namespace EPICSServerClient.ViewModels
{
    public class ShellViewModel
    {
        public DelegateCommand<Window> ExitCommand { get; set; }
        public ShellViewModel()
        {
            ExitCommand = new DelegateCommand<Window>(ExitApplication);
        }

        private void ExitApplication(Window window)
        {
            MessageBox.Show("hello this is exit");
            //window.Close();

        }
    }
}
