﻿using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext _db;

        public BookRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Book book)
        {
            _db.Books.Add(book);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id, string userId)
        {
            var book = _db.Books.FirstOrDefault(x => x.Id == id);
            if (book != null)
            {
                book.WhoDeleted = userId;
                book.Deleted = true;
                book.Active = false;
                book.DateDeleted = DateTime.Now;
                await _db.SaveChangesAsync();
            }
        }

        public async Task<Book> FindByIdAsync(int id)
        {
            var book = await _db.Books.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return book;
        }

        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            var listBooks = _db.Books.Where(x => x.Active && !x.Deleted).AsNoTracking().AsEnumerable();
            return listBooks ?? Enumerable.Empty<Book>();
        }
    }
}
