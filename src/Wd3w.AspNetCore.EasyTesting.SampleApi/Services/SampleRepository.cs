using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Wd3w.AspNetCore.EasyTesting.SampleApi.Entities;

namespace Wd3w.AspNetCore.EasyTesting.SampleApi.Services
{
    public class SampleRepository
    {
        private readonly SampleDb _db;

        public SampleRepository(SampleDb db)
        {
            _db = db;
        }

        public async Task<IEnumerable<SampleDataEntity>> GetSamplesAsync()
        {
            return await _db.SampleDataEntities.ToListAsync();
        }
    }
}