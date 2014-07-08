using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimsTemplate.Models.ViewModels;
using System.IO;
using System.Web.Helpers;
using SimsTemplate.Models;
using System.ComponentModel.DataAnnotations;

namespace SimsTemplate.Controllers
{

    /// <summary>
    /// @Author: Aubrey Fowler
    /// Avatar Controller - will handle the actions 
    /// the user will take to upload a new picture
    /// and edit the picture.
    /// </summary>
    public class AvatarController : Controller
    {

        /// <summary>
        /// Connect to the database.
        /// </summary>
        TechproContext db = new TechproContext();

        /// <summary>
        /// @Author: Aubrey Fowler
        /// GET: /Avatar/
        /// Avatar home page.
        /// User can upload a new picture here if they want a new avatar.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            //check if they are logged in
            if (SessionHandler.Logon)
            {
                AvatarViewModel avm = new AvatarViewModel();
                return View(avm);
            }
            else 
            {
                //kick them out if they are not logged in
                return RedirectToAction("Index","Login");                    
            }

        }

        /// <summary>
        /// @Author: Aubrey Fowler
        /// POST :/Upload/
        /// Upload your avatar picture.
        /// Will save the chosen image to the directory: /Content/ProfileImages/
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Upload(AvatarViewModel model)
        {

            var image = WebImage.GetImageFromRequest();

            if (image != null)
            {

                 //resize it if it is huge :: 900 max
                if (image.Width > 900)
                {
                    image.Resize(900, ((900 * image.Height) / image.Width));
                }

                var filename = Path.GetFileName(image.FileName);

                //check if it is an image
                if (!filename.EndsWith("jpg", false, null) && !filename.EndsWith("jpeg", false, null))
                {
                    return View("~/Views/Avatar/Index.cshtml", model);
                }

                image.Save(Path.Combine("~/Content/ProfileImages/", filename));
                filename = Path.Combine("~/Content/ProfileImages/", filename);
                model.ImageUrl = Url.Content(filename);

                var editModel = new AvatarEditorModel()
                   {
                       Avatar = model,
                       Width = image.Width,
                       Height = image.Height,
                       Top = image.Height * 0.1,
                       Left = image.Width * 0.9,
                       Right = image.Width * 0.9,
                       Bottom = image.Height * 0.9
                   };

                //we can edit the image now
                return View("~/Views/Avatar/Editor.cshtml", editModel);

            }

            //there was no image so go back to the main page
            return View("~/Views/Avatar/Index.cshtml", model);
        }

        /// <summary>
        /// @Author: Aubrey Fowler
        /// POST :/Edit/
        /// Crop your avatar picture.
        /// Crops the image, saves it to the folder and updates the database.
        /// </summary>
        /// <param name="editor"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(AvatarEditorModel editor)
        {

            try
            {

                //create a new web image based on the one that was provided by the user
                var image = new WebImage(@"~" + editor.Avatar.ImageUrl);

                //perform the crop
                var height = image.Height;
                var width = image.Width;

                int topcrop = Convert.ToInt32(editor.Top);
                int leftcrop = Convert.ToInt32(editor.Left);
                int off = Convert.ToInt32(height - editor.Bottom);
                int offagain = Convert.ToInt32(width - editor.Right);

                //var originalFile = editor.Avatar.ImageUrl;
                image.Crop(topcrop, leftcrop, off, offagain);

                editor.Avatar.ImageUrl = Url.Content("/Content/ProfileImages/" + Path.GetFileName(image.FileName));
                image.Resize(260, 260, true, false);

                image.Save(@"~" + editor.Avatar.ImageUrl);

                //save the changes to the database - this code works
                User v = db.Users.Single(vh => vh.id == SessionHandler.UID);
                v.image_url = @"~" + editor.Avatar.ImageUrl;
                db.SaveChanges();
                
                //System.IO.File.Delete(Server.MapPath(originalFile));

                ViewBag.Message = "\nimage: " + editor.Avatar.ImageUrl + " \nTop:" 
                    + editor.Top + " \nBottom: " + editor.Bottom + " \nLeft: "
                    + editor.Left + " \nRight:" + editor.Right + " \nHeight:"
                    + editor.Height + " \nwidth:" + editor.Width
                    + " \nTOPPP: " + off + " \nWIDE: " + offagain;

                //go to the profile page
                return RedirectToAction("Index" , "Profile");

            }
            catch (DirectoryNotFoundException ex)
            {
                ViewBag.Message = "" + ex.Message + "___ directory not found." + " Top:" 
                    + editor.Top + " Bottom: " + editor.Bottom + " Left: "
                    + editor.Left + " Right:" + editor.Right 
                    + " Height:" + editor.Height 
                    + " width:"
                    + editor.Width;
                return View("~/Views/Avatar/Index.cshtml", editor.Avatar);
            }
            catch (InvalidOperationException ex)
            {
                ViewBag.Message = "" + ex.Message + "___ failed to save to the database." 
                    + " Top:" + editor.Top + " Bottom: " + editor.Bottom + " Left: "
                    + editor.Left + " Right:" + editor.Right + " Height:" + editor.Height
                    + " width:" + editor.Width;
                return View("~/Views/Avatar/Index.cshtml", editor.Avatar);
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex) 
            {
                ViewBag.Message = "" + ex.Message + "___ file path is too long - failed to upload." + " Top:" + editor.Top + " Bottom: " + editor.Bottom + " Left: "
                    + editor.Left + " Right:"
                    + editor.Right + " Height:" + editor.Height + " width:" + editor.Width 
                    ;
                return View("~/Views/Avatar/Index.cshtml", editor.Avatar);
            }
            catch (Exception ex)
            {
                ViewBag.Message = "" + ex.Message + " Top:" + editor.Top + " Bottom: " + editor.Bottom + " Left: "
                    + editor.Left + " Right:" + editor.Right
                    + " Height:" + editor.Height + " width:" + editor.Width 
                  ;
                return View("~/Views/Avatar/Index.cshtml", editor.Avatar);
            }



        }

    }
}




