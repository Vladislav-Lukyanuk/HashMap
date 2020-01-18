using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using HashMap;
using FizzWare.NBuilder;
using System.Collections.Generic;
using HashMapTests.Model;

namespace HashMapUnitTests
{
    [TestClass]
    public class HashMapTests
    {
        private HashMap<Int32, House> houseMap = new HashMap<Int32, House>(140, 0.5f, 1.5f);

        [TestInitialize]
        public void CleanUpBeforeTesting()
        {
            houseMap.Clear();
        }

        [TestMethod]
        public void TestPuttingObject()
        {
            Int32 key = GetRandomKey();
            House house = GetRandomHouse();
            houseMap.Put(key, house);
            Assert.IsTrue(houseMap.ContainsKey(key));
            Assert.IsTrue(houseMap.ContainsValue(house));
        }

        [TestMethod]
        public void TestGettingObject()
        {
            Int32 key = GetRandomKey();
            House house = GetRandomHouse();
            houseMap.Put(key, house);

            house = houseMap.Get(key);
            Assert.IsNotNull(house);
        }

        [TestMethod]
        public void TestGettingKeys()
        {
            List<Int32> keys = GetRandomKeys(10);
            FillMapOfHouse(keys);

            Assert.AreEqual(houseMap.Size(), keys.Count);
            foreach (Int32 key in houseMap.Keys)
            {
                Assert.IsTrue(keys.Contains(key));
            }
        }

        [TestMethod]
        public void TestGettingValues()
        {
            List<House> houses = GetRandomHouses(10);
            FillMapOfHouse(houses);

            Assert.AreEqual(houseMap.Size(), houses.Count);
            foreach (House house in houseMap.Values)
            {
                Assert.IsTrue(houses.Contains(house));
            }
        }

        [TestMethod]
        public void TestRemoveObject()
        {
            Int32 key = GetRandomKey();
            House house = GetRandomHouse();
            houseMap.Put(key, house);
            House houseToCompare = houseMap.Remove(key);
            Assert.AreEqual(house, houseToCompare);
        }

        [TestMethod]
        public void TestHashMapSize()
        {
            List<House> houses = GetRandomHouses(9);
            FillMapOfHouse(houses);
            Assert.AreEqual(9, houseMap.Size());
        }

        [TestMethod]
        public void TestHashMapClear()
        {
            List<House> houses = GetRandomHouses(9);
            FillMapOfHouse(houses);
            houseMap.Clear();
            Assert.AreEqual(0, houseMap.Size());
        }

        [TestMethod]
        public void TestHashMapContainsValue()
        {
            List<House> houses = GetRandomHouses(3);
            FillMapOfHouse(houses);
            Assert.IsTrue(houseMap.ContainsValue(houses[1]));
        }

        [TestMethod]
        public void TestHashMapContainsKey()
        {
            List<Int32> keys = GetRandomKeys(3);
            FillMapOfHouse(keys);
            Assert.IsTrue(houseMap.ContainsKey(keys[1]));
        }

        [TestMethod]
        public void TestHashMapIsEmpty()
        {
            List<Int32> keys = GetRandomKeys(3);
            FillMapOfHouse(keys);
            Assert.IsFalse(houseMap.isEmpty());
            houseMap.Clear();
            Assert.IsTrue(houseMap.isEmpty());
        }

        [TestMethod]
        public void TestHashMapPutAll()
        {
            HashMap<Int32, House> mapToInsert = GetMapOfHouse(5);
            houseMap.PutAll(mapToInsert);
            Assert.AreEqual(mapToInsert.Size(), houseMap.Size());
            foreach (KeyValuePair<Int32, House> keyPair in mapToInsert)
            {
                House house = houseMap.Remove(keyPair.Key);
                Assert.AreEqual(house, mapToInsert.Get(keyPair.Key));
            }
        }

        [TestMethod]
        public void TestExtendingOfHashMap()
        {
            houseMap.PutAll(GetMapOfHouse(5));
            Int32 size = houseMap.Size();
            Assert.AreEqual(size, houseMap.Size());
            houseMap.PutAll(GetMapOfHouse(150));
            Assert.IsTrue(size < houseMap.Size());
        }

        #region helper functions

        private House GetRandomHouse()
        {
            return Builder<House>.CreateNew()
            .With(c => c.WindowsNumber = Faker.RandomNumber.Next(1, 7))
            .With(c => c.DoorNumber = Faker.RandomNumber.Next(1, 4))
            .With(c => c.FloorsNumber = Faker.RandomNumber.Next(1, 3))
            .With(c => c.HasGarage = true)
            .Build();
        }

        private Int32 GetRandomKey() => Faker.RandomNumber.Next(0, Int32.MaxValue);

        private List<Int32> GetRandomKeys(Int32 size)
        {
            List<Int32> keys = new List<Int32>();

            for (Int32 i = 0; i < size; i++)
            {
                keys.Add(GetRandomKey());
            }

            return keys;
        }

        private List<House> GetRandomHouses(Int32 size)
        {
            return (List<House>) Builder<House>.CreateListOfSize(size).All()
           .With(c => c.WindowsNumber = Faker.RandomNumber.Next(1, 7))
           .With(c => c.DoorNumber = Faker.RandomNumber.Next(1, 4))
           .With(c => c.FloorsNumber = Faker.RandomNumber.Next(1, 3))
           .With(c => c.HasGarage = true)
           .Build();
        }

        private HashMap<Int32, House> GetMapOfHouse(Int32 size)
        {
            HashMap<Int32, House> houseMap = new HashMap<Int32, House>(140, 0.5f, 1.5f);

            for (Int32 i = 0; i < size; i++) {
                houseMap.Put(GetRandomKey(), GetRandomHouse());
            }

            return houseMap;
        }

        private void FillMapOfHouse(Int32 size)
        {
            for (Int32 i = 0; i < size; i++)
            {
                houseMap.Put(GetRandomKey(), GetRandomHouse());
            }
        }

        private void FillMapOfHouse(List<Int32> keys)
        {
            for (Int32 i = 0; i < keys.Count; i++)
            {
                houseMap.Put(keys[i], GetRandomHouse());
            }
        }

        private void FillMapOfHouse(List<House> houses)
        {
            for (Int32 i = 0; i < houses.Count; i++)
            {
                houseMap.Put(GetRandomKey(), houses[i]);
            }
        }

        private void FillMapOfHouse(List<Int32> keys, List<House> houses)
        {
            Int32 length = Math.Min(keys.Count, houses.Count);
            for (Int32 i = 0; i < length; i++)
            {
                houseMap.Put(keys[i], houses[i]);
            }
        }

        #endregion
    }
}
