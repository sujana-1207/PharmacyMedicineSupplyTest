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
        Mock<ISupplyRepo> supplyRepo;
        List<PharmacyDTO> pharmacies;
        List<string> stock;
        Mock<IMedicineStockProvider> proMock;
        List<MedicineDemand> x;
        [SetUp]
        public void Setup()
        {
            pharmacies = new List<PharmacyDTO> {
            new PharmacyDTO{ pharmacyName="Appolo Pharmacy"}
            };
            stock = new List<string>() { "Medicine1", "Medicine2" };
            x = new List<MedicineDemand>(){
                new MedicineDemand{MedicineName="Medicine1",Count=18 }
            };
            supplyRepo = new Mock<ISupplyRepo>();
            supplyRepo.Setup(m => m.GetPharmacies()).Returns(pharmacies);
            proMock = new Mock<IMedicineStockProvider>();
           
        }

        [Test]
        public async Task TestProviderLayerEnoughStock()
        {
            proMock.Setup(s => s.GetStock("Medicine1")).Returns(Task.FromResult(10));
            var pro = new PharmacySupplyProvider(supplyRepo.Object,proMock.Object);
            List<PharmacyMedicineSupply> res=await pro.GetSupply(x);
            supplyList = new List<PharmacyMedicineSupply>
            {
                new PharmacyMedicineSupply{ PharmacyName="Appolo Pharmacy",MedicineName="Medicine1",SupplyCount=10},
                
            };
            Assert.AreEqual(supplyList[0].SupplyCount, res[0].SupplyCount);
            Assert.AreEqual(supplyList[0].MedicineName, res[0].MedicineName);
            Assert.AreEqual(supplyList[0].PharmacyName, res[0].PharmacyName);
        }
        [Test]
        public async Task TestProviderLayerNotEnoughStock()
        {
            proMock.Setup(s => s.GetStock("Medicine1")).Returns(Task.FromResult(10));
            var pro = new PharmacySupplyProvider(supplyRepo.Object,proMock.Object);
            List<MedicineDemand> x = new List<MedicineDemand>(){
                new MedicineDemand{MedicineName="Medicine1",Count=55 }
            };
            List<PharmacyMedicineSupply> res = await pro.GetSupply(x);
            supplyList = new List<PharmacyMedicineSupply>
            {
                new PharmacyMedicineSupply{ PharmacyName="Appolo Pharmacy",MedicineName="Medicine1",SupplyCount=10},

            };
            Assert.AreEqual(supplyList[0].SupplyCount, res[0].SupplyCount);
            Assert.AreEqual(supplyList[0].MedicineName, res[0].MedicineName);
            Assert.AreEqual(supplyList[0].PharmacyName, res[0].PharmacyName);
        }
        [Test]
        public async Task TestProviderLayerNoMedicine()
        {
            proMock.Setup(s => s.GetStock("Medicine8")).Returns(Task.FromResult(-1)) ;
            var pro = new PharmacySupplyProvider(supplyRepo.Object,proMock.Object);
            List<MedicineDemand> x = new List<MedicineDemand>(){
                new MedicineDemand{MedicineName="Medicine8",Count=21 }
            };
            List<PharmacyMedicineSupply> res = await pro.GetSupply(x);

            Assert.IsTrue(res.IsNullOrEmpty());
            
        }
        [Test]
        public async Task TestProviderLayerEnoughStockNotDivisible()
        {
            proMock.Setup(s => s.GetStock(It.IsAny<string>())).Returns(Task.FromResult(50));
            pharmacies.Add(new PharmacyDTO { pharmacyName = "G.K Pharmacies" });
            var pro = new PharmacySupplyProvider(supplyRepo.Object,proMock.Object);
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