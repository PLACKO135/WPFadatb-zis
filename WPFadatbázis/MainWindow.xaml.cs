using System;
using System.Collections.Generic;
using System.IO;
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
using Microsoft.Win32;
using MySql.Data.MySqlClient;

namespace WPFadatbázis
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=hardver;";
        List<Termek> termek = new List<Termek>();
        MySqlConnection connection;

        public MainWindow()
        {
            InitializeComponent();

            AdatbazisMegnyitas();
            KategoriaBetoltese();
            GyartokBetoltese();

            TermekekBetolteseListaba();
            AdatbazisBezarasa();


        }
        private void AdatbazisMegnyitas()
        {
            try
            {
                connection = new MySqlConnection(connectionString);
                connection.Open();
            }
            catch (Exception)
            {

                MessageBox.Show("Nem tud kapcsolódni az adatbázishoz");
                this.Close();
            }
        }
        private void AdatbazisBezarasa()
        {
            connection.Close();
            connection.Dispose();
        }

        private void KategoriaBetoltese()
        {
            string kategoriakrendezve = "SELECT DISTINCT kategória From termekek ORDER BY kategória;";
            MySqlCommand command = new MySqlCommand(kategoriakrendezve, connection);
            MySqlDataReader reader = command.ExecuteReader();

            cbKategoria.Items.Add("-Nincs megadva-");
            while (reader.Read())
            {
                cbKategoria.Items.Add(reader.GetString(0));
            }

            reader.Close();
            cbKategoria.SelectedIndex = 0;

        }

        private void GyartokBetoltese()
        {
            string gyartokrendezve = "SELECT DISTINCT gyártó From termekek ORDER BY gyártó;";
            MySqlCommand command = new MySqlCommand(gyartokrendezve, connection);
            MySqlDataReader reader = command.ExecuteReader();

            cbGyarto.Items.Add("-Nincs megadva-");
            while (reader.Read())
            {
                cbGyarto.Items.Add(reader.GetString(0));
            }

            reader.Close();
            cbGyarto.SelectedIndex = 0;

        }

        private void TermekekBetolteseListaba()
        {
            string osszes = "SELECT * FROM termékek";
            MySqlCommand command = new MySqlCommand(osszes, connection);
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Termek uj = new Termek(reader.GetString("Kategória"),
                                        reader.GetString("Gyártó"),
                                            reader.GetString("Név"),
                                            reader.GetInt32("Ár"),
                                            reader.GetInt32("Garidő"));
                termek.Add(uj);
            }
            reader.Close();
            dgTermekek.ItemsSource = termek;

        }
        private string SzukitoLekerdezesEloallitasa()
        {
            bool vanfeltetel = false;
            string szukitettLista= "SELECT * FROM termékek";
            if (cbGyarto.SelectedIndex>0|| cbKategoria.SelectedIndex>0|| txtTermek.Text !="")
            {
                szukitettLista += "WHERE";
            }
            if (cbGyarto.SelectedIndex > 0)
            {
                szukitettLista += $"gyártó='{cbGyarto.SelectedItem}'";
                vanfeltetel = true;
            }
            if (cbKategoria.SelectedIndex > 0)
            {
                if (vanfeltetel)
                {
                    szukitettLista += " AND ";
                }

                szukitettLista += $"kategória='{cbKategoria.SelectedItem}'";
                vanfeltetel = true;
            }
            if (txtTermek.Text != "")
            {
                if (vanfeltetel)
                {
                    szukitettLista += " AND ";
                }

                szukitettLista += $"név LIKE'%{txtTermek.Text}%'";
            }
            return szukitettLista;
        }

        private void btnSzukit_Click(object sender, RoutedEventArgs e)
        {
            termek.Clear();
            string szukitett = SzukitoLekerdezesEloallitasa();

            MySqlCommand command = new MySqlCommand(szukitett, connection);
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Termek uj = new Termek(reader.GetString(1),
                                        reader.GetString(2),
                                            reader.GetString(3),
                                            reader.GetInt32(4),
                                            reader.GetInt32(5));
                termek.Add(uj);
            }
            reader.Close();
            dgTermekek.Items.Refresh();

        }
        
        private void btnMentes_Click(object sender, RoutedEventArgs e)
        {
           
        }
    }
}
