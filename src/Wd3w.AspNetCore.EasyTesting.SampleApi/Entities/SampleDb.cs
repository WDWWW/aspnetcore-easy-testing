using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Wd3w.AspNetCore.EasyTesting.SampleApi.Entities
{
    public class SampleDb : DbContext
    {
        public SampleDb(DbContextOptions<SampleDb> options) : base(options)
        {
        }

        public DbSet<SampleDataEntity> SampleDataEntities { get; set; }
    }

    public class SampleDataEntity
    {
        [Key]
        public long Id { get; set; }

        public string Data { get; set; }
    }
}