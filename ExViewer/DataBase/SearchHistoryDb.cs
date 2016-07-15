﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Windows.Foundation;
using static System.Runtime.InteropServices.WindowsRuntime.AsyncInfo;


namespace ExViewer.DataBase
{
    class SearchHistoryDb : DbContext
    {
        private const string dbFilename = "SearchHistory.db";
        private static object syncroot = new object();
        private static bool created = false;

        public static SearchHistoryDb Create()
        {
            var db = new SearchHistoryDb();
            if(!created)
                lock(syncroot)
                    if(!created)
                    {
                        db.Database.EnsureCreated();
                        created = true;
                    }
            return db;
        }

        public static void Delete()
        {
            if(created)
                lock(syncroot)
                    if(created)
                    {
                        new SearchHistoryDb().Database.EnsureDeleted();
                        created = false;
                    }
        }

        public static IAsyncAction DeleteAsync()
        {
            return Task.Run((Action)Delete).AsAsyncAction();
        }

        protected SearchHistoryDb()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename={dbFilename}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SearchHistory>().HasKey(sh => sh.Time);
            modelBuilder.Entity<SearchHistory>().Ignore(sh => sh.Highlight);
        }

        public DbSet<SearchHistory> SearchHistorySet
        {
            get;
            set;
        }
    }

    class SearchHistory : IEquatable<SearchHistory>
    {
        public string Content
        {
            get; set;
        }

        public string Highlight
        {
            get; private set;
        }

        public SearchHistory SetHighlight(string highlight)
        {
            Highlight = highlight;
            return this;
        }

        public DateTimeOffset Time
        {
            get; set;
        }

        public static SearchHistory Create(string content)
        {
            return new SearchHistory
            {
                Content = (content ?? string.Empty).Trim(),
                Time = DateTimeOffset.UtcNow
            };
        }

        public bool Equals(SearchHistory other)
        {
            return this.Content == other.Content;
        }

        public override bool Equals(object obj)
        {
            if(obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            return Equals((SearchHistory)obj);
        }

        public override int GetHashCode()
        {
            return Content.GetHashCode();
        }

        public override string ToString()
        {
            return Content;
        }
    }
}