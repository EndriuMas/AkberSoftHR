using System;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;  // ← ВАЖНО: Microsoft, не System!
using System.Windows;

namespace AkberSoft
{
    public partial class MainWindow : Window
    {
        // ⚠️ ПРОВЕРЬ СЕРВЕР: замени PRO-ANDREW\SQLEXPRESS на свой!
        private string connectionString = @"Data Source=PRO-ANDREW\SQLEXPRESS;Initial Catalog=akbersoft;Integrated Security=True;TrustServerCertificate=True;";

        private ObservableCollection<Worker> workers = new ObservableCollection<Worker>();

        public MainWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            workers.Clear();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = @"
                        SELECT e.Id, e.FullName, e.Position, e.Phone, e.Email, 
                               COALESCE(t.Name, d.Name, 'Без отдела') as DeptName
                        FROM Employees e
                        LEFT JOIN Teams t ON e.TeamId = t.Id
                        LEFT JOIN Departments d ON e.DepartmentId = d.Id
                        ORDER BY e.Id";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            workers.Add(new Worker
                            {
                                Id = reader.GetInt32(0),
                                FullName = reader.GetString(1),
                                Position = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                Phone = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                Email = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                Department = reader.GetString(5)
                            });
                        }
                    }
                }
                MyDataGrid.ItemsSource = workers;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка");
            }
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e) => LoadData();

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new InputDialog();
            if (dialog.ShowDialog() == true)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = "INSERT INTO Employees (FullName, Position, Phone, Email) VALUES (@n, @p, @ph, @e)";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@n", dialog.FullName);
                        cmd.Parameters.AddWithValue("@p", string.IsNullOrEmpty(dialog.Position) ? "" : dialog.Position);
                        cmd.Parameters.AddWithValue("@ph", string.IsNullOrEmpty(dialog.Phone) ? "" : dialog.Phone);
                        cmd.Parameters.AddWithValue("@e", string.IsNullOrEmpty(dialog.Email) ? "" : dialog.Email);
                        cmd.ExecuteNonQuery();
                    }
                }
                LoadData();
            }
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            if (MyDataGrid.SelectedItem is Worker selected)
            {
                var dialog = new InputDialog(selected);
                if (dialog.ShowDialog() == true)
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        string sql = "UPDATE Employees SET FullName=@n, Position=@p, Phone=@ph, Email=@e WHERE Id=@id";
                        using (SqlCommand cmd = new SqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", selected.Id);
                            cmd.Parameters.AddWithValue("@n", dialog.FullName);
                            cmd.Parameters.AddWithValue("@p", string.IsNullOrEmpty(dialog.Position) ? "" : dialog.Position);
                            cmd.Parameters.AddWithValue("@ph", string.IsNullOrEmpty(dialog.Phone) ? "" : dialog.Phone);
                            cmd.Parameters.AddWithValue("@e", string.IsNullOrEmpty(dialog.Email) ? "" : dialog.Email);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    LoadData();
                }
            }
            else MessageBox.Show("Выберите сотрудника!", "Ошибка");
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (MyDataGrid.SelectedItem is Worker selected)
            {
                if (MessageBox.Show($"Удалить {selected.FullName}?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("DELETE FROM Employees WHERE Id=@id", conn))
                        {
                            cmd.Parameters.AddWithValue("@id", selected.Id);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    LoadData();
                }
            }
            else MessageBox.Show("Выберите сотрудника!", "Ошибка");
        }
    }

    public class Worker
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Position { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }
    }
}