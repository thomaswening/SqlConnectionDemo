
// useful links
// https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/retrieving-and-modifying-data
// https://www.c-sharpcorner.com/UploadFile/201fc1/sql-server-database-connection-in-csharp-using-adonet/
// https://www.guru99.com/c-sharp-access-database.html

using System.Data.SqlClient;
using System.Text;

namespace SqlConnectionDemo
{
    public partial class SqlConnectionDemoForm : Form
    {
        #region Fields

        // Declare the connection string for the SQL server instance - in this case it can even be modified in the form
        private readonly string defaultConnectionString = @"Server=localhost\MSSQLSERVER01;Database=test;Trusted_Connection=True;";

        private string connectionString = string.Empty;
        private string firstName = string.Empty;
        private string lastName = string.Empty;

        #endregion Fields

        #region Constructors

        public SqlConnectionDemoForm()
        {
            InitializeComponent();
            BindData();
        }

        #endregion Constructors

        #region Methods

        private void BindData()
        {
            txtUserInputConnectionString.Text = defaultConnectionString;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            UnbindData();

            if (ValidateData())
            {
                string? id = GetId(firstName, lastName);

                if (id is null)
                {
                    MessageBox.Show("No matching records found.");
                }
                else
                {
                    try
                    {
                        ExecuteQuery(Modes.DELETE, id);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Something went wrong.\n Error message: {ex.Message}");
                    }
                }
            }
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            UnbindData();

            if (ValidateData())
            {
                try
                {
                    ExecuteQuery(Modes.INSERT, id: "", firstName, lastName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Something went wrong.\n Error message: {ex.Message}");
                }
            }
        }

        private void btnOpenConnection_Click(object sender, EventArgs e)
        {
            string result;

            UnbindData();

            try
            {
                using SqlConnection connection = new(connectionString);
                connection.Open();
                result = "Connection open!";
                connection.Close();
            }
            catch (Exception ex)
            {
                result = $"Connection failed. Please revise connection string.\nError message: {ex.Message}";
            }

            MessageBox.Show(result);
        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            UnbindData();

            if (ValidateData())
            {
                string? id = GetId(firstName, lastName);

                if (id is null)
                {
                    MessageBox.Show("No matching records found.");
                }
                else
                {
                    UpdateForm updateForm = new(firstName, lastName);
                    updateForm.ShowDialog();

                    if (!updateForm.IsCanceled)
                    {
                        try
                        {
                            ExecuteQuery(Modes.UPDATE, id, updateForm.FirstName, updateForm.LastName);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Something went wrong.\n Error message: {ex.Message}");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Looks for records in table persons with matching values for first_name, last_name.
        /// </summary>
        /// <param name="first_name"></param>
        /// <param name="last_name"></param>
        /// <returns>The id of the first such record that is found</returns>
        private string? GetId(string first_name = "", string last_name = "")
        {
            string? id = null;

            try
            {
                // First, we open a SqlConnection with the connection string
                using SqlConnection connection = new(connectionString);

                // Then we define a SELECT query as a string...
                string query = "SELECT id FROM persons WHERE first_name='@first_name' AND last_name='@last_name'";

                // ... and create a new SqlCommand with it
                SqlCommand command = new(query, connection);
                command.Parameters.Add(new SqlParameter("first_name", firstName));
                command.Parameters.Add(new SqlParameter("last_name", lastName));

                // We open the SqlConnection and create a SqlDataReader which reads the query result from the SQL instance
                connection.Open();
                using SqlDataReader reader = command.ExecuteReader();

                // If the query returned any records...
                if (reader.HasRows)
                {
                    // ... we read the first one of the records and get the value of type object of the 0th column and convert it into a string.
                    reader.Read();
                    id = reader.GetValue(0).ToString();
                }

                // Because we used the 'using' keyword for instantiating the SqlConnection and the SqlDataReader,
                // the following two lines of code are superfluous.

                //reader.Close();
                //connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Something went wrong.\n Error message: {ex.Message}");
            }

            return id;
        }

        private void UnbindData()
        {
            if (!string.IsNullOrEmpty(txtUserInputConnectionString.Text))
            {
                connectionString = txtUserInputConnectionString.Text;
            }
            else
            {
                connectionString = defaultConnectionString;
            }

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

        private void ExecuteQuery(Modes mode, string id, params string[] values)
        {
            try
            {
                // Again, first the connection
                using SqlConnection connection = new(connectionString);

                // Depending on the non-query command (INS, UPD, DEL) we define a (non-)query string.
                // To reduce the risk of a SQL injection attack, we define SqlParameter variables preceded by a '@'.
                string query = mode switch
                {
                    Modes.INSERT => "INSERT INTO persons (first_name, last_name) VALUES ('@first_name', '@last_name')",
                    Modes.UPDATE => "UPDATE persons SET first_name='@first_name', last_name='@last_name' WHERE id=@id",
                    Modes.DELETE => "DELETE FROM persons WHERE id=@id",
                    _ => throw new NotImplementedException(),
                };

                // ... and use it to construct a command.
                SqlCommand command = new(query, connection);

                // This time, after opening the connection we instantiate a SqlDataAdapter...
                connection.Open();
                SqlDataAdapter adapter = new();

                // ..., replace the placeholders in the non-query strings with a SqlParameter,
                // save the command in the appropriate {Insert/Update/Delete}Command property and execute that.
                switch (mode)
                {
                    case Modes.INSERT:
                        command.Parameters.Add(new SqlParameter("first_name", values[0])); // e.g. replaces '@first_name' with the value of values[0]
                        command.Parameters.Add(new SqlParameter("last_name", values[1]));

                        adapter.InsertCommand = command;
                        adapter.InsertCommand.ExecuteNonQuery();
                        break;

                    case Modes.UPDATE:
                        command.Parameters.Add(new SqlParameter("first_name", values[0]));
                        command.Parameters.Add(new SqlParameter("last_name", values[1]));
                        command.Parameters.Add(new SqlParameter("id", id));

                        adapter.UpdateCommand = command;
                        adapter.UpdateCommand.ExecuteNonQuery();
                        break;

                    case Modes.DELETE:
                        command.Parameters.Add(new SqlParameter("id", id));

                        adapter.DeleteCommand = command;
                        adapter.DeleteCommand.ExecuteNonQuery();
                        break;
                }

                StringBuilder message = new("The record was successfully ");

                switch (mode)
                {
                    case Modes.INSERT:
                        message.Append("inserted.");
                        break;

                    case Modes.UPDATE:
                        message.Append("updated.");
                        break;

                    case Modes.DELETE:
                        message.Append("deleted.");
                        break;
                }

                MessageBox.Show(message.ToString());
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion Methods
    }
}