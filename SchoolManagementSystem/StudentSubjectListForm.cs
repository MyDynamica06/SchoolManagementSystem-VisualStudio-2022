using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SchoolManagementSystem
{
    public partial class StudentSubjectListForm : Form
    {
        private string currentStudentSubjectId; // This will store the current student subject's ID
        private string connectionString = "Server=DESKTOP-8SAE4SF\\SQLEXPRESS;Database=CsharpStudentDb;Trusted_Connection=True;";
        private SqlConnection cnn;
        public StudentSubjectListForm()
        {
            InitializeComponent();
            cnn = new SqlConnection(connectionString);
        }

        private void LoadSubjects()
        {
            try
            {
                string sql = "SELECT id FROM subjects";
                SqlDataAdapter da = new SqlDataAdapter(sql, cnn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                cmbSubjectId.DataSource = dt;
                cmbSubjectId.DisplayMember = "subject_id";
                cmbSubjectId.ValueMember = "id";
                cmbSubjectId.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading subjects: " + ex.Message);
            }
        }

        private void LoadAdmissionNos()
        {
            try
            {
                string sql = "SELECT admission_no FROM students";
                SqlDataAdapter da = new SqlDataAdapter(sql, cnn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                cmbAdmissionNo.DataSource = dt;
                cmbAdmissionNo.DisplayMember = "admission_no";
                cmbAdmissionNo.ValueMember = "admission_no";
                cmbAdmissionNo.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading admission numbers: " + ex.Message);
            }
        }

        private void RefreshData()
        {
            string sql = "SELECT * FROM student_subject";
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
                    else
                    {
                        dataGridView1.DataSource = null;
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
            cmbSubjectId.SelectedIndex = -1;
            cmbAdmissionNo.SelectedIndex = -1;
            currentStudentSubjectId = null;
        }

        private void StudentSubjectListForm_Load(object sender, EventArgs e)
        {
            LoadSubjects();
            LoadAdmissionNos();
            RefreshData();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Validate that all required fields are filled
            if (cmbSubjectId.SelectedIndex == -1 || cmbAdmissionNo.SelectedIndex == -1)
            {
                MessageBox.Show("Please fill all required fields before saving.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string sql = "INSERT INTO student_subject(subject_id, admission_no) VALUES('" + cmbSubjectId.SelectedValue + "','" + cmbAdmissionNo.SelectedValue + "')";
            ExecuteQuery(sql, "Record Added Successfully!");
            ClearForm();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            // Validate that all required fields are filled
            if (cmbSubjectId.SelectedIndex == -1 || cmbAdmissionNo.SelectedIndex == -1)
            {
                MessageBox.Show("Please Select Your Update Rows", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                string updateQuery = "UPDATE student_subject SET " +
                                     "subject_id = '" + cmbSubjectId.SelectedValue + "', " +
                                     "admission_no = '" + cmbAdmissionNo.SelectedValue + "' " +
                                     "WHERE id = '" + currentStudentSubjectId + "'";

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
                    string deleteQuery = "DELETE FROM student_subject WHERE id='" + userId + "'";
                    ExecuteQuery(deleteQuery, "Record Deleted Successfully!");
                    ClearForm();
                    RefreshData();
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

        private void dataGridView1_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                currentStudentSubjectId = row.Cells["id"].Value.ToString();
                cmbSubjectId.SelectedValue = row.Cells["subject_id"].Value;
                cmbAdmissionNo.SelectedValue = row.Cells["admission_no"].Value;
            }
        }
    }
}
