using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using StudentExercisesMVC.Models;
using StudentExercisesMVC.Models.ViewModels;
using StudentExercisesMVC.Repositories;

namespace StudentExercisesMVC.Controllers
{
    public class StudentsController : Controller
    {
        private readonly IConfiguration _config;
        private string _connectionString;

        public StudentsController(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("DefaultConnection");
        }

        public SqlConnection Connection => new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        // GET: Students
        public ActionResult Index()
        {
            List<Student> students = StudentRepository.GetAllStudents(_connectionString);
            return View(students);
        }

        // GET: Students/Details/5
        public ActionResult Details(int id)
        {
            Student student = StudentRepository.GetStudent(id, _connectionString);
            return View(student);
        }

        // GET: Students/Create
        public ActionResult Create()
        {
            StudentCreateViewModel model = new StudentCreateViewModel(_connectionString);
            return View(model);
        }

        // POST: Students/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([FromForm] StudentCreateViewModel model)
        {
            int redirectId = StudentRepository.CreateStudent(model, _connectionString);
            return RedirectToAction(nameof(Index));
        }

        // GET: Students/Edit/5
        public ActionResult Edit(int id)
        {
            var model = new StudentEditViewModel(id, _connectionString);
            return View(model);
        }

        // POST: Students/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, StudentEditViewModel model)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Student
                                            SET FirstName = @firstName,
                                                LastName = @lastName,
                                                SlackHandle = @handle,
                                                CohortId = @cId
                                            WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@firstName", model.Student.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastName", model.Student.LastName));
                        cmd.Parameters.Add(new SqlParameter("@handle", model.Student.SlackHandle));
                        cmd.Parameters.Add(new SqlParameter("@cId", model.Student.CohortId));
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        cmd.ExecuteNonQuery();
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch (Exception)
            {
                return View(model);
            }
        }

        // GET: Students/Delete/5
        public ActionResult DeleteConfirm(int id)
        {
            var student = StudentRepository.GetStudent(id, _connectionString);
            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete([FromForm] int id)
        {
            try
            {
                int rowsAffected = StudentRepository.DeleteStudent(id, _connectionString);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction(nameof(Details), new { id = id });
            }
        }
    }
}