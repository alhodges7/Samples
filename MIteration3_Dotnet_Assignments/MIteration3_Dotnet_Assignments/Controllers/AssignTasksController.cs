using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MIteration3_Dotnet_Assignments.Models;

namespace MIteration3_Dotnet_Assignments.Controllers
{
    public class AssignTasksController : Controller
    {

        public ActionResult Create()
        {
            MindtreeMVCExerciseEntities dc = new MindtreeMVCExerciseEntities();
            ViewBag.faculty = new SelectList(dc.tbl_faculty.ToList(), "facultyid", "faculty_name");
            return View();
        }

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            MindtreeMVCExerciseEntities dc = new MindtreeMVCExerciseEntities();
         
            tbl_task task = new tbl_task();
            task.facultyid = int.Parse(collection["facultyid"]);
            task.details = collection["details"];
            task.creator = collection["creator"];
            task.duration = int.Parse(collection["duration"]);

            dc.tbl_task.Add(task);
            dc.SaveChanges();
            return RedirectToAction("Create");
        }
    }
}
