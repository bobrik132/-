using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace РоссийскоеМолоко
{
    public partial class MainWindow : Window
    {
        private string connectionString =
            @"Server=DESKTOP-HGH5128;Database=РоссийскоеМолоко;Trusted_Connection=True;";

        public MainWindow()
        {
            InitializeComponent();
            dpДатаПоставки.SelectedDate = DateTime.Now;

            ЗагрузитьПоставщиков();
            ЗагрузитьПродукцию();
            ЗагрузитьПоставки();
        }

        private void ЗагрузитьПоставщиков()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(
                    "SELECT КодПоставщика, Название FROM Поставщики", connection);

                DataTable table = new DataTable();
                adapter.Fill(table);
                cbПоставщик.ItemsSource = table.DefaultView;
            }
        }

        private void ЗагрузитьПродукцию()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(
                    "SELECT КодПродукции, Наименование FROM Продукция", connection);

                DataTable table = new DataTable();
                adapter.Fill(table);
                cbПродукция.ItemsSource = table.DefaultView;
            }
        }

        private void ЗагрузитьПоставки()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
                    SELECT 
                        Поставки.КодПоставки AS [№],
                        Поставщики.Название AS [Поставщик],
                        Продукция.Наименование AS [Продукция],
                        Поставки.ДатаПоставки AS [Дата],
                        Поставки.Количество AS [Количество]
                    FROM Поставки
                    INNER JOIN Поставщики 
                        ON Поставки.КодПоставщика = Поставщики.КодПоставщика
                    INNER JOIN Продукция 
                        ON Поставки.КодПродукции = Продукция.КодПродукции
                    ORDER BY Поставки.КодПоставки DESC";

                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable table = new DataTable();
                adapter.Fill(table);
                dgПоставки.ItemsSource = table.DefaultView;
            }
        }

        private void ДобавитьПоставку_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cbПоставщик.SelectedValue == null || cbПродукция.SelectedValue == null)
                {
                    MessageBox.Show("Выберите поставщика и продукцию.");
                    return;
                }

                if (!decimal.TryParse(txtКоличество.Text, out decimal количество))
                {
                    MessageBox.Show("Введите корректное количество.");
                    return;
                }

                if (dpДатаПоставки.SelectedDate == null)
                {
                    MessageBox.Show("Выберите дату поставки.");
                    return;
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                        INSERT INTO Поставки (КодПоставщика, КодПродукции, ДатаПоставки, Количество)
                        VALUES (@КодПоставщика, @КодПродукции, @ДатаПоставки, @Количество)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@КодПоставщика", cbПоставщик.SelectedValue);
                        command.Parameters.AddWithValue("@КодПродукции", cbПродукция.SelectedValue);
                        command.Parameters.AddWithValue("@ДатаПоставки", dpДатаПоставки.SelectedDate.Value);
                        command.Parameters.AddWithValue("@Количество", количество);

                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Поставка добавлена.");
                ЗагрузитьПоставки();
                ОчиститьПоля();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void Обновить_Click(object sender, RoutedEventArgs e)
        {
            ЗагрузитьПоставки();
        }

        private void Очистить_Click(object sender, RoutedEventArgs e)
        {
            ОчиститьПоля();
        }

        private void ОчиститьПоля()
        {
            cbПоставщик.SelectedIndex = -1;
            cbПродукция.SelectedIndex = -1;
            dpДатаПоставки.SelectedDate = DateTime.Now;
            txtКоличество.Text = "";
        }
    }
}