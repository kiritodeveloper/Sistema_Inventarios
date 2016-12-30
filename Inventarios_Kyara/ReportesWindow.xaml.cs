using System.Windows;
using System.Data;
using Microsoft.Reporting.WinForms;
using System.Data.SqlClient;
using System.Configuration;

namespace Inventarios_Kyara
{
    /// <summary>
    /// Lógica de interacción para blusasReporte.xaml
    /// </summary>
    public partial class blusasReporte : Window
    {
        private int reportNum;
        public blusasReporte(int num)
        {
            reportNum = num;
            InitializeComponent();
        }

        private void reporteV_Loaded(object sender, RoutedEventArgs e)
        {
            ReportViewerDemo.Reset();
            //Reporte de blusas
            DataTable dt = new DataTable();

            if (reportNum == 0)
            {
                string DBConn = ConfigurationManager.ConnectionStrings["Inventarios_Kyara.Properties.Settings.InventarioKyaraConnectionString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(DBConn))
                {
                    using (SqlCommand cmd = new SqlCommand("getArticulos", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                };
                ReportViewerDemo.LocalReport.ReportEmbeddedResource = "Inventarios_Kyara.repArticulos.rdlc";
            }

            if (reportNum == 1)
            {
                InventarioKyaraDataSetTableAdapters.BlusasTableAdapter blusasAdap = new InventarioKyaraDataSetTableAdapters.BlusasTableAdapter();
                dt = blusasAdap.GetData();
                ReportViewerDemo.LocalReport.ReportEmbeddedResource = "Inventarios_Kyara.repBlusas.rdlc";
            }

            if (reportNum == 2)
            {
                InventarioKyaraDataSetTableAdapters.PantalonesTableAdapter pantAdap = new InventarioKyaraDataSetTableAdapters.PantalonesTableAdapter();
                dt = pantAdap.GetData();
                ReportViewerDemo.LocalReport.ReportEmbeddedResource = "Inventarios_Kyara.repPants.rdlc";
            }

            if(reportNum == 3)
            {
                InventarioKyaraDataSetTableAdapters.OtrosTableAdapter otrosAdap = new InventarioKyaraDataSetTableAdapters.OtrosTableAdapter();
                dt = otrosAdap.GetData();
                ReportViewerDemo.LocalReport.ReportEmbeddedResource = "Inventarios_Kyara.repOtro.rdlc";
            }
            ReportDataSource ds;
            if (reportNum != 0)
                 ds = new ReportDataSource("InventarioKyaraDataSet", dt);
            else ds = new ReportDataSource("customDataSet", dt);

            ReportViewerDemo.LocalReport.DataSources.Add(ds);
            ReportViewerDemo.RefreshReport();
        }
    }
}
