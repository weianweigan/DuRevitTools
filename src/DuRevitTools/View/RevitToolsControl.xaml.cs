namespace DuRevitTools
{
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;

    /// <summary>
    /// Interaction logic for RevitToolsControl.
    /// </summary>
    public partial class RevitToolsControl : UserControl
    {
        private RevitToolsViewModel _viewModel = new RevitToolsViewModel();

        public RevitToolsViewModel ViewModel => _viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="RevitToolsControl"/> class.
        /// </summary>
        public RevitToolsControl()
        {
            this.InitializeComponent();

            DataContext = ViewModel;
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                string.Format(System.Globalization.CultureInfo.CurrentUICulture, "Invoked '{0}'", this.ToString()),
                "RevitTools");
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Hyperlink)
            {
                var link = sender as Hyperlink;
                if (link != null)
                {
                    ViewModel.StartUrl(link.NavigateUri.ToString());
                }
            }
        }
    }
}