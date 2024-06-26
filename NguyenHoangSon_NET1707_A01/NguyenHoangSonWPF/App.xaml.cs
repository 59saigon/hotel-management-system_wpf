﻿using DataAccess.IRepositories;
using DataAccess.Repositories;
using Microsoft.Extensions.DependencyInjection;
using NguyenHoangSonWPF.Admin;
using NguyenHoangSonWPF.Admin.AdminDialog;
using NguyenHoangSonWPF.Customers;
using System.Configuration;
using System.Data;
using System.Windows;

namespace NguyenHoangSonWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider serviceProvider;

        public App()
        {
            ServiceCollection services = new ServiceCollection();

            ConfigureServices(services);
            serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            PageServices(services);

            WindowServices(services);

            RepositoryServices(services);
        }

        private void PageServices(ServiceCollection services)
        {
            services.AddSingleton<CustomerManagementPage>();
            services.AddSingleton<RoomManagementPage>();
            services.AddSingleton<BookingManagementPage>();
        }

        private void WindowServices(ServiceCollection services)
        {
            services.AddSingleton<Home>();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<AdminWindow>();
            services.AddSingleton<BookingDetailManagementWindow>();
            services.AddSingleton<HistoryBookingDetailWindow>();
            services.AddSingleton<HistoryBookingWindow>();
            services.AddSingleton<BookingCreateOrUpdateDialog>();
            services.AddSingleton<CustomerCreateOrUpdateDialog>();
            services.AddSingleton<RoomCreateOrUpdateDialog>();
            services.AddSingleton<CartWindow>();
        }

        private void RepositoryServices(ServiceCollection services)
        {
            services.AddScoped<IRoomTypeRepository, RoomTypeRepository>();
            services.AddScoped<IRoomRepository, RoomRepository>();
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IBookingDetailRepository, BookingDetailRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            var mainWindow = serviceProvider.GetService<MainWindow>();
            if (mainWindow != null)
            {
                mainWindow.Show();
            }
        }
    }

}
