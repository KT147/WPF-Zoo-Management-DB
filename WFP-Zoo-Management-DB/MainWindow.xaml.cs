using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace WFP_Zoo_Management_DB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        SqlConnection sqlConnection;
        public MainWindow()
        {
            InitializeComponent();

            string connectionString = ConfigurationManager.ConnectionStrings["WFP_Zoo_Management_DB.Properties.Settings.cDBConnectionString"].ConnectionString;

            sqlConnection = new SqlConnection(connectionString);

            ShowZoos();
            ShowAnimals();

        }

        private void ShowZoos()
        {

            try
            {
                string query = "SELECT * FROM Zoo";
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);

                using (sqlDataAdapter)
                {
                    DataTable zooTable = new DataTable();

                    sqlDataAdapter.Fill(zooTable);
                    listZoos.DisplayMemberPath = "Location";
                    listZoos.SelectedValuePath = "Id";
                    listZoos.ItemsSource = zooTable.DefaultView;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

        }

        private void ShowAnimals()
        {
            string query = "SELECT * FROM Animal";

            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);

            try
            {
                using (sqlDataAdapter)
                {
                    DataTable animalTable = new DataTable();

                    sqlDataAdapter.Fill(animalTable);
                    listAnimals.DisplayMemberPath = "Name";
                    listAnimals.SelectedValuePath = "Id";
                    listAnimals.ItemsSource = animalTable.DefaultView;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

        }

        private void ShowAssociatedAnimals()
        {

            try
            {
                string query = @"
                                SELECT *
                                FROM Animal a
                                INNER JOIN ZooAnimal za 
                                ON a.Id = za.AnimalId
                                WHERE za.ZooId = @ZooId";

                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                using (sqlDataAdapter)
                {
                    DataRowView row = (DataRowView)listZoos.SelectedItem;

                    sqlCommand.Parameters.AddWithValue("@ZooId", row["Id"]);
                    DataTable animalTable = new DataTable();

                    sqlDataAdapter.Fill(animalTable);
                    listAssociatedAnimals.DisplayMemberPath = "Name";
                    listAssociatedAnimals.SelectedValuePath = "Id";
                    listAssociatedAnimals.ItemsSource = animalTable.DefaultView;
                }
            }
            catch (Exception e)
            {
                // MessageBox.Show(e.ToString());
            }

        }


        private void ShowSelectedZooInTextBox()
        {
            try
            {
                DataRowView row = (DataRowView)listZoos.SelectedItem;

                myTextBox.Text = row["Location"].ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ShowSelectedAnimalInTextBox()
        {
            try
            {
                DataRowView row = (DataRowView)listAnimals.SelectedItem;

                myTextBox.Text = row["Name"].ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void listZoos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            ShowAssociatedAnimals();
            ShowSelectedZooInTextBox();

        }

        private void DeleteZoo_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                string query = "Delete from Zoo where Id=@ZooId";

                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();

                DataRowView row = (DataRowView)listZoos.SelectedItem;
                sqlCommand.Parameters.AddWithValue("@ZooId", row["Id"]);

                sqlCommand.ExecuteScalar();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowZoos();
            }
        }

        private void DeleteAnimal_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                string query = "Delete from Animal where Id=@AnimalId";

                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();

                DataRowView row = (DataRowView)listAnimals.SelectedItem;
                sqlCommand.Parameters.AddWithValue("@AnimalId", row["Id"]);

                
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowAnimals();
            }
        }

        private void DeleteAssociatedAnimal_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                string query = "Delete from ZooAnimal where AnimalId=@AnimalId";

                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();

                DataRowView row = (DataRowView)listAssociatedAnimals.SelectedItem;
                sqlCommand.Parameters.AddWithValue("@AnimalId", row["Id"]);


                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowAssociatedAnimals();
            }
        }


        private void AddZoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "INSERT INTO Zoo (Location) VALUES (@Location)";

                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

                sqlCommand.Parameters.AddWithValue("@Location", myTextBox.Text);

                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowZoos();
            }
        }

        private void AddAnimal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "INSERT INTO Animal (Name) VALUES (@Name)";

                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

                sqlCommand.Parameters.AddWithValue("@Name", myTextBox.Text);

                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                ShowAnimals();
            }
        }

        private void AddAnimalToZoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "INSERT INTO ZooAnimal VALUES (@ZooId, @AnimalId)";

                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

                DataRowView zooRow = (DataRowView)listZoos.SelectedItem;
                DataRowView animalRow = (DataRowView)listAnimals.SelectedItem;

                sqlCommand.Parameters.AddWithValue("@ZooId", zooRow["Id"]);
                sqlCommand.Parameters.AddWithValue("@AnimalId", animalRow["Id"]);

                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();

                ShowAssociatedAnimals();
            }
        }

        private void UpdateZoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "UPDATE Zoo Set Location = @Location WHERE Id = @ZooId";

                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

                DataRowView zooRow = (DataRowView)listZoos.SelectedItem;
                DataRowView animalRow = (DataRowView)listAnimals.SelectedItem;

                sqlCommand.Parameters.AddWithValue("@ZooId", zooRow["Id"]);
                sqlCommand.Parameters.AddWithValue("@Location", myTextBox.Text);

                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();

                ShowZoos();
            }
        }

        private void UpdateAnimal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "UPDATE Animal Set Name = @Name WHERE Id = @AnimalId";

                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

                DataRowView zooRow = (DataRowView)listZoos.SelectedItem;
                DataRowView animalRow = (DataRowView)listAnimals.SelectedItem;

                sqlCommand.Parameters.AddWithValue("@AnimalId", animalRow["Id"]);
                sqlCommand.Parameters.AddWithValue("@Name", myTextBox.Text);

                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();

                ShowAnimals();
            }
        }

        private void listAnimals_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowSelectedAnimalInTextBox();
        }
    }
}
