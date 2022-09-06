namespace SqlConnectionDemo
{
    public partial class UpdateForm : Form
    {
        #region Fields

        private string firstName = string.Empty;
        private bool isCanceled = false;
        private string lastName = string.Empty;

        #endregion Fields

        #region Properties

        public string FirstName => firstName;

        public bool IsCanceled => isCanceled;

        public string LastName => lastName;

        #endregion Properties

        #region Constructors

        public UpdateForm(string firstName, string lastName)
        {
            this.firstName = firstName;
            this.lastName = lastName;

            InitializeComponent();
            BindData();
        }

        #endregion Constructors

        #region Methods

        private void BindData()
        {
            txtFirstName.Text = firstName;
            txtLastName.Text = lastName;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            isCanceled = true;
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (ValidateData())
            {
                UnbindData();
                this.Close();
            }
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

        #endregion Methods
    }
}