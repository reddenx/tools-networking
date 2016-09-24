//using SMT.Utilities.DynamicApi.Dto;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace App.TestingGrounds
//{
//    public class ComplexDto
//    {
//        public string Data;
//        public ComplexDto SubDto;
//    }

//    public class GenericDto<T>
//    {
//        public T Value;
//        public T[] MoreValues;
//        public string Data;
//    }

//    [DynamicApi("SomeTest")]
//    public interface IManager
//    {
//        float TestPrimitives(float low, float high);
//        float TestPrimitiveVoid();
//        void TestVoid();
//        void TestVoidPrimitive(float input1);
//        ComplexDto TestComplexReturn();
//        ComplexDto TestComplexBoth(ComplexDto input1, ComplexDto input2);
//        void TestComplexInput(ComplexDto input1);
//        void TestMixed(ComplexDto input1, float input2);
//        ComplexDto TestMixed2(float derp);
//        GenericDto<string> Generic1(GenericDto<ComplexDto> input1, GenericDto<string> input2, GenericDto<int> input3, float input4);
//        void Generic2(GenericDto<ComplexDto> input1, GenericDto<string> input2, GenericDto<int> input3, float input4);
//    }

//    public static class DynamicApiTesting
//    {
//        public static void Run()
//        {
//            var random = new Random();
//            var client = new SMT.Utilities.DynamicApi.DynamicApiClient<IManager>("http://localhost:56983/", "derp");

//            Console.WriteLine(client.Call(c => c.TestPrimitives(1, 100)));
//            Console.WriteLine(client.Call(c => c.TestPrimitiveVoid()));
//            client.Call(c => c.TestVoid());
//            client.Call(c => c.TestVoidPrimitive(100));
//            var returnVal = client.Call(c => c.TestComplexReturn());
//            Console.WriteLine(client.Call(c => c.TestComplexBoth(new ComplexDto() { Data = "some data", SubDto = null }, new ComplexDto() { Data = "more data", SubDto = new ComplexDto() { SubDto = null, Data = "sub data" } })));
//            client.Call(c => c.TestComplexInput(new ComplexDto() { Data = "second data test", SubDto = new ComplexDto() { Data = "more sub data", SubDto = new ComplexDto() { Data = "EVEN MORE" } } }));
//            client.Call(c => c.TestMixed(new ComplexDto() { Data = "ughhhh test data is boring" }, 100));
//            var returnVal2 = client.Call(c => c.TestMixed2(12));
//            var returnVal3 = client.Call(c => c.Generic1(
//                new GenericDto<ComplexDto>() { Data = "stuff N things", MoreValues = new[] { returnVal, returnVal2 }, Value = returnVal },
//                new GenericDto<string>() { Data = "more stuff N things", MoreValues = new[] { "one", "two", "three" }, Value = "another" },
//                new GenericDto<int>() { Data = "AGH stuff N thingsss", MoreValues = null, Value = 42 },
//                12f));
//            client.Call(c => c.Generic2(
//                new GenericDto<ComplexDto>() { Data = "stuff N things", MoreValues = new[] { returnVal, returnVal2 }, Value = returnVal },
//                new GenericDto<string>() { Data = "more stuff N things", MoreValues = new string[] { }, Value = "another" },
//                new GenericDto<int>() { Data = "AGH stuff N thingsss", MoreValues = null, Value = 42 },
//                12f));


//            Console.ReadLine();
//        }
//    }
//}
