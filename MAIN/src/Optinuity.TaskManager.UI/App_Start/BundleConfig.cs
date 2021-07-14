using System.Web;
using System.Web.Optimization;

namespace Optinuity.TaskManager.UI
{
    /// <summary>
    /// Bundle Config
    /// </summary>
    public class BundleConfig
    {
        /// <summary>
        /// Registers the bundles.
        /// </summary>
        /// <param name="bundles">The bundles.</param>
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/content/js/bundled_js").Include(
                        "~/Content/js/jquery/jquery.js",
                       // "~/Content/js/jquery/jquery.mobile.custom.js",
                        "~/Content/js/jquery/jquery-migrate.js",
                        "~/Content/js/jquery/jquery-ui.js",
                        "~/Content/js/jquery/jquery.cookie.js",
                        "~/Content/js/jquery.validate.js",
                        "~/Content/js/jquery.validate.unobtrusive.js",
                        "~/Content/js/plugins/jquery_ui_touch_punch/jquery.ui.touch-punch.js",
                        "~/Content/js/plugins/modernizr/modernizr.js",
                        "~/Content/js/bootstrap.js",
                        "~/Content/js/flatty-theme.js",
                        "~/Content/js/jquery.inputmask.bundle.js",
                        "~/Content/js/demo.js",
                        "~/Content/js/bootstrap-growl.js",
                        "~/Content/js/bootstrap-datepicker.js",
                        "~/Content/js/TaskManager.js",
                       // "~/Content/js/bootstrap-multiselect.js",
                        "~/Content/js/custom.js"
                        ));

            bundles.Add(new StyleBundle("~/content/css/bundled_css").Include(
                "~/Content/css/bootstrap.css",
                "~/Content/css/national-light-theme.css",
                "~/Content/css/custom.css",
                "~/Content/css/datepicker.css",
               // "~/Content/css/bootstrap-multiselect.css",
                "~/Content/css/print.css"
                //"~/Content/js/summernote.css" 
            ));


            bundles.Add(new StyleBundle("~/content/css/plugins/jquery-ui/cupertino/jquery-ui_bundled").Include(
                 "~/Content/css/plugins/jquery-ui/cupertino/jquery-ui.css"
            ));


        }
    }
}