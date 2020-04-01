using System;
using System.Collections.Generic;
using System.Linq;
using GitStat.Core.Contracts;
using GitStat.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace GitStat.Persistence
{
    public class CommitRepository : ICommitRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CommitRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void AddRange(Commit[] commits)
        {
            _dbContext.Commits.AddRange(commits);
        }

        public IEnumerable<Commit> GetAllCommits()
        {
            return _dbContext.Commits.Include(d => d.Developer).ToList();
        }

        public Commit GetCommitById(int id)
        {
            return _dbContext.Commits.Find(id);
        }
    }
}