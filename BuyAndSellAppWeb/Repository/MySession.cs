using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuyAndSellAppWeb.Repository
{
    public class MySession
    {
        public MySession() { }

        // Gets the current session.
        public static MySession Current
        {
            get
            {
                MySession session =
                    (MySession)HttpContext.Current.Session["__MySession__"];
                if (session == null)
                {
                    session = new MySession();
                    HttpContext.Current.Session["__MySession__"] = session;
                }
                return session;

            }
        }
        public SharePointContext spcontext { get; set; }

    }
}
