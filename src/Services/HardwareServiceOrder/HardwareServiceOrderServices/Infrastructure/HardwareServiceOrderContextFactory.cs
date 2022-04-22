﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareServiceOrderServices.Infrastructure
{
    public class HardwareServiceOrderContextFactory : IDesignTimeDbContextFactory<HardwareServiceOrderContext>
    {
        public HardwareServiceOrderContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                                   .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                                                   .AddJsonFile("secrets/appsettings.secrets.json", optional: true)
                                                   .AddEnvironmentVariables()
                                                   .AddUserSecrets<HardwareServiceOrderContext>(optional: true)
                                                   .Build();

            var optionsBuilder = new DbContextOptionsBuilder<HardwareServiceOrderContext>();
            string connectionString = config.GetConnectionString("HardwareServiceOrderConnectionString");

            optionsBuilder.UseSqlServer(connectionString);

            return new HardwareServiceOrderContext(optionsBuilder.Options);
        }
    }
}