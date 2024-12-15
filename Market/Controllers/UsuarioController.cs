using Market.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Market.Controllers
{
    public class UsuarioController : Controller
    {
        dbMarketplaceEntities db = new dbMarketplaceEntities();
        // GET: User
        public ActionResult Index(int? page)
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.tbl_categoria.Where(x => x.cat_estado == 1).OrderByDescending(x => x.cat_id).ToList();
            IPagedList<tbl_categoria> stu = list.ToPagedList(pageindex, pagesize);


            return View(stu);
        }
        public ActionResult Registrar()
        {

            return View();
        }

        [HttpPost]
        public ActionResult Registrar(tbl_usuario uvm, HttpPostedFileBase imgfile)
        {
            string path = subirArchivo(imgfile);
            if (path.Equals("-1"))
            {
                ViewBag.error = "No se pudo subir la imagen....";
            }
            else
            {
                tbl_usuario u = new tbl_usuario();
                u.u_nombre = uvm.u_nombre;
                u.u_email = uvm.u_email;
                u.u_clave = uvm.u_clave;
                u.u_imagen = path;
                u.u_contacto = uvm.u_contacto;
                db.tbl_usuario.Add(u);
                db.SaveChanges();
                return RedirectToAction("Login");

            }

            return View();
        } //method......................... end.....................

        public ActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Login(tbl_usuario avm)
        {
            tbl_usuario ad = db.tbl_usuario.Where(x => x.u_email == avm.u_email && x.u_clave == avm.u_clave).SingleOrDefault();
            if (ad != null)
            {

                Session["u_id"] = ad.u_id.ToString();
                return RedirectToAction("Index");

            }
            else
            {
                ViewBag.error = "Usuario o contraseña incorrecto";

            }

            return View();
        }


        [HttpGet]
        public ActionResult CreatePublic()
        {
            List<tbl_categoria> li = db.tbl_categoria.ToList();
            ViewBag.categorylist = new SelectList(li, "cat_id", "cat_nombre");
            return View();
        }

        [HttpPost]
        public ActionResult CreatePublic(tbl_producto pvm, HttpPostedFileBase imgfile)
        {
            List<tbl_categoria> li = db.tbl_categoria.ToList();
            ViewBag.categorylist = new SelectList(li, "cat_id", "cat_nombre");


            string path = subirArchivo(imgfile);
            if (path.Equals("-1"))
            {
                ViewBag.error = "La imagen no se pudo subir....";
            }
            else
            {
                tbl_producto p = new tbl_producto();
                p.pro_nombre = pvm.pro_nombre;
                p.pro_precio = pvm.pro_precio;
                p.pro_imagen = path;
                p.pro_fk_cat = pvm.pro_fk_cat;
                p.pro_descrip = pvm.pro_descrip;
                p.pro_fk_user = Convert.ToInt32(Session["u_id"].ToString());
                db.tbl_producto.Add(p);
                db.SaveChanges();
                Response.Redirect("Index");

            }

            return View();
        }


        public ActionResult Publicaciones(int? id, int? page)
        {
            int pagesize = 6, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;

            // Si 'id' es nulo, lista todos los productos, de lo contrario filtra por categoría.
            var list = id.HasValue
                ? db.tbl_producto.Where(x => x.pro_fk_cat == id).OrderByDescending(x => x.pro_id).ToList()
                : db.tbl_producto.OrderByDescending(x => x.pro_id).ToList();

            IPagedList<tbl_producto> stu = list.ToPagedList(pageindex, pagesize);

            return View(stu);
        }


        [HttpPost]
        public ActionResult Publicaciones(int? id, int? page, string buscar)
        {
            int pagesize = 6, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;

            // Si 'buscar' tiene valor, busca productos por nombre.
            var list = !string.IsNullOrWhiteSpace(buscar)
                ? db.tbl_producto.Where(x => x.pro_nombre.Contains(buscar)).OrderByDescending(x => x.pro_id).ToList()
                : db.tbl_producto.Where(x => x.pro_fk_cat == id || !id.HasValue).OrderByDescending(x => x.pro_id).ToList();

            IPagedList<tbl_producto> stu = list.ToPagedList(pageindex, pagesize);

            return View(stu);
        }




        public ActionResult VerPublicacion(int? id)
        {
            VerPublicacion ad = new VerPublicacion();
            tbl_producto p = db.tbl_producto.Where(x => x.pro_id == id).SingleOrDefault();
            ad.pro_id = p.pro_id;
            ad.pro_nombre = p.pro_nombre;
            ad.pro_imagen = p.pro_imagen;
            ad.pro_precio = p.pro_precio;
            ad.pro_descrip = p.pro_descrip;
            tbl_categoria cat = db.tbl_categoria.Where(x => x.cat_id == p.pro_fk_cat).SingleOrDefault();
            ad.cat_nombre = cat.cat_nombre;
            tbl_usuario u = db.tbl_usuario.Where(x => x.u_id == p.pro_fk_user).SingleOrDefault();
            ad.u_nombre = u.u_nombre;
            ad.u_imagen = u.u_imagen;
            ad.u_contacto = u.u_contacto;
            ad.pro_fk_user = u.u_id;


            return View(ad);
        }


        public ActionResult ModificarPublicacion(int? id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            tbl_producto p = db.tbl_producto.FirstOrDefault(x => x.pro_id == id);
            if (p == null)
            {
                return HttpNotFound();
            }

            VerPublicacion ad = new VerPublicacion
            {
                pro_id = p.pro_id,
                pro_nombre = p.pro_nombre,
                pro_imagen = p.pro_imagen,
                pro_precio = p.pro_precio,
                pro_descrip = p.pro_descrip,
                cat_nombre = db.tbl_categoria.FirstOrDefault(c => c.cat_id == p.pro_fk_cat)?.cat_nombre,
            };


            return View(ad);
        }

        [HttpPost]
        public ActionResult ModificarPublicacion(tbl_producto pvm)
        {
            
                tbl_producto p = db.tbl_producto.FirstOrDefault(x => x.pro_id == pvm.pro_id);
                if (p == null)
                {
                    return HttpNotFound();
                }

                p.pro_nombre = pvm.pro_nombre;                
                p.pro_precio = pvm.pro_precio;
                p.pro_descrip = pvm.pro_descrip;

                db.SaveChanges();

                return RedirectToAction("VerPublicacion", new { id = pvm.pro_id });
        }

            

        public ActionResult Salir()
        {
            Session.RemoveAll();
            Session.Abandon();

            return RedirectToAction("Index");
        }
        public ActionResult DeletePublic(int? id)
        {

            tbl_producto p = db.tbl_producto.Where(x => x.pro_id == id).SingleOrDefault();
            db.tbl_producto.Remove(p);
            db.SaveChanges();

            return RedirectToAction("Index");
        }





        public string subirArchivo(HttpPostedFileBase file)
        {
            Random r = new Random();
            string path = "-1";
            int random = r.Next();
            if (file != null && file.ContentLength > 0)
            {
                string extension = Path.GetExtension(file.FileName);
                if (extension.ToLower().Equals(".jpg") || extension.ToLower().Equals(".jpeg") || extension.ToLower().Equals(".png"))
                {
                    try
                    {

                        path = Path.Combine(Server.MapPath("~/Content/upload"), random + Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        path = "~/Content/upload/" + random + Path.GetFileName(file.FileName);

                        //    ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        path = "-1";
                    }
                }
                else
                {
                    Response.Write("<script>alert('Only jpg ,jpeg or png formats are acceptable....'); </script>");
                }
            }

            else
            {
                Response.Write("<script>alert('Please select a file'); </script>");
                path = "-1";
            }



            return path;
        }

    }
}