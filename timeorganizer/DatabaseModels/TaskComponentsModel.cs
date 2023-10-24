using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace timeorganizer.DatabaseModels //model podzadan obecnie nie wykorzystywany
{
    internal class TaskComponentsModel
    {
        [PrimaryKey]
        public int Id { get; set; } // id zadania

        public string Name { get; set; }
        public string Description { get; set; }
        public int TaskId { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; }
        public bool TaskComplited { get; set; }
        public bool IsActive { get; set; }

        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime LastUpdated { get; set; }


    }
}
