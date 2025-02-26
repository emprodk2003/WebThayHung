using Application.SeedWorks;
using AutoMapper;
using Data.EF;
using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.SeedWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly WebDbContext _context;
        public UnitOfWork(WebDbContext context, IMapper mapper)
        {
            _context = context;


        }
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
