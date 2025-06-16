using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Numerics;
using chatlib;
using api.Models;


namespace api.Models
{

    public class AplicationDBContext : DbContext
    {
        public AplicationDBContext() : base("name=ChatAppDB")
        {
        }

        public DbSet<UserModel> Users { get; set; }
        public DbSet<ValidationSessionModel> ValidationSessions { get; set; }

    }
}