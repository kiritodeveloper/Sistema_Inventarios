using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Media;

namespace Inventarios_Kyara
{
    class articulos
    {
        public MainWindow window;
        string DBConn = ConfigurationManager.ConnectionStrings["Inventarios_Kyara.Properties.Settings.InventarioKyaraConnectionString"].ConnectionString;
        public InventarioKyaraDataSet inventarioKyaraDataSet;
        public InventarioKyaraDataSetTableAdapters.ArticulosTableAdapter inventarioKyaraDataSetArticulosTableAdapter;
        public InventarioKyaraDataSetTableAdapters.TiposTableAdapter inventarioKyaraDataSetTiposTableAdapter;
        public InventarioKyaraDataSetTableAdapters.MarcasTableAdapter inventarioKyaraDataSetMarcasTableAdapter;
        public InventarioKyaraDataSetTableAdapters.ColoresTableAdapter inventarioKyaraDataSetColoresTableAdapter;

        private string selectedID;
        private bool cambioColores = false;
        private bool borrarLabel = true;

        public void agregarArticulo()
        {
            using (SqlConnection conn = new SqlConnection(DBConn))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                //insertamos articulo y datos
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "insertaArticulo";
                cmd.Parameters.AddWithValue("@codigo", window.codArtBox.Text);
                cmd.Parameters.AddWithValue("@marcaID", window.marcaArtCBox.SelectedValue);
                cmd.Parameters.AddWithValue("@tipoID", window.tipoArtCBox.SelectedValue);
                cmd.Parameters.AddWithValue("@precio", window.precioArtBox.Text);
                if (window.descArtBox.Text != "")
                    cmd.Parameters.AddWithValue("@descripcion", window.descArtBox.Text);

                var outparam = cmd.Parameters.Add("@respuesta", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;
                
                conn.Open();
                cmd.ExecuteNonQuery();
                string respuesta = cmd.Parameters["@respuesta"].Value.ToString();
                window.artResLabel.Content = respuesta;
                conn.Close();

                if (respuesta == "Insertado Correctamente")
                {
                    addColores();
                    window.artResLabel.BorderBrush = Brushes.ForestGreen;
                }
                else  window.artResLabel.BorderBrush = Brushes.IndianRed;

            }
            //Recargamos los articulos para visualizarlos
            borrarLabel = false;
            buscarArticulo(window.codArtBox.Text);

        }

        public void modArticulo()
        {
            using (SqlConnection conn = new SqlConnection(DBConn))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "modArticulo";
                cmd.Parameters.AddWithValue("@codigo", window.codArtBox.Text);
                cmd.Parameters.AddWithValue("@marcaID", window.marcaArtCBox.SelectedValue);
                cmd.Parameters.AddWithValue("@tipoID", window.tipoArtCBox.SelectedValue);
                cmd.Parameters.AddWithValue("@precio", window.precioArtBox.Text);
                if (window.descArtBox.Text != "")
                    cmd.Parameters.AddWithValue("@descripcion", window.descArtBox.Text);

                cmd.Parameters.Add("@respuesta", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;

                conn.Open();
                cmd.ExecuteNonQuery();
                string respuesta = cmd.Parameters["@respuesta"].Value.ToString();
                window.artResLabel.Content = respuesta;
                conn.Close();

                if (respuesta == "Actualizado correctamente")
                {
                    window.artResLabel.BorderBrush = Brushes.ForestGreen;
                    if (cambioColores == true)
                    {
                        //Mal optimizado!! Borra primero todos los colores de ese articulo
                        borrarColores();
                        //Vuelve a agregar los colores nuevos.
                        addColores();
                    }
                }
                else window.artResLabel.BorderBrush = Brushes.IndianRed;
            }
            borrarLabel = false;
            //Recargamos los articulos para visualizarlos
            buscarArticulo(window.codArtBox.Text);
        }

        public void borrarArticulo()
        {
            //borramos primero las relaciones ColoresXArticulos
            borrarColores();
            //borramos el articulo especificado
            using (SqlConnection conn = new SqlConnection(DBConn))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                //insertamos articulo y datos
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "delArticulo";
                cmd.Parameters.AddWithValue("@codigo", window.codArtBox.Text);
                cmd.Parameters.Add("@respuesta", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@result", SqlDbType.Int).Direction = ParameterDirection.Output;

                conn.Open();
                cmd.ExecuteNonQuery();
                string respuesta = cmd.Parameters["@respuesta"].Value.ToString();
                int result = (int)cmd.Parameters["@result"].Value;
                window.artResLabel.Content = respuesta;
                if(result == 0)
                    window.artResLabel.BorderBrush = Brushes.ForestGreen;
                if(result == -1)
                    window.artResLabel.BorderBrush = Brushes.IndianRed;
                conn.Close();
            }
            borrarLabel = false;
            limpiarCampos();
        }

        public void borrarColores()
        {
            using (SqlConnection conn = new SqlConnection(DBConn))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "delColores";
                cmd.Parameters.AddWithValue("@codigo", window.codArtBox.Text);
                cmd.Parameters.Add("@respuesta", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        public void addColores()
        {
            using (SqlConnection conn = new SqlConnection(DBConn))
            using (SqlCommand cmd = conn.CreateCommand())
            {                  
                //insertamos colores disponibles de ese articulo
                int sValue = 0;

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "isrtColorXArt";
                cmd.Parameters.AddWithValue("@codigo", window.codArtBox.Text);
                cmd.Parameters.Add(new SqlParameter("@colorID", 0));
                cmd.Parameters.Add("@respuesta", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;

                conn.Open();
                for (int i = 0; i < window.coloresDispCBox.Items.Count; i++)
                {
                    DataRowView oDataRowView = window.coloresDispCBox.Items[i] as DataRowView;
                    sValue = (int)oDataRowView.Row[0];
                    cmd.Parameters["@colorID"].Value = sValue;
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
        }

        public void cambiarSeleccion()
        {
            if (window.articulosDG.SelectedIndex >= 0)
            {
                DataRowView rowView = window.articulosDG.SelectedItem as DataRowView;
                selectedID = rowView.Row[0].ToString();
                window.codArtBox.Text = rowView.Row[0].ToString();
                window.marcaArtCBox.SelectedValue = rowView.Row[1].ToString();
                window.tipoArtCBox.SelectedValue = rowView.Row[2].ToString(); //3 y 4 son los nombres de marca y tipo, se omiten
                window.precioArtBox.Text = rowView.Row[5].ToString();
                window.descArtBox.Text = rowView.Row[6].ToString();
                load_ColoresDisp(selectedID);
                window.agregarArtBtn.IsEnabled = false;
                window.modArtsBtn.IsEnabled = true;
                window.elimArtsBtn.IsEnabled = true;
                window.codArtBox.IsEnabled = false;
                cambioColores = false;
            }
        }

        public void addColorDisp()
        {
            cambioColores = true;
            DataRowView drv = (DataRowView)window.coloresArtsCBox.SelectedItem;
            int colID = (int)drv[0];

            //Verificar que no esta 
            foreach(DataRowView dr in window.coloresDispCBox.Items)
            {
                if((int)dr[0] == colID)
                    return;
            }

            //Insertar
            window.coloresDispCBox.Items.Add(window.coloresArtsCBox.SelectedItem);
            window.coloresDispCBox.SelectedIndex = window.coloresDispCBox.Items.Count - 1;
            
        }

        public void delColorDisp()
        {
            cambioColores = true;
            if (window.coloresDispCBox.Items.Count > 0)
            {
                window.coloresDispCBox.Items.RemoveAt(window.coloresDispCBox.SelectedIndex);
                if (window.coloresDispCBox.Items.Count > 0)
                    window.coloresDispCBox.SelectedIndex = 0;
            }

        }

        public void load_Colores()
        {
            inventarioKyaraDataSetColoresTableAdapter.Fill(inventarioKyaraDataSet.Colores);
            System.Windows.Data.CollectionViewSource coloresViewSource = ((System.Windows.Data.CollectionViewSource)(window.FindResource("coloresViewSource")));
            coloresViewSource.View.MoveCurrentToFirst();
        }

        public void load_ColoresDisp(string codigo)
        {
            window.coloresDispCBox.Items.Clear();
            using (SqlConnection conn = new SqlConnection(DBConn))
            {
                using (SqlCommand cmd = new SqlCommand("getColoresDisp", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@codArtic", codigo);

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        window.coloresDispCBox.DataContext = dt;
                        foreach (DataRow dr in dt.Rows)
                        {
                            DataRowView drv = dt.DefaultView[dt.Rows.IndexOf(dr)];
                            window.coloresDispCBox.Items.Add(drv);
                        }
                        if (window.coloresDispCBox.Items.Count > 0)
                            window.coloresDispCBox.SelectedIndex = 0;
                    }
                }
            }
        }

        public void load_Marcas()
        {
            // Cargar datos en la tabla Marcas. Puede modificar este código según sea necesario.
           inventarioKyaraDataSetMarcasTableAdapter.Fill(inventarioKyaraDataSet.Marcas);
            System.Windows.Data.CollectionViewSource marcasViewSource = ((System.Windows.Data.CollectionViewSource)(window.FindResource("marcasViewSource")));
            marcasViewSource.View.MoveCurrentToFirst();
        }

        public void load_Tipos()
        {            
            // Cargar datos en la tabla Tipos. Puede modificar este código según sea necesario.
             inventarioKyaraDataSetTiposTableAdapter.Fill(inventarioKyaraDataSet.Tipos);
            System.Windows.Data.CollectionViewSource tiposViewSource = ((System.Windows.Data.CollectionViewSource)(window.FindResource("tiposViewSource")));
            tiposViewSource.View.MoveCurrentToFirst();
        }

        public void load_Articulos() {
            using (SqlConnection conn = new SqlConnection(DBConn))
            {
                using (SqlCommand cmd = new SqlCommand("getArticulos", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        window.articulosDG.ItemsSource = dt.DefaultView;
                    }
                }
            }
            if (window.IsLoaded == true)
            {
                window.articulosDG.Columns[1].Visibility = System.Windows.Visibility.Hidden;
                window.articulosDG.Columns[2].Visibility = System.Windows.Visibility.Hidden;
            }
        }

        public void buscarArticulo(string codigo)
        {
            limpiarCampos();
            using (SqlConnection conn = new SqlConnection(DBConn))
            {
                using (SqlCommand cmd = new SqlCommand("buscarArticulos", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@codArticulo", codigo);
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        window.articulosDG.ItemsSource = dt.DefaultView;
                        if(window.articulosDG.Items.Count > 0)
                            window.articulosDG.SelectedIndex = 0;
                        else
                        {
                            window.artResLabel.Content = "No se encontro el articulo!";
                            window.artResLabel.BorderBrush = Brushes.IndianRed;
                        }
                    }
                }
            }
            if (window.IsLoaded == true)
            {
                window.articulosDG.Columns[1].Visibility = System.Windows.Visibility.Hidden;
                window.articulosDG.Columns[2].Visibility = System.Windows.Visibility.Hidden;
            }
        }

        public void limpiarCampos()
        {
            if (borrarLabel)
            {
                window.artResLabel.Content = "";
                window.artResLabel.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF102774"));
            }
            borrarLabel = true;
            window.codArtBox.Text = "";
            window.marcaArtCBox.SelectedIndex = 0;
            window.tipoArtCBox.SelectedIndex = 0; 
            window.precioArtBox.Text = "";
            window.descArtBox.Text = "";
            window.buscarArtBox.Text = "";
            window.articulosDG.SelectedIndex = -1;
            window.articulosDG.ItemsSource = null;
            window.coloresDispCBox.Items.Clear();
            window.agregarArtBtn.IsEnabled = true;
            window.modArtsBtn.IsEnabled = false;
            window.elimArtsBtn.IsEnabled = false;
            cambioColores = false;
            window.codArtBox.IsEnabled = true;
            window.codArtBox.Focus();
        }


    }
}
