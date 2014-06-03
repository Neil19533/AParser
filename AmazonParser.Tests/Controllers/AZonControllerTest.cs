using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Net;

namespace AmazonParser.Tests.Controllers
{
    [TestClass]
    public class AZonControllerTest
    {
        [TestMethod]
        public void GetReturn404()
        {
            //Arange
            var AmzController = new AmazonParser.Controllers.AZonController();

            
            var Expectedresult = new HttpResponseMessage();
            Expectedresult.StatusCode = HttpStatusCode.NotFound;
            //Act
            var Result = AmzController.Get("B001RYQJQ2");


            //Assert
            Assert.AreEqual(Expectedresult.StatusCode, Result.StatusCode);

        }
    }
}
