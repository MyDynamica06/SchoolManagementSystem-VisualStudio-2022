using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SchoolManagementSystem
{
    public partial class StudentListForm : Form
    {
        private string currentStudentId; // This will store the current student's ID
        private string connectionString = "Server=DESKTOP-8SAE4SF\\SQLEXPRESS;Database=CsharpStudentDb;Trusted_Connection=True;";
        private SqlConnection cnn;


        public StudentListForm()
        {
            InitializeComponent();
            cnn = new SqlConnection(connectionString);
        }


        private void RefreshData()
        {
            string sql = "SELECT * FROM students";
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
            txtAdmissionNo.Clear();
            txtFirstName.Clear();
            txtLastName.Clear();
            txtFullName.Clear();
            rbMale.Checked = false;
            rbFemale.Checked = false;
            dateTimePicker1.Value = DateTime.Now;
            txtStuNicNo.Clear();
            txtPhoneNo.Clear();
            cmbGradeId.SelectedIndex = -1;
            cmbMedium.SelectedIndex = -1;
            dateTimePicker2.Value = DateTime.Now;
            txtAddress.Clear();
            currentStudentId = null;
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            // Validate that all required fields are filled
            if (string.IsNullOrWhiteSpace(txtAdmissionNo.Text) ||
                string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(txtLastName.Text) ||
                string.IsNullOrWhiteSpace(txtFullName.Text) ||
                (!rbMale.Checked && !rbFemale.Checked) ||
                string.IsNullOrWhiteSpace(txtStuNicNo.Text) ||
                string.IsNullOrWhiteSpace(txtPhoneNo.Text) ||
                cmbGradeId.SelectedIndex == -1 ||
                cmbMedium.SelectedIndex == -1 ||
                string.IsNullOrWhiteSpace(txtAddress.Text))
            {
                MessageBox.Show("Please fill all required fields before saving.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string gender = rbMale.Checked ? "Male" : "Female";
            string sql = "INSERT INTO students(admission_no, first_name, last_name, full_name, gender, date_of_birth, stu_nic_no, tp_no, grade_id, medium, date_of_admission, resident_address) VALUES('" + txtAdmissionNo.Text + "','" + txtFirstName.Text + "','" + txtLastName.Text + "','" + txtFullName.Text + "','" + gender + "','" + dateTimePicker1.Value.ToString("yyyy-MM-dd") + "','" + txtStuNicNo.Text + "','" + txtPhoneNo.Text + "','" + cmbGradeId.SelectedValue + "','" + cmbMedium.Text + "','" + dateTimePicker2.Value.ToString("yyyy-MM-dd") + "','" + txtAddress.Text + "')";
            ExecuteQuery(sql, "Record Added Successfully!");
            ClearForm(); // Clear all form fields after saving
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            // Validate that all required fields are filled
            if (string.IsNullOrWhiteSpace(txtAdmissionNo.Text) ||
                string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(txtLastName.Text) ||
                string.IsNullOrWhiteSpace(txtFullName.Text) ||
                (!rbMale.Checked && !rbFemale.Checked) ||
                string.IsNullOrWhiteSpace(txtStuNicNo.Text) ||
                string.IsNullOrWhiteSpace(txtPhoneNo.Text) ||
                cmbGradeId.SelectedIndex == -1 ||
                cmbMedium.SelectedIndex == -1 ||
                string.IsNullOrWhiteSpace(txtAddress.Text))
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
                string updateQuery = "UPDATE students SET " +
                                     "admission_no = '" + txtAdmissionNo.Text + "', " +
                                     "first_name = '" + txtFirstName.Text + "', " +
                                     "last_name = '" + txtLastName.Text + "', " +
                                     "full_name = '" + txtFullName.Text + "', " +
                                     "gender = '" + (rbMale.Checked ? "Male" : "Female") + "', " +
                                     "date_of_birth = '" + dateTimePicker1.Value.ToString("yyyy-MM-dd") + "', " +
                                     "stu_nic_no = '" + txtStuNicNo.Text + "', " +
                                     "tp_no = '" + txtPhoneNo.Text + "', " +
                                     "grade_id = '" + cmbGradeId.Text + "', " +
                                     "medium = '" + cmbMedium.Text + "', " +
                                     "date_of_admission = '" + dateTimePicker2.Value.ToString("yyyy-MM-dd") + "', " +
                                     "resident_address = '" + txtAddress.Text + "' " +
                                     "WHERE id = '" + currentStudentId + "'";

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
                    string deleteQuery = "DELETE FROM students WHERE id='" + userId + "'";
                    ExecuteQuery(deleteQuery, "Record Deleted Successfully!");
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
                currentStudentId = row.Cells["id"].Value.ToString();
                txtAdmissionNo.Text = row.Cells["admission_no"].Value.ToString();
                txtFirstName.Text = row.Cells["first_name"].Value.ToString();
                txtLastName.Text = row.Cells["last_name"].Value.ToString();
                txtFullName.Text = row.Cells["full_name"].Value.ToString();
                if (row.Cells["gender"].Value.ToString() == "Male")
                {
                    rbMale.Checked = true;
                }
                else
                {
                    rbFemale.Checked = true;
                }
                dateTimePicker1.Value = Convert.ToDateTime(row.Cells["date_of_birth"].Value);
                txtStuNicNo.Text = row.Cells["stu_nic_no"].Value.ToString();
                txtPhoneNo.Text = row.Cells["tp_no"].Value.ToString();
                cmbGradeId.Text = row.Cells["grade_id"].Value.ToString();
                cmbMedium.Text = row.Cells["medium"].Value.ToString();
                dateTimePicker2.Value = Convert.ToDateTime(row.Cells["date_of_admission"].Value);
                txtAddress.Text = row.Cells["resident_address"].Value.ToString();
            }
        }

        private void StudentListForm_Load(object sender, EventArgs e)
        {
            RefreshData();
        }
    }
}

