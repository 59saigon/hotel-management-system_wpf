﻿using BusinessObject.Models;
using BusinessObject.Shared;
using DataAccess.IRepositories;
using Microsoft.Extensions.Configuration;
using NguyenHoangSonWPF.Admin;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NguyenHoangSonWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ICustomerRepository customerRepository;
        private readonly IBookingRepository bookingRepository;
        private readonly IBookingDetailRepository bookingDetailRepository;
        private readonly IRoomRepository roomRepository;
        private readonly IRoomTypeRepository roomTypeRepository;

        public MainWindow(ICustomerRepository _customerRepository,
            IBookingRepository _bookingRepository,
            IRoomRepository _roomRepository,
            IRoomTypeRepository _roomTypeRepository,
            IBookingDetailRepository _bookingDetailRepository
            )
        {
            InitializeComponent();
            this.bookingDetailRepository = _bookingDetailRepository;
            this.customerRepository = _customerRepository;
            this.bookingRepository = _bookingRepository;
            this.roomRepository = _roomRepository;
            this.roomTypeRepository = _roomTypeRepository;
        }

        public void resetFormLogin()
        {
            txtBoxUsername.Text = null;
            pwdBoxPassword.Password = null;
        }

        private void OnLogin(object sender, RoutedEventArgs e)
        {
            string email = txtBoxUsername.Text.ToString();
            string password = pwdBoxPassword.Password.ToString();
            var account = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("account");
            if (!String.IsNullOrEmpty(email) && !String.IsNullOrEmpty(password))
            {
                if (ShareService.IsValid(email))
                {
                    Customer customer = customerRepository.FindByEmailAndPassword(email, password);
                    if (email.Equals(account["email"]) && password.Equals(account["password"]))
                    {
                        Session.Username = email;
                        Session.Role = "Admin";
                        this.Hide();
                        AdminWindow adminWindow = new AdminWindow(this, customerRepository, bookingRepository, bookingDetailRepository, roomRepository, roomTypeRepository);
                        adminWindow.Show();
                        resetFormLogin();
                    }
                    else if (customer != null)
                    {
                        Session.Username = email;
                        Session.Role = "Customer";
                        this.Hide();
                        Home home = new Home(this, customerRepository, bookingRepository, roomRepository, bookingDetailRepository, customer);
                        home.Show();
                        resetFormLogin();
                    }
                    else
                    {
                        MessageBox.Show("Wrong email or password!");
                    }
                }
                else
                {
                    MessageBox.Show("Your email is not format input!");
                }
            }
            else
            {
                MessageBox.Show("Please enter email and password!");
            }
        }
    }
}