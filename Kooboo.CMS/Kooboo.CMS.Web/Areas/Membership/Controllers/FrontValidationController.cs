using Kooboo.CMS.Common;
using Kooboo.CMS.Membership.Models;
using Kooboo.CMS.Membership.Services;
using Kooboo.CMS.Sites;
using Kooboo.CMS.Web.Areas.Membership.Models;
using Kooboo.CMS.Common.Persistence.Non_Relational;
using Kooboo.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kooboo.CMS.Membership.Persistence;
using Kooboo.CMS.Common.DataViolation;

namespace Kooboo.CMS.Web.Areas.Membership.Controllers
{
    // allow anonymos user access to username and email validation
    //[Kooboo.CMS.Web.Authorizations.Authorization(AreaName = "Membership", Group = "", Name = "Member", Order = 1)]
    public class FrontValidationController : ControllerBase
    {
        #region .ctor
        MembershipUserManager _manager = null;
        IMembershipUserProvider _membershipUserProvider = null;
        public FrontValidationController(MembershipUserManager manager, IMembershipUserProvider provider)
        {
            this._manager = manager;
            this._membershipUserProvider = provider;
        }
        #endregion

        public virtual ActionResult IsUserNameAvailable(string userName)
        {
            var membershipUser = new MembershipUser() { Membership = Membership, UserName = userName }.AsActual();
            if (membershipUser != null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public virtual ActionResult IsEmailAvailable(string email)
        {
            var membershipUser = _membershipUserProvider.QueryUserByEmail(Membership, email);
            if (membershipUser != null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
  
    }
}