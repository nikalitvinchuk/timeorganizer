using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace timeorganizer.DatabaseModels //model podzadan obecnie nie wykorzystywany
{
    [Table("TaskComponents")]
    public class TaskComponents
    {
        [PrimaryKey,AutoIncrement]
        public int Id { get; set; } // id zadania

        public string Name { get; set; }
        public string Description { get; set; }
        [ForeignKey(typeof(Task))]
        public int TaskId { get; set; }
        [ForeignKey(typeof(Users))]
        public int UserId { get; set; }
        public string Status { get; set; }
        public bool TaskComplited { get; set; }
        public bool IsActive { get; set; }

        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime LastUpdated { get; set; }


    }
}
