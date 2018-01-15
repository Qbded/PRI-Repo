using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.NetworkInformation;

namespace PRI_KATALOGOWANIE_PLIKÓW.popup_forms
{
    public partial class UserDetermineAddressForm : Form
    {
        #region Zmienne globalne

        public IPAddress resultRef;
        private Tuple<string,IPAddress> global_IP_address_container;
        private List<Tuple<string,IPAddress>> local_IP_addresses_container;
        private IPAddress selected_address;

        #endregion

        #region Konstruktor

        public UserDetermineAddressForm(ref IPAddress result)
        {
            this.resultRef = result;

            IP_addresses_grab();
            InitializeComponent();

            LV_detected_addresses.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        #endregion

        #region Funkcje pomocnicze

        private List<Tuple<String,IPAddress>> Names_and_IPs_from_adapters_get()
        {
            List<Tuple<String, IPAddress>> result = new List<Tuple<string, IPAddress>>();

            var grabbed_values = from interface_grabbed in NetworkInterface.GetAllNetworkInterfaces()
                                 where interface_grabbed.OperationalStatus == OperationalStatus.Up
                                 select new { interface_name = interface_grabbed.Name, ip = IP_from_unicast_get(interface_grabbed) };

            foreach (var entity in grabbed_values)
            {
                result.Add(new Tuple<String, IPAddress>(entity.interface_name, entity.ip));
            }

            return result;
        }

        private IPAddress IP_from_unicast_get(NetworkInterface passed_interface)
        {
            return (from ip in passed_interface.GetIPProperties().UnicastAddresses
                    where ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork
                    select ip.Address).SingleOrDefault();
        }

        private void IP_addresses_grab()
        {
            try
            {
                // get host IP addresses
                local_IP_addresses_container = Names_and_IPs_from_adapters_get();
                global_IP_address_container = new Tuple<string, IPAddress>("Adres IP zewnętrzny", IPAddress.Parse(new WebClient().DownloadString("http://icanhazip.com").TrimEnd('\n')));
            }
            catch
            {
                MessageBox.Show("Błąd podczas pobierania adresów IP");
                LV_detected_addresses.Enabled = false;
            }
        }

        #endregion

        #region Logika okna

        private void UserDetermineAddressForm_VisibleChanged(object sender, EventArgs e)
        {
            // Wypełniamy listę załadowanymi adresami.
            foreach(Tuple<string,IPAddress> pair in local_IP_addresses_container)
            {
                ListViewItem item_to_add = new ListViewItem();
                item_to_add.Text = pair.Item1;
                item_to_add.SubItems.Add(pair.Item2.ToString());
                //item
                LV_detected_addresses.Items.Add(item_to_add);
            }

            ListViewItem global_to_add = new ListViewItem();
            global_to_add.Text = global_IP_address_container.Item1;
            global_to_add.SubItems.Add(global_IP_address_container.Item2.ToString());
            LV_detected_addresses.Items.Add(global_to_add);
        }

        private void BT_continue_Click(object sender, EventArgs e)
        {
            if(selected_address == null)
            {
                MessageBox.Show("Nie wybrano poprawnego adresu IP!");
            } 
            else
            {
                resultRef = selected_address;
                this.Close();
            }
        }

        private void BT_manual_input_Click(object sender, EventArgs e)
        {
            IPAddressInputForm ipAddressInputForm = new IPAddressInputForm(ref selected_address);
            ipAddressInputForm.Owner = this;
            ipAddressInputForm.ShowDialog();
            selected_address = ipAddressInputForm.resultRef;

            if (selected_address == null)
            {
                MessageBox.Show("Format podanego adresu IP nie może zostać uznany za poprawny!");
            }
            else
            {
                MessageBox.Show("Wpisano ręcznie adres IP: " + selected_address.ToString());
            }
        }

        private void LV_detected_addres_ItemActivate(object sender, EventArgs e)
        {
            ListViewItem selected_item = LV_detected_addresses.SelectedItems[0];
            selected_address = IPAddress.Parse(selected_item.SubItems[1].Text);
            MessageBox.Show("Wybrano adapter: " + selected_item.Text + '\n' +
                            "O adresie IP: " + selected_item.SubItems[1].Text + '\n');
        }

        #endregion
    }
}
