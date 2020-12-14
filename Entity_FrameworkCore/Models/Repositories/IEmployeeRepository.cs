using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Entity_FrameworkCore.Models.Repositories
{
   public interface IEmployeeRepository
    {
        Employee GetEmployee(int Id);
        IEnumerable<Employee> GetAllEmployee();
        Employee Add(Employee employee);
        Employee Update(Employee employeeChanges);
        Employee Delete(int Id);

    }
}
