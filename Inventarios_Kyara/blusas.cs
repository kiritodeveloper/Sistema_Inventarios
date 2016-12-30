using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Inventarios_Kyara
{
    class blusas
    {

        public MainWindow window;
        string DBConn = ConfigurationManager.ConnectionStrings["Inventarios_Kyara.Properties.Settings.InventarioKyaraConnectionString"].ConnectionString;
        //private bool borrarLabel = true;
        //private bool cambioColores = false;
        private string selectedID;

        public void agregarBlusa()
        {
            using (SqlConnection conn = new SqlConnection(DBConn))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                //insertamos articulo y datos
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "insertaTop";
                cmd.Parameters.AddWithValue("@codigo", window.blusasCodBox.Text);
                cmd.Parameters.AddWithValue("@cantS", window.cantSBox.Text);
                cmd.Parameters.AddWithValue("@cantM", window.cantMBox.Text);
                cmd.Parameters.AddWithValue("@cantL", window.cantLBox.Text);
                cmd.Parameters.AddWithValue("@cantXL", window.cantXLBox.Text);
                cmd.Parameters.AddWithValue("@cant2X", window.cant2XBox.Text);
                cmd.Parameters.AddWithValue("@cant3X", window.cant3XBox.Text);

                cmd.Parameters.Add("@respuesta", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@result", SqlDbType.Int).Direction = ParameterDirection.Output;

                conn.Open();
                cmd.ExecuteNonQuery();
                string respuesta = cmd.Parameters["@respuesta"].Value.ToString();
                int result = (int)cmd.Parameters["@result"].Value;
                window.blusasResLbl.Content = respuesta;
                conn.Close();

                window.blusasResLbl.BorderBrush = Brushes.IndianRed;

                if (result == 0)
                    window.blusasResLbl.BorderBrush = Brushes.ForestGreen;

                else if (result == -2)
                    return;

            }
            buscarBlusa(window.blusasCodBox.Text);

        }

        public void selectionChanged()
        {
            if (window.blusasDG.SelectedIndex >= 0)
            {
                DataRowView rowView = window.blusasDG.SelectedItem as DataRowView;
                selectedID = rowView.Row[0].ToString();
                window.blusasCodBox.Text = rowView.Row[0].ToString();
                window.cantSBox.Text = rowView.Row[1].ToString();
                window.cantMBox.Text = rowView.Row[2].ToString();
                window.cantLBox.Text = rowView.Row[3].ToString();
                window.cantXLBox.Text = rowView.Row[4].ToString();
                window.cant2XBox.Text = rowView.Row[5].ToString();
                window.cant3XBox.Text = rowView.Row[6].ToString();
                window.agregarBlusaBtn.IsEnabled = false;
                window.modBlusasBtn.IsEnabled = true;
                window.eliminarBlusaBtn.IsEnabled = true;
                window.blusasCodBox.IsEnabled = false;
                //cambioColores = false;
            }
        }

        public void load_Blusas()
        {
            using (SqlConnection conn = new SqlConnection(DBConn))
            {
                using (SqlCommand cmd = new SqlCommand("getBlusas", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        if(dt.Rows.Count > 0)  
                            window.blusasDG.ItemsSource = dt.DefaultView;
                    }
                }
            }
        }

        public void modBlusa(int op)
        {
            using (SqlConnection conn = new SqlConnection(DBConn))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "modificaTop";
                cmd.Parameters.AddWithValue("@codigo", window.blusasCodBox.Text);
                cmd.Parameters.AddWithValue("@cantS", window.cantSBox.Text);
                cmd.Parameters.AddWithValue("@cantM", window.cantMBox.Text);
                cmd.Parameters.AddWithValue("@cantL", window.cantLBox.Text);
                cmd.Parameters.AddWithValue("@cantXL", window.cantXLBox.Text);
                cmd.Parameters.AddWithValue("@cant2X", window.cant2XBox.Text);
                cmd.Parameters.AddWithValue("@cant3X", window.cant3XBox.Text);

                cmd.Parameters.Add("@respuesta", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;

                conn.Open();
                cmd.ExecuteNonQuery();
                string respuesta = cmd.Parameters["@respuesta"].Value.ToString();
                window.blusasResLbl.Content = respuesta;
                conn.Close();
                
                window.blusasResLbl.BorderBrush = Brushes.IndianRed;
                if (respuesta == "Actualizado correctamente")
                    window.blusasResLbl.BorderBrush = Brushes.ForestGreen;        
            }
            //Recargamos los articulos para visualizarlos, a menos que se trate de eliminacion por modificacion a 0s
            if (op == 0)
                buscarBlusa(window.blusasCodBox.Text);
            else
            {
                limpiarCampos();
                window.blusasResLbl.BorderBrush = Brushes.ForestGreen;
                window.blusasResLbl.Content = "Eliminado correctamente.";
            }
        }

        public void borrarBlusa()
        {
            //borramos primero las relaciones ColoresXArticulos
            borrarColores();
            //borramos el articulo especificado
            using (SqlConnection conn = new SqlConnection(DBConn))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                //insertamos articulo y datos
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "delBlusa";
                cmd.Parameters.AddWithValue("@codigo", window.blusasCodBox.Text);
                cmd.Parameters.Add("@respuesta", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;

                conn.Open();
                cmd.ExecuteNonQuery();
                string respuesta = cmd.Parameters["@respuesta"].Value.ToString();
                window.blusasResLbl.Content = respuesta;
                window.blusasResLbl.BorderBrush = Brushes.ForestGreen;
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
                cmd.Parameters.AddWithValue("@codigo", window.blusasCodBox.Text);
                cmd.Parameters.Add("@respuesta", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        public void buscarBlusa(string codigo)
        {
           // limpiarCampos();
            using (SqlConnection conn = new SqlConnection(DBConn))
            {
                using (SqlCommand cmd = new SqlCommand("buscarBlusa", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@codArticulo", codigo);
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        window.blusasDG.ItemsSource = dt.DefaultView;
                        if (window.blusasDG.Items.Count > 0)
                            window.blusasDG.SelectedIndex = 0;
                        else
                        {
                            window.blusasResLbl.Content = "El artículo no tiene entradas o no existe!";
                            window.blusasResLbl.BorderBrush = Brushes.IndianRed;
                        }
                    }
                }
            }
        }

        public void limpiarCampos()
        {
            window.blusasResLbl.Content = "";
            window.blusasResLbl.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF102774"));
            window.blusasCodBox.Text = "";
            window.cantSBox.Text = "0";
            window.cantMBox.Text = "0";
            window.cantLBox.Text = "0";
            window.cantXLBox.Text = "0";
            window.cant2XBox.Text = "0";
            window.cant3XBox.Text = "0";
            window.buscarBlusaBox.Text = "";
            window.blusasDG.SelectedIndex = -1;
            window.blusasDG.ItemsSource = null;
            window.agregarBlusaBtn.IsEnabled = true;
            window.modBlusasBtn.IsEnabled = false;
            window.eliminarBlusaBtn.IsEnabled = false;
            window.blusasCodBox.IsEnabled = true;
            window.blusasCodBox.Focus();
        }

        public DataTable blusasDT()
        {
            using (SqlConnection conn = new SqlConnection(DBConn))
            {
                using (SqlCommand cmd = new SqlCommand("getBlusas", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }
    

    }
}
