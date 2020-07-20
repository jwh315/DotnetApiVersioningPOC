using System;
using System.Threading.Tasks;
using ApiVersioningDemo.Middleware;
using Microsoft.AspNetCore.Http;

namespace ApiVersioningDemo.ApiVersionMigrations
{
    public class AddUserTypeEnum : IVersionMigration
    {
        public void Up(IVersionMigrationHelper migrationHelper)
        {
        }

        public void Down(IVersionMigrationHelper migrationHelper)
        {
        }

        public Version GetVersionTag()
        {
            return Version.Parse("1.0.2");
        }
    }
}