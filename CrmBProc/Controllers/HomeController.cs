using CrmBProc.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CrmBProc.Controllers
{
    public class HomeController : Controller
    {

        BPCRMEntities db = new BPCRMEntities();


        public ActionResult Index()
        {
            return View();
        }
        public ActionResult About()
        {
            
            return View();
        }





        //поставщики
        public ActionResult SignIn_supplier()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SignIn_supplier(Suppliers_tbl sup)
        {
            Suppliers_tbl supplier = db.Suppliers_tbl.Where(x => x.supplier_login == sup.supplier_login && x.supplier_password == sup.supplier_password).SingleOrDefault();
            if (supplier != null)
            {
                Session["supplier_id"] = supplier.supplier_id;
                Session["supplier_fullname"] = supplier.supplier_fullname;
                Session["supplier_login"] = supplier.supplier_login;
                Session["supplier_company"] = supplier.supplier_companyname;
                Session["supplier_address"] = supplier.supplier_address;
                return RedirectToAction("supplier_tab");
            }
            else
            {
                ViewBag.msg = "Неверный логин или пароль!";
            }
            return View();
        }
        public ActionResult supplier_tab(int? id, int? page)
        {
            if (Session["supplier_id"] == null)
            {
                return RedirectToAction("Index");
            }
            int pagesize = 15, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            List<Order_table> li = db.Order_table.ToList();
            IPagedList<Order_table> ind = li.ToPagedList(pageindex, pagesize);
            return View(ind);
        }
        [HttpGet]
        public ActionResult supplier_register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult supplier_register(Suppliers_tbl sup)
        {
            Suppliers_tbl supplier = new Suppliers_tbl();
            try
            {
                supplier.supplier_login = sup.supplier_login;
                supplier.supplier_password = sup.supplier_password;
                supplier.supplier_fullname = sup.supplier_fullname;
                supplier.supplier_companyname = sup.supplier_companyname;
                supplier.supplier_address = sup.supplier_address;
                db.Suppliers_tbl.Add(supplier);
                db.SaveChanges();
                return RedirectToAction("SignIn_supplier");
            }
            catch (Exception)
            {
                ViewBag.msg = "Не удалось зарегистрироваться!";
            }
            return View();
        }
        public ActionResult supplier_profile()
        {
            if (Session["supplier_id"] == null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }
        public async Task<ActionResult> supplier_order_list_confirm(int? id)
        {
            if (Session["supplier_id"] == null)
            {
                return RedirectToAction("Index");
            }
            
            if (id != null)
            {
                Order_table inquery = await db.Order_table.FirstOrDefaultAsync(x => x.order_id == id);
                if (inquery != null)
                {
                    Session["order_id"] = inquery.order_id;
                    Session["order_name_inquiry"] = inquery.Employee_tbl.employee_fullname;
                    Session["order_deparment_inquiry"] = inquery.Branches_tbl.branch_name;
                    Session["order_date"] = inquery.date_fix_order;
                    Session["order_material"] = inquery.Request_tbl.request_material;
                    Session["order_technical_character"] = inquery.Request_tbl.request_technical_character;
                    Session["order_quantity"] = inquery.Request_tbl.request_quanitity;
                    Session["order_dedline"] = inquery.Request_tbl.request_deadline;
                    Session["order_send_name"] = inquery.order_send_emp;
                    Session["order_send_name_position"] = inquery.order_position;
                    Session["order_time"] = inquery.date_fix_order;
                    Session["order_group"] = inquery.order_group;
                    Session["order_term"] = inquery.order_terms;
                    return View(inquery);
                }
            }
            return HttpNotFound();
        }
        [HttpPost]
        public ActionResult supplier_order_list_confirm(Order_status_tbl ordst, string answer)
        {
            if (Session["supplier_id"] == null)
            {
                return RedirectToAction("Index");
            }
           
            Order_status_tbl order_status = new Order_status_tbl();

            if (ModelState.IsValid && !String.IsNullOrWhiteSpace(answer))
            {
                switch (answer)
                {
                    case "Принять":
                        order_status.order_status_FK_Status = 2;
                        order_status.order_status_FK_Supplier = Convert.ToInt32(Session["supplier_id"]);
                        order_status.order_status_FK_Order = Convert.ToInt32(Session["order_id"]);
                        break;
                    case "Документ":
                        return RedirectToAction("supplier_order_list_confirm_document");
                    default:
                        ViewBag.error = "Something went wrong";
                        break;
                }
            }
            db.Order_status_tbl.Add(order_status);
            db.SaveChanges();
            return RedirectToAction("supplier_order_list_confirm");
        }
        public ActionResult supplier_order_list_confirm_document()
        {
            if (Session["supplier_id"] == null)
            {
                return RedirectToAction("Index");
            }
          

            return View();     
        }

        public ActionResult supplier_tab_sup(int? id, int? page)
        {
            if (Session["supplier_id"] == null)
            {
                return RedirectToAction("Index");
            }
           
            int pagesize = 15, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            List<Order_status_tbl> li = db.Order_status_tbl.ToList();
            IPagedList<Order_status_tbl> ind = li.ToPagedList(pageindex, pagesize);

            return View(ind);
        }
        public async Task<ActionResult> supplier_order_sup_confirm(int? id)
        {
            if (Session["supplier_id"] == null)
            {
                return RedirectToAction("Index");
            }
            if (id != null)
            {
                Order_status_tbl order_status = await db.Order_status_tbl.FirstOrDefaultAsync(x => x.order_status_id == id);
                if (order_status != null)
                {
                    Session["order_status_id"] = order_status.order_status_id;
                    Session["order_id"] = order_status.Order_table.order_id;
                    //Session["warehouse_FK_emp"] = order_status.Employee_tbl.employee_id;
                    //Session["order_name_inquiry"] = order_status.Employee_tbl.employee_fullname;
                    //Session["order_deparment_inquiry"] = order_status.Branches_tbl.branch_name;
                    //Session["order_date"] = order_status.Order_table.date_fix_order;
                    //Session["order_material"] = order_status.Request_tbl.request_material;
                   // Session["order_technical_character"] = order_status.Request_tbl.request_technical_character;
                   // Session["order_quantity"] = order_status.Request_tbl.request_quanitity;
                   // Session["order_dedline"] = order_status.Request_tbl.request_deadline;
                   // Session["order_send_name"] = order_status.Order_table.order_send_emp;
                   // Session["order_send_name_position"] = order_status.Order_table.order_position;
                   // Session["order_time"] = order_status.Order_table.date_fix_order;
                   // Session["order_group"] = order_status.Order_table.order_group;
                   // Session["order_term"] = order_status.Order_table.order_terms;
                    return View(order_status);
                }
            }
            return HttpNotFound();
        }
        [HttpPost]
        public ActionResult supplier_order_sup_confirm(Warehouse_sup_tbl req, string answer)
        {
            if (Session["supplier_id"] == null)
            {
                return RedirectToAction("Index");
            }
          
            return RedirectToAction("supplier_tab_sup");
        }



        public ActionResult supplier_order_sup_naklad()
        {
            if (Session["supplier_id"] == null)
            {
                return RedirectToAction("Index");
            }
           
            return View();
        }








        //содрудники
        public ActionResult SignIn_employee()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SignIn_employee(Employee_tbl emp)
        {
            Employee_tbl employee = db.Employee_tbl.Where(x => x.employee_login == emp.employee_login && x.employee_password == emp.employee_password && x.position_FK_emp == 2).SingleOrDefault();
            Employee_tbl employee_head = db.Employee_tbl.Where(x => x.employee_login == emp.employee_login && x.employee_password == emp.employee_password  && x.position_FK_emp ==1).SingleOrDefault();
            Employee_tbl employee_dep_post = db.Employee_tbl.Where(x => x.employee_login == emp.employee_login && x.employee_password == emp.employee_password  && x.position_FK_emp ==3).SingleOrDefault();
            if (employee != null)
            {
                Session["employee_id"] = employee.employee_id;
                Session["employee_fullname"] = employee.employee_fullname;
                Session["employee_login"] = employee.employee_login;
                Session["employee_birthday"] = employee.employee_birthday;
                Session["employee_phnumber"] = employee.employee_phnumber;
                Session["employee_position"] = employee.Position_tbl.position_name;
                Session["employee_branch"] = employee.Branches_tbl.branch_name;
                Session["employee_branch_id"] = employee.Branches_tbl.branch_id;
                return RedirectToAction("employee_tab");
            }
            if(employee_head != null)
            {
                Session["employee_id"] = employee_head.employee_id;
                Session["employee_fullname"] = employee_head.employee_fullname;
                Session["employee_login"] = employee_head.employee_login;
                Session["employee_birthday"] = employee_head.employee_birthday;
                Session["employee_phnumber"] = employee_head.employee_phnumber;
                Session["employee_position"] = employee_head.Position_tbl.position_name;
                Session["employee_branch"] = employee_head.Branches_tbl.branch_name;
                Session["employee_branch_id"] = employee_head.Branches_tbl.branch_id;
                return RedirectToAction("employee_head_tab");
            }
            if (employee_dep_post != null)
            {
                Session["employee_id"] = employee_dep_post.employee_id;
                Session["employee_fullname"] = employee_dep_post.employee_fullname;
                Session["employee_login"] = employee_dep_post.employee_login;
                Session["employee_birthday"] = employee_dep_post.employee_birthday;
                Session["employee_phnumber"] = employee_dep_post.employee_phnumber;
                Session["employee_position"] = employee_dep_post.Position_tbl.position_name;
                Session["employee_branch"] = employee_dep_post.Branches_tbl.branch_name;
                Session["employee_branch_id"] = employee_dep_post.Branches_tbl.branch_id;
                return RedirectToAction("employee_dep_post_tab");
            }
            else
            {
                ViewBag.msg = "Неверный логин или пароль!";
            }
            return View();
        }
        public ActionResult employee_tab()
        {
            if (Session["employee_id"] == null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }
        public ActionResult employee_tab_profile()
        {
            if (Session["employee_id"] == null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }
        [HttpGet]
        public ActionResult employee_tab_inquiry()
        {
            if (Session["employee_id"] == null)
            {
                return RedirectToAction("Index");
            }
            int department = Convert.ToInt32(Session["employee_branch_id"]);
            List<Employee_tbl> li = db.Employee_tbl.Where(x=>x.branch_FK_emp == department).ToList();
            ViewBag.list = new SelectList(li, "employee_id", "Position_tbl.position_name");
            ViewBag.list_name = new SelectList(li, "employee_id", "employee_fullname");
            List<Branches_tbl> branch = db.Branches_tbl.ToList();
            ViewBag.list_branch = new SelectList(branch, "branch_id", "branch_name");
            return View();
        }
        [HttpPost]
        public ActionResult employee_tab_inquiry(Inquery_tbl inc)
        {
            if (Session["employee_id"] == null)
            {
                return RedirectToAction("Index");
            }
            List<Employee_tbl> li = db.Employee_tbl.ToList();
            ViewBag.list = new SelectList(li, "employee_id", "Position_tbl.position_name", "Branches_tbl.branch_name");
            ViewBag.list_name = new SelectList(li, "employee_id", "employee_fullname");
            List<Branches_tbl> branch = db.Branches_tbl.ToList();
            ViewBag.list_branch = new SelectList(branch, "branch_id", "branch_name");
            Inquery_tbl inquery = new Inquery_tbl();
            try
            {
                inquery.inquery_FK_emp = inc.inquery_FK_emp;
                inquery.inquery_date = DateTime.Now;
                inquery.inquery_material = inc.inquery_material;
                inquery.inquery_technical_character = inc.inquery_technical_character;
                inquery.inquery_quantity = inc.inquery_quantity;
                inquery.inquery_deadline = inc.inquery_deadline;
                inquery.inquery_FK_head = inc.inquery_FK_head;
                inquery.inquery_description = inc.inquery_description;
                inquery.inquery_FK_branch = inc.inquery_FK_branch;
                inquery.inquery_FK_status = 1;

                db.Inquery_tbl.Add(inquery);
                db.SaveChanges();
                return RedirectToAction("employee_tab_inquiry");
            }
            catch (Exception)
            {
                ViewBag.msg = "Не удалось формировать запрос!";
            }
            return View();
        }
        public ActionResult employee_tab_inquiry_menu()
        {
            if (Session["employee_id"] == null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }
        public ActionResult employee_tab_inquery_list(int? id, int? page)
        {
            if (Session["employee_id"] == null)
            {
                return RedirectToAction("Index");
            }
            int pagesize = 15, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            int employee = Convert.ToInt32(Session["employee_id"]);
            List<Inquery_tbl> li = db.Inquery_tbl.Where(x=>x.inquery_FK_emp == employee).ToList();
            IPagedList<Inquery_tbl> ind = li.ToPagedList(pageindex, pagesize);
            return View(ind);
        }
        public ActionResult employee_tab_inquery_status(int? id, int? page)
        {
            if (Session["employee_id"] == null)
            {
                return RedirectToAction("Index");
            }
            int pagesize = 15, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            int status = Convert.ToInt32(Session["employee_id"]);
            List<Inquery_status_tbl> li = db.Inquery_status_tbl.Where(x => x.inquery_status_FK_Emp == status).ToList();
            IPagedList<Inquery_status_tbl> ind = li.ToPagedList(pageindex, pagesize);

            return View(ind);
        }






        //департамент по поставкам
        public ActionResult employee_dep_post_tab()
        {
            if (Session["employee_id"] == null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }
        public ActionResult employee_dep_post_tab_profile()
        {
            if (Session["employee_id"] == null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }
        public ActionResult employee_dep_post_request(int? id, int? page)
        {
            if (Session["employee_id"] == null)
            {
                return RedirectToAction("Index");
            }
            int pagesize = 15, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            List<Request_tbl> li = db.Request_tbl.ToList();
            IPagedList<Request_tbl> ind = li.ToPagedList(pageindex, pagesize);
            return View(ind);
        }
        [HttpGet]
        public ActionResult employee_dep_post_order()
        {
            if (Session["employee_id"] == null)
            {
                return RedirectToAction("Index");
            }

            List<Branches_tbl> li = db.Branches_tbl.ToList();
            ViewBag.list_branch = new SelectList(li, "branch_id", "branch_name");

            List<Employee_tbl> employee = db.Employee_tbl.ToList();
            ViewBag.list_employee = new SelectList(employee, "employee_id", "employee_fullname");

            List<Request_tbl> request = db.Request_tbl.ToList();
            ViewBag.list_request = new SelectList(request, "request_id", "request_material");
            return View();
        }
        [HttpPost]
        public ActionResult employee_dep_post_order(Order_table ord)
        {
            if (Session["employee_id"] == null)
            {
                return RedirectToAction("Index");
            }
            List<Branches_tbl> li = db.Branches_tbl.ToList();
            ViewBag.list_branch = new SelectList(li, "branch_id", "branch_name");

            List<Employee_tbl> employee = db.Employee_tbl.ToList();
            ViewBag.list_employee = new SelectList(employee, "employee_id", "employee_fullname");

            List<Request_tbl> request = db.Request_tbl.ToList();
            ViewBag.list_request = new SelectList(request, "request_id", "request_material");
            Order_table order = new Order_table();
            try
            {
                order.order_FK_emp = ord.order_FK_emp;
                order.order_FK_branch = ord.order_FK_branch;
                order.order_FK_request = ord.order_FK_request;
                order.order_group = ord.order_group;
                order.order_terms = ord.order_terms;
                order.date_fix_order = DateTime.Now;
                order.order_send_emp = Convert.ToString(Session["employee_fullname"]);
                order.order_position = Convert.ToString(Session["employee_position"]);
                db.Order_table.Add(order);
                db.SaveChanges();
                return RedirectToAction("employee_dep_post_order_list");
            }
            catch (Exception)
            {
                ViewBag.msg = "Не удалось cформировать заказ!";
            }
            return View();
        }
        public ActionResult employee_dep_post_order_list(int? id, int? page)
        {
            if (Session["employee_id"] == null)
            {
                return RedirectToAction("Index");
            }
            int pagesize = 15, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            List<Order_table> li = db.Order_table.ToList();
            IPagedList<Order_table> ind = li.ToPagedList(pageindex, pagesize);
            return View(ind);
        }
        public async Task<ActionResult> employee_dep_post_order_list_document(int? id)
        {
            if (Session["employee_id"] == null)
            {
                return RedirectToAction("Index");
            }
            if (id != null)
            {
                Order_table order = await db.Order_table.FirstOrDefaultAsync(x => x.order_id == id);
                if (order != null)
                {
                    Session["order_id"] = order.order_id;
                    Session["order_name_inquiry"] = order.Employee_tbl.employee_fullname;
                    Session["order_deparment_inquiry"] = order.Branches_tbl.branch_name;
                    Session["order_date"] = order.date_fix_order;
                    Session["order_material"] = order.Request_tbl.request_material;
                    Session["order_technical_character"] = order.Request_tbl.request_technical_character;
                    Session["order_quantity"] = order.Request_tbl.request_quanitity;
                    Session["order_dedline"] = order.Request_tbl.request_deadline;
                    Session["order_send_name"] = order.order_send_emp;
                    Session["order_send_name_position"] = order.order_position;
                    Session["order_time"] = order.date_fix_order;
                    Session["order_group"] = order.order_group;
                    Session["order_term"] = order.order_terms;
                    return View(order);
                }
            }
            return HttpNotFound();
        }







        //руководители
        public ActionResult employee_head_tab()
        {
            if (Session["employee_id"] == null)
            {
                return RedirectToAction("Index");
            }
           
            return View();
        }
        public ActionResult employee_head_tab_profile()
        {
            if (Session["employee_id"] == null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }
        [HttpGet]
        public ActionResult employee_head_tab_inquiry()
        {
            if (Session["employee_id"] == null)
            {
                return RedirectToAction("Index");
            }
            int department = Convert.ToInt32(Session["employee_branch_id"]);
            List<Employee_tbl> li = db.Employee_tbl.Where(x => x.branch_FK_emp == department).ToList();
            ViewBag.list = new SelectList(li, "employee_id", "Position_tbl.position_name");
            ViewBag.list_name = new SelectList(li, "employee_id", "employee_fullname");
            return View();
        }
        [HttpPost]
        public ActionResult employee_head_tab_inquiry(Inquery_tbl inc)
        {
            if (Session["employee_id"] == null)
            {
                return RedirectToAction("Index");
            }
            int department = Convert.ToInt32(Session["employee_branch_id"]);
            List<Employee_tbl> li = db.Employee_tbl.Where(x=>x.branch_FK_emp == department).ToList();
            ViewBag.list = new SelectList(li, "employee_id", "Position_tbl.position_name");

            ViewBag.list_name = new SelectList(li, "employee_id", "employee_fullname");
            Inquery_tbl inquery = new Inquery_tbl();
            try
            {
                inquery.inquery_FK_emp = inc.inquery_FK_emp;
                inquery.inquery_date = DateTime.Now;
                inquery.inquery_material = inc.inquery_material;
                inquery.inquery_technical_character = inc.inquery_technical_character;
                inquery.inquery_quantity = inc.inquery_quantity;
                inquery.inquery_deadline = inc.inquery_deadline;
                inquery.inquery_FK_head = inc.inquery_FK_head;
                inquery.inquery_description = inc.inquery_description;
                inquery.inquery_FK_status = 1;

                db.Inquery_tbl.Add(inquery);
                db.SaveChanges();
                return RedirectToAction("employee_head_tab_inquiry");
            }
            catch (Exception)
            {
                ViewBag.msg = "Не удалось формировать запрос!";
            }
            return View();
        }
        public ActionResult employee_head_tab_inquery_list(int? id, int? page)
        {
            if (Session["employee_id"] == null)
            {
                return RedirectToAction("Index");
            }
            int pagesize = 15, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            int employee = Convert.ToInt32(Session["employee_id"]);
            List<Inquery_tbl> li = db.Inquery_tbl.Where(x => x.inquery_FK_emp == employee).ToList();
            IPagedList<Inquery_tbl> ind = li.ToPagedList(pageindex, pagesize);
            return View(ind);
        }
        public ActionResult employee_head_tab_inquiry_menu()
        {
            if (Session["employee_id"] == null)
            {
                return RedirectToAction("Index");
            }

            return View();
        }
        public ActionResult employee_head_tab_inquery_list_department()
        {
            if (Session["employee_id"] == null)
            {
                return RedirectToAction("Index");
            }
           
            List<Inquery_tbl> li = db.Inquery_tbl.OrderByDescending(x => x.inquery_id).ToList();
            ViewData["List"] = li;
            return View();
        }
        public async Task<ActionResult> employee_head_inquiry_permission(int? id)
        {
            if (Session["employee_id"] == null)
            {
                return RedirectToAction("Index");
            }
            if (id != null)
            {
                Inquery_tbl inquery = await db.Inquery_tbl.FirstOrDefaultAsync(x => x.inquery_id == id);
                if (inquery != null)
                {
                    Session["inquiry_id"] = inquery.inquery_id;
                    Session["inquiry_FK_emp"] = inquery.inquery_FK_emp;
                    Session["inquiry_date"] = inquery.inquery_date;
                    Session["inquiry_material"] = inquery.inquery_material;
                    Session["inquiry_technical_character"] = inquery.inquery_technical_character;
                    Session["inquiry_quantity"] = inquery.inquery_quantity;
                    Session["inquiry_deadline"] = inquery.inquery_deadline;
                    Session["inquiry_FK_head"] = inquery.Employee_tbl1.employee_fullname;
                    Session["inquiry_description"] = inquery.inquery_description;
                    Session["inquiry_FK_branch"] = inquery.Branches_tbl.branch_name;
                    Session["inquiry_FK_branch_id"] = inquery.Branches_tbl.branch_id;
                    return View(inquery);
                }
            }
            return HttpNotFound();
        }
        [HttpPost]
        public ActionResult employee_head_inquiry_permission(Inquery_status_tbl inqstat, string answer)
        {
            if (Session["employee_id"] == null)
            {
                return RedirectToAction("Index");
            }

            Inquery_status_tbl inquery_status = new Inquery_status_tbl();

            if (ModelState.IsValid && !String.IsNullOrWhiteSpace(answer))
            {
                switch (answer)
                {
                    case "Принять":
                        inquery_status.inquery_status_FK_Inquery = Convert.ToInt32(Session["inquiry_id"]);
                        inquery_status.inquery_status_FK_Emp = Convert.ToInt32(Session["inquiry_FK_emp"]);
                        inquery_status.inquery_status_FK_Branch = Convert.ToInt32(Session["inquiry_FK_branch_id"]);
                        inquery_status.inquery_status_FK_Status = 2;
                        break;
                    case "Отклонить":
                        inquery_status.inquery_status_FK_Inquery = Convert.ToInt32(Session["inquiry_id"]);
                        inquery_status.inquery_status_FK_Emp = Convert.ToInt32(Session["inquiry_FK_emp"]);
                        inquery_status.inquery_status_FK_Branch = Convert.ToInt32(Session["inquiry_FK_branch_id"]);
                        inquery_status.inquery_status_FK_Status = 3;
                        break;
                    case "Документ":
                        return RedirectToAction("employee_head_inquiry_permission_document");
                    default:
                        ViewBag.error = "Something went wrong";
                        break;
                }
            }
            db.Inquery_status_tbl.Add(inquery_status);
            db.SaveChanges();
            return RedirectToAction("employee_head_tab_inquery_list_department");
              
        }
        public ActionResult employee_head_inquiry_permission_document()
        {
            if (Session["employee_id"] == null)
            {
                return RedirectToAction("Index");
            }
            
            return View();
             
        }
        public ActionResult employee_head_tab_request()
        {
            if (Session["employee_id"] == null)
            {
                return RedirectToAction("Index");
            }

            return View();
        }
        public ActionResult employee_head_tab_department(int? id, int? page)
        {
            if (Session["employee_id"] == null)
            {
                return RedirectToAction("Index");
            }
            int pagesize = 15, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            int department = Convert.ToInt32(Session["employee_branch_id"]);
            List<Employee_tbl> li = db.Employee_tbl.Where(x => x.branch_FK_emp == department).ToList();
            IPagedList<Employee_tbl> ind = li.ToPagedList(pageindex, pagesize);
            return View(ind);
        }
        public ActionResult employee_head_tab_accepted_inquiry(int? id, int? page)
        {
            if (Session["employee_id"] == null)
            {
                return RedirectToAction("Index");
            }
            int pagesize = 15, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            int department = Convert.ToInt32(Session["employee_branch_id"]);
            List<Inquery_status_tbl> li = db.Inquery_status_tbl.Where(x => x.inquery_status_FK_Status == 2 && x.inquery_status_FK_Branch == department).ToList();//только принятые запросы
            IPagedList<Inquery_status_tbl> ind = li.ToPagedList(pageindex, pagesize);
            return View(ind);
        }
        [HttpGet]
        public async Task<ActionResult> employee_head_tab_send_request(int? id)
        {
            if (Session["employee_id"] == null)
            {
                return RedirectToAction("Index");
            }
            List<Employee_tbl> li = db.Employee_tbl.Where(x=>x.position_FK_emp == 3).ToList();
            ViewBag.list = new SelectList(li, "employee_id", "Position_tbl.position_name");

            if (id != null)
            {
                Inquery_status_tbl inquery = await db.Inquery_status_tbl.FirstOrDefaultAsync(x => x.inquery_status_id == id);
                if (inquery != null)
                {
                    Session["inquiry_request_id"] = inquery.inquery_status_id;
                    Session["inquiry_request_INQ_id"] = inquery.inquery_status_FK_Inquery;
                    Session["inquery_request_material"] = inquery.Inquery_tbl.inquery_material;
                    Session["inquery_request_quantity"] = inquery.Inquery_tbl.inquery_quantity;
                    Session["inquery_request_technical_character"] = inquery.Inquery_tbl.inquery_technical_character;
                    Session["inquery_request_dedline"] = inquery.Inquery_tbl.inquery_deadline;
                    Session["inquery_request_FK_Emp"] = inquery.inquery_status_FK_Emp;
                    Session["inquery_request_FK_Branch"] = inquery.inquery_status_FK_Branch;
                    return View(inquery);
                }
            }
            return HttpNotFound();
        }
        [HttpPost]
        public ActionResult employee_head_tab_send_request(Request_tbl req, string answer)
        {
            if (Session["employee_id"] == null)
            {
                return RedirectToAction("Index");
            }

            List<Employee_tbl> li = db.Employee_tbl.Where(x => x.position_FK_emp == 3).ToList();
            ViewBag.list = new SelectList(li, "employee_id", "Position_tbl.position_name");

            Request_tbl request = new Request_tbl();

            if (ModelState.IsValid && !String.IsNullOrWhiteSpace(answer))
            {
                switch (answer)
                {
                    case "Отправить":
                        request.request_FK_emp_head = Convert.ToInt32(Session["employee_id"]);
                        request.request_material = Convert.ToString(Session["inquery_request_material"]);
                        request.request_quanitity = Convert.ToInt32(Session["inquery_request_quantity"]);
                        request.request_technical_character = Convert.ToString(Session["inquery_request_technical_character"]);
                        request.request_date_fix = DateTime.Now;
                        request.request_deadline = Convert.ToDateTime(Session["inquery_request_dedline"]);
                        request.request_FK_branch_send = 3;
                        request.request_FK_inquery = Convert.ToInt32(Session["inquiry_request_INQ_id"]);
                        request.request_FK_branch = Convert.ToInt32(Session["inquery_request_FK_Branch"]); ;
                        break;
                    default:
                        ViewBag.error = "Something went wrong";
                        break;
                }
            }
            db.Request_tbl.Add(request);
            db.SaveChanges();
            return RedirectToAction("employee_head_tab_accepted_inquiry");

        }
        public ActionResult employee_head_tab_branch_request(int? id, int? page)
        {
            if (Session["employee_id"] == null)
            {
                return RedirectToAction("Index");
            }
            int pagesize = 15, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            int employee = Convert.ToInt32(Session["employee_id"]);
            List<Request_tbl> li = db.Request_tbl.Where(x => x.request_FK_emp_head == employee).ToList();
            IPagedList<Request_tbl> ind = li.ToPagedList(pageindex, pagesize);
            return View(ind);
        }
        public async Task<ActionResult> employee_head_tab_send_request_document(int? id)
        {
            if (Session["employee_id"] == null)
            {
                return RedirectToAction("Index");
            }
            if (id != null)
            {
                Request_tbl request = await db.Request_tbl.FirstOrDefaultAsync(x => x.request_id == id);
                if (request != null)
                {
                    Session["request_id"] = request.request_id;
                    Session["request_emp_head_name"] = request.Employee_tbl.employee_fullname;
                    Session["request_date"] = request.request_date_fix;
                    Session["request_material"] = request.request_material;
                    Session["request_technical_character"] = request.request_technical_character;
                    Session["request_quantity"] = request.request_quanitity;
                    Session["request_dedline"] = request.request_deadline;
                    Session["request_department_ot"] = request.Branches_tbl.branch_name;
                    Session["request_department_k"] = request.Branches_tbl1.branch_name;
                    return View(request);
                }
            }
            return HttpNotFound();
        }









        //админ
        public ActionResult SignIn_admin()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SignIn_admin(Admin_tbl adm)
        {
            Admin_tbl admin = db.Admin_tbl.Where(x => x.admin_login == adm.admin_login && x.admin_password == adm.admin_password).SingleOrDefault();
            if (admin != null)
            {
                Session["admin_id"] = admin.admin_id;
                Session["admin_login"] = admin.admin_login;
                return RedirectToAction("admin_tab");
            }
            else
            {
                ViewBag.msg = "Неверный логин или пароль!";
            }
            return View();
        }
        public ActionResult admin_tab()
        {
            if (Session["admin_id"] == null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }
        public ActionResult admin_tab_supplier()
        {
            if (Session["admin_id"] == null)
            {
                return RedirectToAction("Index");
            }

            return View();
        }
        public ActionResult admin_tab_employee()
        {
            if (Session["admin_id"] == null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }
        public ActionResult admin_tab_supplier_list(int? id, int? page)
        {
            if (Session["admin_id"] == null)
            {
                return RedirectToAction("Index");
            }
            int pagesize = 15, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            List<Suppliers_tbl> li = db.Suppliers_tbl.ToList();
            IPagedList<Suppliers_tbl> ind = li.ToPagedList(pageindex, pagesize);
            return View(ind);
        }
        public ActionResult Delete_supplier(int id)
        {
            var model = db.Suppliers_tbl.Find(id);
            db.Suppliers_tbl.Remove(model);
            db.SaveChanges();
            return RedirectToAction("admin_tab_supplier_list");
        }
        [HttpGet]
        public ActionResult Edit_supplier(int? id)
        {
            var model = db.Suppliers_tbl.Find(id);
           // int sid = Convert.ToInt32(Session["ad_id"]);
            //List<Caegory_tbl> li = db.Caegory_tbl.Where(x => x.cat_fk_adid == sid).ToList();
            //ViewBag.list = new SelectList(li, "cat_id", "cat_name");
            return View(model);
        }
        [HttpPost]
        public ActionResult Edit_supplier(Suppliers_tbl sup)
        {
            //int sid = Convert.ToInt32(Session["ad_id"]);
            //List<Caegory_tbl> li = db.Caegory_tbl.Where(x => x.cat_fk_adid == sid).ToList();
            //ViewBag.list = new SelectList(li, "cat_id", "cat_name");
            db.Entry(sup).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("admin_tab_supplier_list");
        }
        public async Task<ActionResult> Details_supplier(int? id)
        {
            if (id != null)
            {
                Suppliers_tbl supplier = await db.Suppliers_tbl.FirstOrDefaultAsync(x => x.supplier_id == id);
                if (supplier != null)
                {
                    return View(supplier);
                }
            }
            return HttpNotFound();
        }
        [HttpGet]
        public ActionResult admin_tab_employee_regist()
        {
            List<Branches_tbl> li = db.Branches_tbl.ToList();
            ViewBag.list = new SelectList(li, "branch_id", "branch_name");
            List<Position_tbl> position = db.Position_tbl.ToList();
            ViewBag.list_position = new SelectList(position, "position_id", "position_name");
            return View();
        }
        [HttpPost]
        public ActionResult admin_tab_employee_regist(Employee_tbl emp)
        {
            if (Session["admin_id"] == null)
            {
                return RedirectToAction("Index");
            }
            List<Branches_tbl> li = db.Branches_tbl.ToList();
            ViewBag.list = new SelectList(li, "branch_id", "branch_name");
            List<Position_tbl> position = db.Position_tbl.ToList();
            ViewBag.list_position = new SelectList(position, "position_id", "position_name");
            Employee_tbl employee = new Employee_tbl();
            try
            {
                employee.employee_login = emp.employee_login;
                employee.employee_password = emp.employee_password;
                employee.employee_fullname = emp.employee_fullname;
                employee.employee_birthday = emp.employee_birthday;
                employee.employee_phnumber = emp.employee_phnumber;
                employee.branch_FK_emp = emp.branch_FK_emp;
                employee.position_FK_emp = emp.position_FK_emp;
                db.Employee_tbl.Add(employee);
                db.SaveChanges();
                return RedirectToAction("admin_tab_employee");
            }
            catch (Exception)
            {
                ViewBag.msg = "Не удалось зарегистрировать!";
            }
            return View();
        }
        public ActionResult admin_tab_employee_list(int? id, int? page)
        {
            if (Session["admin_id"] == null)
            {
                return RedirectToAction("Index");
            }
            int pagesize = 15, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            List<Employee_tbl> li = db.Employee_tbl.ToList();
            IPagedList<Employee_tbl> ind = li.ToPagedList(pageindex, pagesize);
            return View(ind);
        }
        public ActionResult Delete_admin_employee(int id)
        {
            if (Session["admin_id"] == null)
            {
                return RedirectToAction("Index");
            }
            var model = db.Employee_tbl.Find(id);
            db.Employee_tbl.Remove(model);
            db.SaveChanges();
            return RedirectToAction("admin_tab_employee_list");
        }
        [HttpGet]
        public ActionResult Edit_admin_employee(int? id)
        {
            if (Session["admin_id"] == null)
            {
                return RedirectToAction("Index");
            }
            var model = db.Employee_tbl.Find(id);
            List<Branches_tbl> li = db.Branches_tbl.ToList();
            ViewBag.list = new SelectList(li, "branch_id", "branch_name");
            return View(model);
        }
        [HttpPost]
        public ActionResult Edit_admin_employee(Suppliers_tbl sup)
        {
            if (Session["admin_id"] == null)
            {
                return RedirectToAction("Index");
            }
            List<Branches_tbl> li = db.Branches_tbl.ToList();
            ViewBag.list = new SelectList(li, "branch_id", "branch_name");
            db.Entry(sup).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("admin_tab_employee_list");
        }
        public async Task<ActionResult> Details_admin_employee(int? id)
        {
            if (Session["admin_id"] == null)
            {
                return RedirectToAction("Index");
            }
            if (id != null)
            {
                Employee_tbl employee = await db.Employee_tbl.FirstOrDefaultAsync(x => x.employee_id == id);
                if (employee != null)
                {
                    return View(employee);
                }
            }
            return HttpNotFound();
        }
        [HttpGet]
        public ActionResult admin_tab_branch()
        {
            if (Session["admin_id"] == null)
            {
                return RedirectToAction("Index");
            }
            int admin_id = Convert.ToInt32(Session["admin_id"].ToString());
            List<Branches_tbl> li = db.Branches_tbl.OrderByDescending(x => x.branch_id).ToList();
            ViewData["List"] = li;
            return View();
        }
        [HttpPost]
        public ActionResult admin_tab_branch(Branches_tbl bra)
        {
            List<Branches_tbl> li = db.Branches_tbl.OrderByDescending(x => x.branch_id).ToList();
            ViewData["List"] = li;
            Branches_tbl branch = new Branches_tbl();
            branch.branch_name = bra.branch_name;
            db.Branches_tbl.Add(branch);
            db.SaveChanges();
            return RedirectToAction("admin_tab_branch");
        }
        public ActionResult admin_tab_employee_inquiry_list(int? id, int? page)
        {
            if (Session["admin_id"] == null)
            {
                return RedirectToAction("Index");
            }
            int pagesize = 15, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            List<Inquery_tbl> li = db.Inquery_tbl.ToList();
            IPagedList<Inquery_tbl> ind = li.ToPagedList(pageindex, pagesize);
            return View(ind);
        }
        [HttpGet]
        public ActionResult admin_tab_new_position()
        {
            if (Session["admin_id"] == null)
            {
                return RedirectToAction("Index");
            }
            int admin_id = Convert.ToInt32(Session["admin_id"].ToString());
            List<Position_tbl> li = db.Position_tbl.OrderByDescending(x => x.position_id).ToList();
            ViewData["List"] = li;
            return View();
        }
        [HttpPost]
        public ActionResult admin_tab_new_position(Position_tbl pos)
        {
            List<Position_tbl> li = db.Position_tbl.OrderByDescending(x => x.position_id).ToList();
            ViewData["List"] = li;
            Position_tbl position = new Position_tbl();
            position.position_name = pos.position_name;
            db.Position_tbl.Add(position);
            db.SaveChanges();
            return RedirectToAction("admin_tab_new_position");
        }

        public ActionResult admin_tab_supplier_list_order(int? id, int? page)
        {
            if (Session["admin_id"] == null)
            {
                return RedirectToAction("Index");
            }
            int pagesize = 15, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            List<Order_status_tbl> li = db.Order_status_tbl.ToList();
            IPagedList<Order_status_tbl> ind = li.ToPagedList(pageindex, pagesize);
            return View(ind);
        }
    }
}