using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Inventarios_Kyara
{
    class Configuracion
    {
        public MainWindow window;
        string DBConn = ConfigurationManager.ConnectionStrings["Inventarios_Kyara.Properties.Settings.InventarioKyaraConnectionString"].ConnectionString;
        public InventarioKyaraDataSet inventarioKyaraDataSet;
        public InventarioKyaraDataSetTableAdapters.MarcasTableAdapter inventarioKyaraDataSetMarcasTableAdapter;
        public InventarioKyaraDataSetTableAdapters.TiposTableAdapter inventarioKyaraDataSetTiposTableAdapter;
        public InventarioKyaraDataSetTableAdapters.ColoresTableAdapter inventarioKyaraDataSetColoresTableAdapter;
        private System.Windows.Data.CollectionViewSource marcasViewSource;
        private System.Windows.Data.CollectionViewSource tiposViewSource;
        private System.Windows.Data.CollectionViewSource colsViewSource;


        public void load_Datos()
        {
            // Cargar datos en la tabla Marcas. Puede modificar este código según sea necesario.
            marcasViewSource = ((System.Windows.Data.CollectionViewSource)(window.FindResource("marcasConfViewSource")));
            marcasViewSource.View.MoveCurrentToFirst();
            tiposViewSource = ((System.Windows.Data.CollectionViewSource)(window.FindResource("tiposConfViewSource")));
            tiposViewSource.View.MoveCurrentToFirst();
            colsViewSource = ((System.Windows.Data.CollectionViewSource)(window.FindResource("coloresConfViewSource")));
            colsViewSource.View.MoveCurrentToFirst();
            // window.confMarcasList.SelectedIndex = 0;
        }

        public int addMarcaDisp()
        {
            using (SqlConnection conn = new SqlConnection(DBConn))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                //insertamos articulo y datos
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "insertaMarca";
                cmd.Parameters.AddWithValue("@Nom", window.confMarcasBox.Text);

                cmd.Parameters.Add("@respuesta", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@result", SqlDbType.Int).Direction = ParameterDirection.Output;

                conn.Open();
                cmd.ExecuteNonQuery();
                string respuesta = cmd.Parameters["@respuesta"].Value.ToString();
                int result = (int)cmd.Parameters["@result"].Value;
                window.configResLbl.Content = respuesta;
                conn.Close();

                window.configResLbl.BorderBrush = Brushes.IndianRed;

                if (result == 0)
                {
                    window.configResLbl.BorderBrush = Brushes.ForestGreen;
                    inventarioKyaraDataSetMarcasTableAdapter.Fill(inventarioKyaraDataSet.Marcas);
                }

                return result;
            }
        }

        public void borrarMarca()
        {
            //borramos el articulo especificado
            using (SqlConnection conn = new SqlConnection(DBConn))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                //insertamos articulo y datos
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "delMarca";
                cmd.Parameters.AddWithValue("@intMarca", window.confMarcasList.SelectedValue);
                cmd.Parameters.Add("@respuesta", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;
                 
                conn.Open();
                cmd.ExecuteNonQuery();
                string respuesta = cmd.Parameters["@respuesta"].Value.ToString();
                window.configResLbl.Content = respuesta;
                window.configResLbl.BorderBrush = Brushes.ForestGreen;
                inventarioKyaraDataSetMarcasTableAdapter.Fill(inventarioKyaraDataSet.Marcas);
                conn.Close();
            }
        }

        public int addTipoDisp(string tipoSTR, int cat)
        {
            using (SqlConnection conn = new SqlConnection(DBConn))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                //insertamos articulo y datos
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "insertaTipo";
                cmd.Parameters.AddWithValue("@Nom", tipoSTR);
                cmd.Parameters.AddWithValue("@cat", cat);

                cmd.Parameters.Add("@respuesta", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@result", SqlDbType.Int).Direction = ParameterDirection.Output;

                conn.Open();
                cmd.ExecuteNonQuery();
                string respuesta = cmd.Parameters["@respuesta"].Value.ToString();
                int result = (int)cmd.Parameters["@result"].Value;
                window.configResLbl.Content = respuesta;
                conn.Close();

                window.configResLbl.BorderBrush = Brushes.IndianRed;

                if (result == 0)
                {
                    window.configResLbl.BorderBrush = Brushes.ForestGreen;
                    inventarioKyaraDataSetTiposTableAdapter.Fill(inventarioKyaraDataSet.Tipos);
                }

                return result;
            }
        }

        public void borrarTipo(int idTipo)
        {
            //borramos el articulo especificado
            using (SqlConnection conn = new SqlConnection(DBConn))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                //insertamos articulo y datos
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "delTipo";
                cmd.Parameters.AddWithValue("@idTipo", idTipo);
                cmd.Parameters.Add("@respuesta", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;

                conn.Open();
                cmd.ExecuteNonQuery();
                string respuesta = cmd.Parameters["@respuesta"].Value.ToString();
                window.configResLbl.Content = respuesta;
                window.configResLbl.BorderBrush = Brushes.ForestGreen;
                inventarioKyaraDataSetTiposTableAdapter.Fill(inventarioKyaraDataSet.Tipos);
                conn.Close();
            }
        }

        public int addColorDips(string colorSTR)
        {
            using (SqlConnection conn = new SqlConnection(DBConn))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                //insertamos articulo y datos
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "insertaColor";
                cmd.Parameters.AddWithValue("@Color", colorSTR);

                cmd.Parameters.Add("@respuesta", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@result", SqlDbType.Int).Direction = ParameterDirection.Output;

                conn.Open();
                cmd.ExecuteNonQuery();
                string respuesta = cmd.Parameters["@respuesta"].Value.ToString();
                int result = (int)cmd.Parameters["@result"].Value;
                window.configResLbl.Content = respuesta;
                conn.Close();

                window.configResLbl.BorderBrush = Brushes.IndianRed;

                if (result == 0)
                {
                    window.configResLbl.BorderBrush = Brushes.ForestGreen;
                    inventarioKyaraDataSetColoresTableAdapter.Fill(inventarioKyaraDataSet.Colores);
                }

                return result;
            }
        }

        public void borrarColor()
        {
            //borramos el articulo especificado
            using (SqlConnection conn = new SqlConnection(DBConn))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                //insertamos articulo y datos
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "delColor";
                cmd.Parameters.AddWithValue("@IDCol", window.confColoresList.SelectedValue);
                cmd.Parameters.Add("@respuesta", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;

                conn.Open();
                cmd.ExecuteNonQuery();
                string respuesta = cmd.Parameters["@respuesta"].Value.ToString();
                window.configResLbl.Content = respuesta;
                window.configResLbl.BorderBrush = Brushes.ForestGreen;
                inventarioKyaraDataSetColoresTableAdapter.Fill(inventarioKyaraDataSet.Colores);
                conn.Close();
            }
        }



    }
}
