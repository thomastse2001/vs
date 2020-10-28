using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcApp1.DAO;
using MvcApp1.Models;

namespace MvcApp1.Controllers
{
    [HandleError]
    public class StudentController : Controller
    {
        //// Should get list from database.
        //IList<Student> studentList = new List<Student>
        //{
        //    new Student() { StudentId = 1, StudentName = "John", Age = 18, Email = "john@a.com" } ,
        //    new Student() { StudentId = 2, StudentName = "Steve",  Age = 21, Email = "steve@a.com" } ,
        //    new Student() { StudentId = 3, StudentName = "Bill",  Age = 25, Email = "bill@a.com" } ,
        //    new Student() { StudentId = 4, StudentName = "Ram" , Age = 20, Email = "ram@a.com" } ,
        //    new Student() { StudentId = 5, StudentName = "Ron" , Age = 31, Email = "ron@a.com" } ,
        //    new Student() { StudentId = 6, StudentName = "Chris" , Age = 17, Email = "chris@a.com" } ,
        //    new Student() { StudentId = 7, StudentName = "Rob" , Age = 19, Email = "rob@a.com" }
        //};

        // GET: Student
        public ActionResult Index()
        {
            //ViewBag.TotalStudents = studentList.Count();

            // Get data from database.
            IList<Student> listOfStudents = StudentDao.GetFullList();
            ViewBag.TotalStudents = listOfStudents.Count();

            IList<Student> blackList = new List<Student>();
            blackList.Add(new Student() { StudentName = "Black One", Age = 20 });
            blackList.Add(new Student() { StudentName = "Black Two", Age = 22 });
            ViewData["blackList"] = blackList;

            //return View(studentList);
            return View(listOfStudents);
        }

        //public string Index()
        //{
        //    return "This is Index action method of StudentController";
        //}

        //[HttpPost]
        //public ActionResult Edit(Student std)
        //{
        //    // update database.

        //    return RedirectToAction("Index");
        //}

        //[HttpDelete]
        //public ActionResult Delete(int id)
        //{
        //    // update database.

        //    return RedirectToAction("Index");
        //}

        //// http://localhost/student/find/1
        //// http://localhost/studentgetbyid/1
        //[ActionName("find")]
        //public ActionResult GetById(int id)
        //{
        //    // get from database.
        //    return View();
        //}

        //[NonAction]
        //public Student GetStudent(int id)
        //{
        //    return studentList.Where(sbyte => sbyte.StudentId == id).FirstOrDefault();
        //}

        // http://localhost/student/create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Student std)
        {
            if (!ModelState.IsValid)
            {
                return View(std);
            }

            if (std == null)
            {
                ModelState.AddModelError(string.Empty, "Student is NULL.");
                return View(std);
            }

            std.StudentName = std.StudentName.Trim();

            // check whether name already exists in database.
            bool nameAlreadyExists = StudentDao.StudentNameExists(std.StudentName);
            if (nameAlreadyExists)
            {
                ModelState.AddModelError(string.Empty, "Student Name already exists.");
                return View(std);
            }

            // Update database.
            int i = StudentDao.InsertRecord(std);
            if (i == 1) { return RedirectToAction("Index"); }
            else if (i < 1)
            {
                ModelState.AddModelError(string.Empty, "Fail to create it in database.");
                return View(std);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "More than ONE record are created in database.");
                return View(std);
            }
        }

        // http://localhost/student/edit/1
        // http://localhost/student/edit?id=1
        public ActionResult Edit(int id)
        {
            //var std = studentList.Where(s => s != null && s.StudentId == id).FirstOrDefault();
            return View(StudentDao.GetRecordById(id));
        }

        //// http://localhost/student/edit?id=1&name=John
        //public ActionResult Edit(int id, string name)
        //{
        //    // do something.
        //    return View();
        //}

        // http://localhost/student/edit
        [HttpPost]
        //public ActionResult Edit([Bind(Include = "StudentId, StudentName")] Student std) // Using this form will only shows StudentId and StudentName.
        public ActionResult Edit(Student std)
        {
            if (!ModelState.IsValid)
            {
                return View(std);
            }

            if (std == null)
            {
                ModelState.AddModelError(string.Empty, "Student is NULL.");
                return View(std);
            }

            std.StudentName = std.StudentName.Trim();

            // check whether name already exists in database.
            bool nameAlreadyExists = StudentDao.StudentNameExists(std.StudentName, std.StudentId);
            if (nameAlreadyExists)
            {
                ModelState.AddModelError(string.Empty, "Student Name already exists.");
                return View(std);
            }

            // Update database.
            int i = StudentDao.UpdateRecord(std);
            if (i == 1) { return RedirectToAction("Index"); }
            else if (i < 1)
            {
                ModelState.AddModelError(string.Empty, "Fail to update it in database.");
                return View(std);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "More than ONE record are updated in database.");
                return View(std);
            }
        }

        //[HttpPost]
        //public ActionResult Edit([Bind(Exclude = "Age")] Student std)
        //{
        //    var name = std.StudentName;
        //    // update database.
        //    return RedirectToAction("Index");
        //}

        //[AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        //public ActionResult GetAndPostAction()
        //{
        //    return RedirectToAction("Index");
        //}

        // http://localhost/student/delete/1
        // http://localhost/student/delete?id=1
        public ActionResult Delete(int id)
        {
            return View(StudentDao.GetRecordById(id));
        }

        // http://localhost/student/delete
        [HttpPost]
        public ActionResult Delete(Student std)
        {
            if (std == null) { return RedirectToAction("Index"); }

            // Update database.
            int i = StudentDao.DeleteRecord(std.StudentId);
            if (i > 0) { return RedirectToAction("Index"); }
            else
            {
                ModelState.AddModelError(string.Empty, "Fail to delete it in database.");
                return View(std);
            }
        }

        // http://localhost/student/details/1
        // http://localhost/student/details?id=1
        public ActionResult Details(int id)
        {
            return View(StudentDao.GetRecordById(id));
        }
    }
}