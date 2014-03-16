using System;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using StructureMap;

namespace Tweets
{
    public class StructureMapControllersFactory : DefaultControllerFactory
    {
        private readonly Container container;

        public StructureMapControllersFactory(Container container)
        {
            this.container = container;
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
                throw new HttpException((int) HttpStatusCode.NotFound, requestContext.HttpContext.Request.Path);
            return (IController) container.GetInstance(controllerType);
        }
    }
}