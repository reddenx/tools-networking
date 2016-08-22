using App.TestingGrounds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.WebTesting.Models
{
    public class ManagerTest : IManager
    {
        private Random Rand = new Random();

        public ManagerTest()
        { }

        public float GetRandom(float low, float high)
        {
            return (float)(Rand.NextDouble() * (high - low) + low);
        }
    }
}