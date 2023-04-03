using Microsoft.AspNetCore.Mvc;
using SimchaFund.Data;
using SimchaFund.Web.Models;
using System.Diagnostics;

namespace SimchaFund.Web.Controllers
{
    public class HomeController : Controller
    {
        string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=SimchaFund; Integrated Security=true;";
        public IActionResult Index()
        {
            var mgr = new Manager(_connectionString);
            var vm = new ViewModel
            {
                Simchas = mgr.GetAllSimchas(),
                TotalContributorCount = mgr.GetTotalContributorCount()
            };
            if (TempData["NewSimcha"] != null)
            {
                vm.Message = (string)TempData["NewSimcha"];
            }
            else if (TempData["Update"] != null)
            {
                vm.Message = (string)TempData["Update"];
            }
            foreach (var s in vm.Simchas)
            {
                s.ContributorCount = mgr.GetContributorCount(s.Id);
                s.Total = mgr.GetTotalPerSimcha(s.Id);
            }
            return View(vm);
        }
        public IActionResult Contributors()
        {
            var mgr = new Manager(_connectionString);
            var vm = new ViewModel
            {
                Contributors = mgr.GetAllContributors()             
            };
            vm.GrandTotal = mgr.GetGrandTotal(vm.Contributors);
            if (TempData["NewContributor"] != null)
            {
                vm.Message = (string)TempData["NewContributor"];
            }
            else if (TempData["Deposit"] != null)
            {
                vm.Message = (string)TempData["Deposit"];
            }
            else if (TempData["Edit"] != null)
            {
                vm.Message = (string)TempData["Edit"];
            }
            foreach (var c in vm.Contributors)
            {
                c.Balance = mgr.GetBalance(c.Id);
            }
            return View(vm);
        }
        public IActionResult History(int contribId)
        {
            var mgr = new Manager(_connectionString);
            var vm = new ViewModel
            {
                Actions = mgr.GetHistory(contribId),
                ContributorName = mgr.GetContributorName(contribId),
                Balance = mgr.GetBalance(contribId)
            };
            return View(vm);
        }
        public IActionResult Contributions(int simchaId)
        {
            var mgr = new Manager(_connectionString);
            var vm = new ViewModel
            {
                SimchaId = simchaId,
                SimchaName = mgr.GetSimchaName(simchaId),
                Contributors = mgr.GetAllContributors(),
            };
            int counter = 0;
            foreach (var c in vm.Contributors)
            {
                c.Index = counter;
                c.Balance = mgr.GetBalance(c.Id);
                c.YesContribute = mgr.YesContribute(c.Id, simchaId);
                counter++;
            }
            return View(vm);
        }
        [HttpPost]
        public IActionResult Deposit(Payment p)
        {
            var mgr = new Manager(_connectionString);
            mgr.AddDeposit(p);
            TempData["Deposit"] = "Deposit added successfully!";
            return Redirect("/home/contributors");
        }
        [HttpPost]
        public IActionResult Edit(Contributor c)
        {
            var mgr = new Manager(_connectionString);
            mgr.UpdateContributor(c);
            TempData["Edit"] = "Edit successfully completed!";
            return Redirect("/home/contributors");
        }
        [HttpPost]
        public IActionResult UpdateContributions(List<Payment> contributors, int simchaId)
        {
            var mgr = new Manager(_connectionString);
            mgr.ClearContributions(simchaId);
            foreach (var p in contributors)
            {
                p.SimchaId = simchaId;
            }
            mgr.AddContributions(contributors);
            TempData["Update"] = "Successfully Updated!";
            return Redirect("/home/index");
        }
        [HttpPost]
        public IActionResult AddSimcha(Simcha simcha)
        {
            var mgr = new Manager(_connectionString);
            mgr.AddSimcha(simcha);
            TempData["NewSimcha"] = "Simcha added successfully!";
            return Redirect("/home/index");
        }
        [HttpPost]
        public IActionResult NewContributor(Contributor contributor, int initialDeposit)
        {
            var mgr = new Manager(_connectionString);
            mgr.AddContributor(contributor);
            var deposit = new Payment
            {
                ContributorId = contributor.Id,
                Amount = initialDeposit,
                Date = DateTime.Now,
            };
            mgr.AddDeposit(deposit);
            TempData["NewContributor"] = "Contributor added successfully!";
            return Redirect("/home/contributors");
        }
    }
}