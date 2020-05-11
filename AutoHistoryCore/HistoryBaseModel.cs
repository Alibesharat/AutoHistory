using System;
using System.ComponentModel.DataAnnotations;

namespace AutoHistory 
{ 
    

    public class HistoryBaseModel
    {
       
       public string Hs_Change { get; set; }

        public bool IsDeleted { get; set; }
    }
}
