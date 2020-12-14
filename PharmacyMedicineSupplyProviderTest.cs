using Castle.Core.Internal;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using PharmacyMedicineSupplyService.Models;
using PharmacyMedicineSupplyService.Provider;
using PharmacyMedicineSupplyService.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PharmacyMedicineSupplyTest
{
    public class Tests
    {
        List<PharmacyMedicineSupply> supplyList;
        Mock<ISupply> supplyRepo;
        List<PharmacyDTO> pharmacies;
        List<MedicineStock> stock;
        [SetUp]
        public void Setup()
        {
            pharmacies = new List<PharmacyDTO> {
            new PharmacyDTO{ pharmacyName="Appolo Pharmacy"}
            };
            stock = new List<MedicineStock>{
             new MedicineStock{ Name="Medicine1",Number_Of_Tablets_In_Stock=20},
             new MedicineStock { Name="Medicine2",Number_Of_Tablets_In_Stock=30}
            };
            supplyRepo = new Mock<ISupply>();
            supplyRepo.Setup(m => m.GetPharmacies()).Returns(pharmacies);

        }

        [Test]
        public async Task TestProviderLayerEnoughStock()
        {
            var pro = new PharmacySupplyProvider(supplyRepo.Object);
            List<MedicineDemand> x = new List<MedicineDemand>(){
                new MedicineDemand{MedicineName="Medicine1",Count=18 }
            };
            List<PharmacyMedicineSupply> res=await pro.GetSupply(x);
            supplyList = new List<PharmacyMedicineSupply>
            {
                new PharmacyMedicineSupply{ PharmacyName="Appolo Pharmacy",MedicineName="Medicine1",SupplyCount=18},
                
            };
            Assert.AreEqual(supplyList[0].SupplyCount, res[0].SupplyCount);
            Assert.AreEqual(supplyList[0].MedicineName, res[0].MedicineName);
            Assert.AreEqual(supplyList[0].PharmacyName, res[0].PharmacyName);
        }
        [Test]
        public async Task TestProviderLayerNotEnoughStock()
        {
            var pro = new PharmacySupplyProvider(supplyRepo.Object);
            List<MedicineDemand> x = new List<MedicineDemand>(){
                new MedicineDemand{MedicineName="Medicine1",Count=55 }
            };
            List<PharmacyMedicineSupply> res = await pro.GetSupply(x);
            supplyList = new List<PharmacyMedicineSupply>
            {
                new PharmacyMedicineSupply{ PharmacyName="Appolo Pharmacy",MedicineName="Medicine1",SupplyCount=50},

            };
            Assert.AreEqual(supplyList[0].SupplyCount, res[0].SupplyCount);
            Assert.AreEqual(supplyList[0].MedicineName, res[0].MedicineName);
            Assert.AreEqual(supplyList[0].PharmacyName, res[0].PharmacyName);
        }
        [Test]
        public async Task TestProviderLayerNoMedicine()
        {
            var pro = new PharmacySupplyProvider(supplyRepo.Object);
            List<MedicineDemand> x = new List<MedicineDemand>(){
                new MedicineDemand{MedicineName="Medicine8",Count=21 }
            };
            List<PharmacyMedicineSupply> res = await pro.GetSupply(x);

            Assert.IsTrue(res.IsNullOrEmpty());
            
        }
        [Test]
        public async Task TestProviderLayerEnoughStockNotDivisible()
        {
            pharmacies.Add(new PharmacyDTO { pharmacyName = "G.K Pharmacies" });
            var pro = new PharmacySupplyProvider(supplyRepo.Object);
            List<MedicineDemand> x = new List<MedicineDemand>(){
                new MedicineDemand{MedicineName="Medicine1",Count=19 }
            };
            List<PharmacyMedicineSupply> res = await pro.GetSupply(x);
            supplyList = new List<PharmacyMedicineSupply>
            {
                new PharmacyMedicineSupply{ PharmacyName="Appolo Pharmacy",MedicineName="Medicine1",SupplyCount=9},
                new PharmacyMedicineSupply{ PharmacyName="G.K Pharmacies",MedicineName="Medicine1",SupplyCount=10}

            };
            Assert.AreEqual(supplyList[0].SupplyCount, res[0].SupplyCount);
            Assert.AreEqual(supplyList[0].MedicineName, res[0].MedicineName);
            Assert.AreEqual(supplyList[0].PharmacyName, res[0].PharmacyName);

            Assert.AreEqual(supplyList[1].SupplyCount, res[1].SupplyCount);
            Assert.AreEqual(supplyList[1].MedicineName, res[1].MedicineName);
            Assert.AreEqual(supplyList[1].PharmacyName, res[1].PharmacyName);
        }
    }
}