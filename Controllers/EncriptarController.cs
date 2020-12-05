using ExercicioCrpito.Context;
using ExercicioCrpito.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace Prova_Encripitar.Controllers
{
    public class EncriptarController : Controller
    {

        private readonly Contexto db = new Contexto();
        private static string AesIV256BD = @"%j?TmFP6$B45lk$@";
        private static string AesKey256BD = @"rxmBUJy]&,;3jKwDTzf(cui$<nc2EQr)";


        public ActionResult Inicio() 
        {
            return View();
        }

        // GET: Encriptar
        public ActionResult Index()
        {
            //List< ExercicioCrpito.Models.EncriptarModel> encriptars = db.encriptars.ToList();
            //AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            //aes.BlockSize = 128;
            //aes.KeySize = 256;
            //aes.IV = Encoding.UTF8.GetBytes(AesIV256BD);
            //aes.Key = Encoding.UTF8.GetBytes(AesKey256BD);
            //aes.Mode = CipherMode.CBC;
            //aes.Padding = PaddingMode.PKCS7;

            //for (int i = 0; i < encriptars.Count; i++)
            //{
            //    byte[] src = System.Convert.FromBase64String(encriptars[i].Texto);
            //    using (ICryptoTransform decrypt = aes.CreateDecryptor())
            //    {
            //        byte[] dest = decrypt.TransformFinalBlock(src, 0, src.Length);
            //        encriptars[i].Texto = Encoding.Unicode.GetString(dest);
            //    }
            //}
            return View(db.encriptars.ToList());
        }
        

        #region Create
        public ActionResult Create()
        {
            return View();
        }
        #endregion

        #region Create - Post

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Create(EncriptarModel encriptarModel)
        {
            if (ModelState.IsValid)
            {
                AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
                aes.BlockSize = 128;
                aes.KeySize = 256;
                aes.IV = Encoding.UTF8.GetBytes(AesIV256BD);
                aes.Key = Encoding.UTF8.GetBytes(AesKey256BD);
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                byte[] src = Encoding.Unicode.GetBytes(encriptarModel.Texto);

                using (ICryptoTransform encrypt = aes.CreateEncryptor())
                {
                    byte[] dest = encrypt.TransformFinalBlock(src, 0, src.Length);

                    //Converte byte array para string de base 64
                    encriptarModel.Texto = Convert.ToBase64String(dest);
                }
                db.encriptars.Add(encriptarModel);
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(encriptarModel);
        }

        #endregion

        #region Edit

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            ExercicioCrpito.Models.EncriptarModel encriptarModel = db.encriptars.Find(id);
            if (encriptarModel == null)
            {
                return HttpNotFound();
            }

            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            aes.BlockSize = 128;
            aes.KeySize = 256;
            aes.IV = Encoding.UTF8.GetBytes(AesIV256BD);
            aes.Key = Encoding.UTF8.GetBytes(AesKey256BD);
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            //Converter string para um byte array 64bits
            byte[] src = Convert.FromBase64String(encriptarModel.Texto);

            //Decripitar
            using (ICryptoTransform decrypt = aes.CreateDecryptor())
            {
                byte[] dest = decrypt.TransformFinalBlock(src, 0, src.Length);
                encriptarModel.Texto = Encoding.Unicode.GetString(dest);
            }

            return View(encriptarModel);
        }

        #endregion

        #region Edit - Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EncriptarModel encriptarModel)
        {
            ExercicioCrpito.Models.EncriptarModel encriptar = db.encriptars.Find(encriptarModel.id);
            encriptarModel.Texto = encriptar.Texto;
 

            db.Entry(encriptar).State = EntityState.Detached;

            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            aes.BlockSize = 128;
            aes.KeySize = 256;
            aes.IV = Encoding.UTF8.GetBytes(AesIV256BD);
            aes.Key = Encoding.UTF8.GetBytes(AesKey256BD);
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            byte[] src = Encoding.Unicode.GetBytes(encriptarModel.Texto);

            using (ICryptoTransform encrypt = aes.CreateEncryptor())
            {
                byte[] dest = encrypt.TransformFinalBlock(src, 0, src.Length);

                //Converte byte array para string de base 64
                encriptarModel.Texto = Convert.ToBase64String(dest);
            }


            db.Entry(encriptarModel).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        #endregion
    }
}