using System.Windows;

namespace AkberSoft
{
    public partial class InputDialog : Window
    {
        public string FullName { get; set; }
        public string Position { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public InputDialog(Worker edit = null)
        {
            InitializeComponent();
            if (edit != null)
            {
                tbName.Text = edit.FullName;
                tbPos.Text = edit.Position;
                tbPhone.Text = edit.Phone;
                tbEmail.Text = edit.Email;
            }
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            FullName = tbName.Text.Trim();
            if (string.IsNullOrEmpty(FullName))
            {
                MessageBox.Show("Введите ФИО!", "Ошибка");
                return;
            }
            Position = tbPos.Text.Trim();
            Phone = tbPhone.Text.Trim();
            Email = tbEmail.Text.Trim();
            DialogResult = true;
            Close();
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}