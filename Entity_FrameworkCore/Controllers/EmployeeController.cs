using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Entity_FrameworkCore.Models;
using Entity_FrameworkCore.Models.Repositories;
using Entity_FrameworkCore.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Entity_FrameworkCore.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IWebHostEnvironment hostingEnvironment;

        public EmployeeController(IEmployeeRepository employeeRepository, IWebHostEnvironment hostingEnvironment)
        {
            _employeeRepository = employeeRepository;
            this.hostingEnvironment = hostingEnvironment;

        }
        // GET: Employee
        public ActionResult Index()
        {
            var model = _employeeRepository.GetAllEmployee();
            return View(model);

        }

        // GET: Employee/Details/5
        public ActionResult Details(int id)
        {
            var emp = _employeeRepository.GetEmployee(id);
            return View(emp);
        }

        // GET: Employee/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;

                // If the Photo property on the incoming model object is not null, then the user
                // has selected an image to upload.
                if (model.Photo != null)
                {
                    // The image must be uploaded to the images folder in wwwroot
                    // To get the path of the wwwroot folder we are using the inject
                    // HostingEnvironment service provided by ASP.NET Core

                    string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");

                    // To make sure the file name is unique we are appending a new
                    // GUID value and an underscore to the file name

                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Use CopyTo() method provided by IFormFile interface to
                    // copy the file to wwwroot/images folder

                    model.Photo.CopyTo(new FileStream(filePath, FileMode.Create));
                }

                Employee newEmployee = new Employee
                {
                    Name = model.Name,
                    Salary = model.Salary,
                    Departement = model.Department,
                    // Store the file name in PhotoPath property of the employee object
                    // which gets saved to the Employees database table
                    PhotoPath = uniqueFileName
                };

                _employeeRepository.Add(newEmployee);
                return RedirectToAction("details", new { id = newEmployee.Id });
            }

            return View();

        }

        // GET: Employee/Edit/5
        public ActionResult Edit(int id)
        {
            //Employee emp = _employeeRepository.GetEmployee(id);
            //if (emp == null)
            //    return NotFound();
            //else
            //return View(emp);

            Employee employee = _employeeRepository.GetEmployee(id);
            EmployeeEditViewModel employeeEditViewModel = new EmployeeEditViewModel
            {
                Id = employee.Id,
                Name = employee.Name,
                Salary = employee.Salary,
                Department = employee.Departement,
                ExistingPhotoPath = employee.PhotoPath
            };
            return View(employeeEditViewModel);
        }

        // POST: Employee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EmployeeEditViewModel model)
        {
             // Check if the provided data is valid, if not rerender the edit view
            // so the user can correct and resubmit the edit form
            if (ModelState.IsValid)
            {
                // Retrieve the employee being edited from the database
                Employee employee = _employeeRepository.GetEmployee(model.Id);
                // Update the employee object with the data in the model object
                employee.Name = model.Name;
                employee.Salary = model.Salary;
                employee.Departement = model.Department;

                // If the user wants to change the photo, a new photo will be
                // uploaded and the Photo property on the model object receives
                // the uploaded photo. If the Photo property is null, user did
                // not upload a new photo and keeps his existing photo
                if (model.Photo != null)
                {
                    // If a new photo is uploaded, the existing photo must be
                    // deleted. So check if there is an existing photo and delete
                    if (model.ExistingPhotoPath != null)
                    {
                        string filePath = Path.Combine(hostingEnvironment.WebRootPath,
                            "images", model.ExistingPhotoPath);
                        System.IO.File.Delete(filePath);
                    }
                    // Save the new photo in wwwroot/images folder and update
                    // PhotoPath property of the employee object which will be
                    // eventually saved in the database
                    employee.PhotoPath = ProcessUploadedFile(model);
                }

                // Call update method on the repository service passing it the
                // employee object to update the data in the database table
                Employee updatedEmployee = _employeeRepository.Update(employee);
                if (updatedEmployee != null)
                    return RedirectToAction("index");
                else
                    return NotFound();
            }

            return View(model);
        }

        [NonAction]
        private string ProcessUploadedFile(EmployeeEditViewModel model)
        {
            string uniqueFileName = null;

            if (model.Photo != null)
            {
                string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Photo.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }

        // GET: Employee/Delete/5
        public ActionResult Delete(int id)
        {
            Employee emp = _employeeRepository.GetEmployee(id);
            if (emp == null)
                return NotFound();
            else
            return View(emp);
        }

        // POST: Employee/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Employee e)
        {
            try
            {
                Employee e1 = _employeeRepository.Delete(e.Id);
                if (e1 != null)
                    return RedirectToAction(nameof(Index));
                else
                    return View();
            }
            catch
            {
                return View();
            }
        }
    }
}