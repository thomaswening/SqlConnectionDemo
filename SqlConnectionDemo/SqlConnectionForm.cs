using System.Data.SqlClient;

namespace SqlConnectionDemo
{
    public partial class SqlConnectionDemoForm : Form
    {
        private readonly string defaultConnectionString = @"Server=localhost\MSSQLSERVER01;Database=test;Trusted_Connection=True;";
        private string connectionString = string.Empty;
        private string firstName = string.Empty;
        private string lastName = string.Empty;

        public SqlConnectionDemoForm()
        {
            InitializeComponent();
            BindData();
        }

        private void BindData()
        {
            txtUserInputConnectionString.Text = defaultConnectionString;
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

        private void btnInsert_Click(object sender, EventArgs e)
        {
            UnbindData();

            if (ValidateData())
            {
                try
                {
                    using SqlConnection connection = new(connectionString);

                    string query = $"INSERT INTO persons (first_name, last_name) VALUES ('{firstName}', '{lastName}')";
                    SqlCommand command = new(query, connection);

                    connection.Open();
                    SqlDataAdapter adapter = new()
                    {
                        InsertCommand = command
                    };
                    adapter.InsertCommand.ExecuteNonQuery();

                    command.Dispose();
                    connection.Close();

                    MessageBox.Show("The record was successfully inserted.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Something went wrong.\n Error message: {ex.Message}");
                }
            }
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
                    UpdateForm updateForm = new(id, firstName, lastName);
                    updateForm.ShowDialog();

                    if (!updateForm.IsCanceled)
                    {
                        try
                        {
                            using SqlConnection connection = new(connectionString);

                            string query = $"UPDATE persons SET first_name='{updateForm.FirstName}', last_name='{updateForm.LastName}' WHERE id='{id}'";
                            SqlCommand command = new(query, connection);

                            connection.Open();
                            SqlDataAdapter adapter = new()
                            {
                                UpdateCommand = command
                            };
                            adapter.UpdateCommand.ExecuteNonQuery();

                            command.Dispose();
                            connection.Close();

                            MessageBox.Show("The record was successfully updated.");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Something went wrong.\n Error message: {ex.Message}");
                        }
                    }
                }
            }
        }

        private string? GetId(string first_name = "", string last_name = "")
        {
            string? id = null;

            try
            {
                using SqlConnection connection = new(connectionString);

                string query = $"SELECT id FROM persons WHERE first_name='{first_name}' AND last_name='{last_name}'";
                SqlCommand command = new(query, connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        id = reader.GetValue(0).ToString();
                    }
                }

                connection.Close(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Something went wrong.\n Error message: {ex.Message}");
            }

            return id;
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
                        using SqlConnection connection = new(connectionString);

                        string query = $"DELETE FROM persons WHERE id='{id}'";
                        SqlCommand command = new(query, connection);

                        connection.Open();
                        SqlDataAdapter adapter = new()
                        {
                            UpdateCommand = command
                        };
                        adapter.UpdateCommand.ExecuteNonQuery();

                        command.Dispose();
                        connection.Close();

                        MessageBox.Show("The record was successfully deleted.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Something went wrong.\n Error message: {ex.Message}");
                    }
                }
            }
        }
    }
}