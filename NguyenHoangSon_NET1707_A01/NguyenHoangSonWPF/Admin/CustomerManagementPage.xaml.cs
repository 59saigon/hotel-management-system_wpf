﻿using BusinessObject.Models;
using BusinessObject.Shared;
using BusinessObject.Views;
using DataAccess.IRepositories;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NguyenHoangSonWPF.Admin.AdminDialog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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

namespace NguyenHoangSonWPF.Admin
{
    /// <summary>
    /// Interaction logic for CustomerManagementPage.xaml
    /// </summary>
    public partial class CustomerManagementPage : Page
    {
        private readonly ICustomerRepository customerRepository;

        public CustomerManagementPage(ICustomerRepository _customerRepository)
        {
            InitializeComponent();
            this.customerRepository = _customerRepository;
            this.listView.SelectionChanged += ListView_SelectionChanged;
        }

        #region Main
        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetButtonEnabled(listView.SelectedItems.Count > 0);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            listView.ItemsSource = GetListView();
            ClearFieldsExisting();
        }

        public void RefreshListView()
        {
            listView.ItemsSource = GetListView();
            ClearFieldsExisting();
        }

        private void Button_Reload(object sender, RoutedEventArgs e)
        {
            listView.ItemsSource = GetListView();
            // clear
            ClearFieldsExisting();
        }

        private IEnumerable<CustomerView> GetListView()
        {
            List<CustomerView> views = new List<CustomerView>();
            foreach (var item in customerRepository.GetAll())
            {
                if (item.CustomerStatus != Convert.ToByte(2))
                {
                    views.Add(ConvertModelToView(item));
                }
            }

            return views;
        }

        private void Button_Search(object sender, RoutedEventArgs e)
        {
            CustomerView customerViewFilter = GetCustomerViewFilter();
            IEnumerable<Customer> models = new List<Customer>();
            if (IsCustomerViewFilterValid(customerViewFilter))
            {
                models = customerRepository.GetAllByFilter(customerViewFilter);
            }
            else
            {
                models = customerRepository.GetAllByFilter(null);
            }

            List<CustomerView> views = new List<CustomerView>();

            foreach (var model in models)
            {
                views.Add(ConvertModelToView((Customer)model));
            }

            listView.ItemsSource = views;
        }

        private CustomerView GetCustomerViewFilter()
        {
            return new CustomerView()
            {
                CustomerId = !string.IsNullOrEmpty(searchById.Text) ? (int?)int.Parse(searchById.Text) : null,
                EmailAddress = !string.IsNullOrEmpty(searchByEmail.Text) ? searchByEmail.Text : null,
                CustomerFullName = !string.IsNullOrEmpty(searchByFullName.Text) ? searchByFullName.Text : null,
                Telephone = !string.IsNullOrEmpty(searchByTelephone.Text) ? searchByTelephone.Text : null,
                CustomerBirthday = searchByBirthday.SelectedDate.HasValue ? searchByBirthday.SelectedDate.Value : (DateTime?)null,
            };
        }

        private bool IsCustomerViewFilterValid(CustomerView customerViewFilter)
        {
            return customerViewFilter.CustomerId.HasValue ||
                   !string.IsNullOrEmpty(customerViewFilter.EmailAddress) ||
                   !string.IsNullOrEmpty(customerViewFilter.CustomerFullName) ||
                   !string.IsNullOrEmpty(customerViewFilter.Telephone) ||
                   customerViewFilter.CustomerBirthday.HasValue;
        }


        private void Button_Edit(object sender, RoutedEventArgs e)
        {
            int count = listView.SelectedItems.Count;
            if (count > 0)
            {
                List<CustomerView> customerViews = listView.SelectedItems.Cast<CustomerView>().ToList();
                customerViews.ForEach(customerView =>
                {
                    CustomerCreateOrUpdateDialog dialog = new CustomerCreateOrUpdateDialog(customerRepository, this, customerRepository.GetById(Convert.ToInt32(customerView.CustomerId)));

                    dialog.Show();
                });
            }
            else
            {
                MessageBox.Show("Please select customer");
            }
        }

        private void Button_Delete(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Do you wan't remove customer seledted?", "Remove customer", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                List<CustomerView> views = listView.SelectedItems.Cast<CustomerView>().ToList();
                views.ForEach(view => customerRepository.Update(UpdateCustomerStatusDeleted((int)view.CustomerId)));

                RefreshListView();
            }
        }

        private void Button_OpenCreate(object sender, RoutedEventArgs e)
        {
            CustomerCreateOrUpdateDialog dialog = new CustomerCreateOrUpdateDialog(customerRepository, this, null);

            dialog.Show();
        }
        #endregion

        #region Mapping View, Model + Get ViewFilter 
        public CustomerView ConvertModelToView(Customer model)
        {
            return new CustomerView()
            {
                CustomerId = model.CustomerId,
                CustomerFullName = model.CustomerFullName,
                Telephone = model.Telephone,
                EmailAddress = model.EmailAddress,
                CustomerBirthday = model.CustomerBirthday,
                CustomerStatus = ShareService.SetStatus((byte)model.CustomerStatus),
                Password = model.Password,
            };
        }

        public Customer UpdateCustomerStatusDeleted(int customerId)
        {
            Customer c = customerRepository.GetById(Convert.ToInt32(customerId));
            c.CustomerStatus = Convert.ToByte(2);

            return c;
        }


        #endregion

        #region other func
        private void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void SetButtonEnabled(bool enabled)
        {
            btnEdit.IsEnabled = enabled;
            btnDelete.IsEnabled = enabled;
        }

        private void ClearFieldsExisting()
        {
            searchById.Clear();
            searchByEmail.Clear();
            searchByFullName.Clear();
            searchByTelephone.Clear();
            searchByBirthday.SelectedDate = null;
        }
        #endregion
    }
}
