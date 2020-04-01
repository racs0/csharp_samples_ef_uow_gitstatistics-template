using System;
using System.IO;
using System.Linq;
using System.Text;
using GitStat.Core.Contracts;
using GitStat.Persistence;

namespace GitStat.ImportConsole
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Import der Commits in die Datenbank");
            using (IUnitOfWork unitOfWorkImport = new UnitOfWork())
            {
                Console.WriteLine("Datenbank löschen");
                unitOfWorkImport.DeleteDatabase();
                Console.WriteLine("Datenbank migrieren");
                unitOfWorkImport.MigrateDatabase();
                Console.WriteLine("Commits werden von commits.txt eingelesen");
                var commits = ImportController.ReadFromCsv();
                if (commits.Length == 0)
                {
                    Console.WriteLine("!!! Es wurden keine Commits eingelesen");
                    return;
                }
                Console.WriteLine(
                    $"  Es wurden {commits.Count()} Commits eingelesen, werden in Datenbank gespeichert ...");
                unitOfWorkImport.CommitRepository.AddRange(commits);
                int countDevelopers = commits.GroupBy(c => c.Developer).Count();
                int savedRows = unitOfWorkImport.SaveChanges();
                Console.WriteLine(
                    $"{countDevelopers} Developers und {savedRows - countDevelopers} Commits wurden in Datenbank gespeichert!");
                Console.WriteLine();
                var csvCommits = commits.Select(c =>
                    $"{c.Developer.Name};{c.Date};{c.Message};{c.HashCode};{c.FilesChanges};{c.Insertions};{c.Deletions}");
                File.WriteAllLines("commits.csv", csvCommits, Encoding.UTF8);
            }
            Console.WriteLine("Datenbankabfragen");
            Console.WriteLine("=================");
            using (IUnitOfWork unitOfWork = new UnitOfWork())
            {
                var commits = unitOfWork.CommitRepository.GetAllCommits();
                var commitsWithSums = commits
                    .GroupBy(d => d.Developer.Name)
                    .Select(g => new
                    {
                        Name = g.Key,
                        Commit = g.Select(c => c.Developer.Commits).Count(),
                        FilesChanges = g.Sum(fc => fc.FilesChanges),
                        Insertions = g.Sum(i => i.Insertions),
                        Deletions = g.Sum(d => d.Deletions)
                    }).OrderByDescending(o => o.Commit).ToList();

                Console.WriteLine("");
                Console.WriteLine("Statistik der Commits der Developer");
                Console.WriteLine("-----------------------------------");

                foreach (var item in commitsWithSums)
                {
                    Console.WriteLine("Developer: {0} | Commits: {1} | FileChanges: {2} | Insertions: {3} | Deletions: {4}",item.Name,item.Commit,item.FilesChanges,item.Insertions,item.Deletions);
                }


                Console.WriteLine("");
                Console.WriteLine("Commit mit Id 4");
                Console.WriteLine("-----------------------------------");
                var commitByID = unitOfWork.CommitRepository.GetCommitById(4);

                Console.WriteLine("Developer: {0} | Date: {1} | FileChanges: {2} | Insertions: {3} | Deletions: {4}",commitByID.Developer.Name, commitByID.Date, commitByID.FilesChanges,commitByID.Insertions,commitByID.Deletions);

                Console.WriteLine("");
                Console.WriteLine("Commits der letzten 4 Wochen");
                Console.WriteLine("-----------------------------------");
                DateTime help = commits.OrderBy(s => s.Date).Last().Date.AddDays(-28);
                var lastCommits = commits.Where(c => c.Date >= help).OrderBy(s => s.Date);

                foreach (var item in lastCommits)
                {
                    Console.WriteLine("Developer: {0} | Date: {1} | FileChanges: {2} | Insertions: {3} | Deletions: {4}", item.Developer.Name, item.Date, item.FilesChanges, item.Insertions, item.Deletions);
                }
            }
            Console.Write("Beenden mit Eingabetaste ...");
            Console.ReadLine();
        }


    }
}
