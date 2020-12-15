using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using PharmacyMedicineSupplyService.Controllers;
using PharmacyMedicineSupplyService.Models;
using PharmacyMedicineSupplyService.Provider;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PharmacyMedicineSupplyTest
{
    class PharmacyMedicineSupplyControllerTest
    {
        List<PharmacyMedicineSupply> supplyList;
        Mock<IPharmacySupplyProvider> providerRepo;
        List<MedicineDemand> demand,emptyDemand,wrongDemand;
        [SetUp]
        public void Setup()
        {
            emptyDemand = new List<MedicineDemand>();
            demand = new List<MedicineDemand>() {
                new MedicineDemand{MedicineName="Medicine1",Count=18 }
            };
            wrongDemand = new List<MedicineDemand>() {
                new MedicineDemand{MedicineName="Medicine8",Count=18 }
            };
            supplyList = new List<PharmacyMedicineSupply>()
            {
                new PharmacyMedicineSupply{ PharmacyName="Appolo Pharmacy",MedicineName="Medicine1",SupplyCount=18},

            };
            providerRepo = new Mock<IPharmacySupplyProvider>();
        }
        [Test]
        public void TestControllerLayerCorrectInput()
        {
            providerRepo.Setup(m => m.GetSupply(demand)).Returns(Task.FromResult(supplyList));
            var pro = new PharmacySupplyController(providerRepo.Object);
            var res = pro.Get(demand).Result as ObjectResult;
            Assert.AreEqual(res.StatusCode, 200);
        }
        [Test]
        public void TestControllerLayerIncorrectInput1()
        {
            providerRepo.Setup(m => m.GetSupply(wrongDemand)).Returns(Task.FromResult(new List<PharmacyMedicineSupply>()));
            var pro = new PharmacySupplyController(providerRepo.Object);
            var res = pro.Get(wrongDemand).Result as ObjectResult;
            Assert.AreEqual(res.StatusCode, 404);
        }
       
    }
}
