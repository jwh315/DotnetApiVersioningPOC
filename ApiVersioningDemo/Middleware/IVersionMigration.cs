using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ApiVersioningDemo.Middleware
{
    public interface IVersionMigration
    {
        public void Up(IVersionMigrationHelper versionMigrationHelper);

        public void Down(IVersionMigrationHelper versionMigrationHelper);

        public Version GetVersionTag();
    }
}