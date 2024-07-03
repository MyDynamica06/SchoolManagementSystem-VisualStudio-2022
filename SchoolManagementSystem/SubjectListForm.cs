using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SchoolManagementSystem
{
    public partial class SubjectListForm : Form
    {
        private string currentSubjectId; // This will store the current subject's ID
        private string connectionString = "Server=DESKTOP-8SAE4SF\\SQLEXPRESS;Database=CsharpStudentDb;Trusted_Connection=True;";
        private SqlConnection cnn;
        public SubjectListForm()
        {
            InitializeComponent();
            cnn = new SqlConnection(connectionString);
        }

        private void RefreshData()
        {
            string sql = "SELECT * FROM subjects";
            try
            {
                cnn.Open();
                using (SqlCommand command = new SqlCommand(sql, cnn))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    DataTable dt = new DataTable();
                    if (reader.HasRows)
                    {
                        dt.Load(reader);
                        dataGridView1.DataSource = dt;
                    }
                }
                cnn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot open connection! " + ex.Message);
            }
        }

        private void ExecuteQuery(string query, string successMessage)
        {
            try
            {
                cnn.Open();
                using (SqlCommand cmd = new SqlCommand(query, cnn))
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show(successMessage, "Information");
                }
                cnn.Close();
                RefreshData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void ClearForm()
        {
            txtSubjectName.Clear();
            txtSubjectIndex.Clear();
            txtSubjectNumber.Clear();
            txtSubjectOrder.Clear();
            currentSubjectId = null;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Validate that all required fields are filled
            if (string.IsNullOrWhiteSpace(txtSubjectName.Text) ||
                string.IsNullOrWhiteSpace(txtSubjectIndex.Text) ||
                string.IsNullOrWhiteSpace(txtSubjectNumber.Text) ||
                string.IsNullOrWhiteSpace(txtSubjectOrder.Text))
            {
                MessageBox.Show("Please fill all required fields before saving.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string sql = "INSERT INTO subjects(subject_name, subject_index, subject_number, subject_order) " +
                         "VALUES('" + txtSubjectName.Text + "','" + txtSubjectIndex.Text + "','" + txtSubjectNumber.Text + "','" + txtSubjectOrder.Text + "')";
            ExecuteQuery(sql, "Subject Added Successfully!");
            ClearForm(); // Clear all form fields after saving
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            // Validate that all required fields are filled
            if (string.IsNullOrWhiteSpace(txtSubjectName.Text) ||
                string.IsNullOrWhiteSpace(txtSubjectIndex.Text) ||
                string.IsNullOrWhiteSpace(txtSubjectNumber.Text) ||
                string.IsNullOrWhiteSpace(txtSubjectOrder.Text))
            {
                MessageBox.Show("Please fill all required fields before saving.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Check if the connection is open, if so close it
                if (cnn.State == ConnectionState.Open)
                {
                    cnn.Close();
                }

                cnn.Open();

                // Update query
                string updateQuery = "UPDATE subjects SET " +
                                     "subject_name = '" + txtSubjectName.Text + "', " +
                                     "subject_index = '" + txtSubjectIndex.Text + "', " +
                                     "subject_number = '" + txtSubjectNumber.Text + "', " +
                                     "subject_order = '" + txtSubjectOrder.Text + "', " +
                                     "updated_at = GETDATE() " +
                                     "WHERE id = '" + currentSubjectId + "'";

                SqlCommand updateCmd = new SqlCommand(updateQuery, cnn);
                updateCmd.ExecuteNonQuery();

                MessageBox.Show("Subject updated successfully!", "Information");

                cnn.Close();

                // Clear all text boxes and controls
                ClearForm();
                RefreshData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating data: " + ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DialogResult dr = MessageBox.Show("Do you want to delete?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (dr == DialogResult.Yes)
                {
                    string subjectId = dataGridView1.SelectedRows[0].Cells["id"].Value.ToString();
                    string deleteQuery = "DELETE FROM subjects WHERE id='" + subjectId + "'";
                    ExecuteQuery(deleteQuery, "Subject Deleted Successfully!");
                }
            }
            else
            {
                MessageBox.Show("Please select a row to delete.", "Information");
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Do you want to Close?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (dr == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void SubjectListForm_Load(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void dataGridView1_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                currentSubjectId = row.Cells["id"].Value.ToString();
                txtSubjectName.Text = row.Cells["subject_name"].Value.ToString();
                txtSubjectIndex.Text = row.Cells["subject_index"].Value.ToString();
                txtSubjectNumber.Text = row.Cells["subject_number"].Value.ToString();
                txtSubjectOrder.Text = row.Cells["subject_order"].Value.ToString();

            }
        }
    }
}
