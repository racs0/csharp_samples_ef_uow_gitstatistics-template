﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using GitStat.Core.Entities;

namespace GitStat.Core.Contracts
{
    public interface ICommitRepository
    {
        void AddRange(Commit[] commits);
        IEnumerable<Commit> GetAllCommits();
        Commit GetCommitById(int id);
    }
}
