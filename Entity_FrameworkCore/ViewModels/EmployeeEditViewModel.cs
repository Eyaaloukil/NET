using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Entity_FrameworkCore.ViewModels
{
    public class EmployeeEditViewModel : CreateViewModel
    {
        public int Id { get; set; }
        public string ExistingPhotoPath { get; set; }
    }
}
