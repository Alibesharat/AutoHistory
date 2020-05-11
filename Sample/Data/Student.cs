using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sample.Data
{
    public class Student  : AutoHistory.HistoryBaseModel
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
