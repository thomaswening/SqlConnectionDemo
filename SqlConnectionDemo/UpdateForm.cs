using System.Data.SqlClient;

namespace SqlConnectionDemo
{
    public partial class UpdateForm : Form
    {
        private string firstName = string.Empty;
        private string lastName = string.Empty;
        private string id = string.Empty;
        private bool isCanceled = false;

        public string LastName => lastName;
        public string FirstName => firstName;
        public bool IsCanceled => isCanceled;

        public UpdateForm(string id, string firstName, string lastName)
        {
            this.id = id;
            this.firstName = firstName;
            this.lastName = lastName;

            InitializeComponent();
            BindData();
        }

        private void BindData()
        {
            txtFirstName.Text = firstName;
            txtLastName.Text = lastName;
        }

        private void UnbindData() 
        {
            firstName = txtFirstName.Text;
            lastName = txtLastName.Text;
        }

        private bool ValidateData()
        {
            if (string.IsNullOrEmpty(txtFirstName.Text) || string.IsNullOrEmpty(txtLastName.Text))
            {
                MessageBox.Show("Please provide a first name and a last name.");
                return false;
            }

            return true;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (ValidateData())
            {
                UnbindData();
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            isCanceled = true;
            this.Close();
        }
    }
}