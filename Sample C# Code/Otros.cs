using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Media;

namespace Inventarios_Kyara
{
    class Otros
    {
        public MainWindow window;
        string DBConn = ConfigurationManager.ConnectionStrings["Inventarios_Kyara.Properties.Settings.InventarioKyaraConnectionString"].ConnectionString;
        private string selectedID;


        public void agregarOtro()
        {
            using (SqlConnection conn = new SqlConnection(DBConn))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                //insertamos articulo y datos
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "insertaOtro";
                cmd.Parameters.AddWithValue("@codigo", window.otrosCodBox.Text);
                cmd.Parameters.AddWithValue("@cantidad", window.cantOtroBox.Text);

                cmd.Parameters.Add("@respuesta", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@result", SqlDbType.Int).Direction = ParameterDirection.Output;

                conn.Open();
                cmd.ExecuteNonQuery();
                string respuesta = cmd.Parameters["@respuesta"].Value.ToString();
                int result = (int)cmd.Parameters["@result"].Value;
                window.otrosResLbl.Content = respuesta;
                conn.Close();

                window.otrosResLbl.BorderBrush = Brushes.IndianRed;

                if (result == 0)
                    window.otrosResLbl.BorderBrush = Brushes.ForestGreen;

                else if (result == -2)
                    return;

            }
            buscarOtro(window.otrosCodBox.Text);

        }

        public void selectionChanged()
        {
            if (window.OtrosDG.SelectedIndex >= 0)
            {
                DataRowView rowView = window.OtrosDG.SelectedItem as DataRowView;
                selectedID = rowView.Row[0].ToString();
                window.otrosCodBox.Text = rowView.Row[0].ToString();
                window.cantOtroBox.Text = rowView.Row[1].ToString();
                window.agregarOtrosBtn.IsEnabled = false;
                window.modOtrosBtn.IsEnabled = true;
                window.eliminarOtrosBtn.IsEnabled = true;
                window.otrosCodBox.IsEnabled = false;
                //cambioColores = false;
            }
        }

        public void load_Otro()
        {
            using (SqlConnection conn = new SqlConnection(DBConn))
            {
                using (SqlCommand cmd = new SqlCommand("getOtro", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        if(dt.Rows.Count > 0)
                            window.OtrosDG.ItemsSource = dt.DefaultView;
                    }
                }
            }
        }

        public void modOtro(int op)
        {
            using (SqlConnection conn = new SqlConnection(DBConn))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "modificaOtro";
                cmd.Parameters.AddWithValue("@codigo", window.otrosCodBox.Text);
                cmd.Parameters.AddWithValue("@cantidad", window.cantOtroBox.Text);

                cmd.Parameters.Add("@respuesta", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@result", SqlDbType.Int).Direction = ParameterDirection.Output;

                conn.Open();
                cmd.ExecuteNonQuery();
                string respuesta = cmd.Parameters["@respuesta"].Value.ToString();
                int result = (int)cmd.Parameters["@result"].Value;
                window.otrosResLbl.Content = respuesta;
                conn.Close();

                window.otrosResLbl.BorderBrush = Brushes.IndianRed;
                if (result == 0)
                    window.otrosResLbl.BorderBrush = Brushes.ForestGreen;
            }
            //Recargamos los articulos para visualizarlos, a menos que se trate de eliminacion por modificacion a 0s (op != 0)
            if (op == 0)
                buscarOtro(window.otrosCodBox.Text);
            else
            {
                limpiarCampos();
                window.otrosResLbl.BorderBrush = Brushes.ForestGreen;
                window.otrosResLbl.Content = "Eliminado correctamente.";
            }

        }

        public void borrarOtro()
        {
            //borramos primero las relaciones ColoresXArticulos
            borrarColores();
            //borramos el articulo especificado
            using (SqlConnection conn = new SqlConnection(DBConn))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                //insertamos articulo y datos
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "delOtro";
                cmd.Parameters.AddWithValue("@codigo", window.otrosCodBox.Text);
                cmd.Parameters.Add("@respuesta", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;

                conn.Open();
                cmd.ExecuteNonQuery();
                string respuesta = cmd.Parameters["@respuesta"].Value.ToString();
                window.otrosResLbl.Content = respuesta;
                window.otrosResLbl.BorderBrush = Brushes.ForestGreen;
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
                cmd.Parameters.AddWithValue("@codigo", window.otrosCodBox.Text);
                cmd.Parameters.Add("@respuesta", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        public void buscarOtro(string codigo)
        {
            // limpiarCampos();
            using (SqlConnection conn = new SqlConnection(DBConn))
            {
                using (SqlCommand cmd = new SqlCommand("buscarOtro", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@codArticulo", codigo);
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        window.OtrosDG.ItemsSource = dt.DefaultView;
                        if (window.OtrosDG.Items.Count > 0)
                            window.OtrosDG.SelectedIndex = 0;
                        else
                        {
                            window.otrosResLbl.Content = "El artículo no tiene entradas o no existe!";
                            window.otrosResLbl.BorderBrush = Brushes.IndianRed;
                        }
                    }
                }
            }
        }

        public void limpiarCampos()
        {
            window.otrosResLbl.Content = "";
            window.otrosResLbl.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF102774"));
            window.otrosCodBox.Text = "0";
            window.cantOtroBox.Text = "0";
            window.OtrosDG.SelectedIndex = -1;
            window.OtrosDG.ItemsSource = null;
            window.agregarOtrosBtn.IsEnabled = true;
            window.modOtrosBtn.IsEnabled = false;
            window.eliminarOtrosBtn.IsEnabled = false;
            window.otrosCodBox.IsEnabled = true;
            window.otrosCodBox.Focus();
        }
    }
}
