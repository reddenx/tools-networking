//using App.TestingGrounds;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

//namespace App.WebTesting.Models
//{

//    public class ManagerTest : IManager
//    {
//        private static Random Rand = new Random();

//        public ManagerTest()
//        { }

//        private float GetFloat(float low, float high)
//        {
//            return (float)(Rand.NextDouble() * (high - low) + low);
//        }

//        public float TestPrimitives(float low, float high)
//        {
//            return low + high;
//        }

//        public float TestPrimitiveVoid()
//        {
//            return 42;
//        }

//        public void TestVoid()
//        {
//            return;
//        }

//        public void TestVoidPrimitive(float input1)
//        {
//            return;
//        }

//        public ComplexDto TestComplexReturn()
//        {
//            return new ComplexDto() { Data = "TestComplexReturn", SubDto = new ComplexDto() { Data = "SUB TestComplexReturn" } };
//        }

//        public ComplexDto TestComplexBoth(ComplexDto input1, ComplexDto input2)
//        {
//            input1.SubDto = input2;
//            return input1;
//        }

//        public void TestComplexInput(ComplexDto input1)
//        {
//            return;
//        }

//        public void TestMixed(ComplexDto input1, float input2)
//        {
//            return;
//        }

//        public ComplexDto TestMixed2(float derp)
//        {
//            return new ComplexDto() { Data = "derp is " + derp };
//        }

//        public GenericDto<string> Generic1(GenericDto<ComplexDto> input1, GenericDto<string> input2, GenericDto<int> input3, float input4)
//        {
//            return input2;
//        }

//        public void Generic2(GenericDto<ComplexDto> input1, GenericDto<string> input2, GenericDto<int> input3, float input4)
//        {
//            return;
//        }
//    }
//}