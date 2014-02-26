using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MIteration3_Dotnet_Assignments.Models;
using MIteration3_Dotnet_Assignments.Models.ViewModels;

namespace MIteration3_Dotnet_Assignments.Controllers
{
    public class ViewTasksController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewAllTasks()
        {
            MindtreeMVCExerciseEntities dc = new MindtreeMVCExerciseEntities();
            var tasks = dc.tbl_task.ToList();
            IList<ViewTasksViewModel> viewTasksVM = ConvertTasksToViewTaskViewModel(dc, tasks);
            return View("ViewTasks", viewTasksVM);
        }

        public ActionResult ViewFacultyTasks()
        {
            MindtreeMVCExerciseEntities dc = new MindtreeMVCExerciseEntities();
            ViewBag.faculty = new SelectList(dc.tbl_faculty.ToList(), "facultyid", "faculty_name");
            return View("SelectFaculty");
        }

        [HttpPost]
        public ActionResult ViewFacultyTasks(int facultyid)
        {
            MindtreeMVCExerciseEntities dc = new MindtreeMVCExerciseEntities();
            var tasks = from t in dc.tbl_task
                        where t.facultyid == facultyid
                        select t;
            IList<ViewTasksViewModel> viewTasksVM = ConvertTasksToViewTaskViewModel(dc, tasks.ToList());

            var name = from f in dc.tbl_faculty
                              where f.facultyid == facultyid
                              select f.faculty_name;
            ViewBag.tableLabel = "Tasks assigned to: " + name.ToList()[0];

            return View("ViewTasks", viewTasksVM);
        }

        private static IList<ViewTasksViewModel> ConvertTasksToViewTaskViewModel(MindtreeMVCExerciseEntities dc, List<tbl_task> tasks)
        {
            IList<ViewTasksViewModel> viewTasksVM = new List<ViewTasksViewModel>();
            foreach (tbl_task t in tasks)
            {
                ViewTasksViewModel task = new ViewTasksViewModel();
                task.details = t.details;
                task.creator = t.creator;
                task.duration = t.duration.HasValue ? t.duration.ToString() : "";
                var name = from f in dc.tbl_faculty
                           where f.facultyid == t.facultyid
                           select f.faculty_name;
                if (name != null)
                {
                    task.facultyName = (name.ToList())[0];
                }
                else
                    task.facultyName = "";
                viewTasksVM.Add(task);
            }
            return viewTasksVM;
        }
    }
}
