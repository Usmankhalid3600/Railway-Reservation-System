using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication4.Models;

namespace WebApplication4.Controllers
{
    public class homeController : Controller
    {
        DataClasses1DataContext dc = new DataClasses1DataContext();
        // GET: Default
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult check()
        {
            return View();
        }
        public ActionResult reserve()
        {
            return View();
        }
        public ActionResult update()
        {
            return View();
        }
        public ActionResult error1()
        {
            return View();
        }
        public ActionResult middle()
        {
            string id = Convert.ToString(Session["id"]);
            string clss = Convert.ToString(Session["class"]);

            var price=0;
            var p = dc.tchecks.First(c => c.trainId == id);
            if (clss == "businessSeats")
            {
              price = p.bticket;
            }

            else
            {
                price = p.eticket;
            }
            Session["price"] = price;
            return View();
        }

        public ActionResult error()
        {
            string source = Request["sourceCity"];
            string destination = Request["destination"];
            string name = Request["Name"];
            string clss = Request["classs"];
            var x = dc.trains.FirstOrDefault(c => c.sourceCity == source && c.destinationCity == destination);

            if (x == null)
            {
                ViewBag.demo = "Sorry! No train is available at the moment for selected route.";
                return View();
            }
           

            var sel = dc.tStatus.First(c => c.trainId == x.trainId);
            if (clss == "businessSeats")
            {
                if (sel.businessSeats<=0)
                {
                    ViewBag.demo = "Sorry! No business class seat is available for the selected train.";
                    return View();
                }
            }
            else
            {
                if (sel.economySeats <= 0)
                {
                    ViewBag.demo = "Sorry! No economy class seat is available for the selected train.";
                    return View();
                }
            }

            Session["name"] = name;
            Session["class"] = clss;
            Session["id"] = x.trainId;
            Session["tname"] = x.trainName;
            Session["source"] = x.sourceCity;
            Session["destination"] = x.destinationCity;
            return RedirectToAction("middle");
                   
        }

        public ActionResult cnfrm()
        {
            passenger p = new passenger();
            p.passnegerName = Convert.ToString(Session["name"]);
            dc.passengers.InsertOnSubmit(p);
            dc.SubmitChanges();
            Session["pid"] = p.passengerId;
            var id = Convert.ToString(Session["id"]);

            tStatus s = dc.tStatus.First(std => std.trainId == id);

            var clss = Convert.ToString(Session["class"]);
            ticket t = new ticket();
            if (clss == "businessSeats")
            {
                s.businessSeats -= 1;
                t.ticketClass = "Business";
            }
            else
            {
                s.economySeats -= 1;
                t.ticketClass = "Economy";
            }

            dc.SubmitChanges();

            t.trainId = id;
            t.passengerId = p.passengerId;
            t.ticketPrice = (int)Session["price"];
            dc.tickets.InsertOnSubmit(t);
            dc.SubmitChanges();

            return RedirectToAction("csst", t);
        }
        public ActionResult csst(ticket t)
        {
            return View(t);
        }

        public ActionResult find()
        {
            string tid = Request["checknumber"];
            var s = dc.tickets.FirstOrDefault(std => std.ticketId == int.Parse(tid));
            if (s != null)
            {
                var nam = dc.passengers.First(st => st.passengerId == s.passengerId);
                var tname = dc.trains.First(st => st.trainId == s.trainId);
                Session["pid"] = nam.passengerId;
                Session["name"] = nam.passnegerName;
                Session["id"] = tname.trainId;
                Session["source"] = tname.sourceCity;
                Session["destination"] = tname.destinationCity;
                Session["tname"] = tname.trainName;
                return RedirectToAction("csst", s);
            }
            else
            {
                return RedirectToAction("error1");
            }
        }

        public ActionResult find1()
        {
            string tid = Request["checknumber"];

            var s = dc.tickets.FirstOrDefault(std => std.ticketId == int.Parse(tid));

            if (s != null)
            {
                var nam = dc.passengers.First(st => st.passengerId == s.passengerId);
                var tname = dc.trains.First(st => st.trainId == s.trainId);
                Session["pid"] = nam.passengerId;
                Session["name"] = nam.passnegerName;
                Session["source"] = tname.sourceCity;
                Session["destination"] = tname.destinationCity;
                Session["id"] = tname.trainId;
                Session["tname"] = tname.trainName;
               
                return View(s);
            }
            else
            {
                return RedirectToAction("error1");
            }
        }

        public ActionResult updated()
        {
            string source = Request["source"];
            string destination = Request["destination"];
            string name = Request["pname"];
            string clss = Request["classs"];
            var x = dc.trains.FirstOrDefault(c => c.sourceCity == source && c.destinationCity == destination);
            if (x == null)
            {
                Session["error"] = "Sorry! No train is available at the moment for selected route.";
                return RedirectToAction("error2");
            }

            var sel = dc.tStatus.First(c => c.trainId == x.trainId);
            if (clss == "businessSeats")
            {
                if (sel.businessSeats <= 0)
                {
                    Session["error"] = "Sorry! No business class seat is available for the selected train.";
                    return RedirectToAction("error2");
                }
            }
            else
            {
                if (sel.economySeats <= 0)
                {
                    Session["error"] = "Sorry! No economy class seat is available for the selected train.";
                    return RedirectToAction("error2");
                }
            }
            string pid = Convert.ToString(Session["pid"]);
            string source1 = Request["source"];
            string destination1 = Request["destination"];
            string name1 = Request["pname"];
            string clss1 = Request["classs"];
            var price = 0;
            var p = dc.tchecks.First(c => c.trainId == x.trainId);
            if (clss == "businessSeats")
            {
                price = p.bticket;
            }

            else
            {
                price = p.eticket;
            }
            ticket s = dc.tickets.First(std => std.passengerId == int.Parse(pid));
            passenger ss = dc.passengers.First(std => std.passengerId == int.Parse(pid));
            ss.passnegerName = name1;

            if (clss1 == "businessSeats")
            {
                s.ticketClass = "Business";
            }
            else
            {
                s.ticketClass = "Economy";
            }
            
            s.trainId = x.trainId;
            s.ticketPrice = price;

            dc.SubmitChanges();
            Session["Error"] = "Ticket updated Successfully";
            return RedirectToAction("error2");
        }
        public ActionResult error2()
        {
            return View();
        }






        }
}