using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SchoolManagementSystem
{
    public partial class GradeListForm : Form
    {
        private string currentGradeId; // This will store the current grade's ID
        private string connectionString = "Server=DESKTOP-8SAE4SF\\SQLEXPRESS;Database=CsharpStudentDb;Trusted_Connection=True;";
        private SqlConnection cnn;

        public GradeListForm()
        {
            InitializeComponent();
            cnn = new SqlConnection(connectionString);
        }

        private void RefreshData()
        {
            string sql = "SELECT * FROM grades";
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
            txtGradeName.Clear();
            txtGradeGroup.Clear();
            txtGradeOrder.Clear();
            currentGradeId = null;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Validate that all required fields are filled
            if (string.IsNullOrWhiteSpace(txtGradeName.Text) ||
                string.IsNullOrWhiteSpace(txtGradeGroup.Text) ||
                string.IsNullOrWhiteSpace(txtGradeOrder.Text))
            {
                MessageBox.Show("Please fill all required fields before saving.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string sql = "INSERT INTO grades(grade_name, grade_group, grade_order) VALUES('" +
                         txtGradeName.Text + "','" + txtGradeGroup.Text + "','" + txtGradeOrder.Text + "')";
            ExecuteQuery(sql, "Grade Added Successfully!");
            ClearForm(); // Clear all form fields after saving
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            // Validate that all required fields are filled
            if (string.IsNullOrWhiteSpace(txtGradeName.Text) ||
                string.IsNullOrWhiteSpace(txtGradeGroup.Text) ||
                string.IsNullOrWhiteSpace(txtGradeOrder.Text))
            {
                MessageBox.Show("Please Select Your Update Rows", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                cnn.Open();

                // Update query
                string updateQuery = "UPDATE grades SET " +
                                     "grade_name = '" + txtGradeName.Text + "', " +
                                     "grade_group = '" + txtGradeGroup.Text + "', " +
                                     "grade_order = '" + txtGradeOrder.Text + "' " +
                                     "updated_at = GETDATE() " +
                                     "WHERE id = '" + currentGradeId + "'";

                SqlCommand updateCmd = new SqlCommand(updateQuery, cnn);
                updateCmd.ExecuteNonQuery();

                MessageBox.Show("Data updated successfully!", "Information");

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
                    string userId = dataGridView1.SelectedRows[0].Cells["id"].Value.ToString();
                    string deleteQuery = "DELETE FROM grades WHERE id='" + userId + "'";
                    ExecuteQuery(deleteQuery, "Grade Deleted Successfully!");
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

        private void GradeListForm_Load(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void dataGridView1_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                currentGradeId = row.Cells["id"].Value.ToString();
                txtGradeName.Text = row.Cells["grade_name"].Value.ToString();
                txtGradeGroup.Text = row.Cells["grade_group"].Value.ToString();
                txtGradeOrder.Text = row.Cells["grade_order"].Value.ToString();
            }
        }
    }
}
