﻿using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace timeorganizer.DatabaseModels
{
    internal class TaskModel
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Name { get; set; } //nazwa zadania
        public string Description { get; set; } //opis zadania
        public string Type { get; set; } //typ zadania
        public int UserId { get; set; } //użytkownik tworzacy
        public string status { get; set; }
        public int RealizedPercent { get; set; } //procent realizacji obliczany na podstawie liczby podzadań/liczba zralizowanych
        public DateTime Created { get; set; } = DateTime.Now; //data utworzenia
        public DateTime Updated { get; set; } //data aktualizacji 




    }
}
