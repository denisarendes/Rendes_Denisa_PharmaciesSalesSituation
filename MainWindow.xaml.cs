using System;
using System.Collections.Generic;
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
using PharmaSitModel;
using System.Data.Entity;
using System.Data;

namespace Rendes_Denisa_PharmaciesSalesSituation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    enum ActionState
    { 
        New,
        Edit,
        Delete,
        Nothing
    }

    public partial class MainWindow : Window
    {
        ActionState action = ActionState.Nothing;

        //setare context
        PharmaSitEntitiesModel ctx = new PharmaSitEntitiesModel();

        CollectionViewSource pharmacyViewSource;
        CollectionViewSource productViewSource;
        CollectionViewSource clientViewSource;
        CollectionViewSource clientOrdersViewSource;

        //pt comunicare cu tabel Pharmacy
        Binding phNameTextBoxBinding = new Binding();
        Binding addressTextBoxBinding = new Binding();

        //Product
        Binding nameTextBoxBinding = new Binding();
        Binding priceTextBoxBinding = new Binding();

        //CLient
        Binding firstNameTextBoxBinding = new Binding();
        Binding lastNameTextBoxBinding = new Binding();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            //binding Pharmacy (se realizeaza binding automat - obiecte de pe form cu sursa de date)
            phNameTextBoxBinding.Path = new PropertyPath("PhName");
            phNameTextBox.SetBinding(TextBox.TextProperty, phNameTextBoxBinding);
            addressTextBoxBinding.Path = new PropertyPath("Address");
            addressTextBox.SetBinding(TextBox.TextProperty, addressTextBoxBinding);

            //binding Product
            nameTextBoxBinding.Path = new PropertyPath("Name");
            nameTextBox.SetBinding(TextBox.TextProperty, nameTextBoxBinding);
            priceTextBoxBinding.Path = new PropertyPath("Price");
            priceTextBox.SetBinding(TextBox.TextProperty, priceTextBoxBinding);

            //binding Client
            firstNameTextBoxBinding.Path = new PropertyPath("FirstName");
            firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameTextBoxBinding);
            lastNameTextBoxBinding.Path = new PropertyPath("LastName");
            lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameTextBoxBinding);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            pharmacyViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("pharmacyViewSource")));
            // se incarca datele
            pharmacyViewSource.Source = ctx.Pharmacies.Local;
            ctx.Pharmacies.Load();

            productViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("productViewSource")));
            productViewSource.Source = ctx.Products.Local;
            ctx.Products.Load();

            clientViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("clientViewSource")));
            clientViewSource.Source = ctx.Clients.Local;
            ctx.Clients.Load();

            clientOrdersViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("clientOrdersViewSource")));
            //clientOrdersViewSource.Source = ctx.Orders.Local;
            //in ordersDataGrid va fi afisat in locul id client - numele si prenume;in locul id prod - denumire; in locul id farmacie - nume
            BindOrdersDataGrid();
            ctx.Orders.Load();

            //pt cmbClient se afiseaza prenume si nume client; pt cmbProduct nume prod; pt cmbPharmacy nume farmacie
            cmbClient.ItemsSource = ctx.Clients.Local;
            //cmbClient.DisplayMemberPath = "FirstName";
            cmbClient.SelectedValuePath = "ClntId";
            cmbProduct.ItemsSource = ctx.Products.Local;
            cmbProduct.DisplayMemberPath = "Name";
            cmbProduct.SelectedValuePath = "ProdId";
            cmbPharmacy.ItemsSource = ctx.Pharmacies.Local;
            cmbPharmacy.DisplayMemberPath = "PhName";
            cmbPharmacy.SelectedValuePath = "PhmId";
        }

        //Pharmacy handlere de even butt
        private void btnNewPh_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;

            btnNewPh.IsEnabled = false;
            btnEditPh.IsEnabled = false;
            btnDeletePh.IsEnabled = false;
            btnSavePh.IsEnabled = true;
            btnCancelPh.IsEnabled = true;
            btnPrevPh.IsEnabled = false;
            btnNextPh.IsEnabled = false;
            pharmacyDataGrid.IsEnabled = false;
            phNameTextBox.IsEnabled = true;
            addressTextBox.IsEnabled = true;

            //se elimina binding-ul existent pe cele 2 textbox-uri
            BindingOperations.ClearBinding(phNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(addressTextBox, TextBox.TextProperty);
            //texbox-urile sunt goale
            phNameTextBox.Text = "";
            addressTextBox.Text = "";
            //focusul e pozitionat automat in textbox phName
            Keyboard.Focus(phNameTextBox);
        }

        private void btnEditPh_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
            //se retin valorile in textbox-uri
            string tmpPhName = phNameTextBox.Text.ToString();
            string tmpAddress = addressTextBox.Text.ToString();

            btnNewPh.IsEnabled = false;
            btnEditPh.IsEnabled = false;
            btnDeletePh.IsEnabled = false;
            btnSavePh.IsEnabled = true;
            btnCancelPh.IsEnabled = true;
            btnPrevPh.IsEnabled = false;
            btnNextPh.IsEnabled = false;
            pharmacyDataGrid.IsEnabled = false;
            phNameTextBox.IsEnabled = true;
            addressTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(phNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(addressTextBox, TextBox.TextProperty);
            //textbox-urilor le sunt atribuite val reitnute in string-uri
            phNameTextBox.Text = tmpPhName;
            addressTextBox.Text = tmpAddress;
            Keyboard.Focus(phNameTextBox);
        }

        private void btnDeletePh_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;

            string tmpPhName = phNameTextBox.Text.ToString();
            string tmpAddress = addressTextBox.Text.ToString();

            btnNewPh.IsEnabled = false;
            btnEditPh.IsEnabled = false;
            btnDeletePh.IsEnabled = false;
            btnSavePh.IsEnabled = true;
            btnCancelPh.IsEnabled = true;
            btnPrevPh.IsEnabled = false;
            btnNextPh.IsEnabled = false;
            pharmacyDataGrid.IsEnabled = false;
            phNameTextBox.IsEnabled = true;
            addressTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(phNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(addressTextBox, TextBox.TextProperty);
            phNameTextBox.Text = tmpPhName;
            addressTextBox.Text = tmpAddress;
        }

        private void btnSavePh_Click(object sender, RoutedEventArgs e)
        {
            //se creeaza un ob din clasa pharmacy
            Pharmacy pharmacy = null;

            if (action == ActionState.New)
            {
                try
                {
                    //se instantiaza entitatea pharmacy cu valorile din form
                    pharmacy = new Pharmacy()
                    {
                        PhName = phNameTextBox.Text.Trim(),
                        Address = addressTextBox.Text.Trim()
                    };
                    //se adauga entitatea creata in context
                    ctx.Pharmacies.Add(pharmacy);
                    pharmacyViewSource.View.Refresh();
                    ctx.SaveChanges();
                    MessageBox.Show("Pharmacy inserted successfully!", "Insert message");
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                btnNewPh.IsEnabled = true;
                btnEditPh.IsEnabled = true;
                btnDeletePh.IsEnabled = true;
                btnSavePh.IsEnabled = false;
                btnCancelPh.IsEnabled = false;
                btnPrevPh.IsEnabled = true;
                btnNextPh.IsEnabled = true;
                pharmacyDataGrid.IsEnabled = true;
                phNameTextBox.IsEnabled = false;
                addressTextBox.IsEnabled = false;

                phNameTextBox.SetBinding(TextBox.TextProperty, phNameTextBoxBinding);
                addressTextBox.SetBinding(TextBox.TextProperty, addressTextBoxBinding);
            }
            else if (action == ActionState.Edit)
            {
                try
                {
                    //entitatea pharmacy e cea selectata din datagrid
                    pharmacy = (Pharmacy)pharmacyDataGrid.SelectedItem;
                    //numele si adresa sunt inlocuite cu noile valori introduse 
                    pharmacy.PhName = phNameTextBox.Text.Trim();
                    pharmacy.Address = addressTextBox.Text.Trim();
                    ctx.SaveChanges();
                    MessageBox.Show("Pharmacy edited successfully!", "Edit message");
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                pharmacyViewSource.View.Refresh();
                //se pozitioneaza pe item-ul curent
                pharmacyViewSource.View.MoveCurrentTo(pharmacy);

                btnNewPh.IsEnabled = true;
                btnEditPh.IsEnabled = true;
                btnDeletePh.IsEnabled = true;
                btnSavePh.IsEnabled = false;
                btnCancelPh.IsEnabled = false;
                btnPrevPh.IsEnabled = true;
                btnNextPh.IsEnabled = true;
                pharmacyDataGrid.IsEnabled = true;
                phNameTextBox.IsEnabled = false;
                addressTextBox.IsEnabled = false;

                phNameTextBox.SetBinding(TextBox.TextProperty, phNameTextBoxBinding);
                addressTextBox.SetBinding(TextBox.TextProperty, addressTextBoxBinding);
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    pharmacy = (Pharmacy)pharmacyDataGrid.SelectedItem;
                    //farmacia selectata e eliminata 
                    ctx.Pharmacies.Remove(pharmacy);
                    ctx.SaveChanges();
                    MessageBox.Show("Pharmacy deleted successfully!", "Delete message");
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                pharmacyViewSource.View.Refresh();

                btnNewPh.IsEnabled = true;
                btnEditPh.IsEnabled = true;
                btnDeletePh.IsEnabled = true;
                btnSavePh.IsEnabled = false;
                btnCancelPh.IsEnabled = false;
                btnPrevPh.IsEnabled = true;
                btnNextPh.IsEnabled = true;
                pharmacyDataGrid.IsEnabled = true;
                phNameTextBox.IsEnabled = false;
                addressTextBox.IsEnabled = false;

                phNameTextBox.SetBinding(TextBox.TextProperty, phNameTextBoxBinding);
                addressTextBox.SetBinding(TextBox.TextProperty, addressTextBoxBinding);
            }
        }

        private void btnCancelPh_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;

            btnNewPh.IsEnabled = true;
            btnEditPh.IsEnabled = true;
            btnDeletePh.IsEnabled = true;
            btnSavePh.IsEnabled = false;
            btnCancelPh.IsEnabled = false;
            btnPrevPh.IsEnabled = true;
            btnNextPh.IsEnabled = true;
            pharmacyDataGrid.IsEnabled = true;
            phNameTextBox.IsEnabled = false;
            addressTextBox.IsEnabled = false;

            phNameTextBox.SetBinding(TextBox.TextProperty, phNameTextBoxBinding);
            addressTextBox.SetBinding(TextBox.TextProperty, addressTextBoxBinding);
        }

        private void btnPrevPh_Click(object sender, RoutedEventArgs e)
        {
            pharmacyViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNextPh_Click(object sender, RoutedEventArgs e)
        {
            pharmacyViewSource.View.MoveCurrentToNext();
        }

        //Product handlere de even butt
        private void btnNewPr_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;

            btnNewPr.IsEnabled = false;
            btnEditPr.IsEnabled = false;
            btnDeletePr.IsEnabled = false;
            btnSavePr.IsEnabled = true;
            btnCancelPr.IsEnabled = true;
            btnPrevPr.IsEnabled = false;
            btnNextPr.IsEnabled = false;
            productDataGrid.IsEnabled = false;
            nameTextBox.IsEnabled = true;
            priceTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(nameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(priceTextBox, TextBox.TextProperty);
            nameTextBox.Text = "";
            priceTextBox.Text = "";
            Keyboard.Focus(nameTextBox);
        }

        private void btnEditPr_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
            string tmpName = nameTextBox.Text.ToString();
            double tmpPrice = double.Parse(priceTextBox.Text);

            btnNewPr.IsEnabled = false;
            btnEditPr.IsEnabled = false;
            btnDeletePr.IsEnabled = false;
            btnSavePr.IsEnabled = true;
            btnCancelPr.IsEnabled = true;
            btnPrevPr.IsEnabled = false;
            btnNextPr.IsEnabled = false;
            productDataGrid.IsEnabled = false;
            nameTextBox.IsEnabled = true;
            priceTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(nameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(priceTextBox, TextBox.TextProperty);
            nameTextBox.Text = tmpName;
            priceTextBox.Text = tmpPrice.ToString();
            Keyboard.Focus(nameTextBox);
        }

        private void btnDeletePr_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;
            string tmpName = nameTextBox.Text.ToString();
            double tmpPrice = double.Parse(priceTextBox.Text);

            btnNewPr.IsEnabled = false;
            btnEditPr.IsEnabled = false;
            btnDeletePr.IsEnabled = false;
            btnSavePr.IsEnabled = true;
            btnCancelPr.IsEnabled = true;
            btnPrevPr.IsEnabled = false;
            btnNextPr.IsEnabled = false;
            productDataGrid.IsEnabled = false;
            nameTextBox.IsEnabled = true;
            priceTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(nameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(priceTextBox, TextBox.TextProperty);
            nameTextBox.Text = tmpName;
            priceTextBox.Text = tmpPrice.ToString();
        }

        private void btnSavePr_Click(object sender, RoutedEventArgs e)
        {
            Product product = null;

            if (action == ActionState.New)
            {
                try
                {
                    product = new Product()
                    {
                        Name = nameTextBox.Text.Trim(),
                        Price = double.Parse(priceTextBox.Text)
                    };
                    ctx.Products.Add(product);
                    productViewSource.View.Refresh();
                    ctx.SaveChanges();
                    MessageBox.Show("Product inserted successfully", "Insert message");
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                btnNewPr.IsEnabled = true;
                btnEditPr.IsEnabled = true;
                btnDeletePr.IsEnabled = true;
                btnSavePr.IsEnabled = false;
                btnCancelPr.IsEnabled = false;
                btnPrevPr.IsEnabled = true;
                btnNextPr.IsEnabled = true;
                productDataGrid.IsEnabled = true;
                nameTextBox.IsEnabled = false;
                priceTextBox.IsEnabled = false;

                nameTextBox.SetBinding(TextBox.TextProperty, nameTextBoxBinding);
                priceTextBox.SetBinding(TextBox.TextProperty, priceTextBoxBinding);
            }
            else if (action == ActionState.Edit)
            {
                try
                {
                    product = (Product)productDataGrid.SelectedItem;
                    product.Name = nameTextBox.Text.Trim();
                    product.Price = double.Parse(priceTextBox.Text);
                    ctx.SaveChanges();
                    MessageBox.Show("Product edited successfully", "Edit message");
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                productViewSource.View.Refresh();
                productViewSource.View.MoveCurrentTo(product);

                btnNewPr.IsEnabled = true;
                btnEditPr.IsEnabled = true;
                btnDeletePr.IsEnabled = true;
                btnSavePr.IsEnabled = false;
                btnCancelPr.IsEnabled = false;
                btnPrevPr.IsEnabled = true;
                btnNextPr.IsEnabled = true;
                productDataGrid.IsEnabled = true;
                nameTextBox.IsEnabled = false;
                priceTextBox.IsEnabled = false;

                nameTextBox.SetBinding(TextBox.TextProperty, nameTextBoxBinding);
                priceTextBox.SetBinding(TextBox.TextProperty, priceTextBoxBinding);
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    product = (Product)productDataGrid.SelectedItem;
                    ctx.Products.Remove(product);
                    ctx.SaveChanges();
                    MessageBox.Show("Product deleted successfully", "Delete message");
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                productViewSource.View.Refresh();

                btnNewPr.IsEnabled = true;
                btnEditPr.IsEnabled = true;
                btnDeletePr.IsEnabled = true;
                btnSavePr.IsEnabled = false;
                btnCancelPr.IsEnabled = false;
                btnPrevPr.IsEnabled = true;
                btnNextPr.IsEnabled = true;
                productDataGrid.IsEnabled = true;
                nameTextBox.IsEnabled = false;
                priceTextBox.IsEnabled = false;

                nameTextBox.SetBinding(TextBox.TextProperty, nameTextBoxBinding);
                priceTextBox.SetBinding(TextBox.TextProperty, priceTextBoxBinding);
            }
        }

        private void btnCancelPr_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;

            btnNewPr.IsEnabled = true;
            btnEditPr.IsEnabled = true;
            btnDeletePr.IsEnabled = true;
            btnSavePr.IsEnabled = false;
            btnCancelPr.IsEnabled = false;
            btnPrevPr.IsEnabled = true;
            btnNextPr.IsEnabled = true;
            productDataGrid.IsEnabled = true;
            nameTextBox.IsEnabled = false;
            priceTextBox.IsEnabled = false;

            nameTextBox.SetBinding(TextBox.TextProperty, nameTextBoxBinding);
            priceTextBox.SetBinding(TextBox.TextProperty, priceTextBoxBinding);
        }

        private void btnPrevPr_Click(object sender, RoutedEventArgs e)
        {
            productViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNextPr_Click(object sender, RoutedEventArgs e)
        {
            productViewSource.View.MoveCurrentToNext();
        }

        //Client handlere de even butt
        private void btnNewC_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;

            btnNewC.IsEnabled = false;
            btnEditC.IsEnabled = false;
            btnDeleteC.IsEnabled = false;
            btnSaveC.IsEnabled = true;
            btnCancelC.IsEnabled = true;
            btnPrevC.IsEnabled = false;
            btnNextC.IsEnabled = false;
            clientDataGrid.IsEnabled = false;
            firstNameTextBox.IsEnabled = true;
            lastNameTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
            firstNameTextBox.Text = "";
            lastNameTextBox.Text = "";
            Keyboard.Focus(firstNameTextBox);
        }

        private void btnEditC_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
            string tmpFirstName = firstNameTextBox.Text.ToString();
            string tmpLastName = lastNameTextBox.Text.ToString();

            btnNewC.IsEnabled = false;
            btnEditC.IsEnabled = false;
            btnDeleteC.IsEnabled = false;
            btnSaveC.IsEnabled = true;
            btnCancelC.IsEnabled = true;
            btnPrevC.IsEnabled = false;
            btnNextC.IsEnabled = false;
            clientDataGrid.IsEnabled = false;
            firstNameTextBox.IsEnabled = true;
            lastNameTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
            firstNameTextBox.Text = tmpFirstName;
            lastNameTextBox.Text = tmpLastName;
            Keyboard.Focus(firstNameTextBox);
        }

        private void btnDeleteC_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;
            string tmpFirstName = firstNameTextBox.Text.ToString();
            string tmpLastName = lastNameTextBox.Text.ToString();

            btnNewC.IsEnabled = false;
            btnEditC.IsEnabled = false;
            btnDeleteC.IsEnabled = false;
            btnSaveC.IsEnabled = true;
            btnCancelC.IsEnabled = true;
            btnPrevC.IsEnabled = false;
            btnNextC.IsEnabled = false;
            clientDataGrid.IsEnabled = false;
            firstNameTextBox.IsEnabled = true;
            lastNameTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
            firstNameTextBox.Text = tmpFirstName;
            lastNameTextBox.Text = tmpLastName;
        }

        private void btnSaveC_Click(object sender, RoutedEventArgs e)
        {
            Client client = null;
            if (action == ActionState.New)
            {
                try
                {
                    client = new Client()
                    {
                        FirstName = firstNameTextBox.Text.Trim(),
                        LastName = lastNameTextBox.Text.Trim()
                    };
                    ctx.Clients.Add(client);
                    clientViewSource.View.Refresh();
                    ctx.SaveChanges();
                    MessageBox.Show("Client inserted successfully", "Insert message");
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                btnNewC.IsEnabled = true;
                btnEditC.IsEnabled = true;
                btnDeleteC.IsEnabled = true;
                btnSaveC.IsEnabled = false;
                btnCancelC.IsEnabled = false;
                btnPrevC.IsEnabled = true;
                btnNextC.IsEnabled = true;
                clientDataGrid.IsEnabled = true;
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;

                firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameTextBoxBinding);
                lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameTextBoxBinding);
            }
            else if (action == ActionState.Edit)
            {
                try
                {
                    client = (Client)clientDataGrid.SelectedItem;
                    client.FirstName = firstNameTextBox.Text.Trim();
                    client.LastName = lastNameTextBox.Text.Trim();
                    ctx.SaveChanges();
                    MessageBox.Show("Client edited successfully", "Edit message");
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                clientViewSource.View.Refresh();
                clientViewSource.View.MoveCurrentTo(client);
                
                btnNewC.IsEnabled = true;
                btnEditC.IsEnabled = true;
                btnDeleteC.IsEnabled = true;
                btnSaveC.IsEnabled = false;
                btnCancelC.IsEnabled = false;
                btnPrevC.IsEnabled = true;
                btnNextC.IsEnabled = true;
                clientDataGrid.IsEnabled = true;
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;

                firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameTextBoxBinding);
                lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameTextBoxBinding);
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    client = (Client)clientDataGrid.SelectedItem;
                    ctx.Clients.Remove(client);
                    ctx.SaveChanges();
                    MessageBox.Show("Client deleted successfully", "Delete message");
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                clientViewSource.View.Refresh();
                
                btnNewC.IsEnabled = true;
                btnEditC.IsEnabled = true;
                btnDeleteC.IsEnabled = true;
                btnSaveC.IsEnabled = false;
                btnCancelC.IsEnabled = false;
                btnPrevC.IsEnabled = true;
                btnNextC.IsEnabled = true;
                clientDataGrid.IsEnabled = true;
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;

                firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameTextBoxBinding);
                lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameTextBoxBinding);
            }
        }

        private void btnCancelC_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;

            btnNewC.IsEnabled = true;
            btnEditC.IsEnabled = true;
            btnDeleteC.IsEnabled = true;
            btnSaveC.IsEnabled = false;
            btnCancelC.IsEnabled = false;
            btnPrevC.IsEnabled = true;
            btnNextC.IsEnabled = true;
            clientDataGrid.IsEnabled = true;
            firstNameTextBox.IsEnabled = false;
            lastNameTextBox.IsEnabled = false;

            firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameTextBoxBinding);
            lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameTextBoxBinding);
        }

        private void btnPrevC_Click(object sender, RoutedEventArgs e)
        {
            clientViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNextC_Click(object sender, RoutedEventArgs e)
        {
            clientViewSource.View.MoveCurrentToNext();
        }

        //Order handlere de even butt
        private void btnNewO_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;

            btnNewO.IsEnabled = false;
            btnEditO.IsEnabled = false;
            btnDeleteO.IsEnabled = false;
            btnSaveO.IsEnabled = true;
            btnCancelO.IsEnabled = true;
            btnPrevO.IsEnabled = true;
            btnNextO.IsEnabled = true;
            ordersDataGrid.IsEnabled = false;
            cmbClient.IsEnabled = true;
            cmbProduct.IsEnabled = true;
            cmbPharmacy.IsEnabled = true;

            cmbClient.SelectedItem = "";
            cmbProduct.SelectedItem = "";
            cmbPharmacy.SelectedItem = "";
        }

        private void btnEditO_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
            string tmpClntId = clntIdTextBox.Text.ToString();
            string tmpProdId = prodIdTextBox.Text.ToString();
            string tmpPhmId = phmIdTextBox.Text.ToString();

            btnNewO.IsEnabled = false;
            btnEditO.IsEnabled = false;
            btnDeleteO.IsEnabled = false;
            btnSaveO.IsEnabled = true;
            btnCancelO.IsEnabled = true;
            btnPrevO.IsEnabled = true;
            btnNextO.IsEnabled = true;
            ordersDataGrid.IsEnabled = false;
            cmbClient.IsEnabled = true;
            cmbProduct.IsEnabled = true;
            cmbPharmacy.IsEnabled = true;

            cmbClient.SelectedItem = tmpClntId;
            cmbProduct.SelectedItem = tmpProdId;
            cmbPharmacy.SelectedItem = tmpPhmId;
        }

        private void btnDeleteO_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;
            string tmpClntId = clntIdTextBox.Text.ToString();
            string tmpProdId = prodIdTextBox.Text.ToString();
            string tmpPhmId = phmIdTextBox.Text.ToString();

            btnNewO.IsEnabled = false;
            btnEditO.IsEnabled = false;
            btnDeleteO.IsEnabled = false;
            btnSaveO.IsEnabled = true;
            btnCancelO.IsEnabled = true;
            btnPrevO.IsEnabled = true;
            btnNextO.IsEnabled = true;
            ordersDataGrid.IsEnabled = false;
            cmbClient.IsEnabled = true;
            cmbProduct.IsEnabled = true;
            cmbPharmacy.IsEnabled = true;

            cmbClient.SelectedItem = tmpClntId;
            cmbProduct.SelectedItem = tmpProdId;
            cmbPharmacy.SelectedItem = tmpPhmId;
        }

        private void btnSaveO_Click(object sender, RoutedEventArgs e)
        {
            Order order = null;

            if (action == ActionState.New)
            {
                try
                { 
                Client client = (Client)cmbClient.SelectedItem;
                Product product = (Product)cmbProduct.SelectedItem;
                Pharmacy pharmacy = (Pharmacy)cmbPharmacy.SelectedItem;

                //instantiere entitate Order
                order = new Order()
                {
                    ClntId = client.ClntId,
                    ProdId = product.ProdId,
                    PhmId = pharmacy.PhmId
                };

                ctx.Orders.Add(order);
                //clientOrdersViewSource.View.Refresh();
                ctx.SaveChanges();
                BindOrdersDataGrid();
                MessageBox.Show("Sale inserted successfully", "Insert message");
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                btnNewO.IsEnabled = true;
                btnEditO.IsEnabled = true;
                btnDeleteO.IsEnabled = true;
                btnSaveO.IsEnabled = false;
                btnCancelO.IsEnabled = false;
                btnNextO.IsEnabled = true;
                btnPrevO.IsEnabled = true;
                ordersDataGrid.IsEnabled = true;
                cmbClient.IsEnabled = false;
                cmbProduct.IsEnabled = false;
                cmbPharmacy.IsEnabled = false;
            }
            else if (action == ActionState.Edit)
            {
                //in selectedOrder e retinuta entitatea Order selectata din datagrid
                dynamic selOrder = ordersDataGrid.SelectedItem;

                try
                {
                    /*
                    Client client = (Client)cmbClient.SelectedItem;
                    Product product = (Product)cmbProduct.SelectedItem;
                    Pharmacy pharmacy = (Pharmacy)cmbPharmacy.SelectedItem;

                    order = (Order)ordersDataGrid.SelectedItem;
                    order.ClntId = client.ClntId;
                    order.ProdId = product.ProdId;
                    order.PhmId = pharmacy.PhmId;

                    ctx.SaveChanges();
                    MessageBox.Show("Sale edited successfully", "Edit message");
                    */

                    //var curentId retine id comanda curenta
                    int curentId = selOrder.OrdId;
                    //se cauta comanda care are id = curentId si se retine in editOrder
                    var editOrder = ctx.Orders.FirstOrDefault(ord => ord.OrdId == curentId);
                    //daca se gaseste
                    if (editOrder != null)
                    {
                        editOrder.ClntId = int.Parse(cmbClient.SelectedValue.ToString());
                        editOrder.ProdId = int.Parse(cmbProduct.SelectedValue.ToString());
                        editOrder.PhmId = int.Parse(cmbPharmacy.SelectedValue.ToString());

                        ctx.SaveChanges();
                        MessageBox.Show("Sale edited successfully", "Edit message");
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                /*
                clientOrdersViewSource.View.Refresh();
                clientOrdersViewSource.View.MoveCurrentTo(order);
                */

                BindOrdersDataGrid();
                clientOrdersViewSource.View.MoveCurrentTo(selOrder);

                btnNewO.IsEnabled = true;
                btnEditO.IsEnabled = true;
                btnDeleteO.IsEnabled = true;
                btnSaveO.IsEnabled = false;
                btnCancelO.IsEnabled = false;
                btnNextO.IsEnabled = true;
                btnPrevO.IsEnabled = true;
                ordersDataGrid.IsEnabled = true;
                cmbClient.IsEnabled = false;
                cmbProduct.IsEnabled = false;
                cmbPharmacy.IsEnabled = false;
            }
            else if (action == ActionState.Delete) 
            {
                try
                {
                    /*
                    order = (Order)ordersDataGrid.SelectedItem;
                    ctx.Orders.Remove(order);
                    ctx.SaveChanges();
                    MessageBox.Show("Sale deleted successfully", "Delete message");
                    */

                    dynamic selOrder = ordersDataGrid.SelectedItem;
                    int curentId = selOrder.OrdId;
                    var delOrder = ctx.Orders.FirstOrDefault(ord => ord.OrdId == curentId);
                    if (delOrder != null)
                    {
                        ctx.Orders.Remove(delOrder);
                        ctx.SaveChanges();
                        BindOrdersDataGrid();
                        MessageBox.Show("Sale deleted successfully", "Delete message");
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                //clientOrdersViewSource.View.Refresh();

                btnNewO.IsEnabled = true;
                btnEditO.IsEnabled = true;
                btnDeleteO.IsEnabled = true;
                btnSaveO.IsEnabled = false;
                btnCancelO.IsEnabled = false;
                btnNextO.IsEnabled = true;
                btnPrevO.IsEnabled = true;
                ordersDataGrid.IsEnabled = true;
                cmbClient.IsEnabled = false;
                cmbProduct.IsEnabled = false;
                cmbPharmacy.IsEnabled = false;
            }
        }

        private void btnCancelO_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;

            btnNewO.IsEnabled = true;
            btnEditO.IsEnabled = true;
            btnDeleteO.IsEnabled = true;
            btnSaveO.IsEnabled = false;
            btnCancelO.IsEnabled = false;
            btnNextO.IsEnabled = true;
            btnPrevO.IsEnabled = true;
            ordersDataGrid.IsEnabled = true;
            cmbClient.IsEnabled = false;
            cmbProduct.IsEnabled = false;
            cmbPharmacy.IsEnabled = false;
        }

        private void btnPrevO_Click(object sender, RoutedEventArgs e)
        {
            clientOrdersViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNextO_Click(object sender, RoutedEventArgs e)
        {
            clientOrdersViewSource.View.MoveCurrentToNext();
        }

        //metoda inlocuieste afisarea datelor incarcate direct din context cu cele rezultate din interogare
        private void BindOrdersDataGrid()
        {
            var queryOrd = from ord in ctx.Orders
                           join clnt in ctx.Clients on ord.ClntId equals clnt.ClntId
                           join prod in ctx.Products on ord.ProdId equals prod.ProdId
                           join phm in ctx.Pharmacies on ord.PhmId equals phm.PhmId
                           select new
                           {
                               ord.OrdId,
                               ord.ClntId,
                               ord.ProdId,
                               ord.PhmId,
                               clnt.FirstName,
                               clnt.LastName,
                               prod.Name,
                               phm.PhName
                           };
            clientOrdersViewSource.Source = queryOrd.ToList();
        }
    }
}
