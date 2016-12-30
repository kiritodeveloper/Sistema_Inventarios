using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Media;

namespace Inventarios_Kyara
{
    class pantalonesC
    { 
        public MainWindow window;
        string DBConn = ConfigurationManager.ConnectionStrings["Inventarios_Kyara.Properties.Settings.InventarioKyaraConnectionString"].ConnectionString;
        private string selectedID;


        public void agregarPantalon()
        {
            using (SqlConnection conn = new SqlConnection(DBConn))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                //insertamos articulo y datos
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "insertaPant";
                cmd.Parameters.AddWithValue("@codigo", window.pantaCodBox.Text);
                cmd.Parameters.AddWithValue("@cant0", window.cantpant0Box.Text);
                cmd.Parameters.AddWithValue("@cant1", window.cantpant1Box.Text);
                cmd.Parameters.AddWithValue("@cant3", window.cantpant3Box.Text);
                cmd.Parameters.AddWithValue("@cant5", window.cantpant5Box.Text);
                cmd.Parameters.AddWithValue("@cant7", window.cantpant7Box.Text);
                cmd.Parameters.AddWithValue("@cant9", window.cantpant9Box.Text);
                cmd.Parameters.AddWithValue("@cant11", window.cantpant11Box.Text);
                cmd.Parameters.AddWithValue("@cant13", window.cantpant13Box.Text);
                cmd.Parameters.AddWithValue("@cant15", window.cantpant15Box.Text);

                cmd.Parameters.Add("@respuesta", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@result", SqlDbType.Int).Direction = ParameterDirection.Output;

                conn.Open();
                cmd.ExecuteNonQuery();
                string respuesta = cmd.Parameters["@respuesta"].Value.ToString();
                int result = (int)cmd.Parameters["@result"].Value;
                window.pantaResLbl.Content = respuesta;
                conn.Close();

                window.pantaResLbl.BorderBrush = Brushes.IndianRed;

                if (result == 0)
                    window.pantaResLbl.BorderBrush = Brushes.ForestGreen;

                else if (result == -2)
                    return;

            }
            buscarPantalon(window.pantaCodBox.Text);

        }

        public void selectionChanged()
        {
            if (window.PantalonesDG.SelectedIndex >= 0)
            {
                DataRowView rowView = window.PantalonesDG.SelectedItem as DataRowView;
                selectedID = rowView.Row[0].ToString();
                window.pantaCodBox.Text = rowView.Row[0].ToString();
                window.cantpant0Box.Text = rowView.Row[1].ToString();
                window.cantpant1Box.Text = rowView.Row[2].ToString();
                window.cantpant3Box.Text = rowView.Row[3].ToString();
                window.cantpant5Box.Text = rowView.Row[4].ToString();
                window.cantpant7Box.Text = rowView.Row[5].ToString();
                window.cantpant9Box.Text = rowView.Row[6].ToString();
                window.cantpant11Box.Text = rowView.Row[7].ToString();
                window.cantpant13Box.Text = rowView.Row[8].ToString();
                window.cantpant15Box.Text = rowView.Row[9].ToString();
                window.agregarPantBtn.IsEnabled = false;
                window.modPantBtn.IsEnabled = true;
                window.eliminarPantBtn.IsEnabled = true;
                window.pantaCodBox.IsEnabled = false;
                //cambioColores = false;
            }
        }
        
        public void load_Pantalones()
        {
            using (SqlConnection conn = new SqlConnection(DBConn))
            {
                using (SqlCommand cmd = new SqlCommand("getPantalones", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        if(dt.Rows.Count > 0)
                            window.PantalonesDG.ItemsSource = dt.DefaultView;
                    }
                }
            }
        }

        public void modPantalon(int op)
        {
            using (SqlConnection conn = new SqlConnection(DBConn))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "modificaPant";
                cmd.Parameters.AddWithValue("@codigo", window.pantaCodBox.Text);
                cmd.Parameters.AddWithValue("@cant0", window.cantpant0Box.Text);
                cmd.Parameters.AddWithValue("@cant1", window.cantpant1Box.Text);
                cmd.Parameters.AddWithValue("@cant3", window.cantpant3Box.Text);
                cmd.Parameters.AddWithValue("@cant5", window.cantpant5Box.Text);
                cmd.Parameters.AddWithValue("@cant7", window.cantpant7Box.Text);
                cmd.Parameters.AddWithValue("@cant9", window.cantpant9Box.Text);
                cmd.Parameters.AddWithValue("@cant11", window.cantpant11Box.Text);
                cmd.Parameters.AddWithValue("@cant13", window.cantpant13Box.Text);
                cmd.Parameters.AddWithValue("@cant15", window.cantpant15Box.Text);

                cmd.Parameters.Add("@respuesta", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@result", SqlDbType.Int).Direction = ParameterDirection.Output;

                conn.Open();
                cmd.ExecuteNonQuery();
                string respuesta = cmd.Parameters["@respuesta"].Value.ToString();
                int result = (int)cmd.Parameters["@result"].Value;
                window.pantaResLbl.Content = respuesta;
                conn.Close();

                window.pantaResLbl.BorderBrush = Brushes.IndianRed;
                if (result == 0)
                    window.pantaResLbl.BorderBrush = Brushes.ForestGreen;
            }
            //Recargamos los articulos para visualizarlos, a menos que se trate de eliminacion por modificacion a 0s (op != 0)
            if (op == 0)
                buscarPantalon(window.pantaCodBox.Text);
            else
            {
                limpiarCampos();
                window.pantaResLbl.BorderBrush = Brushes.ForestGreen;
                window.pantaResLbl.Content = "Eliminado correctamente.";
            }

        }

        public void borrarPantalon()
        {
            //borramos primero las relaciones ColoresXArticulos
            borrarColores();
            //borramos el articulo especificado
            using (SqlConnection conn = new SqlConnection(DBConn))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                //insertamos articulo y datos
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "delPantalon";
                cmd.Parameters.AddWithValue("@codigo", window.pantaCodBox.Text);
                cmd.Parameters.Add("@respuesta", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;

                conn.Open();
                cmd.ExecuteNonQuery();
                string respuesta = cmd.Parameters["@respuesta"].Value.ToString();
                window.pantaResLbl.Content = respuesta;
                window.pantaResLbl.BorderBrush = Brushes.ForestGreen;
                conn.Close();
            }
            limpiarCampos();
        }

        public void borrarColores()
        {
            using (SqlConnection conn = new SqlConnection(DBConn))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "delColores";
                cmd.Parameters.AddWithValue("@codigo", window.pantaCodBox.Text);
                cmd.Parameters.Add("@respuesta", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        public void buscarPantalon(string codigo)
        {
            // limpiarCampos();
            using (SqlConnection conn = new SqlConnection(DBConn))
            {
                using (SqlCommand cmd = new SqlCommand("buscarPantalon", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@codArticulo", codigo);
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        window.PantalonesDG.ItemsSource = dt.DefaultView;
                        if (window.PantalonesDG.Items.Count > 0)
                            window.PantalonesDG.SelectedIndex = 0;
                        else
                        {
                            window.pantaResLbl.Content = "El artículo no tiene entradas o no existe!";
                            window.pantaResLbl.BorderBrush = Brushes.IndianRed;
                        }
                    }
                }
            }
        }

        public void limpiarCampos()
        {
            window.pantaResLbl.Content = "";
            window.pantaResLbl.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF102774"));
            window.pantaCodBox.Text = "0";
            window.cantpant0Box.Text = "0";
            window.cantpant1Box.Text = "0";
            window.cantpant3Box.Text = "0";
            window.cantpant5Box.Text = "0";
            window.cantpant7Box.Text = "0";
            window.cantpant9Box.Text = "0";
            window.cantpant11Box.Text = "0";
            window.cantpant13Box.Text = "0";
            window.cantpant15Box.Text = "0";
            window.buscarPantBox.Text = "0";
            window.PantalonesDG.SelectedIndex = -1;
            window.PantalonesDG.ItemsSource = null;
            window.agregarPantBtn.IsEnabled = true;
            window.modPantBtn.IsEnabled = false;
            window.eliminarPantBtn.IsEnabled = false;
            window.pantaCodBox.IsEnabled = true;
            window.pantaCodBox.Focus();
        }

    }
}
