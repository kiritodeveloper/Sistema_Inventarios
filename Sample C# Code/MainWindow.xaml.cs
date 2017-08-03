using System.Windows;
using MahApps.Metro.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System;
using System.IO;

namespace Inventarios_Kyara
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private articulos articulosC = new articulos();
        private blusas blusasC = new blusas();
        private pantalonesC pantaC = new pantalonesC();
        private Otros otroC = new Otros();
        private Configuracion confC = new Configuracion();
        private int BlusasID = 1;
        private int PantsID = 2;
        private int OtrosID = 3;
        private int ultimoSelected = 0;
        private bool loadedTabC = false;
        public List<string> imgPathList = new List<string>();

        public MainWindow()
        {
            InitializeComponent();

            //Cargas de Tab Articulos
            //--------------------------
            articulosC.window = this;
            articulosC.inventarioKyaraDataSet = ((InventarioKyaraDataSet)(this.FindResource("inventarioKyaraDataSet")));
            articulosC.inventarioKyaraDataSetMarcasTableAdapter = new InventarioKyaraDataSetTableAdapters.MarcasTableAdapter();
            articulosC.inventarioKyaraDataSetTiposTableAdapter = new InventarioKyaraDataSetTableAdapters.TiposTableAdapter();
            articulosC.inventarioKyaraDataSetArticulosTableAdapter = new InventarioKyaraDataSetTableAdapters.ArticulosTableAdapter();
            articulosC.inventarioKyaraDataSetColoresTableAdapter =  new InventarioKyaraDataSetTableAdapters.ColoresTableAdapter();
            //articulosC.load_Articulos();
            articulosC.load_Marcas();
            articulosC.load_Tipos();
            articulosC.load_Colores();

            confC.inventarioKyaraDataSet = ((InventarioKyaraDataSet)(this.FindResource("inventarioKyaraDataSet")));
            confC.inventarioKyaraDataSetMarcasTableAdapter = new InventarioKyaraDataSetTableAdapters.MarcasTableAdapter();
            confC.inventarioKyaraDataSetTiposTableAdapter = new InventarioKyaraDataSetTableAdapters.TiposTableAdapter();
            confC.inventarioKyaraDataSetColoresTableAdapter = new InventarioKyaraDataSetTableAdapters.ColoresTableAdapter();
            confC.window = this;
            confC.load_Datos();
            //Enviar referencia de ventana a cada clase
            //--------------------------
            blusasC.window = this;
            pantaC.window = this;
            otroC.window = this;

        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            loadedTabC = true;
        }

        //Control de cambio de tab
        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (loadedTabC == true)
            {
                TabItem ultimo = (TabItem)tabControl.Items[ultimoSelected];
                ultimo.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#00FFFFFF"));
                TabItem item = (TabItem)tabControl.SelectedItem;
                item.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF001230"));
                ultimoSelected = tabControl.SelectedIndex;
            }
        }

        #region "Control de Articulos"

        private void articulosDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { articulosC.cambiarSeleccion();  }

        private void LimpiarArtsBtn_Clicked(object sender, RoutedEventArgs e)
        { articulosC.limpiarCampos();  }

        private void agregarArtBtn_Clicked(object sender, RoutedEventArgs e)
        {
            int res = 0;
            if (int.TryParse(precioArtBox.Text, out res) == true)
            {
                if (codArtBox.Text == "")
                {
                    artResLabel.Content = "El campo Codigo está vacio!";
                    artResLabel.BorderBrush = Brushes.DarkRed;
                    return;
                }
                if (precioArtBox.Text == "")
                {
                    artResLabel.Content = "El campo Precio está vacio!";
                    artResLabel.BorderBrush = Brushes.DarkRed;
                    return;
                }
                articulosC.agregarArticulo();
            }
            else
            {
                artResLabel.Content = "Ingrese unicamente numeros en el campo de Precio!";
                artResLabel.BorderBrush = Brushes.DarkRed;
                return;
            }
            
                 
        }

        private void modArtsBtn_Clicked(object sender, RoutedEventArgs e)
        {
            if (precioArtBox.Text == "")
            {
                artResLabel.Content = "El campo Precio está vacio!";
                artResLabel.BorderBrush = Brushes.DarkRed;
                return;
            }
            articulosC.modArticulo();
        }

        private void elimArtsBtn_Clicked(object sender, RoutedEventArgs e)
        { articulosC.borrarArticulo();    }

        private void buscarArts()
        { articulosC.buscarArticulo(buscarArtBox.Text); }

        private void buscarArtBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {   buscarArts();   }
        }

        private void colorAddBtn_Clicked(object sender, RoutedEventArgs e)
        { articulosC.addColorDisp();  }

        private void colorDelBtn_Clicked(object sender, RoutedEventArgs e)
        { articulosC.delColorDisp();  }

        private void cargarArticulosBtn_Clicked(object sender, RoutedEventArgs e)
        {
            articulosC.load_Articulos();
            if (articulosDG.Items.Count > 0)
                articulosDG.SelectedIndex = 0;
        }

        private void printArtsBtn_Click(object sender, RoutedEventArgs e)
        {
            blusasReporte BR = new blusasReporte(0);
            BR.ShowDialog();
        }
        
        private void imageViewerBtn_Clicked(object sender, RoutedEventArgs e)
        {
            ImageViewer IV = new ImageViewer();
            IV.setImg(image.Source);
            IV.ShowDialog();
        }

        private void changeImgButton_Clicked(object sender, RoutedEventArgs e)
        {
            var fileDialog = new System.Windows.Forms.OpenFileDialog();
            fileDialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            var result = fileDialog.ShowDialog();
            int currentIndx = coloresDispCBox.SelectedIndex;
            switch (result)
            {
                case System.Windows.Forms.DialogResult.OK:
                    var fileName = fileDialog.FileName;
                    int colorID = (int)coloresDispCBox.SelectedValue;
                    articulosC.set_newImage(codArtBox.Text, colorID, fileName);
                    articulosC.load_ColoresDisp(codArtBox.Text);
                    coloresDispCBox.SelectedIndex = currentIndx;
                    break;
                case System.Windows.Forms.DialogResult.Cancel:
                default:
                    break;
            }
        }

        private void deleteImgBtn_Clicked(object sender, RoutedEventArgs e)
        {
            int colorID = (int)coloresDispCBox.SelectedValue;
            string codigo = codArtBox.Text;
            articulosC.set_newImage(codigo, colorID, "");
            articulosC.load_ColoresDisp(codigo);
        }

        private void coloresDispCBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            enabledImgButtons(false);
            if (loadedTabC == true)
            {
                BitmapImage imagenArticulo = new BitmapImage();
                imagenArticulo.BeginInit();
                imagenArticulo.UriSource = new Uri("pack://application:,,,/Resources/placeholder.png");
                if (coloresDispCBox.Items.Count == imgPathList.Count && coloresDispCBox.Items.Count > 0)
                {
                    int colorIndx = coloresDispCBox.SelectedIndex;
                    string imgPath = imgPathList[colorIndx];
                    if (imgPath != "" && File.Exists(imgPath))
                    {
                        imagenArticulo.UriSource = new Uri(imgPath);
                        enabledImgButtons(true);
                    }                        
                }
                imagenArticulo.EndInit();
                image.Source = imagenArticulo;
            }
        }

        private void enabledImgButtons(bool bl)
        {
            viewImg_Button.IsEnabled = bl;
            deleteImg_Button.IsEnabled = bl;
        }

        #endregion

        #region "Control de Blusas"

        private void blusasDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { blusasC.selectionChanged(); }

        private void LimpiarBlusasBtn_Clicked(object sender, RoutedEventArgs e)
        { blusasC.limpiarCampos();    }

        private void agregarBlusaBtn_Clicked(object sender, RoutedEventArgs e)
        {
            if (checkType(BlusasID) == 0)
            {
                if (checkCantsBlusa() == 0)
                    blusasC.agregarBlusa();
            }
        }

        private void modBlusasBtn_Clicked(object sender, RoutedEventArgs e)
        {
            int check = checkCantsBlusa();
            if (check == 0)
                blusasC.modBlusa(0);
            else if (check == -1)
            {
                MessageBoxResult result = MessageBox.Show("Modificar las cantidades a 0 eliminara la entrada, desea continuar?", "Eliminar", MessageBoxButton.YesNo);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        blusasC.modBlusa(1);
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }
        }

        private void elimBlusaBtn_Clicked(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Esta seguro que desea eliminar la entrada?", "Eliminar", MessageBoxButton.YesNo);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    blusasC.borrarBlusa();
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }

        private void buscarBlusa()
        {
            blusasC.buscarBlusa(buscarBlusaBox.Text);
            cantSBox.Focus();
            cantSBox.SelectAll();
        }

        private void buscarBlusaBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                blusasResLbl.Content = "";
                blusasResLbl.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF102774"));
                buscarBlusa();
            }
        }

        private void cantBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (e.KeyboardDevice.IsKeyDown(Key.Tab))
                ((TextBox)sender).SelectAll();
        }

        private void cargarBlusasBtn_Clicked(object sender, RoutedEventArgs e)
        {
            blusasC.load_Blusas();
            if (blusasDG.Items.Count > 0)
                blusasDG.SelectedIndex = 0;    
        }

        private int checkCantsBlusa()
        {
            try {
                if (int.Parse(cantSBox.Text) + int.Parse(cantMBox.Text) + int.Parse(cantLBox.Text) +
                    int.Parse(cantXLBox.Text) + int.Parse(cant2XBox.Text) + int.Parse(cant3XBox.Text) == 0)
                {
                    blusasResLbl.Content = "Los campos de cantidad estan en 0!";
                    blusasResLbl.BorderBrush = Brushes.IndianRed;
                    cantSBox.Focus();
                    cantSBox.SelectAll();
                    return -1;
                }
                else return 0;
            }
            catch
            {
                blusasResLbl.Content = "Las cantidades deben ser numeros enteros!";
                blusasResLbl.BorderBrush = Brushes.IndianRed;
                cantSBox.Focus();
                cantSBox.SelectAll();
                return -2;
            }
        }

        private void printBlusasBtn_Click(object sender, RoutedEventArgs e)
        {
            blusasReporte BR = new blusasReporte(1);
            BR.ShowDialog();
        }
        #endregion

        #region "Control de Pantalones"

        private void buscarPantBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                pantaResLbl.Content = "";
                pantaResLbl.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF102774"));
                buscarPantalon();
            }
        }

        private void buscarPantalon()
        {
            pantaC.buscarPantalon(buscarPantBox.Text);
            cantpant0Box.Focus();
            cantpant0Box.SelectAll();
        }
        
        private void agregarPantBtn_Clicked(object sender, RoutedEventArgs e)
        {
            if (checkType(PantsID) == 0)
            {
                if (checkCantsPants() == 0)
                    pantaC.agregarPantalon();
            }
        }
        
        private void LimpiarPantBtn_Clicked(object sender, RoutedEventArgs e)
        { pantaC.limpiarCampos(); }

        private void modPantBtn_Clicked(object sender, RoutedEventArgs e)
        {
            int check = checkCantsPants();
            if (check == 0)
                pantaC.modPantalon(0);
            else if (check == -1)
            {
                MessageBoxResult result = MessageBox.Show("Modificar las cantidades a 0 eliminara la entrada, desea continuar?", "Eliminar", MessageBoxButton.YesNo);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        pantaC.modPantalon(1);
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }
        }
        
        private void elimPantBtn_Clicked(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Esta seguro que desea eliminar la entrada?", "Eliminar", MessageBoxButton.YesNo);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    pantaC.borrarPantalon();
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }

        private void pantaDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { pantaC.selectionChanged(); }

        private void cargarPantsBtn_Clicked(object sender, RoutedEventArgs e)
        {
            pantaC.load_Pantalones();
            if (PantalonesDG.Items.Count > 0)
                PantalonesDG.SelectedIndex = 0;
        }

        private int checkCantsPants()
        {
            try
            {
                if (int.Parse(cantpant0Box.Text) + int.Parse(cantpant1Box.Text) + int.Parse(cantpant3Box.Text) +
                    int.Parse(cantpant5Box.Text) + int.Parse(cantpant7Box.Text) + int.Parse(cantpant9Box.Text) +
                    int.Parse(cantpant11Box.Text) + int.Parse(cantpant13Box.Text) + int.Parse(cantpant15Box.Text) == 0)
                {
                    pantaResLbl.Content = "Los campos de cantidad estan en 0!";
                    pantaResLbl.BorderBrush = Brushes.IndianRed;
                    cantpant0Box.Focus();
                    cantpant0Box.SelectAll();
                    return -1;
                }
                return 0;
            }
            catch
            {
                pantaResLbl.Content = "Las cantidades deben ser numeros enteros!";
                pantaResLbl.BorderBrush = Brushes.IndianRed;
                cantpant0Box.Focus();
                cantpant0Box.SelectAll();
                return -2;
            }
        }

        private void printPantBtn_Click(object sender, RoutedEventArgs e)
        {
            blusasReporte BR = new blusasReporte(2);
            BR.ShowDialog();
        }

        #endregion

        #region "Otros"

        private void buscarOtrosBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                otrosResLbl.Content = "";
                otrosResLbl.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF102774"));
                buscarOtro();
            }
        }

        private void buscarOtro()
        {
            otroC.buscarOtro(buscarOtrosBox.Text);
            cantOtroBox.Focus();
            cantOtroBox.SelectAll();
        }

        private void otrosDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { otroC.selectionChanged(); }

        private void elimOtrosBtn_Clicked(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Esta seguro que desea eliminar la entrada?", "Eliminar", MessageBoxButton.YesNo);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    otroC.borrarOtro();
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }

        private void modOtrosBtn_Clicked(object sender, RoutedEventArgs e)
        {
            int check = checkCantsOtro();
            if (check == 0)
                otroC.modOtro(0);
            else if (check == -1)
            {
                MessageBoxResult result = MessageBox.Show("Modificar las cantidades a 0 eliminara la entrada, desea continuar?", "Eliminar", MessageBoxButton.YesNo);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        otroC.modOtro(1);
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }
        }

        private void agregarOtrosBtn_Clicked(object sender, RoutedEventArgs e)
        {
            if (checkType(OtrosID) == 0)
            {
                if (checkCantsOtro() == 0)
                    otroC.agregarOtro();
            }
        }

        private void LimpiarOtrosBtn_Clicked(object sender, RoutedEventArgs e)
        { otroC.limpiarCampos(); }

        private void cargarOtrosBtn_Clicked(object sender, RoutedEventArgs e)
        { otroC.load_Otro();
          if (PantalonesDG.Items.Count > 0)
               PantalonesDG.SelectedIndex = 0;
        }

        private int checkCantsOtro()
        {
            try
            {
                if (int.Parse(cantOtroBox.Text) == 0)
                {
                    otrosResLbl.Content = "El campo de cantidad está en 0!";
                    otrosResLbl.BorderBrush = Brushes.IndianRed;
                    cantOtroBox.Focus();
                    cantOtroBox.SelectAll();
                    return -1;
                }
                return 0;
            }
            catch
            {
                otrosResLbl.Content = "Las cantidades deben ser numeros enteros!";
                otrosResLbl.BorderBrush = Brushes.IndianRed;
                cantOtroBox.Focus();
                cantOtroBox.SelectAll();
                return -2;
            }
        }

        private void printOtrosBtn_Click(object sender, RoutedEventArgs e)
        {
            blusasReporte BR = new blusasReporte(3);
            BR.ShowDialog();
        }
        #endregion

        #region 'Configuracion'
        private void confMarcaAddBtn_Clicked(object sender, RoutedEventArgs e)
        {
            if (confMarcasBox.Text == "")
                return;

            if (confC.addMarcaDisp() == 0)
            {
                confMarcasBox.Text = "";
                reSelectMarcas();
            }
        }

        private void confMarcaDelBtn_Clicked(object sender, RoutedEventArgs e)
        {
            if (confMarcasList.SelectedIndex != -1)
            {
                confC.borrarMarca();
                reSelectMarcas();
            }
        }

        public int checkType(int catActual)
        {
            //borramos el articulo especificado
            string DBConn = ConfigurationManager.ConnectionStrings["Inventarios_Kyara.Properties.Settings.InventarioKyaraConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(DBConn))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                //insertamos articulo y datos
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "checkType";
                if (catActual == 1)
                    cmd.Parameters.AddWithValue("@codArticulo", blusasCodBox.Text);
                if (catActual == 2)
                    cmd.Parameters.AddWithValue("@codArticulo", pantaCodBox.Text);
                if (catActual == 3)
                    cmd.Parameters.AddWithValue("@codArticulo", otrosCodBox.Text);
                cmd.Parameters.AddWithValue("@catActual", catActual);
                cmd.Parameters.Add("@result", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@respuesta", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;

                conn.Open();
                cmd.ExecuteNonQuery();
                string respuesta = cmd.Parameters["@respuesta"].Value.ToString();
                int result = (int)cmd.Parameters["@result"].Value;
                conn.Close();

                if (result == -1)
                {
                    if (catActual == 1)
                    {
                        blusasResLbl.Content = respuesta;
                        blusasResLbl.BorderBrush = Brushes.IndianRed;
                        return -1;
                    }
                    if (catActual == 2)
                    {
                        pantaResLbl.Content = respuesta;
                        pantaResLbl.BorderBrush = Brushes.IndianRed;
                        return -1;
                    }
                    if (catActual == 3)
                    {
                        otrosResLbl.Content = respuesta;
                        otrosResLbl.BorderBrush = Brushes.IndianRed;
                        return -1;
                    }
                }
                return 0;
            }
        }

        public void reSelectMarcas()
        {
            System.Windows.Data.CollectionViewSource marcasViewSource = ((System.Windows.Data.CollectionViewSource)(FindResource("marcasViewSource")));
            marcasViewSource.View.MoveCurrentToFirst();
        }

        private void confTipoDelBtn_Clicked(object sender, RoutedEventArgs e)
        {
            if (confTiposDG.SelectedIndex != -1)
            {
                DataRowView rowView = confTiposDG.SelectedItem as DataRowView;
                int ID = int.Parse(rowView.Row[0].ToString()) ;
                MessageBox.Show(ID.ToString());
                confC.borrarTipo(ID);
                reSelectTipos();
            }
        }

        private void confTipoAddBtn_Clicked(object sender, RoutedEventArgs e)
        {
            string tipoSTR = confTiposBox.Text;
            if (tipoSTR == "")
                return;

            int cat = confCategoriasBox.SelectedIndex + 1;
            
            if(confC.addTipoDisp(tipoSTR,cat) == 0)
            {
                confTiposBox.Text = "";
                reSelectTipos();
            }

        }

        public void reSelectTipos()
        {
            System.Windows.Data.CollectionViewSource tiposViewSource = ((System.Windows.Data.CollectionViewSource)(FindResource("tiposViewSource")));
            tiposViewSource.View.MoveCurrentToFirst();
        }

        private void confColsAddBtn_Clicked(object sender, RoutedEventArgs e)
        {
            string colSTR = confColoresBox.Text;
            if (colSTR == "")
                return;

            if (confC.addColorDips(colSTR) == 0)
            {
                confColoresBox.Text = "";
                reSelectCols();
            }

        }

        private void confColsDelBtn_Clicked(object sender, RoutedEventArgs e)
        {
            if (confColoresList.SelectedIndex != -1)
            {
              //  MessageBox.Show(confColoresList.SelectedValue.ToString());
              confC.borrarColor();
              reSelectCols();
            }
        }

        public void reSelectCols()
        {
            System.Windows.Data.CollectionViewSource colsViewSource = ((System.Windows.Data.CollectionViewSource)(FindResource("coloresViewSource")));
            colsViewSource.View.MoveCurrentToFirst();
        }

        #endregion
    }
}
