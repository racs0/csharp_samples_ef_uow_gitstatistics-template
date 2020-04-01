using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GitStat.Core.Entities;
using Utils;

namespace GitStat.ImportConsole
{
    public class ImportController
    {
        const string Filename = "commits.csv";

        /// <summary>
        /// Liefert die Messwerte mit den dazugehörigen Sensoren
        /// </summary>
        public static Commit[] ReadFromCsv()
        {
            string[] lines = File.ReadAllLines(MyFile.GetFullNameInApplicationTree(Filename));
            Dictionary<string, Developer> developers = new Dictionary<string, Developer>();
            foreach (string line in lines)
            {
                string[] parts = line.Split(';');
                string name = parts[0];

                if (!developers.ContainsKey(name))
                {
                    Developer developer = new Developer(name);
                    developers.Add(name, developer);
                }
            }


            return lines
                  .Select(line =>
                  {
                      string[] parts = line.Split(';');
                      string name = parts[0];
                      DateTime date = DateTime.Parse(parts[1]);
                      string message = parts[2];
                      string hashcode = parts[3];
                      int filesChanges = int.Parse(parts[4]);
                      int insertions = int.Parse(parts[5]);
                      int deletions = int.Parse(parts[6]);

                      if (developers.ContainsKey(name))
                      {
                          return new Commit() { Developer = developers[name], Date = date, Message = message, HashCode = hashcode, FilesChanges = filesChanges, Insertions = insertions, Deletions = deletions };
                      }
                      else
                      {
                          return null;
                      }
                  }).ToArray();
        }

    }
}
