using System.Web;
using System.Web.Optimization;

namespace RRHHApp
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
           
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/css/bootstrap.min.css",
                      "~/Content/css/font-awesome.min.css",
                      "~/Content/css/datepicker3.css",
                      "~/Content/css/styles.css"));

            bundles.Add(new ScriptBundle("~/Content/js").Include(
                        "~/Content/js/jquery-1.11.1.min.js",
                        "~/Content/js/bootstrap.min.js",
                        "~/Content/js/chart.min.js",
                        "~/Content/js/chart-data.js",
                        "~/Content/js/easypiechart.js",
                        "~/Content/js/easypiechart-data.js",
                        "~/Content/js/bootstrap-datepicker.js",
                        "~/Content/js/custom.js"));

        }
    }
}
